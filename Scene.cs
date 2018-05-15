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
        primitives.Add(new Sphere(new OpenTK.Vector3(0, 0, -10), new OpenTK.Vector3(200, 0, 100), 3));
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
