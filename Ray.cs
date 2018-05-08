using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Ray
{
    public Vector3 start;
    public Vector3 direction;
    public float distance;

    public Ray(Vector3 start, Vector3 direction)
    {
        this.start = start;
        this.direction = direction;
    }
}

