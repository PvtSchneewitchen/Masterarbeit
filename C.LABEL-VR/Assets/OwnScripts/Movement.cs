using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class Movement : MonoBehaviour
{
    //public bool _freeFlyFast;
    //public bool _freeFlySlow;
    //public bool _sicknessPreventionMode;
    public Util.MovementMode _movementMode { get; set; }
    public bool _bMovementEnabled { get; set; }
    public float _fMovementDistance { get; set; }

    private float _maxSpeedTrans;
    private float _actualSpeedTransHorizontal;
    private float _actualSpeedTransVertical;
    private float _accelerationTrans;
    private float _decelerationTrans;
    private float _maxAngleRot;
    private float _lastRotationAngle;
    private float _actualSpeedRot;
    private float _accelerationRot;
    private float _decelerationRot;
    private float _stickActivationBoarderFly;
    private float _stickActivationBoarderSPM;
    private Vector3 _cameraGaze;
    private Vector3 _lastTranslation;
    private Vector3 _lastTranslationHorizontal;
    private Vector3 _lastTranslationVertical;
    private Vector2 _leftStickMovement;
    private Vector2 _rightStickMovement;
    private GameObject _mainCameraRig;
    private GameObject _centerEyeAnchor;


    void Start()
    {
        //adjustable values
        _maxSpeedTrans = 0.009f;
        _accelerationTrans = 0.005f;
        _decelerationTrans = _accelerationTrans * 2;
        _maxAngleRot = 0.1f;
        _accelerationRot = 0.02f;
        _decelerationRot = _accelerationRot * 4;
        _stickActivationBoarderFly = 0.5f;
        _stickActivationBoarderSPM = 0.8f;

        //init values
        _fMovementDistance = float.Parse(Util.FindInactiveGameobject(GameObject.Find("InGameOptions"), "InputField_MovementDistance").GetComponent<InputField>().text);
        _bMovementEnabled = true;
        _movementMode = Util.MovementMode.FreeFlySlow;
        _actualSpeedTransHorizontal = 0.0f;
        _actualSpeedTransVertical = 0.0f;
        _actualSpeedRot = 0.0f;
        _mainCameraRig = Util.FindInactiveGameobject(GameObject.Find("SDKSetups"), "OVRCameraRig");
        _centerEyeAnchor = Util.FindInactiveGameobject(GameObject.Find("SDKSetups"), "CenterEyeAnchor");
        print(_fMovementDistance);
    }

    void Update()
    {
        if (_bMovementEnabled)
        {
            UpdateValues();

            if (_movementMode == Util.MovementMode.FreeFlySlow || _movementMode == Util.MovementMode.FreeFlyFast)
            {
                UpdateCameraPosition();
                UpdateCameraRotation();
            }
            else if(_movementMode == Util.MovementMode.SicknessPrevention)
            {
                if(_leftStickMovement.x > _stickActivationBoarderSPM)
                {
                    //right
                }
                else if (_leftStickMovement.x < -_stickActivationBoarderSPM)
                {
                    //left
                }
                else if (_leftStickMovement.y > _stickActivationBoarderSPM)
                {
                    //forwards
                }
                else if (_leftStickMovement.y < -_stickActivationBoarderSPM)
                {
                    //backwards
                }
            }
        }
    }

    private void UpdateValues()
    {
        _cameraGaze = _centerEyeAnchor.transform.forward;
        _leftStickMovement = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        _rightStickMovement = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
    }

    private void UpdateCameraRotation()
    {
        float rotationAngle = 0;

        //rotation movement
        if (Mathf.Abs(_rightStickMovement.x) > _stickActivationBoarderFly)
            rotationAngle = _rightStickMovement.x;

        //rotation smoothing
        if (rotationAngle != 0)
        {
            if (_actualSpeedRot < _maxAngleRot)
                _actualSpeedRot += _accelerationRot * Time.deltaTime;
            else
                _actualSpeedRot = _maxAngleRot;

            _lastRotationAngle = rotationAngle;
        }
        else
        {
            if (_actualSpeedRot > 0)
                _actualSpeedRot -= _decelerationRot * Time.deltaTime;
            else
                _actualSpeedRot = 0;
        }

        if (_movementMode == Util.MovementMode.FreeFlySlow)
            _mainCameraRig.GetComponent<OVRCameraRig>().transform.Rotate(Vector3.up, _lastRotationAngle * _actualSpeedRot);
        else if (_movementMode == Util.MovementMode.FreeFlyFast)
            _mainCameraRig.GetComponent<OVRCameraRig>().transform.Rotate(Vector3.up, rotationAngle);
    }

    private void UpdateCameraPosition()
    {
        Vector3 translation = Vector3.zero;
        Vector3 translationHorizontal = Vector3.zero;
        Vector3 translationVertical = Vector3.zero;

        //horizontal movement
        if (Mathf.Abs(_leftStickMovement.x) > _stickActivationBoarderFly)
            translationHorizontal.x = _leftStickMovement.x;
        if (Mathf.Abs(_leftStickMovement.y) > _stickActivationBoarderFly)
            translationHorizontal.z = _leftStickMovement.y;

        //horizontal smoothing
        if (translationHorizontal != Vector3.zero)
        {
            if (_actualSpeedTransHorizontal < _maxSpeedTrans)
                _actualSpeedTransHorizontal += _accelerationTrans * Time.deltaTime;
            else
                _actualSpeedTransHorizontal = _maxSpeedTrans;

            _lastTranslationHorizontal = translationHorizontal;
        }
        else
        {
            if (_actualSpeedTransHorizontal > 0)
                _actualSpeedTransHorizontal -= _decelerationTrans * Time.deltaTime;
            else
                _actualSpeedTransHorizontal = 0;
        }

        //vertical movement
        if (Mathf.Abs(_rightStickMovement.y) > _stickActivationBoarderFly)
            translationVertical.y = _rightStickMovement.y;

        //vertical smoothing
        if (translationVertical != Vector3.zero)
        {
            if (_actualSpeedTransVertical < _maxSpeedTrans)
                _actualSpeedTransVertical += _accelerationTrans * Time.deltaTime;
            else
                _actualSpeedTransVertical = _maxSpeedTrans;

            _lastTranslationVertical = translationVertical;
        }
        else
        {
            if (_actualSpeedTransVertical > 0)
                _actualSpeedTransVertical -= _decelerationTrans * Time.deltaTime;
            else
                _actualSpeedTransVertical = 0;
        }

        if (_movementMode == Util.MovementMode.FreeFlySlow)
            translation = _lastTranslationHorizontal * _actualSpeedTransHorizontal + _lastTranslationVertical * _actualSpeedTransVertical;
        else if (_movementMode == Util.MovementMode.FreeFlyFast)
            translation = translationHorizontal + translationVertical;

        _mainCameraRig.GetComponent<OVRCameraRig>().transform.Translate(translation);
    }
}
