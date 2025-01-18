using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMover : MonoBehaviour
{
    public float speed = 0.1f;
    public Camera cam;

    private float screenWidthInWorldUnits;
    private float cloudWidth;

    private void Start()
    {
        cloudWidth = GetComponent<SpriteRenderer>().bounds.size.x;
        Vector3 screenEdge = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.nearClipPlane));
        screenWidthInWorldUnits = screenEdge.x;
        float randomX = Random.Range(-screenWidthInWorldUnits, screenWidthInWorldUnits);
        transform.position = new Vector3(randomX, transform.position.y, transform.position.z);
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x + speed, transform.position.y, transform.position.z);

        if (transform.position.x - (cloudWidth/2) > screenWidthInWorldUnits)
        {
            transform.position = new Vector3(-screenWidthInWorldUnits- (cloudWidth / 2), transform.position.y, transform.position.z);
        }
    }
}