using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Scene
{
    public List<Primitive> primitives = new List<Primitive>();
    List<Light> lights = new List<Light>();

    public Scene()
    {
        primitives.Add(new Sphere(new OpenTK.Vector3(0, 0, 5), new OpenTK.Vector3(200, 0, 100), 3));
    }
}
