﻿using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu_Main : SimpleMenu<MainMenu_Main>
{
    private float tutorialHurdleInSeconds = 1;
    private float startTime;
    private Slider tutorialSlider;

    private void OnEnable()
    {
        tutorialSlider = GetComponentInChildren<Slider>();
        tutorialSlider.maxValue = tutorialHurdleInSeconds;
        tutorialSlider.gameObject.SetActive(false);

        startTime = Time.realtimeSinceStartup;
    }

    private void Update()
    {
        if(OVRInput.Get(OVRInput.Button.One))
        {
            var currentTime = Time.realtimeSinceStartup - startTime;

            tutorialSlider.gameObject.SetActive(true);
            tutorialSlider.value = currentTime;

            if (currentTime > tutorialHurdleInSeconds)
            {
                LoadScene(2);
            }
        }
        else
        {
            tutorialSlider.gameObject.SetActive(false);
            tutorialSlider.value = 0;
            startTime = Time.realtimeSinceStartup;
        }
    }

    public void OnClickStartDemo()
    {
        Util.DataLoadInfo._accessMode = Util.AccesMode.Create;
        Util.DataLoadInfo._dataType = Util.Datatype.pcd;
        Util.DataLoadInfo._sessionName = "DemoSession"; 

#if UNITY_EDITOR
        Util.DataLoadInfo._sourceDataPath = "C:\\Users\\gruepazu\\Desktop\\CmoreDemoBuild\\DemoData\\";
#else
        Util.DataLoadInfo._sourceDataPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "DemoData\\"); 
#endif
        //Util.DataLoadInfo._sessionFolderPath = Application.persistentDataPath + "/" + Util.DataLoadInfo._sessionName;

        ReferenceHandler.Instance.GetRightPointerRenderer().enabled = false;
        LoadingScreen.Show();
        SceneManager.LoadScene(1);
    }

    public void OnCreateClick()
    {
        MainMenu_CreateSession.Open();
    }

    public void OnLoadClick()
    {
        string userInfo = "Choose the Save File you want to load!";
        FileBrowserScript.Show(Application.persistentDataPath, ".dat", LoadSessionFromPath, userInfo);
    }

    public void OnQuitClick()
    {
        //Application.Quit();

        if (!Application.isEditor)
            System.Diagnostics.Process.GetCurrentProcess().Kill();
    }

    public void OnTutorialClick()
    {
        LoadScene(2);
    }

    private void LoadSessionFromPath(string path_inp)
    {
        if (path_inp.Contains(".dat"))
        {
            if (!path_inp.Contains("SaveFile"))
            {
                return;
            }

            Util.DataLoadInfo._sourceDataPath = path_inp;
            Util.DataLoadInfo._accessMode = Util.AccesMode.Load;

            LoadScene(1);
        }
    }

    private void LoadScene(int scneneNumber)
    {
        LoadingScreen.Show();
        SceneManager.LoadScene(scneneNumber);
    }
}
