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
    public float specularity;
    public bool isTextured;

    public Primitive(Vector3 color, string type, float specularity, bool isTextured)
    {
        this.color = color;
        this.type = type;
        this.specularity = specularity;
        this.isTextured = isTextured;
    }

    /*public virtual Intersection Intersect(Ray ray)
    {
        return null;
    }*/
}
