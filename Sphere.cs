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

    public Sphere(Vector3 position, Vector3 color, float radius, string type = "normal", bool isTextured = false) : base(color, type, isTextured)
    {
        this.radius = radius;
        this.position = position;
    }

    public Intersection IntersectSphere(Ray ray)
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

    public Intersection IntersectInnerSphere(Ray ray)
    {
        float a = 1;
        float b = 2 * Vector3.Dot(ray.direction, (ray.start - position));
        float c = Vector3.Dot((ray.start - position), (ray.start - position)) - (radius * radius);
        float divider = 1 / 2 * a;
        float t = (float)(-b + Math.Sqrt((b * b) - 4 * a * c)) * divider;
        if (t < 0.1)
            t = (float)(-b - Math.Sqrt((b * b) - 4 * a * c)) * divider;
        ray.distance = t;
        normal = ((ray.start + ray.direction * (float)ray.distance) - position);
        if (Vector3.Dot(normal, ray.direction) > 0) normal = -normal;
        normal.Normalize();
        return new Intersection(ray, this, normal);
        //recursion++;
    }
}
