using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlScript : MonoBehaviour
{

    private string _sLoadPath;
    private Util.Datatype _dataTypeToLoad;
    private bool _bLoadSingleFile;
    private bool _bOptionMode;

    // Use this for initialization
    void Start()
    {
        _sLoadPath = Util.DataLoadInfo.LoadDataPath();
        _dataTypeToLoad = Util.DataLoadInfo.LoadDataType();
        _bLoadSingleFile = Util.DataLoadInfo.LoadFileQuantityInfo();
        _bOptionMode = false;

        Debug.Log("Path: " + _sLoadPath + "  Type: " + _dataTypeToLoad + "    SingleFile?: " + _bLoadSingleFile);

        if (_dataTypeToLoad == Util.Datatype.pcd)
        {
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
                //todo disable unnecessary things when usion option menu
                GameObject.Find("MovementController").GetComponent<Movement>()._bMovementEnabled = false;

                GameObject.Find("InGameOptions").GetComponent<InGameOptionsController>().EnableOptionMenu();
                _bOptionMode = true;
            }
        }
        else
        {
            if (OVRInput.GetDown(OVRInput.Button.Start))
            {
                //todo enable things going back from option mode
                GameObject.Find("MovementController").GetComponent<Movement>()._bMovementEnabled = true;

                GameObject.Find("InGameOptions").GetComponent<InGameOptionsController>().DisableOptionMenu();
                _bOptionMode = false;
            }
        }
    }
}
