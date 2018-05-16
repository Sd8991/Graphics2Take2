using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Raytracer
{
    Ray ray;
    Ray shadowRay;
    Intersection intersection;
    Vector3 intersectionColor;
    int scale = 20;

    public void Render(Camera c, Surface screen, Scene scene)
    {
        Vector2 debugcpos = TranslateToDebug(c.position, screen);
        for (float y = 0; y < screen.height; y++)
            for (float x = 0; x < screen.width / 2; x++)
            {
                ray = c.ShootRay(new Vector3((x * 2 / screen.width), (y / screen.height), 1));
                intersection = scene.intersectScene(ray);
                //intersection = scene.intersectScene(ray);
                //intersectionColor = HandleRay(ray, scene);
                if (intersection != null)
                    screen.pixels[(int)x + (int)y * screen.width] = HandleRay(ray, scene);
                else screen.pixels[(int)x + (int)y * screen.width] = 0;

                if (y == 256 && x % 5 == 0)
                {
                    Vector3 intersectpoint = ray.start + ray.direction * (float)ray.distance;
                    Vector2 debugintersection = TranslateToDebug(intersectpoint, screen);
                    screen.Line((int)debugcpos.X, (int)debugcpos.Y, (int)debugintersection.X, (int)debugintersection.Y, (255 << 16) + (255 << 8));
                }
            }
    }

    public Vector2 TranslateToDebug(Vector3 v, Surface screen)
    {
        return new Vector2(v.X * scale + 3 * screen.width / 4, -v.Z * scale + 3 * screen.height / 4);
    }

    public int HandleRay(Ray ray, Scene scene)
    {
        intersectionColor = Vector3.Zero;
        shadowRay = scene.castShadowRay(intersection);
        foreach (Light light in scene.lights)
            intersectionColor += (scene.intersectShadowRay(shadowRay, light) * intersection.color);
        return CreateColor(intersectionColor);
    }

    public int CreateColor(Vector3 color)
    {
        return (((int)color.X) * 255 << 16) + (((int)color.Y) * 255 << 8) + ((int)color.Z) * 255;
    }
}
