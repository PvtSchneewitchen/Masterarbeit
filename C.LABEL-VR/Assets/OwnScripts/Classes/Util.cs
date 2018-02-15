using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using PostProcess;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

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

        try
        {
            foreach (var child in children)
            {
                if (child.name == sName_inp)
                {
                    return child.gameObject;
                }
            }
        }
        catch
        {
            Debug.Log("FindInactiveGameobject: null");
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

    public static void AlignToCamera(GameObject objectWithCamera_inp, GameObject objectToBeAligned_inp, float distanceToCamera_inp)
    {
        Camera objectCamera;
        try
        {
            objectCamera = objectWithCamera_inp.GetComponent<Camera>();
        }
        catch
        {
            Debug.Log("Util.AlignToCamera(): Object has no Camera Component!");
            return;
        }

        Vector3 forwardVector = objectCamera.transform.forward;
        forwardVector.y = 0;

        objectToBeAligned_inp.transform.rotation = Quaternion.Euler(new Vector3(0, objectWithCamera_inp.transform.rotation.eulerAngles.y, 0));
        objectToBeAligned_inp.transform.position = objectWithCamera_inp.transform.position + forwardVector * distanceToCamera_inp;
    }

    public static GameObject CreateDefaultLabelPoint()
    {
        UnityEngine.Object o = Resources.Load("DefaultLabelpoint");
        GameObject go = UnityEngine.Object.Instantiate(o) as GameObject;
        go.name = o.name;
        return go;
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
        FreeFly,
        TeleportMode
    }

    #endregion

    #region Nested Classes

    public class Tuple<T1>
    {
        public Tuple(T1 item1)
        {
            Item1 = item1;
        }

        public T1 Item1 { get; set; }
    }

    public class Tuple<T1, T2> : Tuple<T1>
    {
        public Tuple(T1 item1, T2 item2) : base(item1)
        {
            Item2 = item2;
        }

        public T2 Item2 { get; set; }
    }

    public static class ClippingDistances
    {
        public static float _distanceToCamera_IngameOptions { get { return 5.0f; } }
        public static float _distanceToCamera_InfoMessage { get { return 4.9f; } }
        public static float _distanceToCamera_Clipping { get { return 4.89f; } }
        public static float _distanceToCamera_ClippingDefault { get { return 0.1f; } }
    }

    public static class EyeBlink
    {
        public static BlinkEffect _blinker = GameObject.Find("CenterEyeAnchor").GetComponent<BlinkEffect>();

        public static void EyeLidDown()
        {
            if (_blinker.state == BlinkEffect.State.Idle)
                _blinker.FadeIn();
        }

        public static void EyeLidUp()
        {

            if (_blinker.state == BlinkEffect.State.FadingIn || _blinker.state == BlinkEffect.State.WaitingForFadeOut)
                _blinker.FadeOut();
        }

        public static void Blink()
        {
            _blinker.Blink();
        }
    }

    public static class DataLoadInfo
    {
        public static string _sDataPath { get; private set; }
        public static Datatype _dataType { get; private set; }

        public static void StoreInfo(string sPath_inp, Datatype dataType_inp)
        {
            _sDataPath = sPath_inp;
            _dataType = dataType_inp;
        }
    }

    public class CustomAttributesContainer
    {
        public Tuple<int, int> _tableIndex { get; set; }

        public float _distance { get; set; }

        public float _intensity { get; set; }

        public float _labelPropability { get; set; }

        public Labeling.LabelGroup _label { get; set; }

        public int _pointValid { get; set; }

        public Vector3 _position_Sensor { get; set; }

        public Vector3 _position_Vehicle { get; set; }

        public bool _groundPoint { get; set; }

        public CustomAttributesContainer()
        {
            _tableIndex = new Tuple<int, int>(0, 0);
            _distance = 0;
            _intensity = 0;
            _labelPropability = 0;
            _label = Util.Labeling.LabelGroup.unlabeled;
            _pointValid = 0;
            _position_Sensor = Vector3.zero;
            _position_Vehicle = Vector3.zero;
            _groundPoint = true;
        }
    }

    public static class InGameOptions
    {
        //GetEditorValues()
        //InitUiComponentValues()
        //InGameOptionsSaveData()
        //InGameOptionsSaveData.GetData()

        public static MovementMode _movementMode { get; set; }
        public static float _fFreeFly_MaxSpeedTrans { get; set; }
        public static float _fFreeFly_AccelerationTrans { get; set; }
        public static float _fFreeFly_MaxSpeedRot { get; set; }
        public static float _fFreeFly_AccelerationRot { get; set; }
        public static float _fSicknessPrevention_TeleportDistance { get; set; }
        public static float _fSicknessPrevention_TurnAngle { get; set; }
        public static bool _bSicknessPrevention_TeleportWithBlink { get; set; }
        public static bool _bAttachOptionsToCamera { get; set; }
        public static bool _bDecreasePointsWhenMoving { get; set; }

        private static string sPathUserOptions = Application.persistentDataPath + "/UserOptions.dat";
        private static string sPathDefaultOptions = Application.persistentDataPath + "/DefaultOptions.dat";

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

        public static void SaveOptions()
        {
            SaveAt(sPathUserOptions);
            Debug.Log("Options Saved at: " + Application.dataPath);
        }

        public static void LoadOptions()
        {
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

        private static void InitUiComponentReferences()
        {
            _dropDown_MovementMode = FindInactiveGameobject(GameObject.Find("InGameOptions"), "Dropdown_MovementMode").GetComponent<Dropdown>();
            _inputField_MaxSpeedTrans = FindInactiveGameobject(GameObject.Find("InGameOptions"), "InputField_MaxSpeedTrans").GetComponent<InputField>();
            _inputField_AccelerationTrans = FindInactiveGameobject(GameObject.Find("InGameOptions"), "InputField_AccelerationTrans").GetComponent<InputField>();
            _inputField_MaxSpeedRot = FindInactiveGameobject(GameObject.Find("InGameOptions"), "InputField_MaxSpeedRot").GetComponent<InputField>();
            _inputField_AccelerationRot = FindInactiveGameobject(GameObject.Find("InGameOptions"), "InputField_AccelerationRot").GetComponent<InputField>();
            _inputField_MovementDistance = FindInactiveGameobject(GameObject.Find("InGameOptions"), "InputField_MovementDistance").GetComponent<InputField>();
            _inputField_TurnAngle = FindInactiveGameobject(GameObject.Find("InGameOptions"), "InputField_TurnAngle").GetComponent<InputField>();
            _toggle_TeleportWithBlink = FindInactiveGameobject(GameObject.Find("InGameOptions"), "Toggle_TeleportWithBlink").GetComponent<Toggle>();
            _toggle_StickWithCamera = FindInactiveGameobject(GameObject.Find("InGameOptions"), "Toggle_StickWithCamera").GetComponent<Toggle>();
            _toggle_DecreasePoints = FindInactiveGameobject(GameObject.Find("InGameOptions"), "Toggle_DecreasePoints").GetComponent<Toggle>();
        }

        private static void GetEditorValues()
        {
            try
            {
                InitUiComponentReferences();

                _movementMode = MovementMode.FreeFly;
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
                Debug.Log("GetEditorValues(): No IngameOption UI Components found");
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

        private static void InitUiComponentValues()
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
                Debug.Log("InitUiComponentValues(): No IngameOption UI Components found");
            }
        }

        [Serializable]
        private class InGameOptionsSaveData
        {
            public MovementMode _movementMode { get; set; }
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

    public static class Labeling
    {
        public static LabelGroup _currentLabel;

        private static bool _bIsInitialized = false;
        //! the number and order of Labelgroup items and Groupcolor items must fit together!!

        static Labeling()
        {
            Materials.Unlabeled = Resources.Load("Materials/Unlabeled") as Material;
            Materials.Bicycle = Resources.Load("Materials/Bicycle") as Material;
            Materials.Building = Resources.Load("Materials/Building") as Material;
            Materials.Bus = Resources.Load("Materials/Bus") as Material;
            Materials.Car = Resources.Load("Materials/Car") as Material;
            Materials.Fence = Resources.Load("Materials/Fence") as Material;
            Materials.Motorcycle = Resources.Load("Materials/Motorcycle") as Material;
            Materials.Person = Resources.Load("Materials/Person") as Material;
            Materials.Pole = Resources.Load("Materials/Pole") as Material;
            Materials.Rider = Resources.Load("Materials/Rider") as Material;
            Materials.Road = Resources.Load("Materials/Road") as Material;
            Materials.Sidewalk = Resources.Load("Materials/Sidewalk") as Material;
            Materials.Sky = Resources.Load("Materials/Sky") as Material;
            Materials.Terrain = Resources.Load("Materials/Terrain") as Material;
            Materials.TrafficLight = Resources.Load("Materials/TrafficLight") as Material;
            Materials.TrafficSign = Resources.Load("Materials/TrafficSign") as Material;
            Materials.Train = Resources.Load("Materials/Train") as Material;
            Materials.Truck = Resources.Load("Materials/Truck") as Material;
            Materials.Void = Resources.Load("Materials/Void") as Material;
            Materials.Vegetation = Resources.Load("Materials/Vegetation") as Material;
            Materials.Wall = Resources.Load("Materials/Wall") as Material;
        }

        public static void SetCurrentGroup(LabelGroup newGroup_inp)
        {
            _currentLabel = newGroup_inp;
        }

        public static Material GetGroupMaterial(LabelGroup group_inp)
        {
            PropertyInfo[] properties = typeof(Materials).GetProperties();
            PropertyInfo p = properties[(int)group_inp];
            var v = properties[(int)group_inp].GetValue(typeof(Material), null);
            Material material_out = (Material)properties[(int)group_inp].GetValue(typeof(Materials), null);
            return material_out;
        }

        public enum LabelGroup
        {
            unlabeled,
            bicycle,
            building,
            bus,
            car,
            fence,
            motorcycle,
            person,
            pole,
            rider,
            road,
            sidewalk,
            sky,
            terrain,
            trafficLight,
            trafficSign,
            train,
            truck,
            vegetation,
            Void,
            wall
        }

        //unlabeled,
        //bicycle = 33,
        //building = 11,
        //bus = 28,
        //car = 26,
        //fence = 13,
        //motorcycle = 32,
        //person = 24,
        //pole = 17,
        //rider = 25,
        //road = 7,
        //sidewalk = 8,
        //sky = 23,
        //terrain = 22,
        //trafficLight = 19,
        //trafficSign = 20,
        //train = 31,
        //truck = 27,
        //vegetation = 21,
        //Void = 38,
        //wall = 12

        private struct Materials
        {
            public static Material Unlabeled { get; set; }
            public static Material Bicycle { get; set; }
            public static Material Building { get; set; }
            public static Material Bus { get; set; }
            public static Material Car { get; set; }
            public static Material Fence { get; set; }
            public static Material Motorcycle { get; set; }
            public static Material Person { get; set; }
            public static Material Pole { get; set; }
            public static Material Rider { get; set; }
            public static Material Road { get; set; }
            public static Material Sidewalk { get; set; }
            public static Material Sky { get; set; }
            public static Material Terrain { get; set; }
            public static Material TrafficLight { get; set; }
            public static Material TrafficSign { get; set; }
            public static Material Train { get; set; }
            public static Material Truck { get; set; }
            public static Material Void { get; set; }
            public static Material Vegetation { get; set; }
            public static Material Wall { get; set; }
        }
    }
    #endregion
}
