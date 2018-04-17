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


        //ground testing

        //var test = Labeling.GetLabelClassColor(Labeling.currentLabelClassID);

        //var clouds = _session._pointClouds;
        //for (int i = 0; i < clouds.Count; i++)
        //{
        //    var points = clouds[i]._validPoints;

        //    for (int j = 0; j < points.Count; j++)
        //    {
        //        var attr = points[j].GetComponent<CustomAttributes>();

        //        if (attr._groundPoint == 1)
        //        {
        //            attr._label = Labeling.currentLabelClassID;
        //        }
        //    }

        //}



        //var nonGrounds = _session.GetCurrentPointCloud()._validPoints.FindAll(x => x.GetComponent<CustomAttributes>()._groundPoint == 0);
        //Debug.Log(nonGrounds.Count);
        //var clusters = Clustering.GetClustersByGmeans(nonGrounds);

        //List<Color32> testcolors = new List<Color32>();
        //for (int i = 0; i < clusters.Count; i++)
        //{
        //    System.Random ran = new System.Random(i);
        //    var r = Convert.ToByte(ran.Next(0, 255));
        //    var g = Convert.ToByte(ran.Next(0, 255));
        //    var b = Convert.ToByte(ran.Next(0, 255));
        //    testcolors.Add(new Color32(r, g, b, Convert.ToByte(255)));
        //}

        //for (int i = 0; i < clusters.Count; i++)
        //{
        //    for (int j = 0; j < clusters[i].Count; j++)
        //    {
        //        clusters[i][j].GetComponent<CustomAttributes>()._clusterLabel = i;
        //        clusters[i][j].GetComponent<MeshRenderer>().material.color = testcolors[i];
        //    }
        //}

        

        //for (int i = 0; i < nonGrounds.Count; i++)
        //{
        //    var attr = nonGrounds[i].GetComponent<CustomAttributes>();

        //    nonGrounds[i].GetComponent<MeshRenderer>().material.color = testcolors[attr._clusterLabel];
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