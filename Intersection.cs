﻿using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Intersection
{
    public Primitive nearestPrimitive;
    public double intersectDistance;
    public Vector3 intersectNorm;
    public Vector3 intersectPoint;
    public Vector3 color;

    public Intersection(Ray ray, Primitive primitive, Vector3 normal)
    {
        nearestPrimitive = primitive;
        intersectDistance = ray.distance;
        intersectPoint = ray.start + (float)intersectDistance * ray.direction;
        intersectNorm = normal;
        color = primitive.color;
    }
}
