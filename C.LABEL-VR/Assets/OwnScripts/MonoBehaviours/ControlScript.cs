using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ControlScript : MonoBehaviour
{
    public LabelSession _session { get; private set; }
    public InGameOptionsController _inGameOptions;
    public Camera _centerEyeAnchor;

    public bool _optionModeActive { get; set; }

    private string _loadPath;
    private Util.Datatype _dataTypeToLoad;


    // Use this for initialization
    void Awake()
    {
        LoadSettings();

        _optionModeActive = false;

        //Debug.Log("Path: " + _sLoadPath + "  Type: " + _dataTypeToLoad);

#if UNITY_EDITOR
        _dataTypeToLoad = Util.Datatype.hdf5_DaimlerLidar;

        if(_dataTypeToLoad == Util.Datatype.pcd)
            _loadPath = "C:\\Users\\gruepazu\\Desktop\\PointClouds\\000000000_LidarImage_000000002.pcd";
        else if(_dataTypeToLoad == Util.Datatype.hdf5_DaimlerLidar)
            _loadPath = "C:\\Users\\gruepazu\\Desktop\\Lidar18.8\\LidarImages_03_05\\LidarImage_000000002.hdf5";
#endif

        
       List<PointCloud> pointClouds = new List<PointCloud>();

        if (_dataTypeToLoad == Util.Datatype.pcd)
        {
            pointClouds = Import.ImportPcd(_loadPath);
        }
        else if (_dataTypeToLoad == Util.Datatype.hdf5_DaimlerLidar)
        {
            pointClouds = Import.ImportHdf5_DaimlerLidar(_loadPath);
        }
        else if (_dataTypeToLoad == Util.Datatype.lidar)
        {
            //TODO Implement lidar Import and call it here
        }

        _session = new LabelSession(pointClouds, 0);
        _session.GetCurrentPointCloud().EnableAllPoints();

        //Export.ExportHdf5_DaimlerLidar("C:\\Users\\gruepazu\\Desktop\\Lidar18.8\\LidarImages_03_05\\Export");

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

    // Update is called once per frame
    void Update()
    {
        CheckOptionButton();
    }

    private void LoadSettings()
    {
        InGameOptions.LoadOptions();

        _loadPath = Util.DataLoadInfo._sDataPath;
        _dataTypeToLoad = Util.DataLoadInfo._dataType;
    }

    private void CheckOptionButton()
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
