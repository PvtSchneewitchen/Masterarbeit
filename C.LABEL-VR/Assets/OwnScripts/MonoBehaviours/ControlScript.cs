using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ControlScript : MonoBehaviour
{
    public LabelSession _session { get; private set; }
    public InGameOptionsController _inGameOptions;
    public Camera _centerEyeAnchor;

    public bool _optionModeActive { get; set; }

    // Use this for initialization
    void Awake()
    {

#if UNITY_EDITOR
        Debug.Log("1");
        Util.DataLoadInfo._dataType = Util.Datatype.hdf5_DaimlerLidar;
        Util.DataLoadInfo._accessMode = Util.AccesMode.Create;
        Util.DataLoadInfo._sessionName = "EditorDev";

        if (Util.DataLoadInfo._dataType == Util.Datatype.pcd)
            Util.DataLoadInfo._sourceDataPath = "C:\\Users\\gruepazu\\Desktop\\PointClouds\\000000000_LidarImage_000000002.pcd";
        else if (Util.DataLoadInfo._dataType == Util.Datatype.hdf5_DaimlerLidar)
            Util.DataLoadInfo._sourceDataPath = "C:\\Users\\gruepazu\\Desktop\\LIdar18.8\\LidarImages_03_05\\";
#endif
        Debug.Log("2");

        if (Util.DataLoadInfo._accessMode == Util.AccesMode.Create)
        {
            Debug.Log("3");
            CreateSessionFolder();
            InGameOptions.LoadOptions(Util.DataLoadInfo._sessionFolderPath);

            List<PointCloud> pointClouds = new List<PointCloud>();

            if (Util.DataLoadInfo._dataType == Util.Datatype.pcd)
            {
                pointClouds = Import.ImportPcd(Util.DataLoadInfo._sourceDataPath);
            }
            else if (Util.DataLoadInfo._dataType == Util.Datatype.hdf5_DaimlerLidar)
            {
                Debug.Log("5");
                pointClouds = Import.ImportHdf5_DaimlerLidar(Util.DataLoadInfo._sourceDataPath);
            }
            else if (Util.DataLoadInfo._dataType == Util.Datatype.lidar)
            {
                //TODO Implement lidar Import and call it here
            }

            Debug.Log(pointClouds.Count);
            _session = new LabelSession(pointClouds, 0);
        }
        else
        {
            Debug.Log("4");
            SessionSaveFile saveFile;

            using (Stream stream = File.Open(Util.DataLoadInfo._sourceDataPath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                saveFile = binaryFormatter.Deserialize(stream) as SessionSaveFile;
            }

            _session = new LabelSession(saveFile);
        }




        _optionModeActive = false;

        //Debug.Log("Path: " + _sLoadPath + "  Type: " + _dataTypeToLoad);




        _session.GetCurrentPointCloud().EnableAllPoints();
        _session._sessionName = Util.DataLoadInfo._sessionName;

        //Labeling.SetCurrentLabelClass(new Tuple<uint, string>(11, "building"));
        //var points = _session.GetCurrentPointCloud()._validPoints;
        //for (int i = 0; i < points.Count; i++)
        //{
        //    var point = points[i];
        //    var attr = points[i].GetComponent<CustomAttributes>();

        //    if(attr._groundPoint == 1)
        //    {
        //        attr._label = Labeling._currentLabelClass;
        //    }
        //}
    }

    private void CreateSessionFolder()
    {
        string folderPath = Application.persistentDataPath + "/" + Util.DataLoadInfo._sessionName;
        Directory.CreateDirectory(folderPath);
        Util.DataLoadInfo._sessionFolderPath = folderPath;
    }

    // Update is called once per frame
    void Update()
    {
        CheckOptionMenuButton();
        CheckCloudChangeButton();
    }

    private void CheckCloudChangeButton()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x > 0.8f)
            {
                _session.GetNextPointCloud();
            }
            else if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x < -0.8f)
            {
                _session.GetPreviousPointCloud();
            }
        }

        OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

    }

    private void CheckOptionMenuButton()
    {
        if (!_optionModeActive)
        {
            if (OVRInput.GetDown(OVRInput.Button.Start))
            {
                _optionModeActive = true;
                _inGameOptions.EnableOptionMenu();
            }
        }
        else
        {
            if (OVRInput.GetDown(OVRInput.Button.Start))
            {
                _optionModeActive = false;
                _inGameOptions.OnCloseMainOptionsClick();
            }
        }
    }
}
