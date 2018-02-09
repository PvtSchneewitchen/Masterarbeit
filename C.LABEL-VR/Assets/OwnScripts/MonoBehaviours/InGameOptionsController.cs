using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRTK;

public class InGameOptionsController : MonoBehaviour
{
    public GameObject _mainCamera;
    public GameObject _inGameOptionsMain;

    public ControlScript _mainControl;
    public Movement _movementController;
    public KeyboardController _digitKeyboard;
    public VRTK_UIPointer _rightControllerUiPointer;
    public VRTK_StraightPointerRenderer _rightControllerPointerRenderer;
    public VRTK_Pointer _leftControllerPointer;

    private GameObject _currentWindow;
    private GameObject _parent;

    //private Color _invalidCollisionColor;
    //private Color _validCollisionColor;
    //private Color _invalidCollisionColor_old;
    //private Color _validCollisionColor_old;

    private void Start()
    {
        //_invalidCollisionColor = new Color32(99, 109, 115, 50);
        //_validCollisionColor = new Color32(255, 255, 255, 150);

        _parent = this.gameObject;

        _rightControllerUiPointer.UIPointerElementClick += OnInputFieldClick;

        if(Util.InGameOptions._bAttachOptionsToCamera)
        {
            _parent.transform.parent = _mainCamera.transform;
        }

        Util.DisableAllChildren(_parent);
    }

    public void EnableOptionMenu()
    {
        //todo disable unnecessary things when usion option menu
        _movementController._bMovementEnabled = false;
        _leftControllerPointer.enabled = false;
        //_mainCamera.GetComponent<Camera>().nearClipPlane = Util.ClippingDistances._distanceToCamera_Clipping;

        _currentWindow = _inGameOptionsMain;
        Util.AlignToCamera(_mainCamera, _currentWindow, Util.ClippingDistances._distanceToCamera_IngameOptions);
        _currentWindow.SetActive(true);


        //_invalidCollisionColor_old = _rightControllerPointerRenderer.invalidCollisionColor;
        //_validCollisionColor_old = _rightControllerPointerRenderer.validCollisionColor;
        //_rightControllerPointerRenderer.invalidCollisionColor = _invalidCollisionColor;
        //_rightControllerPointerRenderer.validCollisionColor = _validCollisionColor;
    }

    public void DisableOptionMenu()
    {
        //todo enable things going back from option mode
        //_mainCamera.GetComponent<Camera>().nearClipPlane = Util.ClippingDistances._distanceToCamera_ClippingDefault;
        _movementController._bMovementEnabled = true;
        _leftControllerPointer.enabled = true;
        //_rightControllerPointerRenderer.invalidCollisionColor = _invalidCollisionColor_old;
        //_rightControllerPointerRenderer.validCollisionColor = _validCollisionColor_old;
        Util.DisableAllChildren(_parent);
    }


    #region Onlick() and Change() methods 

    public void MovementModeChanged(int iModeIndex_inp)
    {
        Util.InGameOptions._movementMode = (Util.MovementMode)iModeIndex_inp;
        _movementController.Stop();
    }

    public void FreeFly_MaxSpeedTransChanged(string sSpeed_input)
    {
        try
        {
            Util.InGameOptions._fFreeFly_MaxSpeedTrans = float.Parse(sSpeed_input);
        }
        catch
        {
            Util.InGameOptions._fFreeFly_MaxSpeedTrans = 0.0f;
        }
    }

    public void FreeFly_AccelerationTransChanged(string sAcceleration_input)
    {
        try
        {
            Util.InGameOptions._fFreeFly_AccelerationTrans = float.Parse(sAcceleration_input);
        }
        catch
        {
            Util.InGameOptions._fFreeFly_AccelerationTrans = 0;
        }
    }

    public void FreeFly_MaxSpeedRotChanged(string sSpeed_input)
    {
        try
        {
            Util.InGameOptions._fFreeFly_MaxSpeedRot = float.Parse(sSpeed_input);
        }
        catch 
        {
            Util.InGameOptions._fFreeFly_MaxSpeedRot = 0;
        }
    }

    public void FreeFly_AccelerationRotChanged(string sAcceleration_input)
    {
        try
        {
            Util.InGameOptions._fFreeFly_AccelerationRot = float.Parse(sAcceleration_input);
        }
        catch 
        {
            Util.InGameOptions._fFreeFly_AccelerationRot = 0;
        }
    }

    public void SicknessPrevention_TeleportDistanceChanged(string sDistance_input)
    {
        try
        {
            Util.InGameOptions._fSicknessPrevention_TeleportDistance = float.Parse(sDistance_input);
        }
        catch 
        {
            Util.InGameOptions._fSicknessPrevention_TeleportDistance = 0;
        }
    }

    public void SicknessPrevention_TurnAngleChanged(string sAngle_input)
    {
        try
        {
            Util.InGameOptions._fSicknessPrevention_TurnAngle = float.Parse(sAngle_input);
        }
        catch
        {
            Util.InGameOptions._fSicknessPrevention_TurnAngle = 0;
        }
    }

    public void SicknessPrevention_TeleportWithBlinkChanged(bool bBlink_inp)
    {

        Util.InGameOptions._bSicknessPrevention_TeleportWithBlink = bBlink_inp;
    }

    public void AttachToCameraChanged(bool bAttach_inp)
    {
        Util.InGameOptions._bAttachOptionsToCamera = bAttach_inp;

        if(bAttach_inp)
        {
            Util.AlignToCamera(_mainCamera, _currentWindow, Util.ClippingDistances._distanceToCamera_IngameOptions);
            _parent.transform.parent = _mainCamera.transform;
        }
        else
        {
            _parent.transform.parent = null;
        }
    }

    public void DecreasePointsWhenMovingChanged(bool bDecrease_inp)
    {
        Util.InGameOptions._bDecreasePointsWhenMoving = bDecrease_inp;

        if(!bDecrease_inp)
        {
            if(_mainControl._session.GetCurrentPointCloud() != null)
                _mainControl._session.GetCurrentPointCloud().EnableAllPoints();
        }
    }

    public void OnInputFieldClick(object sender, UIPointerEventArgs args)
    {
        if(Util.IsGameobjectTypeOf<InputField>(args.currentTarget))
        {
            var inputField = args.currentTarget.GetComponent<InputField>();
            _digitKeyboard.EnableKeyboard(inputField, _currentWindow);
        }
    }

    public void OnCloseMainOptionsClick()
    {
        _mainControl._bOptionMode = false;
        Util.InGameOptions.SaveOptions();
        DisableOptionMenu();
    }

    public void OnRestoreDefaultClick()
    {
        Util.InGameOptions.RestoreDefaultValues();
    }

    public void OnBackToMainMenuClick()
    {
        Util.InGameOptions.SaveOptions();
        SceneManager.LoadScene(0);
    }
    #endregion

}
