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
        Vector2 debugcpos = TranslateToDebug(c.position, screen);
        Vector2 screenleft = TranslateToDebug(c.leftUpperCorner, screen);
        Vector2 screenright = TranslateToDebug(c.rightUpperCorner, screen);       
        foreach (Primitive s in scene.primitives)
            if (s.GetType() == typeof(Sphere)) DebugSphere((Sphere)s, screen); ;
        for (float y = 0; y < screen.height; y = y + mF)
            for (float x = 0; x < screen.width / 2; x = x + mF)
            {
                ray = c.ShootRay(new Vector3((x * 2 / screen.width), (y * 1 / screen.height), 1));
                try
                {
                    recursion = 0;
                    debugReflections = new List<Vector2>();
                    screen.pixels[(int)x + (int)y * screen.width] = CreateColor(HandleRay(ray, scene, 0));
                }
                catch { screen.pixels[(int)x + (int)y * screen.width] = 0; }

                if (y == 256 && x % 25 == 0)
                {
                    Vector3 intersectpoint = ray.start + ray.direction * (float)ray.distance;
                    Vector2 debugintersection = TranslateToDebug(intersectpoint, screen);
                    screen.Line((int)debugcpos.X, (int)debugcpos.Y, (int)debugintersection.X, (int)debugintersection.Y, (255 << 16) + (255 << 8));
                    if (debugReflections.Count >= 2)
                    {
                        for (int i = 0; i < debugReflections.Count - 1; i++)
                        {
                            screen.Line((int)debugReflections[i].X, (int)debugReflections[i].Y, (int)debugReflections[i + 1].X, (int)debugReflections[i + 1].Y, CreateColor(new Vector3(0.5f, 0.5f, 1)));
                        }
                    }
                }
            }
        screen.Line((int)screenleft.X, (int)screenleft.Y, (int)screenright.X, (int)screenright.Y, CreateColor(new Vector3(1, 1, 1)));

    }

    public Vector2 TranslateToDebug(Vector3 v, Surface screen)
    {
        return new Vector2(v.X * scale + 3 * screen.width / 4, -v.Z * scale + 3 * screen.height / 4);
    }


    public Vector3 HandleRay(Ray ray, Scene scene, int recursion, bool refracIntersect = false)
    {
        intersection = scene.intersectScene(ray, refracIntersect);
        debugReflections.Add(TranslateToDebug(intersection.intersectPoint, screen));
        if (intersection != null)
        {
            if (recursion < 4)
            {
                switch (intersection.nearestPrimitive.type)
                {
                    case "normal":
                        {
                            if (intersection.nearestPrimitive.GetType() == typeof(Plane) && intersection.nearestPrimitive.isTextured)
                            {
                                return DirectIllumination(intersection) * 255 * TexturePlane(intersection);
                            }
                            return DirectIllumination(intersection) * 255;
                        }
                    case "mirror": return HandleRay(Reflect(ray, intersection), scene, recursion++, refracIntersect);
                    case "partial": return (0.5f * DirectIllumination(intersection) * 255) + (0.5f * HandleRay(Reflect(ray, intersection), scene, recursion+1, refracIntersect));
                    case "dielectric":
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

    public Vector3 DirectIllumination(Intersection intersection)
    {
        Vector3 color = Vector3.Zero;
        foreach (Light light in scene.lights)
        {
            Vector3 reflect = ((intersection.intersectPoint - light.position) - 2 * Vector3.Dot((intersection.intersectPoint - light.position), intersection.intersectNorm) * intersection.intersectNorm).Normalized();
            float energy = Vector3.Dot((c.position - intersection.intersectPoint), reflect);
            float distance = (light.position - intersection.intersectPoint).Length;
            Vector3 shadowRayDir = (light.position - intersection.intersectPoint).Normalized();
            if (!LightsourceVisible(intersection.intersectNorm, shadowRayDir)) color += Vector3.Zero;
            else//First check if lightsource is reachable
            {
                bool isLighted = CastShadowRay(intersection, shadowRayDir, distance);
                if (isLighted)
                {
                    float NdotL = Vector3.Dot(intersection.intersectNorm, shadowRayDir);
                    float attenuation = 1 / (distance * distance);
                    color += (MathHelper.Clamp(NdotL, 0, 1) * attenuation * light.color * intersection.color * (float)(Math.Pow(energy, intersection.specularity) / energy));
                }
                else color += Vector3.Zero;
            }
        }
        return color;
    }

    public Vector3 GlossyIllumination(Ray ray, Intersection intersection)
    {
        return Vector3.Zero;
    }

    public int CreateColor(Vector3 color)
    {
        return ((int)Math.Min(255, color.X * 255) << 16) + ((int)Math.Min(255, color.Y * 255) << 8) + ((int)Math.Min(255, color.Z * 255));
    }

    public void DebugSphere(Sphere s, Surface screen)
    {
        Vector2 center = TranslateToDebug(s.position, screen);
        Vector2 prevPoint = new Vector2((int)(s.radius * Math.Cos(0) * scale + center.X), (int)(s.radius * Math.Sin(0) * scale + center.Y));       
        for(int i = 0; i < 100; i++)
        {
            Vector2 currPoint = new Vector2((int)(s.radius * Math.Cos((2 * Math.PI / 100) * i) * scale + center.X), (int)(s.radius * Math.Sin((2 * Math.PI / 100) * i) * scale + center.Y));
            screen.Line((int)prevPoint.X, (int)prevPoint.Y, (int)currPoint.X, (int)currPoint.Y, CreateColor(s.color));
            prevPoint = currPoint;
        }
    }

    public bool LightsourceVisible(Vector3 N, Vector3 L)
    {
        if (Vector3.Dot(N, L) >= 0)
            return true;
        return false;
    }

    public bool CastShadowRay(Intersection i, Vector3 dir, float dis)
    {
        shadowRay = new Ray(i.intersectPoint, dir);
        if (i.nearestPrimitive.GetType()== typeof(Sphere))
        { }
        Intersection shadowIntersect = scene.intersectScene(shadowRay, false, dis -( 0.2f * dir.Length));
        if (shadowIntersect == null)
            return true;
        return false;
    }

    public Ray Reflect(Ray ray, Intersection i)
    {
        Vector3 newRayDir = (ray.direction - 2 * i.intersectNorm * (Vector3.Dot(ray.direction, i.intersectNorm))).Normalized();
        return new Ray(i.intersectPoint + 0.1f * newRayDir, newRayDir);
    }

    public Ray Refract(Ray ray, Intersection i, float n1, float n2)
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

    public int TexturePlane(Intersection i)
    {
        Vector3 u = new Vector3(1, 0, 0);
        Vector3 v = new Vector3(0, 0, 1);
        float lambda1 = i.intersectPoint.X;
        float lambda2 = i.intersectPoint.Z;
        return ((int)(2 * lambda1) + (int)lambda2) & 1;
    }
}
