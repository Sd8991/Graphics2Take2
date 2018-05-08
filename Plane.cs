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

    public Plane(float distance, Vector3 color, Vector3 normal) : base(color)
    {
        this.normal = normal;
        this.distance = distance;
    }

    public void Intersect(Ray ray)
    {
        float t = -(Vector3.Dot(ray.start, normal) + distance) / (Vector3.Dot(ray.direction, normal));
    }
}
