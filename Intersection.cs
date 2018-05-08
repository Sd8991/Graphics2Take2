using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Intersection
{
    public Primitive nearest;
    public double intersectDist;
    public Vector3 intersectNorm;
    public int color;


    public Intersection(Ray ray, Scene scene)
    {
        nearest = null;
        intersectDist = double.PositiveInfinity;
        intersectNorm = Vector3.Zero;
        

        foreach (Primitive p in scene.primitives)
        {
            p.Intersect(ray);
            if (ray.distance < intersectDist)
            {
                intersectDist = ray.distance;
                nearest = p;
                color = ((int)nearest.color.X << 16) + ((int)nearest.color.Y << 8) + (int)nearest.color.Z;
            }
        }
    }
}
