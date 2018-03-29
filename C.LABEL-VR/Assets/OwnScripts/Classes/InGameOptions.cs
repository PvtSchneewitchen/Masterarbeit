using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class InGameOptions
{
    //GetEditorValues()
    //InitUiComponentValues()
    //InGameOptionsSaveData()
    //InGameOptionsSaveData.GetData()

    public static Util.MovementMode _movementMode { get; set; }
    public static float _fFreeFly_MaxSpeedTrans { get; set; }
    public static float _fFreeFly_AccelerationTrans { get; set; }
    public static float _fFreeFly_MaxSpeedRot { get; set; }
    public static float _fFreeFly_AccelerationRot { get; set; }
    public static float _fSicknessPrevention_TeleportDistance { get; set; }
    public static float _fSicknessPrevention_TurnAngle { get; set; }
    public static bool _bSicknessPrevention_TeleportWithBlink { get; set; }
    public static bool _bAttachOptionsToCamera { get; set; }
    public static bool _bDecreasePointsWhenMoving { get; set; }

    private static Dropdown _dropDown_MovementMode;
    private static InputField _inputField_MaxSpeedTrans;
    private static InputField _inputField_AccelerationTrans;
    private static InputField _inputField_MaxSpeedRot;
    private static InputField _inputField_AccelerationRot;
    private static InputField _inputField_MovementDistance;
    private static InputField _inputField_TurnAngle;
    private static Toggle _toggle_TeleportWithBlink;
    private static Toggle _toggle_StickWithCamera;
    private static Toggle _toggle_DecreasePoints;

    public static void SaveOptions(string path_inp)
    {
        string sPathUserOptions = path_inp + "/UserOptions.dat";

        SaveAt(sPathUserOptions);
        Debug.Log("Options Saved at: " + Application.dataPath);
    }

    public static void LoadOptions(string path_inp)
    {
        string sPathUserOptions = path_inp + "/UserOptions.dat";
        string sPathDefaultOptions = path_inp + "/DefaultOptions.dat";

        if (File.Exists(sPathUserOptions))
        {
            LoadFrom(sPathUserOptions);
            InitUiComponentValues();
            Debug.Log("Options Loaded from " + sPathUserOptions);
        }
        else
        {
            if (File.Exists(sPathDefaultOptions))
            {
                Debug.Log("No Save Data in " + sPathDefaultOptions + " || default values used");
                LoadFrom(sPathDefaultOptions);
                InitUiComponentValues();
            }
            else
            {
                Debug.Log("No Save or Default Data " + sPathDefaultOptions + " ||  editor values used");
                GetEditorValues();
                InitUiComponentValues();
                SaveAt(sPathDefaultOptions);
            }
        }
    }

    public static void RestoreDefaultValues(string path_inp)
    {
        string sPathUserOptions = path_inp + "/UserOptions.dat";
        string sPathDefaultOptions = path_inp + "/DefaultOptions.dat";

        if (File.Exists(sPathDefaultOptions))
        {
            if (File.Exists(sPathUserOptions))
            {
                File.Delete(sPathUserOptions);
                LoadFrom(sPathDefaultOptions);
            }
            else
            {
                LoadFrom(sPathDefaultOptions);
            }
        }
        else
        {
            UnityEngine.Debug.Log("Something strange happened, no default values");
        }

        InitUiComponentValues();
    }

    private static void InitUiComponentReferences()
    {
        _dropDown_MovementMode = Util.FindInactiveGameobject(GameObject.Find("InGameOptions"), "Dropdown_MovementMode").GetComponent<Dropdown>();
        _inputField_MaxSpeedTrans = Util.FindInactiveGameobject(GameObject.Find("InGameOptions"), "InputField_MaxSpeedTrans_Num").GetComponent<InputField>();
        _inputField_AccelerationTrans = Util.FindInactiveGameobject(GameObject.Find("InGameOptions"), "InputField_AccelerationTrans_Num").GetComponent<InputField>();
        _inputField_MaxSpeedRot = Util.FindInactiveGameobject(GameObject.Find("InGameOptions"), "InputField_MaxSpeedRot_Num").GetComponent<InputField>();
        _inputField_AccelerationRot = Util.FindInactiveGameobject(GameObject.Find("InGameOptions"), "InputField_AccelerationRot_Num").GetComponent<InputField>();
        _inputField_MovementDistance = Util.FindInactiveGameobject(GameObject.Find("InGameOptions"), "InputField_MovementDistance_Num").GetComponent<InputField>();
        _inputField_TurnAngle = Util.FindInactiveGameobject(GameObject.Find("InGameOptions"), "InputField_TurnAngle_Num").GetComponent<InputField>();
        _toggle_TeleportWithBlink = Util.FindInactiveGameobject(GameObject.Find("InGameOptions"), "Toggle_TeleportWithBlink").GetComponent<Toggle>();
        _toggle_StickWithCamera = Util.FindInactiveGameobject(GameObject.Find("InGameOptions"), "Toggle_StickWithCamera").GetComponent<Toggle>();
        _toggle_DecreasePoints = Util.FindInactiveGameobject(GameObject.Find("InGameOptions"), "Toggle_DecreasePoints").GetComponent<Toggle>();
    }

    private static void GetEditorValues()
    {
        try
        {
            InitUiComponentReferences();

            _movementMode = Util.MovementMode.FreeFly;
            _fFreeFly_MaxSpeedTrans = float.Parse(_inputField_MaxSpeedTrans.text);
            _fFreeFly_AccelerationTrans = float.Parse(_inputField_AccelerationTrans.text);
            _fFreeFly_MaxSpeedRot = float.Parse(_inputField_MaxSpeedRot.text);
            _fFreeFly_AccelerationRot = float.Parse(_inputField_AccelerationRot.text);
            _fSicknessPrevention_TeleportDistance = float.Parse(_inputField_MovementDistance.text);
            _fSicknessPrevention_TurnAngle = float.Parse(_inputField_TurnAngle.text);
            _bSicknessPrevention_TeleportWithBlink = _toggle_TeleportWithBlink.isOn;
            _bAttachOptionsToCamera = _toggle_StickWithCamera.isOn;
            _bDecreasePointsWhenMoving = _toggle_DecreasePoints.isOn;
        }
        catch
        {
            UnityEngine.Debug.Log("GetEditorValues(): No IngameOption UI Components found");
        }
    }

    private static void SaveAt(string path_inp)
    {
        InGameOptionsSaveData saveData = new InGameOptionsSaveData();
        Stream stream = File.OpenWrite(path_inp);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, saveData);
        stream.Flush();
        stream.Close();
        stream.Dispose();
    }

    private static void LoadFrom(string path_inp)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = File.Open(path_inp, FileMode.Open);
        object obj = formatter.Deserialize(fileStream);
        InGameOptionsSaveData loadedData = (InGameOptionsSaveData)obj;
        fileStream.Flush();
        fileStream.Close();
        fileStream.Dispose();

        loadedData.GetData();
    }

    public static void InitUiComponentValues()
    {
        try
        {
            InitUiComponentReferences();

            _dropDown_MovementMode.value = (int)_movementMode;
            _inputField_MaxSpeedTrans.text = Convert.ToString(_fFreeFly_MaxSpeedTrans);
            _inputField_AccelerationTrans.text = Convert.ToString(_fFreeFly_AccelerationTrans);
            _inputField_MaxSpeedRot.text = Convert.ToString(_fFreeFly_MaxSpeedRot);
            _inputField_AccelerationRot.text = Convert.ToString(_fFreeFly_AccelerationRot);
            _inputField_MovementDistance.text = Convert.ToString(_fSicknessPrevention_TeleportDistance);
            _inputField_TurnAngle.text = Convert.ToString(_fSicknessPrevention_TurnAngle);
            _toggle_TeleportWithBlink.isOn = _bSicknessPrevention_TeleportWithBlink;
            _toggle_StickWithCamera.isOn = _bAttachOptionsToCamera;
            _toggle_DecreasePoints.isOn = _bDecreasePointsWhenMoving;
        }
        catch
        {
            UnityEngine.Debug.Log("InitUiComponentValues(): No IngameOption UI Components found");
        }
    }

    [Serializable]
    private class InGameOptionsSaveData
    {
        public Util.MovementMode _movementMode { get; set; }
        public float _fFreeFly_MaxSpeedTrans { get; set; }
        public float _fFreeFly_AccelerationTrans { get; set; }
        public float _fFreeFly_MaxSpeedRot { get; set; }
        public float _fFreeFly_AccelerationRot { get; set; }
        public float _fSicknessPrevention_TeleportDistance { get; set; }
        public float _fSicknessPrevention_TurnAngle { get; set; }
        public bool _bSicknessPrevention_TeleportWithBlink { get; set; }
        public bool _bAttachOptionsToCamera { get; set; }
        public bool _bDecreasePointsWhenMoving { get; set; }

        public InGameOptionsSaveData()
        {
            _movementMode = InGameOptions._movementMode;
            _fFreeFly_MaxSpeedTrans = InGameOptions._fFreeFly_MaxSpeedTrans;
            _fFreeFly_AccelerationTrans = InGameOptions._fFreeFly_AccelerationTrans;
            _fFreeFly_MaxSpeedRot = InGameOptions._fFreeFly_MaxSpeedRot;
            _fFreeFly_AccelerationRot = InGameOptions._fFreeFly_AccelerationRot;
            _fSicknessPrevention_TeleportDistance = InGameOptions._fSicknessPrevention_TeleportDistance;
            _fSicknessPrevention_TurnAngle = InGameOptions._fSicknessPrevention_TurnAngle;
            _bSicknessPrevention_TeleportWithBlink = InGameOptions._bSicknessPrevention_TeleportWithBlink;
            _bAttachOptionsToCamera = InGameOptions._bAttachOptionsToCamera;
            _bDecreasePointsWhenMoving = InGameOptions._bDecreasePointsWhenMoving;
        }

        public void GetData()
        {
            InGameOptions._movementMode = _movementMode;
            InGameOptions._fFreeFly_MaxSpeedTrans = _fFreeFly_MaxSpeedTrans;
            InGameOptions._fFreeFly_AccelerationTrans = _fFreeFly_AccelerationTrans;
            InGameOptions._fFreeFly_MaxSpeedRot = _fFreeFly_MaxSpeedRot;
            InGameOptions._fFreeFly_AccelerationRot = _fFreeFly_AccelerationRot;
            InGameOptions._fSicknessPrevention_TeleportDistance = _fSicknessPrevention_TeleportDistance;
            InGameOptions._fSicknessPrevention_TurnAngle = _fSicknessPrevention_TurnAngle;
            InGameOptions._bSicknessPrevention_TeleportWithBlink = _bSicknessPrevention_TeleportWithBlink;
            InGameOptions._bAttachOptionsToCamera = _bAttachOptionsToCamera;
            InGameOptions._bDecreasePointsWhenMoving = _bDecreasePointsWhenMoving;
        }
    }
}
