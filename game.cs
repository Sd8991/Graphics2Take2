using OpenTK;
using System;
using System.IO;

class Game
{
    // member variables
    public Surface screen;
    public Raytracer rayTracer;
    public static Camera c;
    public Scene s;
    // initialize
    public void Init()
    {
        rayTracer = new Raytracer();
        c = new Camera(Vector3.Zero, new Vector3(0,0,1), 90);
        c.Screen();
        s = new Scene();
    }
    // tick: renders one frame
    public void Tick()
    {
        screen.Clear(0);
        rayTracer.Render(c, screen, s);
    }
}