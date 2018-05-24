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

    public void moveCamera(Camera c, ref int moveFactor)
    {
        var keyboard = OpenTK.Input.Keyboard.GetState();
        Vector3 currPos = c.position;
        if (keyboard[OpenTK.Input.Key.W]) c.position += c.direction / 10;//forward
        if (keyboard[OpenTK.Input.Key.A]) c.position += new Vector3(-c.direction.Z, 0, c.direction.X).Normalized() / 10;//left (not based on dir yet)
        if (keyboard[OpenTK.Input.Key.S]) c.position -= c.direction / 10; ;//backward
        if (keyboard[OpenTK.Input.Key.D]) c.position += new Vector3(c.direction.Z, 0, -c.direction.X).Normalized() / 10;//right (not based on dir yet)
        if (keyboard[OpenTK.Input.Key.Space]) c.position += new Vector3(0, 0.1f, 0);//up
        if (keyboard[OpenTK.Input.Key.ShiftLeft]) c.position += new Vector3(0, -0.1f, 0);//down
        if (c.position != currPos) adjustScreen(c, ref moveFactor);
    }

    public void rotateCamera(Camera c, ref int moveFactor)
    {
        var keyboard = OpenTK.Input.Keyboard.GetState();
        Vector3 currDir = c.direction;
        Vector3 rotateNormal = Vector3.Zero;
        if (keyboard[OpenTK.Input.Key.Right]) rotateNormal = new Vector3(c.direction.Z, 0, -c.direction.X).Normalized() / 10; //right
        if (keyboard[OpenTK.Input.Key.Left]) rotateNormal = new Vector3(-c.direction.Z, 0, c.direction.X).Normalized() / 10; //left

        if (rotateNormal != Vector3.Zero) c.direction = (c.direction + rotateNormal).Normalized();
        if (c.direction != currDir) adjustScreen(c, ref moveFactor);
    }

    public void adjustScreen(Camera c, ref int moveFactor)
    {
        c.screenCenter = c.position + c.direction;
        c.Screen();
        moveFactor = 2;
    }


}
