using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Primitive
{
    public Vector3 color;
    public string type;

    public Primitive(Vector3 color, string type)
    {
        this.color = color;
        this.type = type;
    }

    /*public virtual Intersection Intersect(Ray ray)
    {
        return null;
    }*/
}
