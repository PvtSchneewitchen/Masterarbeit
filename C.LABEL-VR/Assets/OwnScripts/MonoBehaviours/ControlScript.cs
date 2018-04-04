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
    public LabelClassDisplayUpdate LabelClassDisplay;

    public bool _optionModeActive { get; set; }

    // Use this for initialization
    void Awake()
    {
#if UNITY_EDITOR
        Util.DataLoadInfo._dataType = Util.Datatype.pcd;
        Util.DataLoadInfo._accessMode = Util.AccesMode.Create;
        Util.DataLoadInfo._sessionName = "EditorDev";

        if (Util.DataLoadInfo._dataType == Util.Datatype.pcd)
            Util.DataLoadInfo._sourceDataPath = "C:\\Users\\gruepazu\\Desktop\\PointClouds\\000000000_LidarImage_000000002.pcd";
        else if (Util.DataLoadInfo._dataType == Util.Datatype.hdf5_DaimlerLidar)
            Util.DataLoadInfo._sourceDataPath = "C:\\Users\\gruepazu\\Desktop\\LidarDaten\\DatenDAG\\2017-08-18_090334\\LidarImages_03_05\\LidarImage_000003049.hdf5";
#endif

        if (Util.DataLoadInfo._accessMode == Util.AccesMode.Create)
        {
            CreateSessionFolder();
            InGameOptions.LoadOptions(Util.DataLoadInfo._sessionFolderPath);

            List<PointCloud> pointClouds = new List<PointCloud>();

            if (Util.DataLoadInfo._dataType == Util.Datatype.pcd)
            {
                pointClouds = Import.ImportPcd(Util.DataLoadInfo._sourceDataPath);
            }
            else if (Util.DataLoadInfo._dataType == Util.Datatype.hdf5_DaimlerLidar)
            {
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


        //ground testing

        var test = Labeling.GetLabelClassColor(Labeling.currentLabelClassID);

        var clouds = _session._pointClouds;
        for (int i = 0; i < clouds.Count; i++)
        {
            var points = clouds[i]._validPoints;

            for (int j = 0; j < points.Count; j++)
            {
                var attr = points[j].GetComponent<CustomAttributes>();

                if (attr._groundPoint == 1)
                {
                    attr._label = Labeling.currentLabelClassID;
                }
            }

        }


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
        CheckLabelClassChangeButton();
    }

    private void CheckCloudChangeButton()
    {
        if (OVRInput.GetDown(OVRInput.Button.Four))
        {
            _session.ShowNextPointCloud();
        }
        else if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            _session.ShowPreviousPointCloud();
        }
    }

    private void CheckLabelClassChangeButton()
    {
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            Labeling.SwitchToNextLabelClass();
            LabelClassDisplay.UpdatePointerDisplay();
        }
        else if (OVRInput.GetDown(OVRInput.Button.One))
        {
            Labeling.SwitchToPreviousLabelClass();
            LabelClassDisplay.UpdatePointerDisplay();
        }
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
//int segments = 4;
//int potentialgroundcounter = 0;

//List<GameObject> testpoints = new List<GameObject>();

//System.Random random = new System.Random(88);
//random.Next(-50, 50);

//        for (int i = 0; i< 1000; i++)
//        {
//            GameObject testpoint = Util.CreateDefaultLabelPoint();
//testpoint.transform.position = new Vector3(random.Next(-10, 10), random.Next(-10, 10), random.Next(-10, 10));
//            testpoints.Add(testpoint);

//            if(testpoint.transform.position.y< 1 && testpoint.transform.position.y> -1)
//            {
//                potentialgroundcounter++;
//            }
//        }

//        //testpoints.OrderBy(x => x.transform.position.x).ThenBy(x => x.transform.position.z);

//        List<Tuple<float, GameObject>> mortonCurvedPointList = new List<Tuple<float, GameObject>>();

//        for (int i = 0; i<testpoints.Count; i++)
//        {
//            var pos = testpoints[i].transform.position;
//mortonCurvedPointList.Add(new Tuple<float, GameObject>(pos.x + pos.z, testpoints[i]));
//        }

//        mortonCurvedPointList.OrderBy(x => x.Item1);


//        int pointsInSegmentcounter = 0;
//int pointsInSegment = potentialgroundcounter / segments + 1;
//Color color = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
//        for (int i = 0; i<mortonCurvedPointList.Count; i++)
//        {

//            if (mortonCurvedPointList[i].Item2.transform.position.y< 1 && mortonCurvedPointList[i].Item2.transform.position.y> -1)
//            {
//                mortonCurvedPointList[i].Item2.GetComponent<MeshRenderer>().material.color = color;
//                pointsInSegmentcounter++;
//            }

//            if (pointsInSegmentcounter >= pointsInSegment)
//            {
//                pointsInSegmentcounter = 0;
//                color = new Color((float) random.NextDouble(), (float) random.NextDouble(), (float) random.NextDouble());
//            }
//        }