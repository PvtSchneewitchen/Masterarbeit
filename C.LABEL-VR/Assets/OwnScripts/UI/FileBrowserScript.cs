using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using VRTK;
using System;

public class FileBrowserScript : Menu<FileBrowserScript>
{
    public Text userInfo;
    public Text pathText;
    public Text fileText;
    public ScrollRect directoryView;
    public ScrollRect fileView;
    public GameObject directoryItem;
    public GameObject fileItem;

    private string currentDirectoryPath;
    private string fileExtention;
    private string currentFilePath;

    private static Action<string> callBackMethod;

    private Dictionary<GameObject, string> directoryItems = new Dictionary<GameObject, string>();
    private Dictionary<GameObject, string> fileItems = new Dictionary<GameObject, string>();

    public static void Show(string startPath_inp, string fileExtention_inp, Action<string> callbackMethod_inp, string userInfo_inp)
    {
        callBackMethod = callbackMethod_inp;

        Open();
        Instance.userInfo.text = userInfo_inp;
        Instance.currentDirectoryPath = startPath_inp;
        Instance.fileExtention = fileExtention_inp;
        Instance.UpdateFileBrowser();
    }

    private void UpdateFileBrowser()
    {
        DirectoryInfo currentDirectory = new DirectoryInfo(Instance.currentDirectoryPath);
        Instance.pathText.text = Instance.currentDirectoryPath;

        UpdateDirectories(currentDirectory);
        UpdateFiles(currentDirectory);
    }

    private void UpdateDirectories(DirectoryInfo currentDirectory_inp)
    {
        DirectoryInfo[] directories = currentDirectory_inp.GetDirectories();

        ResetDirectories();

        for (int i = 0; i < directories.Length; i++)
        {
            string directoryName = directories[i].Name;
            string directoryPath = directories[i].FullName;

            GameObject currentDirectoryItem = Instantiate(directoryItem, Instance.directoryView.content.transform);
            currentDirectoryItem.GetComponentInChildren<Text>().text = directoryName;

            Instance.directoryItems.Add(currentDirectoryItem, directoryPath);
        }
    }

    private void UpdateFiles(DirectoryInfo currentDirectory_inp)
    {
        FileInfo[] files = currentDirectory_inp.GetFiles();

        ResetFiles();

        for (int i = 0; i < files.Length; i++)
        {
            string fileName = files[i].Name;
            string filePath = files[i].FullName;

            if (Instance.fileExtention == "" || Path.GetExtension(filePath) == Instance.fileExtention)
            {
                GameObject currentFileItem = Instantiate(fileItem, Instance.fileView.content.transform);
                currentFileItem.GetComponentInChildren<Text>().text = fileName;

                Instance.fileItems.Add(currentFileItem, filePath);
            }
        }
    }

    private void ResetDirectories()
    {
        for (int i = 0; i < Instance.directoryItems.Count; i++)
        {
            Destroy(Instance.directoryItems.ElementAt(i).Key);
        }
        Instance.directoryItems.Clear();
    }

    private void ResetFiles()
    {
        for (int i = 0; i < Instance.fileItems.Count; i++)
        {
            Destroy(Instance.fileItems.ElementAt(i).Key);
        }
        Instance.fileItems.Clear();
    }

    #region Button Onklick Methods
    public void OnDirectoryClick(object object_inp, UIPointerEventArgs args)
    {
        GameObject clickedObject = args.currentTarget;

        if (clickedObject.name.Contains("DirectoryItem"))
        {
            string clickedDirectoryName = clickedObject.GetComponentInChildren<Text>().text;

            Instance.currentDirectoryPath = Instance.directoryItems.First(entry => entry.Key.GetComponentInChildren<Text>().text == clickedDirectoryName).Value;
            Instance.fileText.text = "";
            UpdateFileBrowser();
        }
    }

    public void OnFileClick(object object_inp, UIPointerEventArgs args)
    {
        GameObject clickedObject = args.currentTarget;

        if (clickedObject.name.Contains("FileItem"))
        {
            string clickedDirectoryName = clickedObject.GetComponentInChildren<Text>().text;
            Instance.currentFilePath = Instance.fileItems.First(entry => entry.Key.GetComponentInChildren<Text>().text == clickedDirectoryName).Value;

            if (Instance.fileText.text == Instance.currentFilePath)
            {
                Instance.fileText.text = "";
            }
            else
            {
                Instance.fileText.text = Instance.currentFilePath;
            }
        }
    }

    public void OnDirectoryBackClick()
    {
        try
        {
            Instance.currentDirectoryPath = Directory.GetParent(Instance.currentDirectoryPath).FullName;
            UpdateFileBrowser();
        }
        catch
        {
            Debug.Log("Toplevel reached!");
        }
    }

    public void OnConfirmClick()
    {
        if(string.IsNullOrEmpty(Instance.currentFilePath))
        {
            string callBackString = Instance.currentDirectoryPath;
            Close();
            callBackMethod(callBackString);
        }
        else
        {
            string callBackString = Instance.fileText.text;
            Close();
            callBackMethod(callBackString);
        }
        
    }

    public void OnCancelClick()
    {
        Close();
    }
    #endregion
}
