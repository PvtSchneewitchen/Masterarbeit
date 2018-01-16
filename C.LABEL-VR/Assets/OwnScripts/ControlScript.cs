using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlScript : MonoBehaviour
{
    public InGameOptionsController _inGameOptions;

    public bool _bOptionMode { get; set; }

    private string _sLoadPath;
    private Util.Datatype _dataTypeToLoad;
    private bool _bLoadSingleFile;

    //Test
    public LabelSession session { get; private set; }

    public PointCLoud currentCLoud { get; private set; }

    public Labeler sessionLabeler { get; set; }
    //

    // Use this for initialization
    void Awake()
    {
        Util.InGameOptions.LoadOptions();

        _sLoadPath = Util.DataLoadInfo.sDataPath;
        _dataTypeToLoad = Util.DataLoadInfo.dataType;
        _bLoadSingleFile = Util.DataLoadInfo.bIsSingleFile;
        _bOptionMode = false;

        Debug.Log("Path: " + _sLoadPath + "  Type: " + _dataTypeToLoad + "    SingleFile?: " + _bLoadSingleFile);

#if UNITY_EDITOR
        _sLoadPath = "C:\\Users\\gruepazu\\Desktop\\PointClouds\\000000000_LidarImage_000000002.pcd";
        _dataTypeToLoad = Util.Datatype.pcd;
        _bLoadSingleFile = true;
#endif

        if (_dataTypeToLoad == Util.Datatype.pcd)
        {
            //for now
            GameObject.Find("Floor").SetActive(false);
            GameObject.Find("ExampleWorldObjects").SetActive(false);
            GameObject camera = Util.FindInactiveGameobject(GameObject.Find("[VRTK_SDKManager]"), "CenterEyeAnchor");
            camera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
            camera.GetComponent<Camera>().backgroundColor = Color.white;

            session = new LabelSession();
            currentCLoud = new PointCLoud();
            sessionLabeler = new Labeler();

            List<KeyValuePair<int, List<Vector3>>> Coordinates = new List<KeyValuePair<int, List<Vector3>>>();

            Coordinates = PcdReader.GetCoordinatesFromSinglePcd(_sLoadPath);

            session = new LabelSession(PointCLoud.CreateListOfPointclouds(Coordinates));
            currentCLoud = session.pointClouds[0];
            currentCLoud.EnableAllPoints();

            //TODO Create new data representation, adapt pcd reader, call function
        }
        else if (_dataTypeToLoad == Util.Datatype.hdf5)
        {
            //TODO Implement hdf Import and call it here
        }
        else if (_dataTypeToLoad == Util.Datatype.lidar)
        {
            //TODO Implement lidar Import and call it here
        }


    }

    // Update is called once per frame
    void Update()
    {
        CheckOptionButton();
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
