using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Plane : Primitive
{
    public Vector3 normal;
    public float distance;

    public Plane(float distance, Vector3 color, Vector3 normal, bool isTextured = false, string type = "normal") : base(color, type, isTextured)
    {
        this.normal = normal;
        this.distance = distance;
    }

    public Intersection IntersectPlane(Ray ray)
    {
        float t = -(Vector3.Dot(ray.start, normal) + distance) / (Vector3.Dot(ray.direction, normal));
        if ((t < ray.distance) && (t > 0))
        {
            ray.distance = t;
        }
        return new Intersection(ray, this, normal);
    }
}
