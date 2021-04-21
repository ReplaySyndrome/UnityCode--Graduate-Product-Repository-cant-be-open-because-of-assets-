using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothFollowCam : MonoBehaviour
{
    public Transform target;
    public float dist = 10.0f;
    public float height = 5.0f;
    public float smoothRoatte = 5.0f;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        float currYAngle = Mathf.LerpAngle(transform.eulerAngles.y, target.eulerAngles.y, smoothRoatte * Time.deltaTime);

        Quaternion rot = Quaternion.Euler(0, currYAngle, 0);

        transform.position = target.position - (rot * Vector3.forward * dist) + Vector3.up * height;
        transform.LookAt(target);
    }
}
