using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Ray
{
    public Vector3 start;
    public Vector3 direction;
    public double distance;
    float idX;  float idY;

    public Ray(Vector3 start, Vector3 direction, float idX, float idY)
    {
        this.start = start;
        this.direction = direction;
        distance = 100000;
        this.idX = idX;
        this.idY = idY;
    }
}

