using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;
using VRTK;


/// <summary>
/// This Monobehaviour Class implements the functionality of movement through the scene.
/// Due to the users setting it either calculates the translation and rotation per frame (free fly)
/// or the translation or rotation per stick push (teleport mode)
/// </summary>
public class Movement : MonoBehaviour
{
    public static Movement Instance { get; set; }

    //in some situations (in game option menu is shown) the movement needs to be disabled
    public bool MovementEnabled { get; set; }

    //this semaphor is to check if a stick is shifted or not
    private int _iStickShiftSemaphor;

    private float _fActualSpeedLateral;
    private float _fActualSpeedLongitudinal;
    private float _fActualSpeedVertical;
    private float _fActualSpeedRotational;

    private Vector2 _rightStickMovement;
    private Vector2 _leftStickMovement;

    private Vector3 _lastCameraPosition = Vector3.zero;
    private Vector3 _lastCameraRotation = Vector3.zero;
    private bool _bPointsEnabled = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //init values
        _iStickShiftSemaphor = 0;
        Instance.MovementEnabled = true;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    void Update()
    {
        if (Instance.MovementEnabled)
        {
            UpdateValues();
            if (MovementOptions.MoveMode == Util.MovementMode.FreeFly)
            {
                UpdateCameraPositionAndRotation_FreeFly();

                if (MovementOptions.ReducePoints)
                    DecreasePointsWhenMoving();
            }
            else if (MovementOptions.MoveMode == Util.MovementMode.TeleportMode)
            {
                UpdateMovement_SicknessPrevention();
            }
        }
    }

    public void Stop()
    {
        _fActualSpeedLateral = 0.0f;
        _fActualSpeedLongitudinal = 0.0f;
        _fActualSpeedVertical = 0.0f;
        _fActualSpeedRotational = 0.0f;
    }

    /// <summary>
    /// This function saves the values of the left and right stick
    /// If the user selected teleport move it increases a semaphor which indicates wether a stick is pressed or not
    /// </summary>
    private void UpdateValues()
    {
        _leftStickMovement = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        _rightStickMovement = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        if (MovementOptions.MoveMode == Util.MovementMode.TeleportMode)
        {
            //TEST auf BOARDERS
            if (Mathf.Abs(_leftStickMovement.x) > InternValues._stickActivationBoarderSPM || Mathf.Abs(_leftStickMovement.y) > InternValues._stickActivationBoarderSPM
                || Mathf.Abs(_rightStickMovement.x) > InternValues._stickActivationBoarderSPM || Mathf.Abs(_rightStickMovement.y) > InternValues._stickActivationBoarderSPM)
            {
                if (_iStickShiftSemaphor == 0)
                    _iStickShiftSemaphor++;
            }
            else
            {
                if (_iStickShiftSemaphor == 2)
                    _iStickShiftSemaphor = 0;
            }
        }
    }

    /// <summary>
    /// Calculates the translation and rotation per frame when in free fly mode
    /// </summary>
    private void UpdateCameraPositionAndRotation_FreeFly()
    {
        int directionLongitudinal = 0;
        int directionLateral = 0;
        int directionVertical = 0;
        int directionRotaional = 0;

        //set all the directions either positive or negativ due to the stick direction input

        if (Mathf.Abs(_leftStickMovement.x) > InternValues._stickActivationBoarderFly)
        {
            directionLateral = (_leftStickMovement.x > 0) ? 1 : -1;
        }

        if (Mathf.Abs(_leftStickMovement.y) > InternValues._stickActivationBoarderFly)
        {
            directionLongitudinal = (_leftStickMovement.y > 0) ? 1 : -1;
        }

        if (Mathf.Abs(_rightStickMovement.y) > InternValues._stickActivationBoarderFly)
        {
            directionVertical = (_rightStickMovement.y > 0) ? 1 : -1;
        }

        if (Mathf.Abs(_rightStickMovement.x) > InternValues._stickActivationBoarderFly)
        {
            directionRotaional = (_rightStickMovement.x > 0) ? 1 : -1;
        }

        //compute the speed for each direction
        if (_rightStickMovement != Vector2.zero || _leftStickMovement != Vector2.zero ||
            _fActualSpeedLateral != 0 || _fActualSpeedLongitudinal != 0 ||
            _fActualSpeedVertical != 0 || _fActualSpeedRotational != 0)
        {
            float maxSpeedTrans = InternValues._fMaxSpeedTrans;
            float maxSpeedRot = InternValues._fMaxSpeedRot;
            float accTrans = InternValues._fAccelerationTrans;
            float accRot = InternValues._fAccelerationRot;
            float decTrans = InternValues._fDecelerationTrans;
            float decRot = InternValues._fDecelerationRot;

            ComputeSpeed(ref _fActualSpeedLateral, directionLateral, maxSpeedTrans, accTrans, decTrans);
            ComputeSpeed(ref _fActualSpeedLongitudinal, directionLongitudinal, maxSpeedTrans, accTrans, decTrans);
            ComputeSpeed(ref _fActualSpeedVertical, directionVertical, maxSpeedTrans, accTrans, decTrans);
            ComputeSpeed(ref _fActualSpeedRotational, directionRotaional, maxSpeedRot, accRot, decRot);

            //translate and rotate by the calculatet values
            OVRManager.instance.transform.Translate(new Vector3(_fActualSpeedLateral, _fActualSpeedVertical, _fActualSpeedLongitudinal) * Time.deltaTime);
            OVRManager.instance.transform.Rotate(Vector3.up, _fActualSpeedRotational * Time.deltaTime);
        }
    }

    /// <summary>
    /// Calculates the new speed for this frame due to input parameters
    /// </summary>
    /// <param name="actualSpeed_inp">The reference of the old speed which will be overitten with the new speed</param>
    /// <param name="direction_inp"></param>
    /// <param name="maxSpeed_inp"></param>
    /// <param name="acceleration_inp"></param>
    /// <param name="deceleration_inp"></param>
    private void ComputeSpeed(ref float actualSpeed_inp, int direction_inp, float maxSpeed_inp, float acceleration_inp, float deceleration_inp)
    {
        if (direction_inp > 0)
        {
            if (actualSpeed_inp <= maxSpeed_inp)
            {
                //if the direction is positive and the actual speed is too, add the normal acceleration value
                //if the direction is positive and the actual speed is negative it means that the users wants to change directions so a higher value is added to the speed to provide quick direction change
                if (actualSpeed_inp > 0)
                    actualSpeed_inp += acceleration_inp;
                else
                    actualSpeed_inp += deceleration_inp * 1.3f;
            }
            else
            {
                actualSpeed_inp = maxSpeed_inp;
            }
        }
        else if (direction_inp < 0)
        {
            //if the direction is negativ and the actual speed is too, subtract the normal acceleration value
            //if the direction is negative and the actual speed is positive it means that the users wants to change directions so a higher value is subtracted from the speed to provide quick direction change
            if (actualSpeed_inp >= -maxSpeed_inp)
            {
                if (actualSpeed_inp < 0)
                    actualSpeed_inp -= acceleration_inp;
                else
                    actualSpeed_inp -= deceleration_inp * 1.3f;
            }
            else
            {
                actualSpeed_inp = -maxSpeed_inp;
            }
        }
        else
        {
            //the speed decreases as long as the speed value is bigger than the deceleration value which is the smalles unit which is subtracted from the speed per frame
            if (Mathf.Abs(actualSpeed_inp) > deceleration_inp)
            {
                actualSpeed_inp -= (actualSpeed_inp > 0) ? deceleration_inp : -deceleration_inp;
            }
            else
            {
                actualSpeed_inp = 0;
            }
        }
    }

    /// <summary>
    /// To provide better FPS this method decreases the points if the user is moving or rotation until the user stops moving or rotating
    /// </summary>
    private void DecreasePointsWhenMoving()
    {
        if (_lastCameraPosition == OVRManager.instance.transform.position && _lastCameraRotation == OVRManager.instance.transform.rotation.eulerAngles
            && _leftStickMovement == Vector2.zero && _rightStickMovement == Vector2.zero)
        {
            if (!_bPointsEnabled)
            {
                ControlScript.Instance.Session.GetCurrentPointCloud().EnableAllPoints();
                _bPointsEnabled = true;
            }
        }
        else
        {
            if (_bPointsEnabled)
            {
                ControlScript.Instance.Session.GetCurrentPointCloud().DecreasePoints();
                _bPointsEnabled = false;
            }
        }

        _lastCameraPosition = OVRManager.instance.transform.position;
        _lastCameraRotation = OVRManager.instance.transform.eulerAngles;
    }

    /// <summary>
    /// This method checks the stick input if a stick is pressed and due to the input it initiates a teleport of the users position or rotation
    /// </summary>
    private void UpdateMovement_SicknessPrevention()
    {
        if (_iStickShiftSemaphor == 1)
        {
            _iStickShiftSemaphor++;

            float x = 0;
            float y = 0;
            float z = 0;
            float r = 0;

            if (Mathf.Abs(_leftStickMovement.x) > InternValues._stickActivationBoarderSPM)
            {
                x = (_leftStickMovement.x > 0) ? InternValues._fTeleportDistance : -InternValues._fTeleportDistance;
            }

            if (Mathf.Abs(_rightStickMovement.y) > InternValues._stickActivationBoarderSPM)
            {
                y = (_rightStickMovement.y > 0) ? InternValues._fTeleportDistance : -InternValues._fTeleportDistance;
            }

            if (Mathf.Abs(_leftStickMovement.y) > InternValues._stickActivationBoarderSPM)
            {
                z = (_leftStickMovement.y > 0) ? InternValues._fTeleportDistance : -InternValues._fTeleportDistance;
            }

            if (Mathf.Abs(_rightStickMovement.x) > InternValues._stickActivationBoarderSPM)
            {
                r = (_rightStickMovement.x > 0) ? InternValues._fTurnAngle : -InternValues._fTurnAngle;
            }

            //simulate a human eye blink
            if (MovementOptions.Twinkle)
                Util.EyeBlink.Blink();

            OVRManager.instance.transform.Translate(new Vector3(x, y, z));
            OVRManager.instance.transform.Rotate(OVRManager.instance.transform.up, r);
        }
    }

    /// <summary>
    /// This nested class privides the Movement class with the converted values from the Ingame options 
    /// </summary>
    public static class InternValues
    {
        public static float _fMaxSpeedTrans { get { return GetMaxSpeedTrans(); } }
        public static float _fMaxSpeedRot { get { return GetMaxSpeedRot(); } }
        public static float _fAccelerationTrans { get { return GetAccelerationTrans(); } }
        public static float _fAccelerationRot { get { return GetAccelerationRot(); } }
        public static float _fDecelerationTrans { get { return GetAccelerationTrans() * 3; } }
        public static float _fDecelerationRot { get { return GetAccelerationRot() * 3; } }

        public static float _fTeleportDistance { get { return GetTeleportDistance(); } }
        public static float _fTurnAngle { get { return GetTurnAngle(); } }

        public static float _stickActivationBoarderFly { get { return 0.2f; } }
        public static float _stickActivationBoarderSPM { get { return 0.7f; } }

        static float scaleFactorTrans = 20;
        static float scaleFactorRot = 2;
        static float scaleFactorTeleport = 100;

        private static float GetMaxSpeedTrans()
        {
            return MovementOptions.TransSpeed / scaleFactorTrans;
        }

        private static float GetAccelerationTrans()
        {
            return MovementOptions.TransAcceleration / (scaleFactorTrans * 150);
        }

        private static float GetMaxSpeedRot()
        {
            return MovementOptions.RotSpeed / scaleFactorRot;
        }

        private static float GetAccelerationRot()
        {
            return MovementOptions.RotAcceleration / (scaleFactorRot * 170);
        }

        private static float GetTeleportDistance()
        {
            return MovementOptions.TeleportDistance / scaleFactorTeleport;
        }

        private static float GetTurnAngle()
        {
            return MovementOptions.TeleportAngle;
        }
    }
}
