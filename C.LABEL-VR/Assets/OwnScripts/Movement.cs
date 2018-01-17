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

    private int _iStickShiftSemaphor;
    private float _fActualSpeedTransLateral;
    private float _fActualSpeedTransLongitudinal;
    private float _fActualSpeedTransVertical;
    private float _fActualSpeedRot;
    private float _fLastLateralGain;
    private float _fMaxLateralGain;
    private float _fLastLongitudinalGain;
    private float _fMaxLongitudinalGain;
    private float _fLastVerticalGain;
    private float _fMaxVerticalGain;
    private float _fLastRotationalGain;
    private float _fMaxRotationalGain;
    private float _fLateralGain;
    private float _fLongitudinalGain;
    private float _fRotationalGain;
    private float _fVerticalGain;
    private float _fDeceleration;
    private float _fMaxSpeed;
    private float _fMaxSpeedRotation;

    private Vector2 _rightStickMovement;
    private Vector2 _leftStickMovement;

    void Start()
    {
        print(Application.persistentDataPath);
        //init values
        _iStickShiftSemaphor = 0;
        _bMovementEnabled = true;
    }

    void Update()
    {
        if (_bMovementEnabled)
        {
            UpdateValues();

            if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow || Util.InGameOptions._movementMode == Util.MovementMode.FreeFlyFast)
            {
                UpdateCameraPositionAndRotation_FreeFly();
            }
            else if (Util.InGameOptions._movementMode == Util.MovementMode.SicknessPrevention)
            {
                UpdateMovement_SicknessPrevention();
            }
        }
    }

    public void Stop()
    {
        _fActualSpeedTransLateral = 0.0f;
        _fActualSpeedTransLongitudinal = 0.0f;
        _fActualSpeedTransVertical = 0.0f;
        _fActualSpeedRot = 0.0f;
        _fMaxLateralGain = 0.0f;
        _fMaxLongitudinalGain = 0.0f;
        _fMaxVerticalGain = 0.0f;
        _fMaxRotationalGain = 0.0f;
    }

    private void UpdateValues()
    {
        _leftStickMovement = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        _rightStickMovement = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        if (_leftStickMovement == Vector2.zero && _rightStickMovement == Vector2.zero)
        {
            if (_iStickShiftSemaphor == 2)
                _iStickShiftSemaphor = 0;
        }
        else
        {
            if (_iStickShiftSemaphor == 0)
                _iStickShiftSemaphor++;
        }
    }

    private void UpdateCameraPositionAndRotation_FreeFly()
    {
        float longitudinal = 0.0f;
        float lateral = 0.0f;
        float vertical = 0.0f;
        float rotation = 0.0f;

        lateral = (Mathf.Abs(_leftStickMovement.x) > InternValues._stickActivationBoarderFly) ? _leftStickMovement.x : 0;
        longitudinal = (Mathf.Abs(_leftStickMovement.y) > InternValues._stickActivationBoarderFly) ? _leftStickMovement.y : 0;
        vertical = (Mathf.Abs(_rightStickMovement.y) > InternValues._stickActivationBoarderFly) ? _rightStickMovement.y : 0;
        rotation = (Mathf.Abs(_rightStickMovement.x) > InternValues._stickActivationBoarderFly) ? _rightStickMovement.x : 0;

        if (Util.InGameOptions._movementMode == Util.MovementMode.FreeFlySlow)
        {
            _fLateralGain = lateral * InternValues._fAccelerationSlow * Time.deltaTime;
            _fLongitudinalGain = longitudinal * InternValues._fAccelerationSlow * Time.deltaTime;
            _fVerticalGain = vertical * InternValues._fAccelerationSlow * Time.deltaTime;
            _fRotationalGain = rotation * InternValues._fAccelerationSlow * InternValues._fRotationFactor * Time.deltaTime;
            _fMaxSpeed = InternValues._fMaxSpeedSlow;
            _fMaxSpeedRotation = _fMaxSpeed * InternValues._fRotationFactor / 2;
            _fDeceleration = InternValues._fDecelerationFactorSlow * Time.deltaTime;
        }
        else
        {
            _fLateralGain = lateral * InternValues._fAccelerationFast * Time.deltaTime;
            _fLongitudinalGain = longitudinal * InternValues._fAccelerationFast * Time.deltaTime;
            _fVerticalGain = vertical * InternValues._fAccelerationFast * Time.deltaTime;
            _fRotationalGain = rotation * InternValues._fAccelerationFast * InternValues._fRotationFactor * Time.deltaTime;
            _fMaxSpeed = InternValues._fMaxSpeedFast;
            _fMaxSpeedRotation = _fMaxSpeed * InternValues._fRotationFactor / 2;
            _fDeceleration = InternValues._fDecelerationFactorFast * Time.deltaTime;
        }

        SmoothLateralSpeed();
        SmoothLongitudinalSpeed();
        SmoothVerticalSpeed();
        SmoothRotationalSpeed();

        Vector3 translation = new Vector3(_fActualSpeedTransLateral, _fActualSpeedTransVertical, _fActualSpeedTransLongitudinal);
        _cameraRig.Translate(translation);
        _cameraRig.Rotate(Vector3.up, _fActualSpeedRot);
    }

    private void SmoothRotationalSpeed()
    {
        if (_fRotationalGain != 0)
        {
            if ((_fActualSpeedRot > 0 && _fRotationalGain < 0) || (_fActualSpeedRot < 0 && _fRotationalGain > 0))
                _fActualSpeedRot += 2 * _fRotationalGain * _fDeceleration;
            else
                _fActualSpeedRot += _fRotationalGain;

            if (Mathf.Abs(_fActualSpeedRot) >= _fMaxSpeedRotation)
                _fActualSpeedRot = (_fRotationalGain > 0) ? _fMaxSpeedRotation : -_fMaxSpeedRotation;

            _fLastRotationalGain = _fRotationalGain;
        }
        else
        {
            if (Mathf.Abs(_fActualSpeedRot) > _fMaxRotationalGain)
                _fActualSpeedRot -= (_fActualSpeedRot > 0) ? Mathf.Abs(_fLastRotationalGain) * _fDeceleration : -Mathf.Abs(_fLastRotationalGain) * _fDeceleration;
            else
                _fActualSpeedRot = 0;
        }

        print("ROTATION speed: " + _fActualSpeedRot + "   maxGain: " + _fMaxRotationalGain + "    actualGain: " + _fRotationalGain);

        _fMaxRotationalGain = (Mathf.Abs(_fLastRotationalGain) * _fDeceleration > Mathf.Abs(_fMaxRotationalGain)) ? Mathf.Abs(_fLastRotationalGain) * _fDeceleration : Mathf.Abs(_fMaxRotationalGain);
    }

    private void SmoothLateralSpeed()
    {
        if (_fLateralGain != 0)
        {
            if ((_fActualSpeedTransLateral > 0 && _fLateralGain < 0) || (_fActualSpeedTransLateral < 0 && _fLateralGain > 0))
                _fActualSpeedTransLateral += 2 * _fLateralGain * _fDeceleration;
            else
                _fActualSpeedTransLateral += _fLateralGain;

            if (Mathf.Abs(_fActualSpeedTransLateral) >= _fMaxSpeed)
                _fActualSpeedTransLateral = (_fLateralGain > 0) ? _fMaxSpeed : -_fMaxSpeed;

            _fLastLateralGain = _fLateralGain;
        }
        else
        {
            if (Mathf.Abs(_fActualSpeedTransLateral) > _fMaxLateralGain)
                _fActualSpeedTransLateral -= (_fActualSpeedTransLateral > 0) ? Mathf.Abs(_fLastLateralGain) * _fDeceleration : -Mathf.Abs(_fLastLateralGain) * _fDeceleration;
            else
                _fActualSpeedTransLateral = 0;
        }

        print("LATERAL speed: " + _fActualSpeedTransLateral + "   maxGain: " + _fMaxLateralGain + "    actualGain: " + _fLateralGain);

        _fMaxLateralGain = (Mathf.Abs(_fLastLateralGain) * _fDeceleration > Mathf.Abs(_fMaxLateralGain)) ? Mathf.Abs(_fLastLateralGain) * _fDeceleration : Mathf.Abs(_fMaxLateralGain);
    }

    private void SmoothLongitudinalSpeed()
    {
        if (_fLongitudinalGain != 0)
        {
            if ((_fActualSpeedTransLongitudinal > 0 && _fLongitudinalGain < 0) || (_fActualSpeedTransLongitudinal < 0 && _fLongitudinalGain > 0))
                _fActualSpeedTransLongitudinal += 2 * _fLongitudinalGain * _fDeceleration;
            else
                _fActualSpeedTransLongitudinal += _fLongitudinalGain;

            if (Mathf.Abs(_fActualSpeedTransLongitudinal) >= _fMaxSpeed)
                _fActualSpeedTransLongitudinal = (_fLongitudinalGain > 0) ? _fMaxSpeed : -_fMaxSpeed;

            _fLastLongitudinalGain = _fLongitudinalGain;
        }
        else
        {
            if (Mathf.Abs(_fActualSpeedTransLongitudinal) > _fMaxLongitudinalGain)
                _fActualSpeedTransLongitudinal -= (_fActualSpeedTransLongitudinal > 0) ? Mathf.Abs(_fLastLongitudinalGain) * _fDeceleration : -Mathf.Abs(_fLastLongitudinalGain) * _fDeceleration;
            else
                _fActualSpeedTransLongitudinal = 0;
        }

        print("LONGITUDINAL speed: " + _fActualSpeedTransLongitudinal + "   maxGain: " + _fMaxLongitudinalGain + "    actualGain: " + _fLongitudinalGain);

        _fMaxLongitudinalGain = (Mathf.Abs(_fLastLongitudinalGain) * _fDeceleration > Mathf.Abs(_fMaxLongitudinalGain)) ? Mathf.Abs(_fLastLongitudinalGain) * _fDeceleration : Mathf.Abs(_fMaxLongitudinalGain);
    }

    private void SmoothVerticalSpeed()
    {
        if (_fVerticalGain != 0)
        {
            if ((_fActualSpeedTransVertical > 0 && _fVerticalGain < 0) || (_fActualSpeedTransVertical < 0 && _fVerticalGain > 0))
                _fActualSpeedTransVertical += 2 * _fVerticalGain * _fDeceleration;
            else
                _fActualSpeedTransVertical += _fVerticalGain;

            if (Mathf.Abs(_fActualSpeedTransVertical) >= _fMaxSpeed)
                _fActualSpeedTransVertical = (_fVerticalGain > 0) ? _fMaxSpeed : -_fMaxSpeed;

            _fLastVerticalGain = _fVerticalGain;
        }
        else
        {
            if (Mathf.Abs(_fActualSpeedTransVertical) > _fMaxVerticalGain)
                _fActualSpeedTransVertical -= (_fActualSpeedTransVertical > 0) ? Mathf.Abs(_fLastVerticalGain) * _fDeceleration : -Mathf.Abs(_fLastVerticalGain) * _fDeceleration;
            else
                _fActualSpeedTransVertical = 0;
        }

        print("VERTICAL speed: " + _fActualSpeedTransVertical + "   maxGain: " + _fMaxVerticalGain + "    actualGain: " + _fVerticalGain);

        _fMaxVerticalGain = (Mathf.Abs(_fLastVerticalGain) * _fDeceleration > Mathf.Abs(_fMaxVerticalGain)) ? Mathf.Abs(_fLastVerticalGain) * _fDeceleration : Mathf.Abs(_fMaxVerticalGain);
    }

    private void UpdateMovement_SicknessPrevention()
    {
        if (_iStickShiftSemaphor == 1)
        {
            _iStickShiftSemaphor++;

            //float x = (Mathf.Abs(_leftStickMovement.x) > AdjustableValues._stickActivationBoarderSPM) ? _leftStickMovement.x * Util.InGameOptions._fSicknessPrevention_TeleportDistance / 10 : 0;
            //float y = (Mathf.Abs(_rightStickMovement.y) > AdjustableValues._stickActivationBoarderSPM) ? _rightStickMovement.y * Util.InGameOptions._fSicknessPrevention_TeleportDistance / 10 : 0;
            //float z = (Mathf.Abs(_leftStickMovement.y) > AdjustableValues._stickActivationBoarderSPM) ? _leftStickMovement.y * Util.InGameOptions._fSicknessPrevention_TeleportDistance / 10 : 0;
            //float r = (Mathf.Abs(_rightStickMovement.x) > AdjustableValues._stickActivationBoarderSPM) ? _rightStickMovement.x : 0;
            float x = _leftStickMovement.x * Util.InGameOptions._fSicknessPrevention_TeleportDistance / 10;
            float y = _rightStickMovement.y * Util.InGameOptions._fSicknessPrevention_TeleportDistance / 10;
            float z = _leftStickMovement.y * Util.InGameOptions._fSicknessPrevention_TeleportDistance / 10;
            float r = _rightStickMovement.x;

            if (Util.InGameOptions._bSicknessPrevention_TeleportWithBlink)
                Util.EyeBlink.Blink();

            _cameraRig.Translate(new Vector3(x, y, z));
            _cameraRig.Rotate(_cameraRig.up, r * Util.InGameOptions._fSicknessPrevention_TurnAngle);
        }
    }

    private void QuickTeleportMove(Vector3 direction_inp)
    {
        if (Util.InGameOptions._bSicknessPrevention_TeleportWithBlink)
            Util.EyeBlink.Blink();

        _cameraRig.position += direction_inp * Util.InGameOptions._fSicknessPrevention_TeleportDistance / 10;
    }

    private void QuickTeleportTurn(float angle_inp)
    {
        if (Util.InGameOptions._bSicknessPrevention_TeleportWithBlink)
            Util.EyeBlink.Blink();

        _cameraRig.Rotate(_cameraRig.up, angle_inp);
    }

    public static class InternValues
    {
        public static float _fMaxSpeedSlow { get { return GetMaxSpeedSlow(); } }
        public static float _fAccelerationSlow { get { return GetAccelerationSlow(); } }
        public static float _fMaxSpeedFast { get { return GetMaxSpeedFast(); } }
        public static float _fAccelerationFast { get { return GetAccelerationFast(); } }

        public static float _fDecelerationFactorSlow { get { return 28.0f; } }
        public static float _fDecelerationFactorFast { get { return 50.0f; } }

        public static float _fRotationFactor { get { return 10.0f; } }

        public static float _stickActivationBoarderFly { get { return 0.2f; } }
        public static float _stickActivationBoarderSPM { get { return 0.6f; } }

        static float scaleFactorSlow = 300;
        static float scaleFactorFast = 100;

        private static float GetAccelerationSlow()
        {
            return Util.InGameOptions._fFreeFlySlow_AccelerationFactor / scaleFactorSlow;
        }

        private static float GetMaxSpeedSlow()
        {
            return Util.InGameOptions._fFreeFlySlow_MaxSpeed / scaleFactorSlow;
        }

        private static float GetAccelerationFast()
        {
            return Util.InGameOptions._fFreeFlyFast_AccelerationFactor / (scaleFactorFast * 2f);
        }

        private static float GetMaxSpeedFast()
        {
            return Util.InGameOptions._fFreeFlyFast_MaxSpeed / scaleFactorFast;
        }
    }
}
