using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.IO;
using VRTK;
using System;

public class AppMenu_Labeling : Menu<AppMenu_Labeling>
{
    public ScrollRect labelClassView;

    public GameObject labelClassItemPrefab;

    private List<GameObject> labelClassItems = new List<GameObject>();

    private Button currentSelectedButton;
    private uint oldID;

    private Color32 selectedColor = new Color32(79, 186, 187, 255);
    private Color32 oldNormalColor;

    public static void Show()
    {
        Open();
        Instance.RefreshLabelClassView();
    }

    private void RefreshLabelClassView()
    {
        for (int i = 0; i < Instance.labelClassItems.Count; i++)
        {
            Destroy(Instance.labelClassItems[i]);
        }
        Instance.labelClassItems.Clear();

        var classInfos = Labeling.GetAllIdsNamesAndMaterials();
        foreach (var classInfo in classInfos)
        {
            GameObject classItem = Instantiate(labelClassItemPrefab, Instance.labelClassView.content.transform);

            var attributes = classItem.GetComponent<LabelClassItem>();
            attributes.ClassName = classInfo.Value.Item1;
            attributes.ClassID = classInfo.Key;
            attributes.ClassColor = classInfo.Value.Item2.color;

            Instance.labelClassItems.Add(classItem);
        }
    }

    private void SelectNewButton(Button button)
    {
        ColorBlock colorBlock = button.colors;

        Instance.oldNormalColor = colorBlock.normalColor;
        colorBlock.normalColor = Instance.selectedColor;

        button.colors = colorBlock;

        Instance.currentSelectedButton = button;
    }

    private void DeselectCurrentButton()
    {
        try
        {
            ColorBlock colorBlock = Instance.currentSelectedButton.colors;

            colorBlock.normalColor = oldNormalColor;

            Instance.currentSelectedButton.colors = colorBlock;

            Instance.currentSelectedButton = null;
        }
        catch { }
    }

    #region OnClicks
    public void OnMovementClick()
    {
        AppMenu_Movement.Show();
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
        AppMenu_Movement.Close();
    }

    public void OnClassItemClick(object objectInp, UIPointerEventArgs args)
    {
        GameObject clickedObject = args.currentTarget;

        if (MenuManager.Instance.MenuStack.Peek() != Instance)
        {
            //this menu is not visible
            return;
        }

        Button clickedButton = clickedObject.GetComponent<Button>();

        if (clickedButton != null && clickedButton.name.Contains("LabelClassItem"))
        {
            if (Instance.currentSelectedButton != null && Instance.currentSelectedButton.GetInstanceID() == clickedButton.GetInstanceID())
            {
                DeselectCurrentButton();
            }
            else
            {
                DeselectCurrentButton();
                SelectNewButton(clickedButton);
            }
        }
    }

    public void OnResetClick()
    {
        ReferenceHandler.Instance.GetSessionHandler().Session.GetCurrentPointCloud().ResetLabels();
    }

    public void OnAddClick()
    {
        LabelClassEditor.Show(AddNewLabelClass, LabelClassEditor.AccessMode.Create, new LabelClassEditor.LabelCLassItemStruct(), "Create the desired LabelClass!");
    }

    private void AddNewLabelClass(LabelClassEditor.LabelCLassItemStruct newLabelCLass)
    {
        Labeling.AddSingleLabelClass(newLabelCLass.ID, newLabelCLass.Name, newLabelCLass.Color);
        RefreshLabelClassView();
    }

    public void OnEditClick()
    {
        LabelClassItem classItem = currentSelectedButton.GetComponent<LabelClassItem>();
        oldID = classItem.ClassID;

        LabelClassEditor.LabelCLassItemStruct lciStruct = new LabelClassEditor.LabelCLassItemStruct
        {
            Color = classItem.ClassColor,
            ID = classItem.ClassID,
            Name = classItem.ClassName
        };

        LabelClassEditor.Show(ChangeLabelClass, LabelClassEditor.AccessMode.Edit, lciStruct, "Create the desired LabelClass!");
    }

    private void ChangeLabelClass(LabelClassEditor.LabelCLassItemStruct editedLabelClass)
    {
        Labeling.EditSingleLabelClass(oldID, editedLabelClass.ID, editedLabelClass.Name, editedLabelClass.Color);
        ReferenceHandler.Instance.GetSessionHandler().Session.GetCurrentPointCloud().RefreshPointsOfLabelCLass(oldID, editedLabelClass.ID);
        RefreshLabelClassView();
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
    #endregion
}
