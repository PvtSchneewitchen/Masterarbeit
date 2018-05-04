using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRTK;

public class AppMenu_Movement : Menu<AppMenu_Movement>
{
    public Dropdown dropdown_MoveMode;
    public Toggle toggle_ReducePoints;
    public InputField inputfield_TransSpeed;
    public InputField inputfield_TransAcceleration;
    public InputField inputfield_RotSpeed;
    public InputField inputfield_RotAcceleration;
    public InputField inputfield_TeleportDistance;
    public InputField inputfield_TeleportAngle;
    public Toggle toogle_Twinkle;

    private InputField currentInputfield;

    public static void Show()
    {
        Open();
        MenuManager.Instance.OnMenuOpenRoutine();
        Instance.SetupUiComponents();
    }

    private void SetupUiComponents()
    {
        MovementOptions.LoadFromSessionPath(Util.DataLoadInfo._sessionFolderPath);

        Instance.dropdown_MoveMode.value = (int)MovementOptions.MoveMode;
        Instance.toggle_ReducePoints.isOn = MovementOptions.ReducePoints;
        Instance.inputfield_TransSpeed.text = Convert.ToString(MovementOptions.TransSpeed);
        Instance.inputfield_TransAcceleration.text = Convert.ToString(MovementOptions.TransAcceleration);
        Instance.inputfield_RotSpeed.text = Convert.ToString(MovementOptions.RotSpeed);
        Instance.inputfield_RotAcceleration.text = Convert.ToString(MovementOptions.RotAcceleration);
        Instance.inputfield_TeleportDistance.text = Convert.ToString(MovementOptions.TeleportDistance);
        Instance.inputfield_TeleportAngle.text = Convert.ToString(MovementOptions.TeleportAngle);
        Instance.toogle_Twinkle.isOn = MovementOptions.Twinkle;
    }

    #region OnClicks
    public void OnInputfieldClick(object objectInp, UIPointerEventArgs args)
    {
        GameObject clickedObject = args.currentTarget;

        if (MenuManager.Instance.menuStack.Peek() != Instance)
        {
            //this menu is not visible
            return;
        }

        InputField clickedInputField = clickedObject.GetComponent<InputField>();

        if (clickedInputField != null)
        {
            currentInputfield = clickedInputField;

            string userInput = "Enter the desired Value!";
            NumPad.Show(FillInputfield, userInput);
        }
    }



    public void OnLabelingClick()
    {
        AppMenu_Labeling.Show();
    }

    public void OnDefaultValuesClick()
    {
        MovementOptions.RestoreDefaultValues();
        SetupUiComponents();
    }

    public void OnExportClick()
    {
        string userInfo = "Choose the directory you want for the export!";
        FileBrowserScript.Show(Util.DataLoadInfo._sourceDataPath, "", StartExport, userInfo);
    }

    public void OnMainMenuClick()
    {
        LoadingScreen.Show();
        MovementOptions.SaveOptions();
        SessionSave.SaveSession(Util.DataLoadInfo._sessionFolderPath);
        Labeling.Reset();
        MetaData.Reset();

        SceneManager.LoadScene(0);
    }

    public void OnResumeClick()
    {
        MenuManager.Instance.OnMenuCloseRoutine();
        Close();
        AppMenu_Labeling.Close();
    }

    #endregion

    #region OnValueChanges
    public void OnMovementModeChange(int newModeIndex)
    {
        MovementOptions.MoveMode = (Util.MovementMode)newModeIndex;
    }

    public void OnReducePointsChange(bool reduce)
    {
        MovementOptions.ReducePoints = reduce;
    }

    public void OnTransSpeedChange(string input)
    {
        MovementOptions.TransSpeed = float.Parse(input);
    }

    public void OnTransAccelerationChange(string input)
    {
        MovementOptions.TransAcceleration = float.Parse(input);
    }

    public void OnRotSpeedChange(string input)
    {
        MovementOptions.RotSpeed = float.Parse(input);
    }

    public void OnRotAccelerationChange(string input)
    {
        MovementOptions.RotAcceleration = float.Parse(input);
    }

    public void OnTeleportDistanceChange(string input)
    {
        MovementOptions.TeleportDistance = float.Parse(input);
    }

    public void OnTeleportAngleChange(string input)
    {
        MovementOptions.TeleportAngle = float.Parse(input);
    }

    public void OnTwinkleChange(bool twinkle)
    {
        MovementOptions.Twinkle = twinkle;
    }
    #endregion

    #region Callbacks
    private void StartExport(string path)
    {
        ReferenceHandler.Instance.GetRightPointerRenderer().enabled = false;
        LoadingScreen.Show();

        FileAttributes attr = File.GetAttributes(path);

        if (!attr.HasFlag(FileAttributes.Directory))
            return;

        if (Util.DataLoadInfo._dataType == Util.Datatype.pcd)
        {
            Export.ExportPcd(path);
        }
        else if (Util.DataLoadInfo._dataType == Util.Datatype.hdf5_DaimlerLidar)
        {
            Export.ExportHdf5_DaimlerLidar(path);
        }
        LoadingScreen.Close();
        ReferenceHandler.Instance.GetRightPointerRenderer().enabled = true;
    }

    private void FillInputfield(string enteredValue)
    {
        currentInputfield.text = enteredValue;
    }
    #endregion
}
