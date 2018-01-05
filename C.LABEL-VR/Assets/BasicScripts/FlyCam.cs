using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics.SymbolStore;
using UnityEngine.Experimental.UIElements;

public class FlyCam : MonoBehaviour
{
    //max speed of camera
    public float speed = 50.0f;
    public bool inverted = true;
    public float mouseSensitivity = 0.25f;
    public bool smoothing = true;
    public float movementAcceleration = 0.1f;
    public float movementDeceleration = 0.1f;
    public float maxCameraSpeed = 2.0f;

    private Vector3 lastMousPos = new Vector3(255, 255, 255);
    private float actualSpeed = 0.0f;
    private Vector3 lastDirection;
    private float cameraSpeed;

    // Use this for initialization
    void Start()
    {
        cameraSpeed = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {

        //mouse look

        lastMousPos = Input.mousePosition - lastMousPos;
        if (!inverted)
            lastMousPos.y = -lastMousPos.y;
        lastMousPos *= mouseSensitivity;
        lastMousPos = new Vector3(this.transform.eulerAngles.x + lastMousPos.y, this.transform.eulerAngles.y + lastMousPos.x, 0);

        if (Input.GetMouseButton((int)MouseButton.RightMouse))
        {
            this.transform.eulerAngles = lastMousPos;
        }
        lastMousPos = Input.mousePosition;




        //camera movement
        Vector3 direction = new Vector3();
        if (Input.GetKey(KeyCode.W))
            direction.z += 1.0f;
        if (Input.GetKey(KeyCode.S))
            direction.z -= 1.0f;
        if (Input.GetKey(KeyCode.A))
            direction.x -= 1.0f;
        if (Input.GetKey(KeyCode.D))
            direction.x += 1.0f;
        direction.Normalize();

        //movement smoothing
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
            cameraSpeed = maxCameraSpeed;
        else
            cameraSpeed = 1.0f;

        if (direction != Vector3.zero)
        {
            //some movement
            if (actualSpeed < cameraSpeed)
                actualSpeed += movementAcceleration * Time.deltaTime * 40;
            else
                actualSpeed = cameraSpeed;

            lastDirection = direction;
        }
        else
        {
            //no movement
            if (actualSpeed > 0)
                actualSpeed -= movementDeceleration * Time.deltaTime * 20;
            else
                actualSpeed = 0;

        }

        if (smoothing)
            this.transform.Translate(lastDirection * actualSpeed * speed * Time.deltaTime);
        else
            this.transform.Translate(direction * speed * Time.deltaTime);
    }
}
