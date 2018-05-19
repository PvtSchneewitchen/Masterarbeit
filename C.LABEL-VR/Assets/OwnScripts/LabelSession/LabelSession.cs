using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LabelSession
{
    public string SessionName { get; set; }
    public List<PointCloud> PointClouds { get; set; }
    private int currentCloud { get; set; }

    public LabelSession(List<PointCloud> pointClouds_inp, int iCurrentCloud)
    {
        PointClouds = pointClouds_inp;
        currentCloud = iCurrentCloud;
    }

    public LabelSession(SessionSave saveFile_inp)
    {
        Labeling.SetSavedLabelClasses(saveFile_inp._labelsession.GetLabelWorkingSet());
        SessionName = saveFile_inp._labelsession.GetSessionName();
        currentCloud = saveFile_inp._labelsession.GetCurrentCloudID();
        PointClouds = saveFile_inp._labelsession.GetPointClouds();

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
        return currentCloud;
    }

    public PointCloud GetCurrentPointCloud()
    {
        return PointClouds.ElementAt(currentCloud);
    }

    public PointCloud GetPointcloud(int index_inp)
    {
        return PointClouds.ElementAt(index_inp);
    }

    public void ShowNextPointCloud()
    {
        if (PointClouds.Count > 1)
        {
            PointClouds.ElementAt(currentCloud).DisableAllPoints();

            currentCloud++;
            if (currentCloud >= PointClouds.Count)
                currentCloud = 0;

            PointClouds.ElementAt(currentCloud).EnableAllPoints();
        }
    }

    public void ShowPreviousPointCloud()
    {
        if (PointClouds.Count > 1)
        {
            PointClouds.ElementAt(currentCloud).DisableAllPoints();

            currentCloud--;
            if (currentCloud <= 0)
                currentCloud = PointClouds.Count-1;

            PointClouds.ElementAt(currentCloud).EnableAllPoints();
        }
    }
}
