using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class FloatingControl : MonoBehaviour
{
    public Direction direction;
    public float speed = 5;
    public float distance;

    Vector3 initPos;


    private void Start()
    {
        initPos = transform.position;
    }

    private void Update()
    {
        Vector3 moveDir = Vector3.zero;
        switch (direction)
        {
            case Direction.X:
                moveDir = Vector3.right;
                break;

            case Direction.Y:
                moveDir = Vector3.up;
                break;

            case Direction.Z:
                moveDir = Vector3.forward;
                break;

            case Direction.XY:
                moveDir = new Vector3(1, 1, 0);
                break;

            case Direction.XZ:
                moveDir = new Vector3(1, 0, 1);
                break;

            case Direction.YZ:
                moveDir = new Vector3(0, 1, 1);
                break;

            case Direction.XYZ:
                moveDir = new Vector3(1, 1, 1);
                break;
        }

        transform.position = initPos + moveDir * Mathf.Sin(Time.time * speed) * distance;
    }
}
