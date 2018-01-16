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
    public GameObject _lengthDisplay;
    public GameObject _leftController;
    public Text _lengthDisplayText;
    public Transform _mainCameraRig;
    public Movement _movementController;
    public VRTK_Pointer _leftControllerPointer;
    public VRTK_StraightPointerRenderer _leftControllerPointerRenderer;

    private Vector2 _leftStickMovement;
    private bool _bTeleportEnabled;
    

    private int _iDefaultPointerLength;

    // Use this for initialization
    void Start()
    {
        _leftControllerPointer.ActivationButtonPressed += PointerTeleport_ActivationButtonPressed;
        _leftControllerPointer.ActivationButtonReleased += PointerTeleport_ActivationButtonReleased;
        _leftControllerPointer.SelectionButtonPressed += PointerTeleport_SelectionButtonPressed;

        _lengthDisplay.transform.position = _leftController.transform.position + _leftController.transform.up * 0.1f;
        _lengthDisplay.SetActive(false);
        _iDefaultPointerLength = 10;
        _leftControllerPointerRenderer.maximumLength = _iDefaultPointerLength;
        _bTeleportEnabled = false;

        if (Util.InGameOptions._movementMode != Util.MovementMode.SicknessPrevention)
            _leftControllerPointerRenderer.enabled = false;
    }

    private void Update()
    {
        if (_bTeleportEnabled && Util.InGameOptions._movementMode == Util.MovementMode.SicknessPrevention)
        {
            _leftStickMovement = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
            UpdatePointerLength();
            UpdatePointerLengthDisplay();
        }
    }



    private void TeleportOnClick()
    {
        if (Util.InGameOptions._bSicknessPrevention_TeleportWithBlink)
        {
            Util.EyeBlink.Blink();
            _mainCameraRig.position = _leftControllerPointerRenderer.actualCursor.transform.position;
        }
        else
        {
            _mainCameraRig.position = _leftControllerPointerRenderer.actualCursor.transform.position;
        }
    }

    private void UpdatePointerLength()
    {
        if (Mathf.Abs(_leftStickMovement.y) > 0)
            _leftControllerPointerRenderer.maximumLength += _leftStickMovement.y;

        if (_leftControllerPointerRenderer.maximumLength <= 0)
        {
            _leftControllerPointerRenderer.maximumLength = 0;
        }
    }

    private void UpdatePointerLengthDisplay()
    {
        _lengthDisplayText.text = Convert.ToString((int)_leftControllerPointerRenderer.tracerLength);
    }

    private void PointerTeleport_SelectionButtonPressed(object sender, ControllerInteractionEventArgs e)
    {
        if (Util.InGameOptions._movementMode == Util.MovementMode.SicknessPrevention)
        {
            TeleportOnClick();
            _leftControllerPointerRenderer.maximumLength = _iDefaultPointerLength;
        }
    }

    private void PointerTeleport_ActivationButtonReleased(object sender, ControllerInteractionEventArgs e)
    {
        if (Util.InGameOptions._movementMode == Util.MovementMode.SicknessPrevention)
        {
            _movementController._bMovementEnabled = true;
            _bTeleportEnabled = false;
        }
        _leftControllerPointerRenderer.enabled = false;
        _lengthDisplay.SetActive(false);
    }

    private void PointerTeleport_ActivationButtonPressed(object sender, ControllerInteractionEventArgs e)
    {
        if (Util.InGameOptions._movementMode == Util.MovementMode.SicknessPrevention)
        {
            _movementController._bMovementEnabled = false;
            _bTeleportEnabled = true;
            _lengthDisplay.SetActive(true);
            _leftControllerPointerRenderer.enabled = true;
        }
    }
}
