using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Direction { X, Y, Z, XY, XZ, YZ, XYZ }
public class RotationObject : MonoBehaviour
{
    public Direction direction;
    public float speed = 20f;
    public bool invertDirection;


    private void Update()
    {
        Vector3 rotDirection = Vector3.zero;
        switch (direction)
        {
            case Direction.X:
                rotDirection = Vector3.right;
                break;

            case Direction.Y:
                rotDirection = Vector3.up;
                break;

            case Direction.Z:
                rotDirection = Vector3.forward;
                break;

            case Direction.XY:
                rotDirection = new Vector3(1, 1, 0);
                break;

            case Direction.XZ:
                rotDirection = new Vector3(1, 0, 1);
                break;

            case Direction.YZ:
                rotDirection = new Vector3(0, 1, 1);
                break;

            case Direction.XYZ:
                rotDirection = new Vector3(1, 1, 1);
                break;
        }

        transform.Rotate(rotDirection * speed * Time.deltaTime * (invertDirection ? 1 : -1), Space.World);
    }
}
