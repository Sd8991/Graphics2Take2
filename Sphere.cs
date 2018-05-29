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

    public Sphere(Vector3 position, Vector3 color, float radius, string type, float specularity = 1, bool isTextured = false) : base(color, type, specularity, isTextured)
    {
        this.radius = radius;
        this.position = position;
    }

    public Intersection IntersectSphere(Ray ray) //gets the intersection with a primitive
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

    public Intersection IntersectInnerSphere(Ray ray) //Intersection method we wanted to use for glass balls, not used
    {
        float t = 0;
        float a = 1;
        float b = Vector3.Dot(2 * ray.direction, -(ray.start - position));
        float c = Vector3.Dot((ray.start - position), (ray.start - position)) - (radius * radius);
        float divider = 1 / (2 * a);
        float sqrt = (b * b) - 4 * a * c;
        if (sqrt > 0)
            t = (float)(-b - (float)Math.Sqrt(sqrt) * divider);
        else return null;
        if (t < 0.1)
            t = (float)(-b + Math.Sqrt(sqrt) * divider);
        ray.distance = t;
        normal = ((ray.start + ray.direction * (float)ray.distance) - position).Normalized();
        if (Vector3.Dot(normal, ray.direction) > 0) normal = -normal;
        return new Intersection(ray, this, normal);
    }
}
