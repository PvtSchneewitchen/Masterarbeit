using GracesGames.SimpleFileBrowser.Scripts;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRKeyboard.Utils;
using VRTK;

public class InGameOptionsController : MonoBehaviour
{
    public GameObject _mainCamera;
    public GameObject _inGameOptionsContainer;
    public GameObject _panelMovement;
    public GameObject _panelLabeling;
    public GameObject _fileBrowserPrefab;
    public GameObject _keyboard;

    public ControlScript _mainControl;
    public KeyboardController _digitKeyboard;
    public VRTK_UIPointer _rightControllerUiPointer;
    public VRTK_StraightPointerRenderer _rightControllerPointerRenderer;
    public PointerTeleport _pointerTeleportScript;
    public Movement _movementScript;
    public PointerLabeler _pointerLabelerScript;
    public LabelClassDisplayUpdate _labelClassDisplay;

    private GameObject _currentPanel;
    private GameObject _lastPanelUsed;

    //private Color _invalidCollisionColor;
    //private Color _validCollisionColor;
    //private Color _invalidCollisionColor_old;
    //private Color _validCollisionColor_old;

    private void Start()
    {
        //_invalidCollisionColor = new Color32(99, 109, 115, 50);
        //_validCollisionColor = new Color32(255, 255, 255, 150);

        _rightControllerUiPointer.UIPointerElementClick += OnInputFieldClick;

        if (InGameOptions._bAttachOptionsToCamera)
        {
            gameObject.transform.parent = _mainCamera.transform;
        }

        Util.DisableAllChildren(gameObject);
    }

    public void EnableOptionMenu()
    {
        //todo disable unnecessary things when usion option menu
        _movementScript._bMovementEnabled = false;
        _pointerLabelerScript.enabled = false;
        _pointerTeleportScript._teleportEnabled = false;
        _labelClassDisplay._enabled = false;

        if (_lastPanelUsed == null)
        {
            _currentPanel = _panelMovement;
        }
        else
        {
            _currentPanel = _lastPanelUsed;
        }

        Util.AlignToCamera(_mainCamera, _inGameOptionsContainer, Util.ClippingDistances._distanceToCamera_IngameOptions);
        _inGameOptionsContainer.SetActive(true);
        _currentPanel.SetActive(true);


        //_invalidCollisionColor_old = _rightControllerPointerRenderer.invalidCollisionColor;
        //_validCollisionColor_old = _rightControllerPointerRenderer.validCollisionColor;
        //_rightControllerPointerRenderer.invalidCollisionColor = _invalidCollisionColor;
        //_rightControllerPointerRenderer.validCollisionColor = _validCollisionColor;
    }

    public void DisableOptionMenu()
    {
        //todo enable things going back from option mode
        //_mainCamera.GetComponent<Camera>().nearClipPlane = Util.ClippingDistances._distanceToCamera_ClippingDefault;
        _movementScript._bMovementEnabled = true;
        _pointerLabelerScript.enabled = true;
        _pointerTeleportScript._teleportEnabled = true;
        _labelClassDisplay._enabled = true; 

        //_rightControllerPointerRenderer.invalidCollisionColor = _invalidCollisionColor_old;
        //_rightControllerPointerRenderer.validCollisionColor = _validCollisionColor_old;
        _currentPanel.SetActive(false);
        Util.DisableAllChildren(gameObject);
    }


    #region Onlick() and Change() methods 

    public void MovementModeChanged(int iModeIndex_inp)
    {
        InGameOptions._movementMode = (Util.MovementMode)iModeIndex_inp;
        _movementScript.Stop();
    }

    public void FreeFly_MaxSpeedTransChanged(string sSpeed_input)
    {
        try
        {
            InGameOptions._fFreeFly_MaxSpeedTrans = float.Parse(sSpeed_input);
        }
        catch
        {
            InGameOptions._fFreeFly_MaxSpeedTrans = 0.0f;
        }
    }

    public void FreeFly_AccelerationTransChanged(string sAcceleration_input)
    {
        try
        {
            InGameOptions._fFreeFly_AccelerationTrans = float.Parse(sAcceleration_input);
        }
        catch
        {
            InGameOptions._fFreeFly_AccelerationTrans = 0;
        }
    }

    public void FreeFly_MaxSpeedRotChanged(string sSpeed_input)
    {
        try
        {
            InGameOptions._fFreeFly_MaxSpeedRot = float.Parse(sSpeed_input);
        }
        catch
        {
            InGameOptions._fFreeFly_MaxSpeedRot = 0;
        }
    }

    public void FreeFly_AccelerationRotChanged(string sAcceleration_input)
    {
        try
        {
            InGameOptions._fFreeFly_AccelerationRot = float.Parse(sAcceleration_input);
        }
        catch
        {
            InGameOptions._fFreeFly_AccelerationRot = 0;
        }
    }

    public void SicknessPrevention_TeleportDistanceChanged(string sDistance_input)
    {
        try
        {
            InGameOptions._fSicknessPrevention_TeleportDistance = float.Parse(sDistance_input);
        }
        catch
        {
            InGameOptions._fSicknessPrevention_TeleportDistance = 0;
        }
    }

    public void SicknessPrevention_TurnAngleChanged(string sAngle_input)
    {
        try
        {
            InGameOptions._fSicknessPrevention_TurnAngle = float.Parse(sAngle_input);
        }
        catch
        {
            InGameOptions._fSicknessPrevention_TurnAngle = 0;
        }
    }

    public void SicknessPrevention_TeleportWithBlinkChanged(bool bBlink_inp)
    {

        InGameOptions._bSicknessPrevention_TeleportWithBlink = bBlink_inp;
    }

    public void AttachToCameraChanged(bool bAttach_inp)
    {
        InGameOptions._bAttachOptionsToCamera = bAttach_inp;

        if (bAttach_inp)
        {
            Util.AlignToCamera(_mainCamera, _currentPanel, Util.ClippingDistances._distanceToCamera_IngameOptions);
            gameObject.transform.parent = _mainCamera.transform;
        }
        else
        {
            gameObject.transform.parent = null;
        }
    }

    public void DecreasePointsWhenMovingChanged(bool bDecrease_inp)
    {
        InGameOptions._bDecreasePointsWhenMoving = bDecrease_inp;

        if (!bDecrease_inp)
        {
            if (_mainControl._session.GetCurrentPointCloud() != null)
                _mainControl._session.GetCurrentPointCloud().EnableAllPoints();
        }
    }

    public void OnInputFieldClick(object sender, UIPointerEventArgs args)
    {
        var target = args.currentTarget;
        if (Util.IsGameobjectTypeOf<InputField>(target))
        {
            var inputField = target.GetComponent<InputField>();
            if(inputField.name.Contains("Num"))
            {
                _digitKeyboard.EnableNumpad(inputField, inputField.gameObject);
            }
            else if(inputField.name.Contains("Key"))
            {
                EnableKeyBoard(_currentPanel, inputField);
            }
        }
    }

    public void OnMovementButtonClick()
    {
        _currentPanel.SetActive(false);
        _currentPanel = _panelMovement;
        _currentPanel.SetActive(true);
    }

    public void OnLabelingButtonClick()
    {
        _currentPanel.SetActive(false);
        _currentPanel = _panelLabeling;
        _currentPanel.SetActive(true);
    }

    public void OnCloseMainOptionsClick()
    {
        _mainControl._optionModeActive = false;
        InGameOptions.SaveOptions(Util.DataLoadInfo._sessionFolderPath);
        //SessionSaveFile.SaveSession(Util.DataLoadInfo._sessionFolderPath);
        DisableOptionMenu();
    }

    public void OnRestoreDefaultClick()
    {
        InGameOptions.RestoreDefaultValues(Util.DataLoadInfo._sessionFolderPath);
    }

    public void OnExportDataClicked()
    {
        Transform rig = _mainCamera.transform.parent.parent;
        rig.transform.position += -rig.transform.forward * 2.5f;

        OpenBrowser("nofilesjustdirectories", Path.GetDirectoryName(Util.DataLoadInfo._sourceDataPath));
    }

    public void OnBackToMainMenuClick()
    {
        InGameOptions.SaveOptions(Util.DataLoadInfo._sessionFolderPath);
        SessionSaveFile.SaveSession(Util.DataLoadInfo._sessionFolderPath);

        Labeling.Reset();
        MetaData.Reset();

        SceneManager.LoadScene(0);
    }

    public void OnKeyboardConfirmClick()
    {
        _keyboard.SetActive(false);
    }

    #endregion

    private void EnableKeyBoard(GameObject position, InputField inputField)
    {
        _keyboard.SetActive(true);
        _keyboard.SetActive(true);
        _keyboard.transform.rotation = position.transform.rotation;
        _keyboard.transform.position = position. transform.position -position.transform.forward - position.transform.up;
        _keyboard.GetComponent<KeyboardManager>().inputField= inputField;
    }

    private void OpenBrowser(string sFileExtension_inp, string startPath)
    {
        OpenFileBrowser(FileBrowserMode.Load, _currentPanel.transform.parent, sFileExtension_inp, startPath);
    }

    private void OpenFileBrowser(FileBrowserMode fileBrowserMode_inp, Transform parent_inp, string sFileExtension_inp, string startPath)
    {
        // Create the file browser and name it
        GameObject fileBrowserObject = Instantiate(_fileBrowserPrefab, parent_inp);
        //fileBrowserObject.transform.parent = null;
        fileBrowserObject.name = "FileBrowser";
        // Set the mode to save or load
        FileBrowser fileBrowserScript = fileBrowserObject.GetComponent<FileBrowser>();
        fileBrowserScript.SetupFileBrowser(ViewMode.Landscape, parent_inp);
        if (fileBrowserMode_inp == FileBrowserMode.Save)
        {
            fileBrowserScript.SaveFilePanel(this, "SaveFileUsingPath", "DemoText", sFileExtension_inp, startPath);
        }
        else
        {
            //caller script, callbackmethod, fileextension
            fileBrowserScript.OpenFilePanel(this, "ExportToPath", sFileExtension_inp, startPath);
        }
    }

    private void ExportToPath(string path_inp)
    {
        if(Util.DataLoadInfo._dataType == Util.Datatype.pcd)
        {
            Export.ExportPcd(path_inp);
        }
        else
        {
            Export.ExportHdf5_DaimlerLidar(path_inp);
        }

        Transform rig = _mainCamera.transform.parent.parent;
        rig.transform.position += rig.transform.forward * 2.5f;
    }
}
