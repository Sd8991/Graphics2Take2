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
    Application a;
    int moveFactor = 1;

    // initialize
    public void Init() //if camera is set at a point other than the origin, you first have to move the camera to get the right view.
    {
        rayTracer = new Raytracer();
        a = new Application();
        c = new Camera(new Vector3(0,0,0) , new Vector3(0,0,1), 90); //set FOV here
        c.direction = c.ProcessTarget(c.position + new Vector3(0, 0, 1)); //set target here
        c.Screen();
        s = new Scene();
    }
    // tick: renders one frame
    public void Tick()
    {
        screen.Clear(0);
        moveFactor = 1;
        a.moveCamera(c, ref moveFactor);
        a.rotateCamera(c, ref moveFactor);
        rayTracer.Render(c, screen, s, moveFactor);
    }
}