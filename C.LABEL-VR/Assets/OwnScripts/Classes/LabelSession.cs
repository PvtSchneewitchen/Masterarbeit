using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LabelSession
{
    public string _sessionName { get; set; }
    public List<PointCloud> _pointClouds { get; set; }
    private int _currentCLoud { get; set; }

    public LabelSession (List<PointCloud> pointClouds_inp, int iCurrentCloud)
	{
        _pointClouds = pointClouds_inp;
        _currentCLoud = iCurrentCloud;
	}

    public LabelSession(SessionSaveFile saveFile_inp)
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

        InGameOptions._bAttachOptionsToCamera = saveFile_inp._ingameOptions._bAttachOptionsToCamera;
        InGameOptions._bDecreasePointsWhenMoving = saveFile_inp._ingameOptions._bDecreasePointsWhenMoving;
        InGameOptions._bSicknessPrevention_TeleportWithBlink = saveFile_inp._ingameOptions._bSicknessPrevention_TeleportWithBlink;
        InGameOptions._fFreeFly_AccelerationRot = saveFile_inp._ingameOptions._fFreeFly_AccelerationRot;
        InGameOptions._fFreeFly_AccelerationTrans = saveFile_inp._ingameOptions._fFreeFly_AccelerationTrans;
        InGameOptions._fFreeFly_MaxSpeedRot = saveFile_inp._ingameOptions._fFreeFly_MaxSpeedRot;
        InGameOptions._fFreeFly_MaxSpeedTrans = saveFile_inp._ingameOptions._fFreeFly_MaxSpeedTrans;
        InGameOptions._fSicknessPrevention_TeleportDistance = saveFile_inp._ingameOptions._fSicknessPrevention_TeleportDistance;
        InGameOptions._fSicknessPrevention_TurnAngle = saveFile_inp._ingameOptions._fSicknessPrevention_TurnAngle;
        InGameOptions._movementMode = saveFile_inp._ingameOptions._movementMode;

        InGameOptions.InitUiComponentValues();

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

    public PointCloud GetCurrentPointCloud ()
	{
        Debug.Log(_pointClouds.Count);
        return _pointClouds.ElementAt(_currentCLoud);
	}

	public PointCloud GetPointcloud (int index_inp)
	{
        return _pointClouds.ElementAt(index_inp);
    }

	public PointCloud GetNextPointCloud ()
	{
        _currentCLoud++;
		if (_currentCLoud >= _pointClouds.Count)
            _currentCLoud = 0;

		return _pointClouds.ElementAt(_currentCLoud);
	}

	public PointCloud GetPreviousPointCloud ()
	{
        _currentCLoud--;
        if (_currentCLoud >= _pointClouds.Count)
            _currentCLoud = 0;

        return _pointClouds.ElementAt(_currentCLoud);
    }
}
