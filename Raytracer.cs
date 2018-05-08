using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Raytracer
{
    public void Render(Camera c, Surface screen)
    {
        for (int x = 0; x < screen.width; x++)
            for (int y = 0; y < screen.height; y++)
                c.ShootRay(new Vector2(x / screen.width, y / screen.height));
    }
}
