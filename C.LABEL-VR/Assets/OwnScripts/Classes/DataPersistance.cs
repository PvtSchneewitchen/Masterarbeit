//using System;
//using System.Linq;
//using System.Collections.Generic;
//using System.IO;
//using System.Runtime.Serialization.Formatters.Binary;
//using UnityEngine;

//public static class DataPersistance
//{
//    public static Session _sessionStorage { get; private set; }

//    static DataPersistance()
//    {

//    }

//    public static void StoreSesseion(LabelSession session_inp)
//    {
//        var pointClouds = session_inp._pointClouds;

//        for (int i = 0; i < pointClouds.Count; i++)
//        {
//            int pointCloudID = pointClouds.ElementAt(i).Key;
//            PointCloud pointCloud = pointClouds.ElementAt(i).Value;
//            List<Vector3> positions = new List<Vector3>();
//            List<int> IDs = new List<int>();
//            List<Tuple<string, uint>> labels = new List<Tuple<string, uint>>();
//            List<int> groundPointLabels = new List<int>();

//            for (int j = 0; j < pointCloud._validPoints.Count; j++)
//            {
//                GameObject point = pointCloud._validPoints.ElementAt(i);
//                CustomAttributes attributes = point.GetComponent<CustomAttributes>();

//                positions.Add(point.transform.position);
//                IDs.Add(attributes._ID);
//                labels.Add(attributes._label);
//                groundPointLabels.Add(attributes._groundPoint);
//            }

//            _sessionStorage._positions.Add(i, positions);
//            _sessionStorage._IDs.Add(i, IDs);
//            _sessionStorage._labels.Add(i, labels);
//            _sessionStorage._groundPointLabels.Add(i, groundPointLabels);
//        }
//    }

//    [Serializable]
//    public class Session
//    {
//        public Dictionary<int, List<Vector3>> _positions { get; set; }
//        public Dictionary<int, List<int>> _IDs { get; set; }
//        public Dictionary<int, List<Tuple<string, uint>>> _labels { get; set; }
//        public Dictionary<int, List<int>> _groundPointLabels { get; set; }

//        public Session()
//        {
//            _positions = new Dictionary<int, List<Vector3>>();
//            _IDs = new Dictionary<int, List<int>>();
//            _labels = new Dictionary<int, List<Tuple<string, uint>>>();
//            _groundPointLabels = new Dictionary<int, List<int>>();
//        }
//    }

//    [Serializable]
//    public class Options
//    {

//    }

//    [Serializable]
//    public class MetaData
//    {

//    }

//    //private static string _sessionName = "Session1";

//    //public static void SaveLabelSession(LabelSession session_inp)
//    //{
//    //    SessionSaveFile._labelsession._pointClouds = session_inp._pointClouds;
//    //    SessionSaveFile._labelsession._currentCLoud = session_inp.GetCurrentPointCloudIndex();
//    //    Saving.SaveSessionAtPersistenceDataPath();
//    //}

//    //public static void SaveIngameOptions()
//    //{
//    //    SessionSaveFile._ingameOptions._bAttachOptionsToCamera = InGameOptions._bAttachOptionsToCamera;
//    //    SessionSaveFile._ingameOptions._bDecreasePointsWhenMoving = InGameOptions._bDecreasePointsWhenMoving;
//    //    SessionSaveFile._ingameOptions._bSicknessPrevention_TeleportWithBlink = InGameOptions._bSicknessPrevention_TeleportWithBlink;
//    //    SessionSaveFile._ingameOptions._fFreeFly_AccelerationRot = InGameOptions._fFreeFly_AccelerationRot;
//    //    SessionSaveFile._ingameOptions._fFreeFly_AccelerationTrans = InGameOptions._fFreeFly_AccelerationTrans;
//    //    SessionSaveFile._ingameOptions._fFreeFly_MaxSpeedRot = InGameOptions._fFreeFly_MaxSpeedRot;
//    //    SessionSaveFile._ingameOptions._fFreeFly_MaxSpeedTrans = InGameOptions._fFreeFly_MaxSpeedTrans;
//    //    SessionSaveFile._ingameOptions._fSicknessPrevention_TeleportDistance = InGameOptions._fSicknessPrevention_TeleportDistance;
//    //    SessionSaveFile._ingameOptions._fSicknessPrevention_TurnAngle = InGameOptions._fSicknessPrevention_TurnAngle;
//    //    SessionSaveFile._ingameOptions._movementMode = InGameOptions._movementMode;
//    //    Saving.SaveSessionAtPersistenceDataPath();
//    //}

//    //public static void SaveMetaData(ExportMetaData metaData_inp)
//    //{
//    //    ExportMetaData.MetaDataType type = metaData_inp._metaDataType;

//    //    if (type == ExportMetaData.MetaDataType.Hdf5_DaimlerLidar)
//    //    {
//    //        SessionSaveFile._exportMetaData.Hdf5_DaimlerLidar._distance = metaData_inp.Hdf5_DaimlerLidar

//    //    }
//    //    SessionSaveFile.
//    //    Saving.SaveSessionAtPersistenceDataPath();
//    //}

//    //public static class Saving
//    //{
//    //    public static void SaveSessionAtPersistenceDataPath()
//    //    {
//    //        Stream stream = File.OpenWrite(Application.persistentDataPath + _sessionName);
//    //        BinaryFormatter binFormatter = new BinaryFormatter();

//    //        binFormatter.Serialize(stream, _sessionSaveFile);

//    //        stream.Flush();
//    //        stream.Close();
//    //        stream.Dispose();
//    //    }
//    //}

//    //public static class Loading
//    //{
//    //    public static SessionSaveFile LoadSessionFromPersistenceDataPath()
//    //    {
//    //        BinaryFormatter binFormatter = new BinaryFormatter();
//    //        FileStream fileStream = File.Open(Application.persistentDataPath + _sessionName, FileMode.Open);
//    //        object loadedObject = binFormatter.Deserialize(fileStream);
//    //        SessionSaveFile loadedSession = (SessionSaveFile)loadedObject;

//    //        fileStream.Flush();
//    //        fileStream.Close();
//    //        fileStream.Dispose();

//    //        return loadedSession;
//    //    }
//    //}
//}
