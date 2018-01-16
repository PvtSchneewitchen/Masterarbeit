using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class Movement : MonoBehaviour
{
    public bool _bMovementEnabled { get; set; }
    public Transform _cameraRig;
    public InGameOptionsController _inGameOptions;

    private bool _bLeftStickMoved;
    private bool _bRightStickMoved;
    private float _stickActivationBoarderSPM;
    private float _stickActivationBoarderFly;
    private float _fRotationFactor;
    private float _fActualSpeedTransLateral;
    private float _fActualSpeedTransLongitudinal;
    private float _fActualSpeedTransVertical;
    private float _fActualSpeedRot;

    private float _fLastLateralGain;
    private float _fMaxLateralGain;
    private float _fLastLongitudinalGain;
    private float _fMaxLongitudinalGain;

    private Vector2 _rightStickMovement;
    private Vector2 _leftStickMovement;

    void Start()
    {
        print(Application.persistentDataPath);
        //adjustable values
        _stickActivationBoarderSPM = 0.8f;
        _stickActivationBoarderFly = 0.2f;
        _fRotationFactor = 5.0f;

        //init values
        _bLeftStickMoved = false;
        _bRightStickMoved = false;
        _bMovementEnabled = true;
        _fActualSpeedTransLateral = 0.0f;
        _fActualSpeedTransLongitudinal = 0.0f;
        _fActualSpeedTransVertical = 0.0f;
        _fActualSpeedRot = 0.0f;
        _fLastLateralGain = 0.0f;
        _fMaxLateralGain = 0.0f;
    }

    void Update()
    {
        if (_bMovementEnabled)
        {
            UpdateValues();

            if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow || Util.InGameOptions._movementMode == Util.MovementMode.FreeFlyFast)
            {
                UpdateCameraPosition_FreeFly();
                //UpdateCameraRotation_FreeFly();
            }
            else if (Util.InGameOptions._movementMode == Util.MovementMode.SicknessPrevention)
            {
                //UpdateMovement_SicknessPrevention();
            }
        }
    }

    public static class AdjustableValues
    {
        public static float _fMaxSpeedSlow { get { return GetMaxSpeedSlow(); } }
        public static float _fAccelerationSlow { get { return GetAccelerationSlow(); } }
        public static float _fDecelerationFactorSlow { get { return 1.0f; } }

        public static float _fMaxSpeedFast { get { return GetMaxSpeedFast(); } }
        public static float _fAccelerationFast { get { return GetAccelerationFast(); } }
        public static float _fDecelerationFactorFast { get { return 5.0f; } }

        static float scaleFactorSlow = 300;
        static float scaleFactorFast = 100;

        private static float GetAccelerationSlow()
        {
            return Util.InGameOptions._fFreeFlySlow_AccelerationFactor/ scaleFactorSlow;
        }

        private static float GetMaxSpeedSlow()
        {
            return Util.InGameOptions._fFreeFlySlow_MaxSpeed/ scaleFactorSlow;
        }

        private static float GetAccelerationFast()
        {
            return Util.InGameOptions._fFreeFlyFast_AccelerationFactor / (scaleFactorFast*2f);
        }

        private static float GetMaxSpeedFast()
        {
            return Util.InGameOptions._fFreeFlyFast_MaxSpeed / scaleFactorFast;
        }
    }

    //private void UpdateCameraRotation_FreeFly()
    //{
    //    float rotationAngle = 0.0f;

    //    rotationAngle = _rightStickMovement.x;

    //    SmoothRotationSpeed(rotationAngle);

    //    rotationAngle = (Mathf.Abs(rotationAngle) < 0.001) ? 0 : rotationAngle;
    //    print(rotationAngle);
    //    _cameraRig.Rotate(Vector3.up, _actualSpeedRot);
    //}

    private void UpdateCameraPosition_FreeFly()
    {
        float longitudinal = 0.0f;
        float lateral = 0.0f;
        float vertical = 0.0f;

        lateral = (Mathf.Abs(_leftStickMovement.x) > _stickActivationBoarderFly) ? _leftStickMovement.x : 0;
        longitudinal = (Mathf.Abs(_leftStickMovement.y) > _stickActivationBoarderFly) ? _leftStickMovement.y : 0;
        vertical = (Mathf.Abs(_rightStickMovement.y) > _stickActivationBoarderFly) ? _rightStickMovement.y : 0;

        SmoothLateralSpeed(lateral);
        SmoothLongitudinalSpeed(longitudinal);
        //SmoothVerticalSpeed(vertical);


        //print(_fActualSpeedTransLateral + " " + _fActualSpeedTransVertical + " " + _fActualSpeedTransLongitudinal);
        Vector3 translation = new Vector3(_fActualSpeedTransLateral, _fActualSpeedTransVertical, _fActualSpeedTransLongitudinal);

        _cameraRig.Translate(translation);
    }

    //private void UpdateMovement_SicknessPrevention()
    //{
    //    if (!_bLeftStickMoved)
    //    {
    //        if (_leftStickMovement.x > _stickActivationBoarderSPM)
    //        {
    //            //right
    //            _bLeftStickMoved = true;
    //            QuickTeleportMove(_cameraRig.right);

    //        }
    //        else if (_leftStickMovement.x < -_stickActivationBoarderSPM)
    //        {
    //            //left
    //            _bLeftStickMoved = true;
    //            QuickTeleportMove(-_cameraRig.right);
    //        }
    //        else if (_leftStickMovement.y > _stickActivationBoarderSPM)
    //        {
    //            //forwards
    //            _bLeftStickMoved = true;
    //            QuickTeleportMove(_cameraRig.forward);
    //        }
    //        else if (_leftStickMovement.y < -_stickActivationBoarderSPM)
    //        {
    //            //backwards
    //            _bLeftStickMoved = true;
    //            QuickTeleportMove(-_cameraRig.forward);
    //        }
    //    }
    //    if (!_bRightStickMoved)
    //    {
    //        if (_rightStickMovement.y > _stickActivationBoarderSPM)
    //        {
    //            //up
    //            _bRightStickMoved = true;
    //            QuickTeleportMove(_cameraRig.up);
    //        }
    //        else if (_rightStickMovement.y < -_stickActivationBoarderSPM)
    //        {
    //            //down
    //            _bRightStickMoved = true;
    //            QuickTeleportMove(-_cameraRig.up);
    //        }
    //        else if (_rightStickMovement.x > _stickActivationBoarderSPM)
    //        {
    //            //right turn
    //            _bRightStickMoved = true;
    //            QuickTeleportTurn(Util.InGameOptions._fSicknessPrevention_TurnAngle);
    //        }
    //        else if (_rightStickMovement.x < -_stickActivationBoarderSPM)
    //        {
    //            //left turn
    //            _bRightStickMoved = true;
    //            QuickTeleportTurn(-Util.InGameOptions._fSicknessPrevention_TurnAngle);
    //        }
    //    }
    //    if (_leftStickMovement == Vector2.zero)
    //    {
    //        _bLeftStickMoved = false;
    //    }
    //    if (_rightStickMovement == Vector2.zero)
    //    {
    //        _bRightStickMoved = false;
    //    }
    //}

    //private void QuickTeleportMove(Vector3 direction_inp)
    //{
    //    if (Util.InGameOptions._bSicknessPrevention_TeleportWithBlink)
    //    {
    //        Util.EyeBlink.Blink();
    //        _cameraRig.position += direction_inp * Util.InGameOptions._fSicknessPrevention_TeleportDistance / 10;

    //    }
    //    else
    //    {
    //        _cameraRig.position += direction_inp * Util.InGameOptions._fSicknessPrevention_TeleportDistance / 10;
    //    }
    //}

    //private void QuickTeleportTurn(float angle_inp)
    //{
    //    if (Util.InGameOptions._bSicknessPrevention_TeleportWithBlink)
    //    {
    //        Util.EyeBlink.Blink();
    //        _cameraRig.Rotate(_mainCameraRig.transform.up, angle_inp);
    //    }
    //    else
    //    {
    //        _cameraRig.Rotate(_mainCameraRig.transform.up, angle_inp);
    //    }
    //}

    private void UpdateValues()
    {
        _leftStickMovement = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        _rightStickMovement = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
    }

    //private void SmoothRotationSpeed(float rotationValue_inp)
    //{
    //    if (rotationValue_inp > 0)
    //    {
    //        if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow)
    //        {
    //            if (_actualSpeedRot < _fMaxSpeedSlow * _fRotationFactor)
    //            {
    //                _actualSpeedRot += _fAccelerationSlow * _fRotationFactor * Time.deltaTime;
    //            }
    //            else
    //                _actualSpeedRot = _fMaxSpeedSlow * _fRotationFactor;
    //        }
    //        else if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlyFast)
    //        {
    //            if (_actualSpeedRot < _fMaxSpeedFast * _fRotationFactor)
    //            {
    //                _actualSpeedRot += _fAccelerationFast * _fRotationFactor * Time.deltaTime;
    //            }
    //            else
    //                _actualSpeedRot = _fMaxSpeedFast * _fRotationFactor;
    //        }
    //    }
    //    else if (rotationValue_inp < 0)
    //    {
    //        if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow)
    //        {
    //            if (_actualSpeedRot > -_fMaxSpeedSlow * _fRotationFactor)
    //            {
    //                _actualSpeedRot -= _fAccelerationSlow * _fRotationFactor * Time.deltaTime;
    //            }
    //            else
    //                _actualSpeedRot = -_fMaxSpeedSlow * _fRotationFactor;
    //        }
    //        else if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlyFast)
    //        {
    //            if (_actualSpeedRot > -_fMaxSpeedFast * _fRotationFactor)
    //            {
    //                _actualSpeedRot -= _fAccelerationFast * _fRotationFactor * Time.deltaTime;
    //            }
    //            else
    //                _actualSpeedRot = -_fMaxSpeedFast * _fRotationFactor;
    //        }
    //    }
    //    else
    //    {
    //        if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow)
    //        {
    //            if (_actualSpeedRot > _resetBoarder)
    //            {
    //                _actualSpeedRot -= _fDecelerationSlow / 2 * _fRotationFactor * Time.deltaTime;
    //            }
    //            else if (_actualSpeedRot < -_resetBoarder)
    //            {
    //                _actualSpeedRot += _fDecelerationSlow / 2 * _fRotationFactor * Time.deltaTime;
    //            }
    //            else
    //                _actualSpeedRot = 0;
    //        }
    //        else if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlyFast)
    //        {
    //            if (_actualSpeedRot > _resetBoarder)
    //            {
    //                _actualSpeedRot -= _fDecelerationFast * _fRotationFactor * Time.deltaTime;
    //            }
    //            else if (_actualSpeedRot < -_resetBoarder)
    //            {
    //                _actualSpeedRot += _fDecelerationFast * _fRotationFactor * Time.deltaTime;
    //            }
    //            else
    //                _actualSpeedRot = 0;
    //        }

    //    }
    //}

    private void SmoothLateralSpeed(float lateralValue_inp)
    {
        float flateralGain = 0.0f;
        float fMaxSpeed = 0.0f;

        if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow)
        {
            flateralGain = lateralValue_inp * AdjustableValues._fAccelerationSlow * Time.deltaTime;
            fMaxSpeed = AdjustableValues._fMaxSpeedSlow;
        }
        else
        {
            flateralGain = lateralValue_inp * AdjustableValues._fAccelerationFast * Time.deltaTime;
            fMaxSpeed = AdjustableValues._fMaxSpeedFast;
        }

        if (lateralValue_inp != 0)
        {
            _fActualSpeedTransLateral += flateralGain;

            if (Mathf.Abs(_fActualSpeedTransLateral) >= fMaxSpeed)
                _fActualSpeedTransLateral = (lateralValue_inp > 0) ? fMaxSpeed : -fMaxSpeed;

            _fLastLateralGain = flateralGain;
        }
        else
        {
            if (Mathf.Abs(_fActualSpeedTransLateral) > _fMaxLateralGain)
                _fActualSpeedTransLateral -= (_fActualSpeedTransLateral > 0) ? Math.Abs(_fLastLateralGain) * AdjustableValues._fDecelerationFactorSlow : - Math.Abs(_fLastLateralGain) * AdjustableValues._fDecelerationFactorSlow;
            else
                _fActualSpeedTransLateral = 0;
        }

        print("LATERAL speed: " + _fActualSpeedTransLateral + "   maxGain: " + _fMaxLateralGain + "    actualGain: " + flateralGain);

        _fMaxLateralGain = (Mathf.Abs(flateralGain) > Mathf.Abs(_fMaxLateralGain)) ? Mathf.Abs(flateralGain) : Mathf.Abs(_fMaxLateralGain);
    }

    private void SmoothLongitudinalSpeed(float longitudinalValue_inp)
    {
        float fLongitudinalGain = 0.0f;
        float fMaxSpeed = 0.0f;

        if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow)
        {
            fLongitudinalGain = longitudinalValue_inp * AdjustableValues._fAccelerationSlow * Time.deltaTime;
            fMaxSpeed = AdjustableValues._fMaxSpeedSlow;
        }
        else
        {
            fLongitudinalGain = longitudinalValue_inp * AdjustableValues._fAccelerationFast * Time.deltaTime;
            fMaxSpeed = AdjustableValues._fMaxSpeedFast;
        }

        if (longitudinalValue_inp != 0)
        {
            _fActualSpeedTransLongitudinal += fLongitudinalGain;

            if (Mathf.Abs(_fActualSpeedTransLongitudinal) >= fMaxSpeed)
                _fActualSpeedTransLongitudinal = (longitudinalValue_inp > 0) ? fMaxSpeed : -fMaxSpeed;

            _fLastLongitudinalGain = fLongitudinalGain;
        }
        else
        {
            if (Mathf.Abs(_fActualSpeedTransLongitudinal) > _fMaxLongitudinalGain)
                _fActualSpeedTransLongitudinal -= (_fActualSpeedTransLongitudinal > 0) ? Math.Abs(_fLastLongitudinalGain) * AdjustableValues._fDecelerationFactorSlow : -Math.Abs(_fLastLongitudinalGain) * AdjustableValues._fDecelerationFactorSlow;
            else
                _fActualSpeedTransLongitudinal = 0;
        }

        print("LONGITUDINAL speed: " + _fActualSpeedTransLongitudinal + "   maxGain: " + _fMaxLongitudinalGain + "    actualGain: " + fLongitudinalGain);

        _fMaxLongitudinalGain = (Mathf.Abs(fLongitudinalGain) > Mathf.Abs(_fMaxLongitudinalGain)) ? Mathf.Abs(fLongitudinalGain) : Mathf.Abs(_fMaxLongitudinalGain);
    }

    //private void SmoothLongitudinalSpeed(float longitudinalValue_inp)
    //{
    //    if (longitudinalValue_inp > 0)
    //    {
    //        if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow)
    //        {
    //            if (_fActualSpeedTransLongitudinal < _fMaxSpeedSlow)
    //            {
    //                _fActualSpeedTransLongitudinal += _fAccelerationSlow * Time.deltaTime;
    //            }
    //            else
    //                _fActualSpeedTransLongitudinal = _fMaxSpeedSlow;
    //        }
    //        else if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlyFast)
    //        {
    //            if (_fActualSpeedTransLongitudinal < _fMaxSpeedFast)
    //            {
    //                _fActualSpeedTransLongitudinal += _fAccelerationFast * Time.deltaTime;
    //            }
    //            else
    //                _fActualSpeedTransLongitudinal = _fMaxSpeedFast;
    //        }
    //    }
    //    else if (longitudinalValue_inp < 0)
    //    {
    //        if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow)
    //        {
    //            if (_fActualSpeedTransLongitudinal > -_fMaxSpeedSlow)
    //            {
    //                _fActualSpeedTransLongitudinal -= _fAccelerationSlow * Time.deltaTime;
    //            }
    //            else
    //                _fActualSpeedTransLongitudinal = -_fMaxSpeedSlow;
    //        }
    //        else if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlyFast)
    //        {
    //            if (_fActualSpeedTransLongitudinal > -_fMaxSpeedFast)
    //            {
    //                _fActualSpeedTransLongitudinal -= _fAccelerationFast * Time.deltaTime;
    //            }
    //            else
    //                _fActualSpeedTransLongitudinal = -_fMaxSpeedFast;
    //        }
    //    }
    //    else
    //    {
    //        if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow)
    //        {
    //            if (_fActualSpeedTransLongitudinal > 0)
    //            {
    //                _fActualSpeedTransLongitudinal -= _fDecelerationSlow * Time.deltaTime;
    //            }
    //            else if (_fActualSpeedTransLongitudinal < -0)
    //            {
    //                _fActualSpeedTransLongitudinal += _fDecelerationSlow * Time.deltaTime;
    //            }
    //            else
    //                _fActualSpeedTransLongitudinal = 0;
    //        }
    //        else if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlyFast)
    //        {
    //            if (_fActualSpeedTransLongitudinal > _resetBoarder)
    //            {
    //                _fActualSpeedTransLongitudinal -= _fDecelerationFast * Time.deltaTime;
    //            }
    //            else if (_fActualSpeedTransLongitudinal < -_resetBoarder)
    //            {
    //                _fActualSpeedTransLongitudinal += _fDecelerationSlow * Time.deltaTime;
    //            }
    //            else
    //                _fActualSpeedTransLongitudinal = 0;
    //        }
    //    }
    //}

    //private void SmoothVerticalSpeed(float verticalValue_inp)
    //{
    //    if (verticalValue_inp > 0)
    //    {
    //        if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow)
    //        {
    //            if (_fActualSpeedTransVertical < _fMaxSpeedSlow)
    //            {
    //                _fActualSpeedTransVertical += _fAccelerationSlow * Time.deltaTime;
    //            }
    //            else
    //                _fActualSpeedTransVertical = _fMaxSpeedSlow;
    //        }
    //        else if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlyFast)
    //        {
    //            if (_fActualSpeedTransVertical < _fMaxSpeedFast)
    //            {
    //                _fActualSpeedTransVertical += _fAccelerationFast * Time.deltaTime;
    //            }
    //            else
    //                _fActualSpeedTransVertical = _fMaxSpeedFast;
    //        }
    //    }
    //    else if (verticalValue_inp < 0)
    //    {
    //        if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow)
    //        {
    //            if (_fActualSpeedTransVertical > -_fMaxSpeedSlow)
    //            {
    //                _fActualSpeedTransVertical -= _fAccelerationSlow * Time.deltaTime;
    //            }
    //            else
    //                _fActualSpeedTransVertical = -_fMaxSpeedSlow;
    //        }
    //        else if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlyFast)
    //        {
    //            if (_fActualSpeedTransVertical > -_fMaxSpeedFast)
    //            {
    //                _fActualSpeedTransVertical -= _fAccelerationFast * Time.deltaTime;
    //            }
    //            else
    //                _fActualSpeedTransVertical = -_fMaxSpeedFast;
    //        }
    //    }
    //    else
    //    {
    //        if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow)
    //        {
    //            if (_fActualSpeedTransVertical > _resetBoarder)
    //            {
    //                _fActualSpeedTransVertical -= _fDecelerationSlow * Time.deltaTime;
    //            }
    //            else if (_fActualSpeedTransVertical < -_resetBoarder)
    //            {
    //                _fActualSpeedTransVertical += _fDecelerationSlow * Time.deltaTime;
    //            }
    //            else
    //                _fActualSpeedTransVertical = 0;
    //        }
    //        else if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlyFast)
    //        {
    //            if (_fActualSpeedTransVertical > _resetBoarder)
    //            {
    //                _fActualSpeedTransVertical -= _fDecelerationFast * Time.deltaTime;
    //            }
    //            else if (_fActualSpeedTransVertical < -_resetBoarder)
    //            {
    //                _fActualSpeedTransVertical += _fDecelerationFast * Time.deltaTime;
    //            }
    //            else
    //                _fActualSpeedTransVertical = 0;
    //        }
    //    }
    //}
}



//public class Movement : MonoBehaviour
//{
//    public bool _bMovementEnabled { get; set; }
//    public Transform _cameraRig;

//    private float _fMaxSpeedSlow;
//    private float _fMaxSpeedFast;
//    private float _fAccelerationSlow;
//    private float _fAccelerationFast;
//    private float _fDecelerationFast;
//    private float _fDecelerationSlow;
//    private float _fRotationFactor;

//    private InGameOptionsController options;
//    private bool _bLeftStickMoved;
//    private bool _bRightStickMoved;
//    private float _fActualSpeedTransLateral;
//    private float _fActualSpeedTransLongitudinal;
//    private float _fActualSpeedTransVertical;
//    private float _lastRotation;
//    private float _actualSpeedRot;
//    private float _stickActivationBoarderFly;
//    private float _stickActivationBoarderSPM;
//    private float _resetBoarder;
//    private Vector2 _leftStickMovement;
//    private Vector2 _rightStickMovement;
//    private GameObject _mainCameraRig;
//    private GameObject _centerEyeAnchor;

//    private float _lastLateral;

//    void Start()
//    {
//        options = GameObject.Find("InGameOptions").GetComponent<InGameOptionsController>();
//        print(Application.persistentDataPath);
//        //adjustable values
//        _stickActivationBoarderFly = 0.5f;
//        _stickActivationBoarderSPM = 0.8f;
//        _resetBoarder = 0.1f;
//        _fDecelerationFast = 1.0f;
//        _fDecelerationSlow = 1.0f;
//        _fRotationFactor = 5.0f;

//        //init values
//        _bLeftStickMoved = false;
//        _bRightStickMoved = false;
//        _bMovementEnabled = true;
//        _fActualSpeedTransLateral = 0.0f;
//        _fActualSpeedTransLongitudinal = 0.0f;
//        _fActualSpeedTransVertical = 0.0f;
//        _actualSpeedRot = 0.0f;
//        _mainCameraRig = Util.FindInactiveGameobject(GameObject.Find("SDKSetups"), "OVRCameraRig");
//        _centerEyeAnchor = Util.FindInactiveGameobject(GameObject.Find("SDKSetups"), "CenterEyeAnchor");
//    }

//    void Update()
//    {
//        if (_bMovementEnabled)
//        {
//            UpdateValues();

//            if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow || Util.InGameOptions._movementMode == Util.MovementMode.FreeFlyFast)
//            {
//                UpdateCameraPosition_FreeFly();
//                UpdateCameraRotation_FreeFly();
//            }
//            else if (Util.InGameOptions._movementMode == Util.MovementMode.SicknessPrevention)
//            {
//                UpdateMovement_SicknessPrevention();
//            }
//        }
//    }

//    private void UpdateCameraRotation_FreeFly()
//    {
//        float rotationAngle = 0.0f;

//        if (Mathf.Abs(_rightStickMovement.x) > _stickActivationBoarderFly)
//            rotationAngle = _rightStickMovement.x;

//        SmoothRotationSpeed(rotationAngle);

//        rotationAngle = (Mathf.Abs(rotationAngle) < 0.001) ? 0 : rotationAngle;
//        print(rotationAngle);
//        _cameraRig.Rotate(Vector3.up, _actualSpeedRot);
//    }

//    private void UpdateCameraPosition_FreeFly()
//    {
//        float longitudinal = 0.0f;
//        float lateral = 0.0f;
//        float vertical = 0.0f;

//        if (Mathf.Abs(_leftStickMovement.x) > _stickActivationBoarderFly)
//            lateral = _leftStickMovement.x;
//        if (Mathf.Abs(_leftStickMovement.y) > _stickActivationBoarderFly)
//            longitudinal = _leftStickMovement.y;
//        if (Mathf.Abs(_rightStickMovement.y) > _stickActivationBoarderFly)
//            vertical = _rightStickMovement.y;

//        SmoothLateralSpeed(lateral);
//        SmoothLongitudinalSpeed(longitudinal);
//        SmoothVerticalSpeed(vertical);


//        _fActualSpeedTransLateral = (Mathf.Abs(_fActualSpeedTransLateral) < 0.005) ? 0 : _fActualSpeedTransLateral;
//        _fActualSpeedTransVertical = (Mathf.Abs(_fActualSpeedTransVertical) < 0.005) ? 0 : _fActualSpeedTransVertical;
//        _fActualSpeedTransLongitudinal = (Mathf.Abs(_fActualSpeedTransLongitudinal) < 0.005) ? 0 : _fActualSpeedTransLongitudinal;
//        print(_fActualSpeedTransLateral + " " + _fActualSpeedTransVertical + " " + _fActualSpeedTransLongitudinal);
//        Vector3 translation = new Vector3(_fActualSpeedTransLateral, _fActualSpeedTransVertical, _fActualSpeedTransLongitudinal);

//        _cameraRig.Translate(translation);
//    }

//    private void UpdateMovement_SicknessPrevention()
//    {
//        if (!_bLeftStickMoved)
//        {
//            if (_leftStickMovement.x > _stickActivationBoarderSPM)
//            {
//                //right
//                _bLeftStickMoved = true;
//                QuickTeleportMove(_cameraRig.right);

//            }
//            else if (_leftStickMovement.x < -_stickActivationBoarderSPM)
//            {
//                //left
//                _bLeftStickMoved = true;
//                QuickTeleportMove(-_cameraRig.right);
//            }
//            else if (_leftStickMovement.y > _stickActivationBoarderSPM)
//            {
//                //forwards
//                _bLeftStickMoved = true;
//                QuickTeleportMove(_cameraRig.forward);
//            }
//            else if (_leftStickMovement.y < -_stickActivationBoarderSPM)
//            {
//                //backwards
//                _bLeftStickMoved = true;
//                QuickTeleportMove(-_cameraRig.forward);
//            }
//        }
//        if (!_bRightStickMoved)
//        {
//            if (_rightStickMovement.y > _stickActivationBoarderSPM)
//            {
//                //up
//                _bRightStickMoved = true;
//                QuickTeleportMove(_cameraRig.up);
//            }
//            else if (_rightStickMovement.y < -_stickActivationBoarderSPM)
//            {
//                //down
//                _bRightStickMoved = true;
//                QuickTeleportMove(-_cameraRig.up);
//            }
//            else if (_rightStickMovement.x > _stickActivationBoarderSPM)
//            {
//                //right turn
//                _bRightStickMoved = true;
//                QuickTeleportTurn(Util.InGameOptions._fSicknessPrevention_TurnAngle);
//            }
//            else if (_rightStickMovement.x < -_stickActivationBoarderSPM)
//            {
//                //left turn
//                _bRightStickMoved = true;
//                QuickTeleportTurn(-Util.InGameOptions._fSicknessPrevention_TurnAngle);
//            }
//        }
//        if (_leftStickMovement == Vector2.zero)
//        {
//            _bLeftStickMoved = false;
//        }
//        if (_rightStickMovement == Vector2.zero)
//        {
//            _bRightStickMoved = false;
//        }
//    }

//    private void QuickTeleportMove(Vector3 direction_inp)
//    {
//        if (Util.InGameOptions._bSicknessPrevention_TeleportWithBlink)
//        {
//            Util.EyeBlink.Blink();
//            _cameraRig.position += direction_inp * Util.InGameOptions._fSicknessPrevention_TeleportDistance / 10;

//        }
//        else
//        {
//            _cameraRig.position += direction_inp * Util.InGameOptions._fSicknessPrevention_TeleportDistance / 10;
//        }
//    }

//    private void QuickTeleportTurn(float angle_inp)
//    {
//        if (Util.InGameOptions._bSicknessPrevention_TeleportWithBlink)
//        {
//            Util.EyeBlink.Blink();
//            _cameraRig.Rotate(_mainCameraRig.transform.up, angle_inp);
//        }
//        else
//        {
//            _cameraRig.Rotate(_mainCameraRig.transform.up, angle_inp);
//        }
//    }

//    private void UpdateValues()
//    {
//        _leftStickMovement = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
//        _rightStickMovement = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

//        float normFactorFast = 100.0f;
//        float normFactorSlow = 300.0f;
//        _fMaxSpeedSlow = Util.InGameOptions._fFreeFlySlow_MaxSpeed / normFactorSlow;
//        _fMaxSpeedFast = Util.InGameOptions._fFreeFlyFast_MaxSpeed / normFactorFast;
//        _fAccelerationSlow = Util.InGameOptions._fFreeFlySlow_AccelerationFactor / normFactorSlow;
//        _fAccelerationFast = Util.InGameOptions._fFreeFlyFast_AccelerationFactor / normFactorFast;
//        _fDecelerationFast = _fAccelerationFast*2;
//        _fDecelerationSlow = _fAccelerationSlow*2;
//    }

//    private void SmoothRotationSpeed(float rotationValue_inp)
//    {
//        if (rotationValue_inp > 0)
//        {
//            if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow)
//            {
//                if (_actualSpeedRot < _fMaxSpeedSlow * _fRotationFactor)
//                {
//                    _actualSpeedRot += _fAccelerationSlow * _fRotationFactor * Time.deltaTime;
//                }
//                else
//                    _actualSpeedRot = _fMaxSpeedSlow * _fRotationFactor;
//            }
//            else if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlyFast)
//            {
//                if (_actualSpeedRot < _fMaxSpeedFast * _fRotationFactor)
//                {
//                    _actualSpeedRot += _fAccelerationFast * _fRotationFactor * Time.deltaTime;
//                }
//                else
//                    _actualSpeedRot = _fMaxSpeedFast * _fRotationFactor;
//            }
//        }
//        else if (rotationValue_inp < 0)
//        {
//            if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow)
//            {
//                if (_actualSpeedRot > -_fMaxSpeedSlow * _fRotationFactor)
//                {
//                    _actualSpeedRot -= _fAccelerationSlow * _fRotationFactor * Time.deltaTime;
//                }
//                else
//                    _actualSpeedRot = -_fMaxSpeedSlow * _fRotationFactor;
//            }
//            else if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlyFast)
//            {
//                if (_actualSpeedRot > -_fMaxSpeedFast * _fRotationFactor)
//                {
//                    _actualSpeedRot -= _fAccelerationFast * _fRotationFactor * Time.deltaTime;
//                }
//                else
//                    _actualSpeedRot = -_fMaxSpeedFast * _fRotationFactor;
//            }
//        }
//        else
//        {
//            if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow)
//            {
//                if (_actualSpeedRot > _resetBoarder)
//                {
//                    _actualSpeedRot -= _fDecelerationSlow/2 * _fRotationFactor * Time.deltaTime;
//                }
//                else if (_actualSpeedRot < -_resetBoarder)
//                {
//                    _actualSpeedRot += _fDecelerationSlow/2 * _fRotationFactor * Time.deltaTime;
//                }
//                else
//                    _actualSpeedRot = 0;
//            }
//            else if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlyFast)
//            {
//                if (_actualSpeedRot > _resetBoarder)
//                {
//                    _actualSpeedRot -= _fDecelerationFast * _fRotationFactor * Time.deltaTime;
//                }
//                else if (_actualSpeedRot < -_resetBoarder)
//                {
//                    _actualSpeedRot += _fDecelerationFast * _fRotationFactor * Time.deltaTime;
//                }
//                else
//                    _actualSpeedRot = 0;
//            }

//        }
//    }
//    //private void SmoothLateralSpeed(float lateralValue_inp)
//    //{
//    //    if (lateralValue_inp != 0)
//    //    {
//    //        if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow)
//    //        {
//    //            if (Mathf.Abs(_fActualSpeedTransLateral) < _fMaxSpeedSlow)
//    //            {
//    //                _fActualSpeedTransLateral += lateralValue_inp * _fAccelerationSlow * Time.deltaTime;
//    //            }
//    //            else
//    //                _fActualSpeedTransLateral = lateralValue_inp * _fMaxSpeedSlow;
//    //        }
//    //        else if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlyFast)
//    //        {
//    //            if (Mathf.Abs(_fActualSpeedTransLateral) < _fMaxSpeedFast)
//    //            {
//    //                _fActualSpeedTransLateral += lateralValue_inp * _fAccelerationFast * Time.deltaTime;
//    //            }
//    //            else
//    //                _fActualSpeedTransLateral = lateralValue_inp * _fMaxSpeedFast;
//    //        }
//    //    }
//    //    else
//    //    {
//    //        if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow)
//    //        {
//    //            if (Mathf.Abs(_fActualSpeedTransLateral) > 0)
//    //            {
//    //                _fActualSpeedTransLateral -= _lastLateral *  _fDecelerationSlow * Time.deltaTime;
//    //            }
//    //            else
//    //                _fActualSpeedTransLateral = 0;
//    //        }
//    //        else if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlyFast)
//    //        {
//    //            if (Mathf.Abs(_fActualSpeedTransLateral) > 0)
//    //            {
//    //                _fActualSpeedTransLateral -= _lastLateral * _fDecelerationFast * Time.deltaTime;
//    //            }
//    //            else
//    //                _fActualSpeedTransLateral = 0;
//    //        }
//    //    }

//    //    _lastLateral = lateralValue_inp;
//    //}

//    private void SmoothLateralSpeed(float lateralValue_inp)
//    {
//        if (lateralValue_inp > 0)
//        {
//            if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow)
//            {
//                if (_fActualSpeedTransLateral < _fMaxSpeedSlow)
//                {
//                    _fActualSpeedTransLateral += _fAccelerationSlow * Time.deltaTime;
//                }
//                else
//                    _fActualSpeedTransLateral = _fMaxSpeedSlow;
//            }
//            else if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlyFast)
//            {
//                if (_fActualSpeedTransLateral < _fMaxSpeedFast)
//                {
//                    _fActualSpeedTransLateral += _fAccelerationFast * Time.deltaTime;
//                }
//                else
//                    _fActualSpeedTransLateral = _fMaxSpeedFast;
//            }
//        }
//        else if (lateralValue_inp < 0)
//        {
//            if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow)
//            {
//                if (_fActualSpeedTransLateral > -_fMaxSpeedSlow)
//                {
//                    _fActualSpeedTransLateral -= _fAccelerationSlow * Time.deltaTime;
//                }
//                else
//                    _fActualSpeedTransLateral = -_fMaxSpeedSlow;
//            }
//            else if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlyFast)
//            {
//                if (_fActualSpeedTransLateral > -_fMaxSpeedFast)
//                {
//                    _fActualSpeedTransLateral -= _fAccelerationFast * Time.deltaTime;
//                }
//                else
//                    _fActualSpeedTransLateral = -_fMaxSpeedFast;
//            }
//        }
//        else
//        {
//            if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow)
//            {
//                if (_fActualSpeedTransLateral > 0)
//                {
//                    _fActualSpeedTransLateral -= _fDecelerationSlow * Time.deltaTime;
//                }
//                else if (_fActualSpeedTransLateral < -0)
//                {
//                    _fActualSpeedTransLateral += _fDecelerationSlow * Time.deltaTime;
//                }
//                else
//                    _fActualSpeedTransLateral = 0;
//            }
//            else if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlyFast)
//            {
//                if (_fActualSpeedTransLateral > _resetBoarder)
//                {
//                    _fActualSpeedTransLateral -= _fDecelerationFast * Time.deltaTime;
//                }
//                else if (_fActualSpeedTransLateral < -_resetBoarder)
//                {
//                    _fActualSpeedTransLateral += _fDecelerationSlow * Time.deltaTime;
//                }
//                else
//                    _fActualSpeedTransLateral = 0;
//            }
//        }
//    }

//    private void SmoothLongitudinalSpeed(float longitudinalValue_inp)
//    {
//        if (longitudinalValue_inp > 0)
//        {
//            if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow)
//            {
//                if (_fActualSpeedTransLongitudinal < _fMaxSpeedSlow)
//                {
//                    _fActualSpeedTransLongitudinal += _fAccelerationSlow * Time.deltaTime;
//                }
//                else
//                    _fActualSpeedTransLongitudinal = _fMaxSpeedSlow;
//            }
//            else if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlyFast)
//            {
//                if (_fActualSpeedTransLongitudinal < _fMaxSpeedFast)
//                {
//                    _fActualSpeedTransLongitudinal += _fAccelerationFast * Time.deltaTime;
//                }
//                else
//                    _fActualSpeedTransLongitudinal = _fMaxSpeedFast;
//            }
//        }
//        else if (longitudinalValue_inp < 0)
//        {
//            if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow)
//            {
//                if (_fActualSpeedTransLongitudinal > -_fMaxSpeedSlow)
//                {
//                    _fActualSpeedTransLongitudinal -= _fAccelerationSlow * Time.deltaTime;
//                }
//                else
//                    _fActualSpeedTransLongitudinal = -_fMaxSpeedSlow;
//            }
//            else if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlyFast)
//            {
//                if (_fActualSpeedTransLongitudinal > -_fMaxSpeedFast)
//                {
//                    _fActualSpeedTransLongitudinal -= _fAccelerationFast * Time.deltaTime;
//                }
//                else
//                    _fActualSpeedTransLongitudinal = -_fMaxSpeedFast;
//            }
//        }
//        else
//        {
//            if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow)
//            {
//                if (_fActualSpeedTransLongitudinal > 0)
//                {
//                    _fActualSpeedTransLongitudinal -= _fDecelerationSlow * Time.deltaTime;
//                }
//                else if (_fActualSpeedTransLongitudinal < -0)
//                {
//                    _fActualSpeedTransLongitudinal += _fDecelerationSlow * Time.deltaTime;
//                }
//                else
//                    _fActualSpeedTransLongitudinal = 0;
//            }
//            else if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlyFast)
//            {
//                if (_fActualSpeedTransLongitudinal > _resetBoarder)
//                {
//                    _fActualSpeedTransLongitudinal -= _fDecelerationFast * Time.deltaTime;
//                }
//                else if (_fActualSpeedTransLongitudinal < -_resetBoarder)
//                {
//                    _fActualSpeedTransLongitudinal += _fDecelerationSlow * Time.deltaTime;
//                }
//                else
//                    _fActualSpeedTransLongitudinal = 0;
//            }
//        }
//    }

//    private void SmoothVerticalSpeed(float verticalValue_inp)
//    {
//        if (verticalValue_inp > 0)
//        {
//            if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow)
//            {
//                if (_fActualSpeedTransVertical < _fMaxSpeedSlow)
//                {
//                    _fActualSpeedTransVertical += _fAccelerationSlow * Time.deltaTime;
//                }
//                else
//                    _fActualSpeedTransVertical = _fMaxSpeedSlow;
//            }
//            else if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlyFast)
//            {
//                if (_fActualSpeedTransVertical < _fMaxSpeedFast)
//                {
//                    _fActualSpeedTransVertical += _fAccelerationFast * Time.deltaTime;
//                }
//                else
//                    _fActualSpeedTransVertical = _fMaxSpeedFast;
//            }
//        }
//        else if (verticalValue_inp < 0)
//        {
//            if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow)
//            {
//                if (_fActualSpeedTransVertical > -_fMaxSpeedSlow)
//                {
//                    _fActualSpeedTransVertical -= _fAccelerationSlow * Time.deltaTime;
//                }
//                else
//                    _fActualSpeedTransVertical = -_fMaxSpeedSlow;
//            }
//            else if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlyFast)
//            {
//                if (_fActualSpeedTransVertical > -_fMaxSpeedFast)
//                {
//                    _fActualSpeedTransVertical -= _fAccelerationFast * Time.deltaTime;
//                }
//                else
//                    _fActualSpeedTransVertical = -_fMaxSpeedFast;
//            }
//        }
//        else
//        {
//            if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow)
//            {
//                if (_fActualSpeedTransVertical > _resetBoarder)
//                {
//                    _fActualSpeedTransVertical -= _fDecelerationSlow * Time.deltaTime;
//                }
//                else if (_fActualSpeedTransVertical < -_resetBoarder)
//                {
//                    _fActualSpeedTransVertical += _fDecelerationSlow * Time.deltaTime;
//                }
//                else
//                    _fActualSpeedTransVertical = 0;
//            }
//            else if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlyFast)
//            {
//                if (_fActualSpeedTransVertical > _resetBoarder)
//                {
//                    _fActualSpeedTransVertical -= _fDecelerationFast * Time.deltaTime;
//                }
//                else if (_fActualSpeedTransVertical < -_resetBoarder)
//                {
//                    _fActualSpeedTransVertical += _fDecelerationFast * Time.deltaTime;
//                }
//                else
//                    _fActualSpeedTransVertical = 0;
//            }
//        }
//    }
//}
