using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Scene
{
    public List<Primitive> primitives = new List<Primitive>();
    public List<Light> lights = new List<Light>();

    public Scene()
    {
        primitives.Add(new Sphere(new Vector3(-5, 0, 10), new Vector3(1, 0, 0.5f), 2));
        primitives.Add(new Sphere(new Vector3(0, 0, 10), new Vector3(1, 0.5f, 1), 2));
        primitives.Add(new Sphere(new Vector3(5, 0, 10), new Vector3(0, 0, 1), 2));
        //primitives.Add(new Plane(10, new Vector3(1, 1, 0), new Vector3(0, -1, 0)));
        lights.Add(new Light(new Vector3(0, 0, 5), new Vector3(1, 0, 1)));
    }

    public Intersection intersectScene(Ray ray)
    {
        double nearestIntersectDist = double.PositiveInfinity;
        Intersection nearestIntersect = null;

        foreach (Primitive p in primitives)
        {
            Intersection currentIntersect = null;

            if (p is Sphere)
            {
                Sphere sphere = (Sphere)p;
                currentIntersect = sphere.Intersect(ray);
            }

            if (p is Plane)
            {
                Plane plane = (Plane)p;
                currentIntersect = plane.Intersect(ray);
            }

            if (currentIntersect != null && currentIntersect.intersectDistance < nearestIntersectDist)
            {
                nearestIntersect = currentIntersect;
                nearestIntersectDist = currentIntersect.intersectDistance;
            }
        }

        return nearestIntersect;
    }

    public Ray castShadowRay(Intersection intersection)
    {
        Vector3 shadowRayDir = (lights[0].position - intersection.intersectPoint).Normalized();
        return new Ray(intersection.intersectPoint + 0.1f*-shadowRayDir, -shadowRayDir);
    }

    public Vector3 intersectShadowRay(Ray shadowRay, Light light)
    {
        double test = (lights[0].position - shadowRay.start).Length;
        Intersection shadowRayIntersect = intersectScene(shadowRay);
        if (shadowRayIntersect == null)
            return Vector3.Zero;
        else return (Vector3.Dot(shadowRayIntersect.intersectNorm, shadowRay.direction) * light.color);
    }
}
