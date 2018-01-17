using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using VRTK;
using PostProcess;
using System.Threading;

public static class Util
{
    #region Methods
    public static void DisableAllChildren(GameObject parent_inp)
    {
        for (int i = 0; i < parent_inp.transform.childCount; i++)
        {
            var child = parent_inp.transform.GetChild(i).gameObject;
            if (child != null)
                child.SetActive(false);
        }
    }

    public static GameObject FindInactiveGameobject(GameObject parent_inp, string sName_inp)
    {
        var children = parent_inp.GetComponentsInChildren<Transform>(true);
        foreach (var child in children)
        {
            if (child.name == sName_inp)
            {
                return child.gameObject;
            }
        }
        return null;
    }

    public static bool IsGameobjectTypeOf<T>(GameObject objectToTest_inp)
    {
        var testVal = objectToTest_inp.GetComponent<T>();

        if (testVal == null)
            return false;
        else
            return true;
    }

    #endregion

    #region Enums

    public enum Datatype
    {
        pcd,
        lidar,
        hdf5
    }

    public enum MovementMode
    {
        FreeFlyFast,
        FreeFlySlow,
        SicknessPrevention
    }

    #endregion

    #region Nested Classes

    public static class ClippingDistances
    {
        public static float _distanceToCamera_IngameOptions { get { return 5.0f; } }
        public static float _distanceToCamera_InfoMessage { get { return 4.9f; } }
        public static float _distanceToCamera_Clipping { get { return 4.89f; } }
        public static float _distanceToCamera_ClippingDefault { get { return 0.1f; } }
    }

    public static class EyeBlink
    {
        public static void EyeLidDown()
        {
            BlinkEffect blinker = GameObject.Find("CenterEyeAnchor").GetComponent<BlinkEffect>();

            if (blinker.state == BlinkEffect.State.Idle)
                blinker.FadeIn();
        }

        public static void EyeLidUp()
        {
            BlinkEffect blinker = GameObject.Find("CenterEyeAnchor").GetComponent<BlinkEffect>();

            if (blinker.state == BlinkEffect.State.FadingIn || blinker.state == BlinkEffect.State.WaitingForFadeOut)
                blinker.FadeOut();
        }

        public static void Blink()
        {
            GameObject.Find("CenterEyeAnchor").GetComponent<BlinkEffect>().Blink();
        }
    }



    public static class DataLoadInfo
    {
        public static string sDataPath { get; private set; }
        public static Datatype dataType { get; private set; }
        public static bool bIsSingleFile { get; private set; }

        public static void StoreInfo(string sPath_inp, Datatype dataType_inp, bool bIsSingleFile_inp)
        {
            sDataPath = sPath_inp;
            dataType = dataType_inp;
            bIsSingleFile = bIsSingleFile_inp;
        }
    }

    public static class InGameOptions
    {
        public static MovementMode _movementMode { get; set; }
        public static float _fFreeFlyFast_MaxSpeed { get; set; }
        public static float _fFreeFlyFast_AccelerationFactor { get; set; }
        public static float _fFreeFlySlow_MaxSpeed { get; set; }
        public static float _fFreeFlySlow_AccelerationFactor { get; set; }
        public static float _fSicknessPrevention_TeleportDistance { get; set; }
        public static float _fSicknessPrevention_TurnAngle { get; set; }
        public static bool _bSicknessPrevention_TeleportWithBlink { get; set; }

        private static string sPathUserOptions = Application.persistentDataPath + "/UserOptions.xml";
        private static string sPathDefaultOptions = Application.persistentDataPath + "/DefaultOptions.xml";

        public static void SaveOptions()
        {
            SaveAt(sPathUserOptions);
            Debug.Log("Options Saved");
        }

        public static void LoadOptions()
        {
            if (File.Exists(sPathUserOptions))
            {
                LoadFrom(sPathUserOptions);
                InitUiComponentValues();
                Debug.Log("Options Loaded");
            }
            else
            {
                if (File.Exists(sPathDefaultOptions))
                {
                    LoadFrom(sPathDefaultOptions);
                    InitUiComponentValues();
                    Debug.Log("No Save Data!, default values used");
                }
                else
                {
                    GetEditorValues();
                    InitUiComponentValues();
                    SaveAt(sPathDefaultOptions);
                    Debug.Log("No Save or Default Data!, editor values used");
                }
            }
        }

        public static void RestoreDefaultValues()
        {
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
                Debug.Log("Something strange happened, no default values");
            }

            InitUiComponentValues();
        }

        private static void GetEditorValues()
        {
            _movementMode = MovementMode.FreeFlySlow;
            _fFreeFlyFast_MaxSpeed = float.Parse(GameObject.Find("InputField_MaxSpeedFast").GetComponent<InputField>().text);
            _fFreeFlyFast_AccelerationFactor = float.Parse(GameObject.Find("InputField_AccelerationFast").GetComponent<InputField>().text);
            _fFreeFlySlow_MaxSpeed = float.Parse(GameObject.Find("InputField_MaxSpeedSlow").GetComponent<InputField>().text);
            _fFreeFlySlow_AccelerationFactor = float.Parse(GameObject.Find("InputField_AccelerationSlow").GetComponent<InputField>().text);
            _fSicknessPrevention_TeleportDistance = float.Parse(GameObject.Find("InputField_MovementDistance").GetComponent<InputField>().text);
            _fSicknessPrevention_TurnAngle = float.Parse(GameObject.Find("InputField_TurnAngle").GetComponent<InputField>().text);
            _bSicknessPrevention_TeleportWithBlink = GameObject.Find("Toggle_TeleportWithBlink").GetComponent<Toggle>().isOn;
        }

        private static void SaveAt(string path_inp)
        {
            InGameOptionsSaveData saveData = new InGameOptionsSaveData();
            var xmlSerializer = new XmlSerializer(saveData.GetType());
            using (var writer = XmlWriter.Create(path_inp))
            {
                xmlSerializer.Serialize(writer, saveData);
            }
        }

        private static void LoadFrom(string path_inp)
        {
            var xmlDeserializer = new XmlSerializer(typeof(InGameOptionsSaveData));
            using (var reader = XmlReader.Create(path_inp))
            {
                var saveData = (InGameOptionsSaveData)xmlDeserializer.Deserialize(reader);
                saveData.GetData();
            }
        }

        private static void InitUiComponentValues()
        {
            GameObject.Find("Dropdown_MovementMode").GetComponent<Dropdown>().value = (int)_movementMode;
            GameObject.Find("InputField_MaxSpeedFast").GetComponent<InputField>().text = Convert.ToString(_fFreeFlyFast_MaxSpeed);
            GameObject.Find("InputField_AccelerationFast").GetComponent<InputField>().text = Convert.ToString(_fFreeFlyFast_AccelerationFactor);
            GameObject.Find("InputField_MaxSpeedSlow").GetComponent<InputField>().text = Convert.ToString(_fFreeFlySlow_MaxSpeed);
            GameObject.Find("InputField_AccelerationSlow").GetComponent<InputField>().text = Convert.ToString(_fFreeFlySlow_AccelerationFactor);
            GameObject.Find("InputField_MovementDistance").GetComponent<InputField>().text = Convert.ToString(_fSicknessPrevention_TeleportDistance);
            GameObject.Find("InputField_TurnAngle").GetComponent<InputField>().text = Convert.ToString(_fSicknessPrevention_TurnAngle);
            GameObject.Find("Toggle_TeleportWithBlink").GetComponent<Toggle>().isOn = _bSicknessPrevention_TeleportWithBlink;
        }

        [Serializable]
        private class InGameOptionsSaveData
        {
            public MovementMode movementMode;
            public float _fFreeFlyFast_MaxSpeed;
            public float _fFreeFlyFast_AccelerationFactor;
            public float _fFreeFlySlow_MaxSpeed;
            public float _fFreeFlySlow_AccelerationFactor;
            public float _fSicknessPrevention_TeleportDistance;
            public float _fSicknessPrevention_TurnAngle;
            public bool _bSicknessPrevention_TeleportWithBlink;

            public InGameOptionsSaveData()
            {
                movementMode = InGameOptions._movementMode;
                _fFreeFlyFast_MaxSpeed = InGameOptions._fFreeFlyFast_MaxSpeed;
                _fFreeFlyFast_AccelerationFactor = InGameOptions._fFreeFlyFast_AccelerationFactor;
                _fFreeFlySlow_MaxSpeed = InGameOptions._fFreeFlySlow_MaxSpeed;
                _fFreeFlySlow_AccelerationFactor = InGameOptions._fFreeFlySlow_AccelerationFactor;
                _fSicknessPrevention_TeleportDistance = InGameOptions._fSicknessPrevention_TeleportDistance;
                _fSicknessPrevention_TurnAngle = InGameOptions._fSicknessPrevention_TurnAngle;
                _bSicknessPrevention_TeleportWithBlink = InGameOptions._bSicknessPrevention_TeleportWithBlink;
            }

            public void GetData()
            {
                Debug.Log(movementMode);
                Debug.Log(_fFreeFlyFast_AccelerationFactor);
                Debug.Log(_bSicknessPrevention_TeleportWithBlink);
                InGameOptions._movementMode = movementMode;
                InGameOptions._fFreeFlyFast_MaxSpeed = _fFreeFlyFast_MaxSpeed;
                InGameOptions._fFreeFlyFast_AccelerationFactor = _fFreeFlyFast_AccelerationFactor;
                InGameOptions._fFreeFlySlow_MaxSpeed = _fFreeFlySlow_MaxSpeed;
                InGameOptions._fFreeFlySlow_AccelerationFactor = _fFreeFlySlow_AccelerationFactor;
                InGameOptions._fSicknessPrevention_TeleportDistance = _fSicknessPrevention_TeleportDistance;
                InGameOptions._fSicknessPrevention_TurnAngle = _fSicknessPrevention_TurnAngle;
                InGameOptions._bSicknessPrevention_TeleportWithBlink = _bSicknessPrevention_TeleportWithBlink;
            }
        }

    
    }
    #endregion
}
