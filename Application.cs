using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.Windows.Input;

class Application
{
    public Application() { }

    public void moveCamera(Camera c)
    {
        Vector3 currPos = c.position;
        if (Keyboard.IsKeyDown(Key.W)) c.position += c.direction / 10;//forward
        if (Keyboard.IsKeyDown(Key.A)) c.position += new Vector3(-0.1f, 0, 0);//left (not based on dir yet)
        if (Keyboard.IsKeyDown(Key.S)) c.position -= c.direction / 10; ;//backward
        if (Keyboard.IsKeyDown(Key.D)) c.position += new Vector3(0.1f, 0, 0);//right (not based on dir yet)
        if (Keyboard.IsKeyDown(Key.Space)) c.position += new Vector3(0, 0.1f, 0);//up
        if (Keyboard.IsKeyDown(Key.LeftShift)) c.position += new Vector3(0, -0.1f, 0);//down
    }

    public void rotateCamera(Camera c)
    {

    }


}
