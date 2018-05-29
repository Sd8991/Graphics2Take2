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
        //Three frontal Balls
        primitives.Add(new Sphere(new Vector3(3, 0, 7), new Vector3(0f, 1, 0.5f), 1, "partial"));
        primitives.Add(new Sphere(new Vector3(0, 0, 8), new Vector3(0.5f, 1f, 0.5f), 1, "normal"));
        primitives.Add(new Sphere(new Vector3(-3, 0, 7), new Vector3(1, 1, 1), 1, "mirror"));
        //Ball Pyramid
        primitives.Add(new Sphere(new Vector3(-10, 0.5f, 6), new Vector3(1, 1, 0), 0.5f, "partial"));
        primitives.Add(new Sphere(new Vector3(-9, 0.5f, 6), new Vector3(1, 1, 0), 0.5f, "partial"));
        primitives.Add(new Sphere(new Vector3(-8, 0.5f, 6), new Vector3(1, 1, 0), 0.5f, "partial"));
        primitives.Add(new Sphere(new Vector3(-10, 0.5f, 5), new Vector3(1, 1, 0), 0.5f, "partial"));
        primitives.Add(new Sphere(new Vector3(-9, 0.5f, 5), new Vector3(1, 1, 0), 0.5f, "partial"));
        primitives.Add(new Sphere(new Vector3(-8, 0.5f, 5), new Vector3(1, 1, 0), 0.5f, "partial"));
        primitives.Add(new Sphere(new Vector3(-10, 0.5f, 4), new Vector3(1, 1, 0), 0.5f, "partial"));
        primitives.Add(new Sphere(new Vector3(-9, 0.5f, 4), new Vector3(1, 1, 0), 0.5f, "partial"));
        primitives.Add(new Sphere(new Vector3(-8, 0.5f, 4), new Vector3(1, 1, 0), 0.5f, "partial"));//
        primitives.Add(new Sphere(new Vector3(-9.5f, -0.4f, 4.5f), new Vector3(1, 1, 0), 0.5f, "partial"));
        primitives.Add(new Sphere(new Vector3(-8.5f, -0.4f, 4.5f), new Vector3(1, 1, 0), 0.5f, "partial"));
        primitives.Add(new Sphere(new Vector3(-9.5f, -0.4f, 5.5f), new Vector3(1, 1, 0), 0.5f, "partial"));
        primitives.Add(new Sphere(new Vector3(-8.5f, -0.4f, 5.5f), new Vector3(1, 1, 0), 0.5f, "partial"));
        primitives.Add(new Sphere(new Vector3(-9, -1.3f, 5), new Vector3(1, 0, 0), 0.5f, "normal"));
        //Line of Balls
        primitives.Add(new Sphere(new Vector3(7, -1, 9), new Vector3(0, 0.75f, 0.5f), 2f, "partial"));
        primitives.Add(new Sphere(new Vector3(7, -0.5f, 5.5f), new Vector3(0.75f, 0.75f, 0), 1.5f, "partial"));
        primitives.Add(new Sphere(new Vector3(7, 0, 3), new Vector3(0, 0.75f, 0.5f), 1f, "partial"));
        primitives.Add(new Sphere(new Vector3(7, 0.25f, 1.25f), new Vector3(0.2f, 0.7f, 0.5f), 0.75f, "normal"));
        primitives.Add(new Sphere(new Vector3(7, 0.5f, 0), new Vector3(0, 0.75f, 0.5f), 0.5f, "mirror"));
        primitives.Add(new Sphere(new Vector3(7, 0.75f, -0.8f), new Vector3(1, 0.5f, 0.25f), 0.25f, "partial"));
        primitives.Add(new Sphere(new Vector3(7, 0.9f, -1.05f), new Vector3(1, 1, 1), 0.1f, "normal"));
        //Planes
        primitives.Add(new Plane(15, new Vector3(1, 1, 0), new Vector3(0, 0, -1))); //back
        primitives.Add(new Plane(1, new Vector3(1, 1, 1), new Vector3(0, -1, 0), true)); //floor
        primitives.Add(new Plane(15, new Vector3(1, 0, 1), new Vector3(0, 1, 0))); //ceiling
        primitives.Add(new Plane(12, new Vector3(0, 1, 1), new Vector3(1, 0, 0))); //left
        primitives.Add(new Plane(12, new Vector3(1, 0, 0), new Vector3(-1, 0, 0))); //right
        primitives.Add(new Plane(15, new Vector3(1, 1, 1), new Vector3(0, 0, 1))); //behind
        //Lights
        lights.Add(new Light(new Vector3(-9, -1.9f, 5), new Vector3(0.05f, 0.05f, 0.05f)));
        lights.Add(new Light(new Vector3(0, -5, 3), new Vector3(0.15f, 0.15f, 0.15f)));
        lights.Add(new Light(new Vector3(5, -7, 10), new Vector3(0.1f, 0.1f, 0.1f)));
        lights.Add(new Light(new Vector3(5, -4, -3), new Vector3(0.1f, 0.2f, 0.2f)));
    }

    //processes intersections for the provided ray
    public Intersection intersectScene(Ray ray, bool refractIntersect,float maxDis = float.PositiveInfinity)
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
                if (refractIntersect)
                    currentIntersect = sphere.IntersectInnerSphere(ray);
                else currentIntersect = sphere.IntersectSphere(ray);
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
