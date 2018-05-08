using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Raytracer
{
    Ray ray;
    Intersection intersection;

    public void Render(Camera c, Surface screen, Scene scene)
    {
        for (int x = 0; x < screen.width; x++)
            for (int y = 0; y < screen.height; y++)
            {
                ray = c.ShootRay(new Vector2(x / screen.width, y / screen.height));
                intersection = new Intersection(ray, scene);
                screen.Plot(x, y, intersection.color);
            }


        /*foreach (Ray ray in rays)
            intersections.Add(new Intersection(ray, scene));

        for (int x = 0; x < screen.width; x++)
            for (int y = 0; y < screen.height; y++)
                screen.Plot()*/
    }
}
