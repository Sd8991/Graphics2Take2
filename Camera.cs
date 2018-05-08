using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Camera
{
    public Vector3 position;
    public Vector3 direction;
    public Vector3 screenCenter;
    public Vector3 leftUpperCorner, rightUpperCorner, leftLowerCorner;
    public Vector3 screenHeight, screenWidth;
    public Vector3 point;

    public Camera(Vector3 position, Vector3 direction)
    {
        this.position = position;
        this.direction = direction;
        screenCenter = position + direction;
    }

    public void Screen()
    {
        leftUpperCorner = screenCenter + new Vector3(-1, -1, 0);
        rightUpperCorner = screenCenter + new Vector3(1, -1, 0);
        leftLowerCorner = screenCenter + new Vector3(-1, 1, 0);
        screenHeight = leftLowerCorner- leftUpperCorner;
        screenWidth = rightUpperCorner - leftUpperCorner;
    }

    public Ray ShootRay(Vector2 point)
    {
        this.point = leftUpperCorner + point.X * screenWidth + point.Y * (screenHeight);
        Vector3 rayDir = (this.point - position).Normalized();
        return new Ray(position, rayDir);
    } 
}
