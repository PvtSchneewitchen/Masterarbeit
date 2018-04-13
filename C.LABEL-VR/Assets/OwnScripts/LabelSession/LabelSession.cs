using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LabelSession
{
    public string _sessionName { get; set; }
    public List<PointCloud> _pointClouds { get; set; }
    private int _currentCLoud { get; set; }

    public LabelSession(List<PointCloud> pointClouds_inp, int iCurrentCloud)
    {
        _pointClouds = pointClouds_inp;
        _currentCLoud = iCurrentCloud;
    }

    public LabelSession(SessionSave saveFile_inp)
    {
        Labeling.SetNewLabelClasses(saveFile_inp._labelsession.GetLabelWorkingSet());
        _sessionName = saveFile_inp._labelsession.GetSessionName();
        _currentCLoud = saveFile_inp._labelsession.GetCurrentCloudID();
        _pointClouds = saveFile_inp._labelsession.GetPointClouds();

        Util.DataLoadInfo._accessMode = Util.AccesMode.Load;
        Util.DataLoadInfo._dataType = saveFile_inp._labelsession.GetSessionDataType();
        Util.DataLoadInfo._sourceDataPath = saveFile_inp._labelsession.GetSessionSourcePath();
        Util.DataLoadInfo._sessionName = saveFile_inp._labelsession.GetSessionName();
        Util.DataLoadInfo._sessionFolderPath = Application.persistentDataPath + "/" + Util.DataLoadInfo._sessionName;

        MovementOptions.ReducePoints = saveFile_inp._ingameOptions._bDecreasePointsWhenMoving;
        MovementOptions.Twinkle = saveFile_inp._ingameOptions._bSicknessPrevention_TeleportWithBlink;
        MovementOptions.RotAcceleration = saveFile_inp._ingameOptions._fFreeFly_AccelerationRot;
        MovementOptions.TransAcceleration = saveFile_inp._ingameOptions._fFreeFly_AccelerationTrans;
        MovementOptions.RotSpeed = saveFile_inp._ingameOptions._fFreeFly_MaxSpeedRot;
        MovementOptions.TransSpeed = saveFile_inp._ingameOptions._fFreeFly_MaxSpeedTrans;
        MovementOptions.TeleportDistance = saveFile_inp._ingameOptions._fSicknessPrevention_TeleportDistance;
        MovementOptions.TeleportAngle = saveFile_inp._ingameOptions._fSicknessPrevention_TurnAngle;
        MovementOptions.MoveMode = saveFile_inp._ingameOptions._movementMode;
        MovementOptions.SaveOptions();

        if (Util.DataLoadInfo._dataType == Util.Datatype.hdf5_DaimlerLidar)
        {
            MetaData.Hdf5_DaimlerLidar._tableIndexToID = saveFile_inp._exportMetaData._hdf5_DaimlerLidar.GetTableIndexToID();
            MetaData.Hdf5_DaimlerLidar._importedContainers = saveFile_inp._exportMetaData._hdf5_DaimlerLidar.GetImportedContainers();
        }

    }

    public int GetCurrentPointCloudIndex()
    {
        return _currentCLoud;
    }

    public PointCloud GetCurrentPointCloud()
    {
        return _pointClouds.ElementAt(_currentCLoud);
    }

    public PointCloud GetPointcloud(int index_inp)
    {
        return _pointClouds.ElementAt(index_inp);
    }

    public void ShowNextPointCloud()
    {
        if (_pointClouds.Count > 1)
        {
            _pointClouds.ElementAt(_currentCLoud).DisableAllPoints();

            _currentCLoud++;
            if (_currentCLoud >= _pointClouds.Count)
                _currentCLoud = 0;

            _pointClouds.ElementAt(_currentCLoud).EnableAllPoints();
        }
    }

    public void ShowPreviousPointCloud()
    {
        if (_pointClouds.Count > 1)
        {
            _pointClouds.ElementAt(_currentCLoud).DisableAllPoints();

            _currentCLoud--;
            if (_currentCLoud <= 0)
                _currentCLoud = _pointClouds.Count-1;

            _pointClouds.ElementAt(_currentCLoud).EnableAllPoints();
        }
    }
}
