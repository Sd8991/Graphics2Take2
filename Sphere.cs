using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Sphere : Primitive
{
    public float radius;
    public Vector3 position;
    public Vector3 normal;

    public Sphere(Vector3 position, Vector3 color, float radius) : base(color)
    {
        this.radius = radius;
        this.position = position;
    }

    public override Intersection Intersect(Ray ray)
    {
        Vector3 c = position - ray.start;
        float t = Vector3.Dot(c, ray.direction);
        Vector3 q = c - t * ray.direction;
        float pp = Vector3.Dot(q, q);
        if (pp > (radius * radius)) return null;
        t -= (float)Math.Sqrt((radius * radius) - pp);
        if ((t < ray.distance) && (t > 0))
        {
            ray.distance = t;
        }
        normal = ((ray.start + ray.direction * (float)ray.distance) - position);
        normal.Normalize();
        return new Intersection(ray, this, normal);
    }
}
