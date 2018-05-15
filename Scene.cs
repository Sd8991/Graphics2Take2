using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Scene
{
    public List<Primitive> primitives = new List<Primitive>();
    List<Light> lights = new List<Light>();

    public Scene()
    {
        primitives.Add(new Sphere(new Vector3(-5, 0, 10), new Vector3(1, 0, 0.5f), 2));
        primitives.Add(new Sphere(new Vector3(0, 0, 10), new Vector3(1, 0.5f, 1), 2));
        primitives.Add(new Sphere(new Vector3(5, 0, 10), new Vector3(0, 0, 1), 2));
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

            if (currentIntersect != null && currentIntersect.intersectDistance < nearestIntersectDist)
            {
                nearestIntersect = currentIntersect;
                nearestIntersectDist = currentIntersect.intersectDistance;
            }
        }

        return nearestIntersect;
    }
}
