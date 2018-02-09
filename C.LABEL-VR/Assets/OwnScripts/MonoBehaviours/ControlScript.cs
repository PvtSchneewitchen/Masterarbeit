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
        _sLoadPath = "C:\\Users\\gruepazu\\Desktop\\PointClouds\\000000000_LidarImage_000000002.pcd";
        _dataTypeToLoad = Util.Datatype.pcd;
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
            //TODO Implement hdf Import and call it here
        }
        else if (_dataTypeToLoad == Util.Datatype.lidar)
        {
            //TODO Implement lidar Import and call it here
        }

        _session = new Session(pointClouds, 0);
        _session.GetCurrentPointCloud().EnableAllPoints();

        Util.Labeling.SetCurrentGroup(Util.Labeling.LabelGroup.motorcycle);

        //CombineMeshes();
    }

    // Update is called once per frame
    void Update()
    {
        CheckOptionButton();
    }

    private void LoadSettings()
    {
        Util.InGameOptions.LoadOptions();

        _sLoadPath = Util.DataLoadInfo.sDataPath;
        _dataTypeToLoad = Util.DataLoadInfo.dataType;
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

    private void CombineMeshes()
    {
        GameObject origin = GameObject.Find("Origin");

        for (int j = 0; j < _session.GetCurrentPointCloud()._points.Count; j++)
        {
            _session.GetCurrentPointCloud()._points[j].transform.parent = origin.transform;
        }

        MeshFilter[] meshFilters = origin.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
            i++;
        }
        origin.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        origin.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        origin.transform.gameObject.SetActive(true);
    }
}
