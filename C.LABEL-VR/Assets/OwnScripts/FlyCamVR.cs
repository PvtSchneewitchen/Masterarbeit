using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics.SymbolStore;
using UnityEngine.Experimental.UIElements;
using VRTK;

public class FlyCamVR : MonoBehaviour
{
    //TODO Change code style when adaoting this script

    //max speed of camera
    public float speed = 5.0f;
    public float stickSensitivity = 0.25f;
    public bool smoothing = true;
    public float movementAcceleration = 0.1f;
    public float movementDeceleration = 0.1f;
    public float maxCameraSpeed = 2.0f;

    //private Vector3 lastMousPos = new Vector3(255, 255, 255);
    private float actualSpeed = 0.0f;
    private Vector3 lastDirection;
    private float cameraSpeed;

    private Vector2 leftStickMovement;
    private Vector2 rightStickMovement;

    // Use this for initialization
    void Start()
    {
        

        if (GameObject.Find("LeftController").GetComponent<VRTK_ControllerEvents>() == null)
        {
            VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_ControllerEvents_ListenerExample", "VRTK_ControllerEvents", "the same"));
            return;
        }
        GameObject.Find("LeftController").GetComponent<VRTK_ControllerEvents>().TouchpadAxisChanged += new ControllerInteractionEventHandler(LeftTouchpadAxisChanged);
        GameObject.Find("RightController").GetComponent<VRTK_ControllerEvents>().TouchpadAxisChanged += new ControllerInteractionEventHandler(RightTouchpadAxisChanged);

        cameraSpeed = 1.0f;

        leftStickMovement = Vector2.zero;
        rightStickMovement = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        //GetComponent<OVRCameraRig>().transform.position += new Vector3(leftStickMovement.x/10, 0.0f, leftStickMovement.y/10);
        GetComponent<OVRCameraRig>().transform.Translate(leftStickMovement.x / 10, rightStickMovement.y / 10, leftStickMovement.y / 10);
        //GetComponent<OVRCameraRig>().transform.position += new Vector3(0.0f, rightStickMovement.y/10, 0.0f);
        GetComponent<OVRCameraRig>().transform.eulerAngles += new Vector3(0.0f, rightStickMovement.x*2, 0.0f);

        Debug.Log("left: " + leftStickMovement + "  right: " + rightStickMovement);
        Debug.Log("transform: " + GetComponent<OVRCameraRig>().transform.position);
    }

    private void LeftTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
    {
            leftStickMovement.x = e.touchpadAxis.x;
            leftStickMovement.y = e.touchpadAxis.y;
    }

    private void RightTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
    {

            rightStickMovement.x = e.touchpadAxis.x;
            rightStickMovement.y = e.touchpadAxis.y;
    }
}

