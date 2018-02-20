using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using PostProcess;
using UnityEngine.UI;

public class PointerTeleport : MonoBehaviour
{
    public bool _bTeleportWithBlink;

    public GameObject _pointerLenghtDisplay;
    public TextMesh _pointerLengthDisplayText;
    public Transform _mainCameraRig;
    public Movement _movementController;
    public VRTK_Pointer _controllerPointer;
    public VRTK_StraightPointerRenderer _controllerPointerRenderer;
    

    
    private bool _bTeleportEnabled;
    private int _iDefaultPointerLength;
    private Vector2 _leftStickMovement;

    // Use this for initialization
    void Start()
    {
        _controllerPointer.ActivationButtonPressed += PointerTeleport_ActivationButtonPressed;
        _controllerPointer.ActivationButtonReleased += PointerTeleport_ActivationButtonReleased;
        _controllerPointer.SelectionButtonPressed += PointerTeleport_SelectionButtonPressed;

        //_pointerLenghtDisplay.transform.position = _leftController.transform.position + _leftController.transform.up * 0.1f;
        _pointerLenghtDisplay.SetActive(false);
        _iDefaultPointerLength = 10;
        _controllerPointerRenderer.maximumLength = _iDefaultPointerLength;
        _bTeleportEnabled = false;

        if (InGameOptions._movementMode != Util.MovementMode.TeleportMode)
            _controllerPointerRenderer.enabled = false;
    }

    private void Update()
    {
        if (_bTeleportEnabled && InGameOptions._movementMode == Util.MovementMode.TeleportMode)
        {
            _leftStickMovement = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
            UpdatePointerLength();
            UpdatePointerLengthDisplay();
        }
    }



    private void TeleportOnClick()
    {
        if (InGameOptions._bSicknessPrevention_TeleportWithBlink)
        {
            Util.EyeBlink.Blink();
            _mainCameraRig.position = _controllerPointerRenderer.actualCursor.transform.position;
        }
        else
        {
            _mainCameraRig.position = _controllerPointerRenderer.actualCursor.transform.position;
        }
    }

    private void UpdatePointerLength()
    {
        if (Mathf.Abs(_leftStickMovement.y) > 0)
            _controllerPointerRenderer.maximumLength += _leftStickMovement.y / 2;

        if (_controllerPointerRenderer.maximumLength <= 0)
        {
            _controllerPointerRenderer.maximumLength = 0;
        }
    }

    private void UpdatePointerLengthDisplay()
    {
        _pointerLengthDisplayText.text = Convert.ToString((int)_controllerPointerRenderer.tracerLength);
    }

    private void PointerTeleport_SelectionButtonPressed(object sender, ControllerInteractionEventArgs e)
    {
        if (InGameOptions._movementMode == Util.MovementMode.TeleportMode)
        {
            TeleportOnClick();
            _controllerPointerRenderer.maximumLength = _iDefaultPointerLength;
        }
    }

    private void PointerTeleport_ActivationButtonReleased(object sender, ControllerInteractionEventArgs e)
    {
        if (InGameOptions._movementMode == Util.MovementMode.TeleportMode)
        {
            _movementController._bMovementEnabled = true;
            _bTeleportEnabled = false;
        }
        _controllerPointerRenderer.enabled = false;
        _pointerLenghtDisplay.SetActive(false);
    }

    private void PointerTeleport_ActivationButtonPressed(object sender, ControllerInteractionEventArgs e)
    {
        if (InGameOptions._movementMode == Util.MovementMode.TeleportMode)
        {
            _movementController._bMovementEnabled = false;
            _bTeleportEnabled = true;
            _pointerLenghtDisplay.SetActive(true);
            _controllerPointerRenderer.enabled = true;
        }
    }
}
