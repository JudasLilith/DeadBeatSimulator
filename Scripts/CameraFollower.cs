using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public Transform target;
    //public float minDistance = 3;
    //public float followSpeed = 10;
    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(target.position.x, target.position.y, -10);
        //float distance = Vector2.Distance(transform.position, target.position);
        
        //if (distance > minDistance)
        //{
        //    transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x, target.position.y, -10), followSpeed * Time.deltaTime);
        //}
    }
}
