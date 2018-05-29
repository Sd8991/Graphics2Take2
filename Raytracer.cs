using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Raytracer
{
    Ray ray;
    public Ray shadowRay;
    Scene scene;
    Surface screen;   
    bool showdebugprimary = true, showdebugsecondary = false, showdebugshadow = true;
    int colorindex = 0;
    Intersection intersection;
    List<Vector2> debugReflections;
    Camera c;
    int scale = 20;
    public int recursion = 0;

    public void Render(Camera c, Surface screen, Scene scene, int mF)
    {
        this.c = c;
        this.scene = scene;
        this.screen = screen;
        Vector2 debugcpos = TranslateToDebug(c.position, screen);               //debug setup: camera
        Vector2 screenleft = TranslateToDebug(c.leftUpperCorner, screen);       //screen
        Vector2 screenright = TranslateToDebug(c.rightUpperCorner, screen);     //    
        foreach (Primitive s in scene.primitives)                               //spheres
            if (s.GetType() == typeof(Sphere)) DebugSphere((Sphere)s, screen);  //

        for (float y = 0; y < screen.height; y = y + mF)            //Loop over all pixels. mF is being used when we move, to get a
            for (float x = 0; x < screen.width / 2; x = x + mF)     //slightly smoother movement
            {
                ray = c.ShootRay(new Vector3((x * 2 / screen.width), (y * 1 / screen.height), 1)); //shoot ray
                try
                {
                    recursion = 0;
                    debugReflections = new List<Vector2>();
                    screen.pixels[(int)x + (int)y * screen.width] = CreateColor(HandleRay(ray, scene, 0)); //color a pixel according to its intersection color
                }
                catch { screen.pixels[(int)x + (int)y * screen.width] = 0; } //if ray doesn't intersect, make pixel black

                if (y == 256 && x % 25 == 0)    //debug rendering
                {
                    colorindex = 0;
                    Vector3 intersectpoint = ray.start + ray.direction * (float)ray.distance;   //set endpoint for debug ray
                    Vector2 debugintersection = TranslateToDebug(intersectpoint, screen);       //
                    if (showdebugprimary)   //draw primary rays
                    {                       
                        screen.Line((int)debugcpos.X, (int)debugcpos.Y, (int)debugintersection.X, (int)debugintersection.Y, (255 << 16) + (255 << 8));
                    }
                    if (debugReflections.Count >= 2 && showdebugsecondary)
                    {
                        for (int i = 0; i < debugReflections.Count - 1; i++)    //draw secondary rays
                        {
                            screen.Line((int)debugReflections[i].X, (int)debugReflections[i].Y, (int)debugReflections[i + 1].X, (int)debugReflections[i + 1].Y, CreateColor(new Vector3(0.5f, 0.5f, 1)));
                        }
                    }
                    foreach (Light l in scene.lights)
                    {
                        bool isLit = CastShadowRay(intersection, shadowRay.direction, (l.position - intersectpoint).Length);
                        if (showdebugshadow && isLit) //draw shadow rays
                        {
                            Vector2 debuglpos = TranslateToDebug(l.position, screen);
                            screen.Line((int)debuglpos.X, (int)debuglpos.Y, (int)debugintersection.X, (int)debugintersection.Y, (255 << (16 - 2 * (colorindex % 8))));
                        }
                        colorindex++; //change debug shadow ray color for next light source
                    }
                    
                }
            }
        screen.Line((int)screenleft.X, (int)screenleft.Y, (int)screenright.X, (int)screenright.Y, CreateColor(new Vector3(1, 1, 1)));
        //draw screen line
    }

    public Vector2 TranslateToDebug(Vector3 v, Surface screen) //converts a position in the scene to a position on the debug screen
    {
        return new Vector2(v.X * scale + 3 * screen.width / 4, -v.Z * scale + 3 * screen.height / 4);
    }

    public void DebugSphere(Sphere s, Surface screen) //draws the provided Sphere to the debug screen
    {
        float debugradius = (float)Math.Sqrt(Math.Pow(s.radius, 2) - Math.Pow(c.position.Y - s.position.Y, 2)); //set debug circle radius based on camera height
        Vector2 center = TranslateToDebug(s.position, screen);
        Vector2 prevPoint = new Vector2((int)(debugradius * Math.Cos(0) * scale + center.X), (int)(debugradius * Math.Sin(0) * scale + center.Y)); //set starting point
        for (int i = 0; i < 100; i++) //draw circle
        {
            Vector2 currPoint = new Vector2((int)(debugradius * Math.Cos((2 * Math.PI / 100) * i) * scale + center.X), (int)(debugradius * Math.Sin((2 * Math.PI / 100) * i) * scale + center.Y));
            screen.Line((int)prevPoint.X, (int)prevPoint.Y, (int)currPoint.X, (int)currPoint.Y, CreateColor(s.color));
            prevPoint = currPoint;
        }
    }

    //determines the color that should be drawn to the screen point that the provided ray was shot at.
    public Vector3 HandleRay(Ray ray, Scene scene, int recursion, bool refracIntersect = false) 
    {
        intersection = scene.intersectScene(ray, refracIntersect); //intersect the ray with the scene
        debugReflections.Add(TranslateToDebug(intersection.intersectPoint, screen)); //adds intersect points to a list for use in the debug view
        if (intersection != null) //if there is an intersection
        {
            if (recursion < 4) //set recursion cap here
            {
                switch (intersection.nearestPrimitive.type) //look which type the primitive has
                {
                    case "normal":
                        {
                            if (intersection.nearestPrimitive.GetType() == typeof(Plane) && intersection.nearestPrimitive.isTextured) //texture the groundplane
                            {
                                return DirectIllumination(intersection) * 255 * TexturePlane(intersection);
                            }
                            return DirectIllumination(intersection) * 255; //return a color using direct illumination
                        }
                    case "mirror": return HandleRay(Reflect(ray, intersection), scene, recursion++, refracIntersect); //return a color based on the color after reflection
                    case "partial": return (0.5f * DirectIllumination(intersection) * 255) + (0.5f * HandleRay(Reflect(ray, intersection), scene, recursion+1, refracIntersect)); //return a color based on the reflection and color of primitive
                    case "dielectric": //return a color based on reflection and refraction, not used
                        {
                            float n1 = 1; float n2 = 1.6f;
                            float R0 = (float)Math.Pow(((n2 - n1) / (n2 + n1)), 2);
                            float pretest = Vector3.Dot(ray.direction, -intersection.intersectNorm) / (intersection.intersectNorm.Length * ray.direction.Length);
                            float test = (float)Math.Pow((1 - pretest) , 5);
                            float f = R0 + (1 - R0) * test;
                            return (f * HandleRay(Reflect(ray, intersection), scene, recursion+1, false) + (1 - f) * HandleRay(Refract(ray, intersection, n1, n2), scene, recursion+1, true));
                        }
                }
            }
            //return Vector3.Zero;
        }
        return Vector3.Zero;
    }

    public Vector3 DirectIllumination(Intersection intersection) //handles coloring of diffuse components
    {
        Vector3 color = Vector3.Zero;
        foreach (Light light in scene.lights) //loop over all lightsources
        {
            float distance = (light.position - intersection.intersectPoint).Length; //calculate distance lightsource to intersection
            Vector3 shadowRayDir = (light.position - intersection.intersectPoint).Normalized(); //calculate direction shadowray
            if (!LightsourceVisible(intersection.intersectNorm, shadowRayDir)) color += Vector3.Zero; //if intersection not reached by light, return 0
            else//First check if lightsource is reachable
            {
                bool isLighted = CastShadowRay(intersection, shadowRayDir, distance);
                if (isLighted)
                {                    
                    float NdotL = Vector3.Dot(intersection.intersectNorm, shadowRayDir); //N dot L
                    float attenuation = 1 / (distance * distance); //calculate attenuation
                    color += MathHelper.Clamp(NdotL, 0, 1) * attenuation * light.color * intersection.color; //add color to total color of this pixel
                }
                else color += Vector3.Zero;
            }
        }
        return color;
    }

    public Ray Reflect(Ray ray, Intersection i) //returns the appropriate reflection of a ray on the provided intersection
    {
        Vector3 newRayDir = (ray.direction - 2 * i.intersectNorm * (Vector3.Dot(ray.direction, i.intersectNorm))).Normalized();
        return new Ray(i.intersectPoint + 0.1f * newRayDir, newRayDir);
    }

    public Ray Refract(Ray ray, Intersection i, float n1, float n2) //should handle refraction, not used
    {
        float Ai = Vector3.Dot(ray.direction, intersection.intersectNorm) / (ray.direction.Length * intersection.intersectNorm.Length);
        float k = 1 - (((n1 / n2) * (n1 / n2)) * (1 - (Ai * Ai)));
        if (k >= 0)
        {
            float sumfin = (float)((n1 / n2) * Ai - Math.Sqrt(k));
            Vector3 newRayDir = ((n1 / n2) * ray.direction + i.intersectNorm * (sumfin)).Normalized();
            return new Ray(i.intersectPoint + 0.1f * newRayDir, newRayDir);
        }
        return Reflect(ray, i);
    }

    public bool CastShadowRay(Intersection i, Vector3 dir, float dis)
    {
        shadowRay = new Ray(i.intersectPoint, dir); //cast shadowray
        Intersection shadowIntersect = scene.intersectScene(shadowRay, false, dis - (0.2f * dir.Length)); //intersect shadowray with scene
        if (shadowIntersect == null) //check if there is a intersection
            return true;
        return false;
    }

    public int CreateColor(Vector3 color) //calculate the definitive color of a pixel
    {
        return ((int)Math.Min(255, color.X * 255) << 16) + ((int)Math.Min(255, color.Y * 255) << 8) + ((int)Math.Min(255, color.Z * 255));
    }

    public bool LightsourceVisible(Vector3 N, Vector3 L)
    {
        if (Vector3.Dot(N, L) >= 0)
            return true;
        return false;
    }

    public int TexturePlane(Intersection i)
    {
        Vector3 u = new Vector3(1, 0, 0);
        Vector3 v = new Vector3(0, 0, 1);
        float lambda1 = i.intersectPoint.X;
        float lambda2 = i.intersectPoint.Z;
        return ((int)(2 * lambda1) + (int)lambda2) & 1;
    }
}
