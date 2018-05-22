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
    Application a = new Application();
    Intersection intersection;
    int scale = 20;

    public void Render(Camera c, Surface screen, Scene scene)
    {
        this.scene = scene;
        a.moveCamera(c);
        a.rotateCamera(c);
        Vector2 debugcpos = TranslateToDebug(c.position, screen);
        Vector2 screenleft = TranslateToDebug(c.leftUpperCorner, screen);
        Vector2 screenright = TranslateToDebug(c.rightUpperCorner, screen);       
        foreach (Primitive s in scene.primitives)
            if (s.GetType() == typeof(Sphere)) DebugSphere((Sphere)s, screen); ;
        for (float y = 0; y < screen.height; y++)
            for (float x = 0; x < screen.width / 2; x++)
            {
                ray = c.ShootRay(new Vector3((x * 2 / screen.width), (y * 1 / screen.height), 1));
                try
                {
                    screen.pixels[(int)x + (int)y * screen.width] = HandleRay(ray, scene);
                }
                catch { screen.pixels[(int)x + (int)y * screen.width] = 0; }

                if (y == 256 && x % 25 == 0)
                {
                    Vector3 intersectpoint = ray.start + ray.direction * (float)ray.distance;
                    Vector2 debugintersection = TranslateToDebug(intersectpoint, screen);
                    screen.Line((int)debugcpos.X, (int)debugcpos.Y, (int)debugintersection.X, (int)debugintersection.Y, (255 << 16) + (255 << 8));
                }
            }
        screen.Line((int)screenleft.X, (int)screenleft.Y, (int)screenright.X, (int)screenright.Y, CreateColor(new Vector3(1, 1, 1)));

    }

    public Vector2 TranslateToDebug(Vector3 v, Surface screen)
    {
        return new Vector2(v.X * scale + 3 * screen.width / 4, -v.Z * scale + 3 * screen.height / 4);
    }

    public int HandleRay(Ray ray, Scene scene)
    {
        intersection = scene.intersectScene(ray);
        if (intersection != null)
        {
            switch (intersection.nearestPrimitive.type)
            {
                case "normal":
                    {
                        if (intersection.nearestPrimitive.GetType() == typeof(Plane) && intersection.nearestPrimitive.isTextured)
                        {
                            return CreateColor(DirectIllumination(intersection) * 255) * TexturePlane(intersection);
                        }
                        return CreateColor(DirectIllumination(intersection) * 255);
                    }
                case "mirror": return HandleRay(Reflect(ray, intersection), scene);
            }
            return 0;
        }
        return 0;
    }

    public Vector3 DirectIllumination(Intersection intersection)
    {
        Vector3 color = Vector3.Zero;
        foreach (Light light in scene.lights)
        {
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
                    color += (MathHelper.Clamp(NdotL, 0, 1) * attenuation * light.color * intersection.color);
                }
                else color += Vector3.Zero;
            }
        }
        return color;
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
        Intersection shadowIntersect = scene.intersectScene(shadowRay, dis -( 0.2f * dir.Length));
        if (shadowIntersect == null)
            return true;
        return false;
    }

    public Ray Reflect(Ray ray, Intersection i)
    {
        Vector3 newRayDir = (ray.direction - 2 * i.intersectNorm * (Vector3.Dot(ray.direction, i.intersectNorm))).Normalized();
        return new Ray(i.intersectPoint + 0.1f * newRayDir, newRayDir);
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
