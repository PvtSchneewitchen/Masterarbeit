using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlScript : MonoBehaviour
{
    public Session _session { get; private set; }
    public InGameOptionsController _inGameOptions;
    public Camera _centerEyeAnchor;

    public bool _bOptionMode { get; set; }

    private string _sLoadPath;
    private Util.Datatype _dataTypeToLoad;


    // Use this for initialization
    void Awake()
    {
        LoadSettings();

        _bOptionMode = false;

        //Debug.Log("Path: " + _sLoadPath + "  Type: " + _dataTypeToLoad);

#if UNITY_EDITOR
        _dataTypeToLoad = Util.Datatype.hdf5;

        if(_dataTypeToLoad == Util.Datatype.pcd)
            _sLoadPath = "C:\\Users\\gruepazu\\Desktop\\PointClouds\\000000000_LidarImage_000000002.pcd";
        else if(_dataTypeToLoad == Util.Datatype.hdf5)
            _sLoadPath = "C:\\Users\\gruepazu\\Desktop\\Lidar18.8\\LidarImages_03_05\\LidarImage_000000002.hdf5";
#endif

        List<PointCloud> pointClouds = new List<PointCloud>();

        if (_dataTypeToLoad == Util.Datatype.pcd)
        {
            Dictionary<int, List<Vector3>> pcdCoordinateLists = new Dictionary<int, List<Vector3>>();

            pcdCoordinateLists = PcdAddon.ReadPcdFromPath(_sLoadPath);

            foreach (var coordinateList in pcdCoordinateLists)
            {
                pointClouds.Add(new PointCloud(coordinateList.Value));
            }
        }
        else if (_dataTypeToLoad == Util.Datatype.hdf5)
        {
            List<HDF5Addon.Lidar_Daimler> containers = HDF5Addon.ReadDaimlerHdf(_sLoadPath);

            foreach (var container in containers)
            {
                pointClouds.Add(new PointCloud(container));
            }
        }
        else if (_dataTypeToLoad == Util.Datatype.lidar)
        {
            //TODO Implement lidar Import and call it here
        }

        _session = new Session(pointClouds, 0);
        _session.GetCurrentPointCloud().EnableAllPoints();

        //Util.Labeling.SetCurrentGroup(Util.Labeling.LabelGroup.motorcycle);

        //CombineMeshes();
    }

    // Update is called once per frame
    void Update()
    {
        CheckOptionButton();
    }

    private void LoadSettings()
    {
        InGameOptions.LoadOptions();

        _sLoadPath = Util.DataLoadInfo._sDataPath;
        _dataTypeToLoad = Util.DataLoadInfo._dataType;
    }

    private void CheckOptionButton()
    {
        if (!_bOptionMode)
        {
            if (OVRInput.GetDown(OVRInput.Button.Start))
            {
                _bOptionMode = true;
                _inGameOptions.EnableOptionMenu();
            }
        }
        else
        {
            if (OVRInput.GetDown(OVRInput.Button.Start))
            {
                _bOptionMode = false;
                _inGameOptions.OnCloseMainOptionsClick();
            }
        }
    }
}
