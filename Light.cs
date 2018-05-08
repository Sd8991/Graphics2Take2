using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Light
{
    private float red, green, blue;
    public Light(Vector3 position, Vector3 color)
    {
        red = color.X;
        green = color.Y;
        blue = color.Z;
    }
}
