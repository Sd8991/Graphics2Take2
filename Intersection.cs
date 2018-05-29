using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Intersection
{
    public Primitive nearestPrimitive;
    public float intersectDistance;
    public Vector3 intersectNorm;
    public Vector3 intersectPoint;
    public Vector3 color;
    public float specularity;

    public Intersection(Ray ray, Primitive primitive, Vector3 normal)
    {
        nearestPrimitive = primitive; // the primitive that is being intersected with
        intersectDistance = ray.distance; //distance camera to intersection
        intersectPoint = ray.start + (float)intersectDistance * ray.direction; //the coordinates of the intersection
        intersectNorm = normal; //normal of the intersection
        color = primitive.color; //color of the primitive
        specularity = primitive.specularity; //specularity of the primitive(not used)
    }
}
