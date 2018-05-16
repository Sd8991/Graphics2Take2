using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Light
{
    public Vector3 position;
    public Vector3 color;
    private float red, green, blue;
    public Light(Vector3 position, Vector3 color)
    {
        this.position = position;
        red = color.X;
        green = color.Y;
        blue = color.Z;
        this.color = color;
    }
}
