using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class PointerTeleport : MonoBehaviour
{
    public static PointerTeleport Instance { get; private set; }

    public bool PointerTeleportEnabled { get; set; }

    private VRTK_Pointer leftPointer;
    private VRTK_StraightPointerRenderer leftPointerRenderer;
    private bool pointerTeleportActivated = false;
    private int _iDefaultPointerLength = 10;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }


    void Start()
    {
        leftPointer = ReferenceHandler.Instance.GetLeftPointer();
        leftPointerRenderer = ReferenceHandler.Instance.GetLeftPointerRenderer();

        leftPointer.ActivationButtonPressed += PointerTeleport_ActivationButtonPressed;
        leftPointer.ActivationButtonReleased += PointerTeleport_ActivationButtonReleased;
        leftPointer.SelectionButtonPressed += PointerTeleport_SelectionButtonPressed;

        PointerLengthDisplay.Instance.Disable();
        leftPointerRenderer.maximumLength = _iDefaultPointerLength;
        PointerTeleportEnabled = true;

        if (MovementOptions.MoveMode != Util.MovementMode.TeleportMode)
            leftPointerRenderer.enabled = false;
    }

    private void Update()
    {        
        if (PointerTeleportEnabled && pointerTeleportActivated)
        {
            Vector2 leftPointerValue = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
            UpdatePointerLength(leftPointerValue);
            UpdatePointerLengthDisplay();
        }
    }

    private void TeleportOnClick()
    {
        if (MovementOptions.Twinkle)
        {
            Util.EyeBlink.Blink();
            OVRManager.instance.transform.position = leftPointerRenderer.actualCursor.transform.position;
        }
        else
        {
            OVRManager.instance.transform.position = leftPointerRenderer.actualCursor.transform.position;
        }
    }

    private void UpdatePointerLength(Vector2 leftPointerValue)
    {
        if (Mathf.Abs(leftPointerValue.y) > 0)
            leftPointerRenderer.maximumLength += leftPointerValue.y / 2;

        if (leftPointerRenderer.maximumLength <= 0)
        {
            leftPointerRenderer.maximumLength = 0;
        }
    }

    private void UpdatePointerLengthDisplay()
    {
        PointerLengthDisplay.Instance.PointerLength = Convert.ToString((int)leftPointerRenderer.tracerLength);
    }

    private void PointerTeleport_SelectionButtonPressed(object sender, ControllerInteractionEventArgs e)
    {
        if (MovementOptions.MoveMode == Util.MovementMode.TeleportMode && PointerTeleportEnabled)
        {
            TeleportOnClick();
            leftPointerRenderer.maximumLength = _iDefaultPointerLength;
        }
    }

    private void PointerTeleport_ActivationButtonReleased(object sender, ControllerInteractionEventArgs e)
    {
        if (MovementOptions.MoveMode == Util.MovementMode.TeleportMode && PointerTeleportEnabled)
        {
            pointerTeleportActivated = false;
            Movement.Instance.MovementEnabled = true;
            PointerLengthDisplay.Instance.Disable();
        }
        leftPointerRenderer.enabled = false;
    }

    private void PointerTeleport_ActivationButtonPressed(object sender, ControllerInteractionEventArgs e)
    {
        if (MovementOptions.MoveMode == Util.MovementMode.TeleportMode && PointerTeleportEnabled)
        {
            pointerTeleportActivated = true;
            PointerLengthDisplay.Instance.Enable();

            Movement.Instance.MovementEnabled = false;
            

            leftPointerRenderer.enabled = true;
        }
    }
}
