using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class SessionSaveFile
{
    public SessionSave_LabelSession _labelsession { get; set; }
    public Sessionsave_MovementOptions _ingameOptions { get; set; }
    public SessionSave_MetaData _exportMetaData { get; set; }

    public SessionSaveFile()
    {
        _labelsession = new SessionSave_LabelSession();
        _ingameOptions = new Sessionsave_MovementOptions();
        _exportMetaData = new SessionSave_MetaData();
    }

    [Serializable]
    public class SessionSave_LabelSession
    {
        //Serializable Pointclouds
        private List<List<float>> _positionsX;
        private List<List<float>> _positionsY;
        private List<List<float>> _positionsZ;
        private List<List<int>> _IDs;
        private List<List<uint>> _Label;
        private List<List<int>> _groundPoint;
        private Dictionary<uint, string> _labelWorkingSet;
        private List<string> _pathToPointCloudData;
        private int _currentCLoud;
        private Util.Datatype _sessionDataType;
        private string _sessionSourcePath;
        private string _sessionName;

        public SessionSave_LabelSession()
        {
            _positionsX = new List<List<float>>();
            _positionsY = new List<List<float>>();
            _positionsZ = new List<List<float>>();
            _groundPoint = new List<List<int>>();
            _IDs = new List<List<int>>();
            _Label = new List<List<uint>>();
            _labelWorkingSet = new Dictionary<uint, string>();
            _pathToPointCloudData = new List<string>();
        }

        public void AssignLabelSessionValues()
        {
            ControlScript ctrl = GameObject.Find("AppController").GetComponent<ControlScript>();

            _sessionDataType = Util.DataLoadInfo._dataType;
            _sessionSourcePath = Util.DataLoadInfo._sourceDataPath;
            _sessionName = ctrl._session._sessionName;
            _currentCLoud = ctrl._session.GetCurrentPointCloudIndex();
            _labelWorkingSet = Labeling.GetLabelWorkingSet();

            List<PointCloud> _pointClouds = ctrl._session._pointClouds;

            for (int i = 0; i < _pointClouds.Count; i++)
            {
                List<GameObject> points = _pointClouds[i]._validPoints;
                _pathToPointCloudData.Add(_pointClouds[i]._pathToPointCloudData);
                _positionsX.Add(new List<float>());
                _positionsY.Add(new List<float>());
                _positionsZ.Add(new List<float>());
                _IDs.Add(new List<int>());
                _Label.Add(new List<uint>());
                _groundPoint.Add(new List<int>());
                for (int j = 0; j < points.Count; j++)
                {
                    Vector3 pos = points[j].transform.position;
                    CustomAttributes attr = points[j].GetComponent<CustomAttributes>();
                    _positionsX[i].Add(pos.x);
                    _positionsY[i].Add(pos.y);
                    _positionsZ[i].Add(pos.z);
                    _IDs[i].Add(attr._ID);
                    _Label[i].Add(attr._label);
                    _groundPoint[i].Add(attr._groundPoint);
                }
            }
        }

        public List<PointCloud> GetPointClouds()
        {
            List<PointCloud> clouds_out = new List<PointCloud>();

            for (int i = 0; i < _positionsX.Count; i++)
            {
                List<InternalDataFormat> idfList = new List<InternalDataFormat>();
                for (int j = 0; j < _positionsX[i].Count; j++)
                {
                    int id = _IDs[i][j];
                    uint label = _Label[i][j];
                    int groundPoint = _groundPoint[i][j];
                    Vector3 pos = new Vector3(_positionsX[i][j], _positionsY[i][j], _positionsZ[i][j]);

                    idfList.Add(new InternalDataFormat(id, pos, label, groundPoint));
                }
                clouds_out.Add(new PointCloud(idfList, _pathToPointCloudData[i], Quaternion.Euler(0, 0, 0), Vector3.one));
            }

            return clouds_out;
        }

        public Dictionary<uint, string> GetLabelWorkingSet()
        {
            return _labelWorkingSet;
        }

        public int GetCurrentCloudID()
        {
            return _currentCLoud;
        }

        public string GetSessionName()
        {
            return _sessionName;
        }

        public string GetSessionSourcePath()
        {
            return _sessionSourcePath;
        }

        public Util.Datatype GetSessionDataType()
        {
            return _sessionDataType;
        }
    }

    [Serializable]
    public class Sessionsave_MovementOptions
    {
        public Util.MovementMode _movementMode { get; set; }
        public float _fFreeFly_MaxSpeedTrans { get; set; }
        public float _fFreeFly_AccelerationTrans { get; set; }
        public float _fFreeFly_MaxSpeedRot { get; set; }
        public float _fFreeFly_AccelerationRot { get; set; }
        public float _fSicknessPrevention_TeleportDistance { get; set; }
        public float _fSicknessPrevention_TurnAngle { get; set; }
        public bool _bSicknessPrevention_TeleportWithBlink { get; set; }
        public bool _bDecreasePointsWhenMoving { get; set; }

        public void AssignInGameOptionValues()
        {
            _movementMode = MovementOptions.MoveMode;
            _bDecreasePointsWhenMoving = MovementOptions.ReducePoints;
            _bSicknessPrevention_TeleportWithBlink = MovementOptions.Twinkle;
            _fFreeFly_AccelerationRot = MovementOptions.RotAcceleration;
            _fFreeFly_AccelerationTrans = MovementOptions.TransAcceleration;
            _fFreeFly_MaxSpeedRot = MovementOptions.RotSpeed;
            _fFreeFly_MaxSpeedTrans = MovementOptions.TransSpeed;
            _fSicknessPrevention_TeleportDistance = MovementOptions.TeleportDistance;
            _fSicknessPrevention_TurnAngle = MovementOptions.TeleportAngle;
        }
    }

    [Serializable]
    public class SessionSave_MetaData
    {
        public SessionSave_MetaData_HDF5DaimlerLidar _hdf5_DaimlerLidar { get; set; }

        [Serializable]
        public class SessionSave_MetaData_HDF5DaimlerLidar
        {
            private List<Dictionary<Tuple<int, int>, int>> _tableIndexToID { get; set; }
            private List<float[,]> _distances { get; set; }
            private List<float[,]> _intensity { get; set; }
            private List<float[,]> _labelProbabilities { get; set; }
            private List<Dictionary<uint, string>> _labelWorkingSet { get; set; }
            private List<uint[,]> _labels { get; set; }
            private List<int[,]> _pointValid { get; set; }
            private List<float[,]> _sensorX { get; set; }
            private List<float[,]> _sensorY { get; set; }
            private List<float[,]> _sensorZ { get; set; }
            private List<float[,]> _vehicleX { get; set; }
            private List<float[,]> _vehicleY { get; set; }
            private List<float[,]> _vehicleZ { get; set; }

            public SessionSave_MetaData_HDF5DaimlerLidar()
            {
                _tableIndexToID = new List<Dictionary<Tuple<int, int>, int>>();
                _labelWorkingSet = new List<Dictionary<uint, string>>();
                _labels = new List<uint[,]>();
                _pointValid = new List<int[,]>();
                _distances = new List<float[,]>();
                _intensity = new List<float[,]>();
                _labelProbabilities = new List<float[,]>();
                _sensorX = new List<float[,]>();
                _sensorY = new List<float[,]>();
                _sensorZ = new List<float[,]>();
                _vehicleX = new List<float[,]>();
                _vehicleY = new List<float[,]>();
                _vehicleZ = new List<float[,]>();
            }

            public void AssignMetaDataValues()
            {
                List<HDF5Addon.Hdf5Container_LidarDaimler> _importedContainer = MetaData.Hdf5_DaimlerLidar._importedContainers;
                _tableIndexToID = MetaData.Hdf5_DaimlerLidar._tableIndexToID;

                for (int i = 0; i < _importedContainer.Count; i++)
                {
                    _distances.Add(_importedContainer[i]._distances);
                    _intensity.Add(_importedContainer[i]._intensity);
                    _labelProbabilities.Add(_importedContainer[i]._labelProbabilities);
                    _labelWorkingSet.Add(_importedContainer[i]._labelWorkingSet);
                    _labels.Add(_importedContainer[i]._labels);
                    _pointValid.Add(_importedContainer[i]._pointValid);
                    _sensorX.Add(_importedContainer[i]._sensorX);
                    _sensorY.Add(_importedContainer[i]._sensorY);
                    _sensorZ.Add(_importedContainer[i]._sensorZ);
                    _vehicleX.Add(_importedContainer[i]._vehicleX);
                    _vehicleY.Add( _importedContainer[i]._vehicleY);
                    _vehicleZ.Add(_importedContainer[i]._vehicleZ);
                }
            }

            public List<Dictionary<Tuple<int, int>, int>> GetTableIndexToID()
            {
                return _tableIndexToID;
            }

            public List<HDF5Addon.Hdf5Container_LidarDaimler> GetImportedContainers()
            {
                List<HDF5Addon.Hdf5Container_LidarDaimler> containerList = new List<HDF5Addon.Hdf5Container_LidarDaimler>();

                for (int i = 0; i < _labels.Count; i++)
                {
                    int rows = _labels[i].GetLength(0);
                    int cols = _labels[i].GetLength(1);
                    HDF5Addon.Hdf5Container_LidarDaimler container = new HDF5Addon.Hdf5Container_LidarDaimler(rows, cols)
                    {
                        _distances = _distances[i],
                        _intensity = _intensity[i],
                        _labelProbabilities = _labelProbabilities[i],
                        _labels = _labels[i],
                        _labelWorkingSet = _labelWorkingSet[i],
                        _pointValid = _pointValid[i],
                        _sensorX = _sensorX[i],
                        _sensorY = _sensorY[i],
                        _sensorZ = _sensorZ[i],
                        _vehicleX = _vehicleX[i],
                        _vehicleY = _vehicleY[i],
                        _vehicleZ = _vehicleZ[i]
                    };

                    containerList.Add(container);
                }
                return containerList;
            }
        }
    }

    public static void SaveSession(string path_inp)
    {
        SessionSaveFile saveFile = new SessionSaveFile();

        //labelsession
        ControlScript ctrl = GameObject.Find("AppController").GetComponent<ControlScript>();

        SessionSave_LabelSession labelSession = new SessionSave_LabelSession();
        Sessionsave_MovementOptions ingameOptions = new Sessionsave_MovementOptions();

        labelSession.AssignLabelSessionValues();
        ingameOptions.AssignInGameOptionValues();

        saveFile._labelsession = labelSession;
        saveFile._ingameOptions = ingameOptions;

        //metadata
        if (Util.DataLoadInfo._dataType == Util.Datatype.hdf5_DaimlerLidar)
        {
            SessionSave_MetaData.SessionSave_MetaData_HDF5DaimlerLidar hdf5DL = new SessionSave_MetaData.SessionSave_MetaData_HDF5DaimlerLidar();
            hdf5DL.AssignMetaDataValues();

            saveFile._exportMetaData._hdf5_DaimlerLidar = hdf5DL;
        }

        string dataPath = path_inp + "/" + ctrl._session._sessionName + "SaveFile.dat";
        using (Stream stream = File.Open(dataPath, FileMode.Create))
        {
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            binaryFormatter.Serialize(stream, saveFile);
        }

        Debug.Log("Session Saved At: " + dataPath);
    }
}
