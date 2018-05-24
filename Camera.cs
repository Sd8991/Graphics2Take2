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
    public Vector3 Point;
    public float fov;

    public Camera(Vector3 position, Vector3 direction, float fov)
    {
        this.position = position;
        this.direction = direction;
        this.fov = fov;
        screenCenter = position + direction;
    }

    public void Screen()
    {
        float width = ProcessFOV(fov);
        leftUpperCorner = screenCenter + new Vector3(-width * direction.Z, -width, width * direction.X);
        rightUpperCorner = screenCenter + new Vector3(width * direction.Z, -width, -width * direction.X);
        leftLowerCorner = screenCenter + new Vector3(-width * direction.Z, width, width * direction.X);
        screenHeight = leftLowerCorner- leftUpperCorner;
        screenWidth = rightUpperCorner - leftUpperCorner;
    }

    public Ray ShootRay(Vector3 point)
    {
        Point = leftUpperCorner + point.X * screenWidth + point.Y * (screenHeight);
        Vector3 rayDir = (Point - position);
        rayDir.Normalize();
        return new Ray(position, rayDir);
    } 

    public float ProcessFOV(float fov)
    {
        fov = fov / 360 * 2 * (float)Math.PI;
        return (float)Math.Tan(fov / 2);
    }
}
