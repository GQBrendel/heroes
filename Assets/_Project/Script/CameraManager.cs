using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AStar_2D;
public class CameraManager : MonoBehaviour {

    private float camSpeed = 2f;
    Vector3 vecUp = new Vector3(0, 0, 1);
    Vector3 vecDown = new Vector3(0, 0, -1);
    void Update() {
      
        if(Input.GetKey(KeyCode.A))
        {
            transform.position += Vector3.left * camSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * camSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += vecUp * camSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += vecDown * camSpeed * Time.deltaTime;
        }

    }
}
               
