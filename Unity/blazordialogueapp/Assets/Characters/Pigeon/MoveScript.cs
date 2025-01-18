using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MoveScript : MonoBehaviour
{
    public float speed = 0.1f;
    public Camera cam;
    private float LeftEdge;
    private float RightEdge;
    private bool MovingRight=true;
    private int RandomNumber;
    public Animator MoveAnimation;
    private Vector3 screenEdge;
    


    private void Start()
    {
        Vector3 screenEdge = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.nearClipPlane));
        LeftEdge = -screenEdge.x;
        RightEdge = screenEdge.x;
        

    }

    private void Update()
    {
        if (MovingRight)
        {
            transform.position = new Vector3(transform.position.x + speed, transform.position.y,transform.position.z);
            if (transform.position.x > RightEdge)
            {
                float NewRightEdge = Random.Range(0,8f);
                RightEdge = NewRightEdge;
                transform.rotation = Quaternion.Euler(0, 180, 0);
                MovingRight = false;
            }
            
        }
        else
        {
            transform.position = new Vector3(transform.position.x - speed, transform.position.y, transform.position.z);
            if (transform.position.x < LeftEdge)
            {
                float NewLeftEdge = Random.Range(0, -8f);
                LeftEdge = NewLeftEdge;
                transform.rotation = Quaternion.Euler(0, 0, 0);
                MovingRight = true;
            }
        }
    }
}
