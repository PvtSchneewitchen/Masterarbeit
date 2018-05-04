using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SessionHandler : MonoBehaviour
{
    public static SessionHandler Instance { get; private set; }

    public LabelSession Session { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void Start()
    {
//#if UNITY_EDITOR
//        Util.DataLoadInfo._dataType = Util.Datatype.pcd;
//        Util.DataLoadInfo._accessMode = Util.AccesMode.Create;
//        Util.DataLoadInfo._sessionName = "EditorDev";

//        if (Util.DataLoadInfo._dataType == Util.Datatype.pcd)
//            Util.DataLoadInfo._sourceDataPath = "C:\\Users\\gruepazu\\Desktop\\PointClouds\\000000000_LidarImage_000000002.pcd";
//        else if (Util.DataLoadInfo._dataType == Util.Datatype.hdf5_DaimlerLidar)
//            Util.DataLoadInfo._sourceDataPath = "C:\\Users\\gruepazu\\Desktop\\LidarDaten\\DatenDAG\\2017-08-18_090334\\LidarImages_03_05\\LidarImage_000003049.hdf5";
//#endif

        if (Util.DataLoadInfo._accessMode == Util.AccesMode.Create)
        {
            CreateSessionFolder();
            MovementOptions.LoadFromSessionPath(Util.DataLoadInfo._sessionFolderPath);

            List<PointCloud> pointClouds = new List<PointCloud>();

            if (Util.DataLoadInfo._dataType == Util.Datatype.pcd)
            {
                pointClouds = Import.ImportPcd(Util.DataLoadInfo._sourceDataPath);
            }
            else if (Util.DataLoadInfo._dataType == Util.Datatype.hdf5_DaimlerLidar)
            {
                pointClouds = Import.ImportHdf5_DaimlerLidar(Util.DataLoadInfo._sourceDataPath);
            }

            Session = new LabelSession(pointClouds, 0);
        }
        else
        {
            SessionSave saveFile;

            using (Stream stream = File.Open(Util.DataLoadInfo._sourceDataPath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                saveFile = binaryFormatter.Deserialize(stream) as SessionSave;
            }

            Session = new LabelSession(saveFile);
        }

        Session.GetCurrentPointCloud().EnableAllPoints();
        Session._sessionName = Util.DataLoadInfo._sessionName;
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
        CheckCloudChangeButton();
    }

    private void CheckCloudChangeButton()
    {
        if (OVRInput.GetDown(OVRInput.Button.Four))
        {
            Session.ShowNextPointCloud();
        }
        else if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            Session.ShowPreviousPointCloud();
        }
    }
}