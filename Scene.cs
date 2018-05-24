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
        primitives.Add(new Sphere(new Vector3(3, -0.5f, 7), new Vector3(0f, 1, 0.5f), 1, "partial"));
        primitives.Add(new Sphere(new Vector3(0, -0.5f, 7), new Vector3(1f, 0.5f, 1f), 1));
        primitives.Add(new Sphere(new Vector3(-3, -0.5f, 7), new Vector3(1, 1, 1), 1, "mirror"));
        primitives.Add(new Plane(20, new Vector3(1, 1, 0), new Vector3(0, 0, -1))); //back
        primitives.Add(new Plane(1, new Vector3(1, 1, 1), new Vector3(0, -1, 0), true)); //floor
        primitives.Add(new Plane(10, new Vector3(1, 0, 1), new Vector3(0, 1, 0))); //ceiling
        primitives.Add(new Plane(10, new Vector3(0, 1, 1), new Vector3(1, 0, 0))); //left
        primitives.Add(new Plane(10, new Vector3(1, 0, 0), new Vector3(-1, 0, 0))); //right
        primitives.Add(new Plane(2, new Vector3(1, 1, 1), new Vector3(0, 0, 1))); //behind
        //lights.Add(new Light(new Vector3(0, -0, 4), new Vector3(0.01f, 0.01f, 0.01f)));
        lights.Add(new Light(new Vector3(0, -5, 3), new Vector3(0.15f, 0.15f, 0.15f)));
        //lights.Add(new Light(new Vector3(5, 0, 2), new Vector3(0, 0, 1)));
    }

    public Intersection intersectScene(Ray ray, float maxDis = float.PositiveInfinity)
    {
        float minDis = 0.1f;
        float nearestIntersectDist = maxDis - 0.2f;
        Intersection nearestIntersect = null;

        foreach (Primitive p in primitives)
        {
            Intersection currentIntersect = null;

            if (p is Sphere)
            {
                Sphere sphere = (Sphere)p;
                currentIntersect = sphere.IntersectSphere(ray);
            }

            if (p is Plane)
            {
                Plane plane = (Plane)p;
                currentIntersect = plane.IntersectPlane(ray);
            }

            if (currentIntersect != null && currentIntersect.intersectDistance < nearestIntersectDist && currentIntersect.intersectDistance > minDis)
            {
                nearestIntersect = currentIntersect;
                nearestIntersectDist = currentIntersect.intersectDistance;
            }
        }

        return nearestIntersect;
    }
}
