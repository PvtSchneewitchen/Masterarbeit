using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using VRTK;
using VRKeyboard.Utils;

namespace GracesGames.SimpleFileBrowser.Scripts
{
    public class MainMenuController : MonoBehaviour
    {
        public GameObject _fileBrowserPrefab;
        public GameObject _mainWindow;
        public GameObject _cameraRig;
        public GameObject _keyboard;

        private Vector3 _uiPosition;
        private Quaternion _uiRotation;
        private GameObject _currentWindow;
        private GameObject _parent;
        private float _cameraDistance;
        private Util.Datatype _dataTypeToLoad;


        private void Start()
        {
            Debug.Log("StartTest");
            _cameraDistance = 5.0f;

            _uiPosition = _cameraRig.transform.position + _cameraRig.transform.forward * _cameraDistance;
            _uiRotation = _cameraRig.transform.rotation;

            _parent = gameObject;

            FirstInit();
        }

        private void Update()
        { 
            _uiPosition = _cameraRig.transform.position + Vector3.forward * _cameraDistance;
            _currentWindow.transform.position = _uiPosition;
        }

        private void FirstInit()
        {
            _currentWindow = _mainWindow;
            _currentWindow.SetActive(true);
            _currentWindow.transform.position = _uiPosition;
            _currentWindow.transform.rotation = _uiRotation;
        }

        public void LoadTestScene()
        {
            string path = "C:\\Users\\gruepazu\\Desktop\\PointClouds\\000000000_LidarImage_000000002.pcd";

            Util.DataLoadInfo._accessMode = Util.AccesMode.Create;
            Util.DataLoadInfo._dataType = Util.Datatype.pcd;
            Util.DataLoadInfo._sourceDataPath = path;
            Util.DataLoadInfo._sessionName = "TestScene";
            Util.DataLoadInfo._sessionFolderPath = Application.persistentDataPath + "/" + Util.DataLoadInfo._sessionName;

            SceneManager.LoadScene("ApplicationScene");
        }

        private GameObject GetNewWindow(string gameObjectName_inp)
        {
            GameObject newWindow = Util.FindInactiveGameobject(_parent, gameObjectName_inp);
            _currentWindow.SetActive(false); 
            newWindow.SetActive(true);
            return newWindow;
        }



        public void OnButtonCLick_CreateNewSession()
        {
            _cameraDistance = 5.0f;
            _currentWindow = GetNewWindow("MainMenu_NewSession");
        }

        public void OnButtonCLick_LoadSession()
        {
            _cameraDistance = 10.0f;
            _currentWindow = GetNewWindow("MainMenu_LoadTypeBrowser");
            OpenBrowser("dat", Application.persistentDataPath);
        }

        public void OnButtonCLick_Quit()
        {
            if (!Application.isEditor) System.Diagnostics.Process.GetCurrentProcess().Kill();

            Application.Quit();
        }

        public void OnButtonCLick_PCD()
        {
            _cameraDistance = 5.0f;
            _currentWindow = GetNewWindow("Keyboard");
            Util.DataLoadInfo._dataType = Util.Datatype.pcd;
        }

        public void OnButtonCLick_HDF5_Daimler()
        {
            _cameraDistance = 5.0f;
            _currentWindow = GetNewWindow("Keyboard");
            Util.DataLoadInfo._dataType = Util.Datatype.hdf5_DaimlerLidar;
        }

        public void OnButtonClick_NameConfirmation()
        {
            _cameraDistance = 10.0f;

            Util.DataLoadInfo._sessionName = _keyboard.GetComponent<KeyboardManager>().inputField.text;

            if (Util.DataLoadInfo._dataType == Util.Datatype.pcd)
            {
                OpenBrowser("pcd", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            }
            else if (Util.DataLoadInfo._dataType == Util.Datatype.hdf5_DaimlerLidar)
            {
                OpenBrowser("hdf5", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            }

            _currentWindow = GetNewWindow("MainMenu_LoadTypeBrowser");
        }

        

        public void OnButtonCLick_BackFromNewSession()
        {
            _cameraDistance = 5.0f;
            _currentWindow = GetNewWindow("MainMenu_Main");
        }

        public void OnButtonCLick_BackFromLoadSessionBrowser()
        {
            _cameraDistance = 5.0f;
            _currentWindow = GetNewWindow("MainMenu_Main");
        }

        public void OnButtonCLick_BackFromLoadTypeBrowser()
        {
            _cameraDistance = 5.0f;
            _currentWindow = GetNewWindow("MainMenu_NewSession");
        }

        public void OnButtonCLick_Tutorial()
        {
            SceneManager.LoadScene(2);
        }

        private void OpenBrowser(string sFileExtension_inp, string startPath)
        {
            OpenFileBrowser(FileBrowserMode.Load, Util.FindInactiveGameobject(GameObject.Find("MainMenu"), "MainMenu_LoadTypeBrowser").transform, sFileExtension_inp, startPath);
        }

        private void OpenFileBrowser(FileBrowserMode fileBrowserMode_inp, Transform parent_inp, string sFileExtension_inp, string startPath)
        {
            // Create the file browser and name it
            GameObject fileBrowserObject = Instantiate(_fileBrowserPrefab, parent_inp);
            fileBrowserObject.name = "FileBrowser";
            // Set the mode to save or load
            FileBrowser fileBrowserScript = fileBrowserObject.GetComponent<FileBrowser>();
            fileBrowserScript.SetupFileBrowser(ViewMode.Landscape, parent_inp);
            if (fileBrowserMode_inp == FileBrowserMode.Save)
            {
                fileBrowserScript.SaveFilePanel(this, "SaveFileUsingPath", "DemoText", sFileExtension_inp, startPath);
            }
            else
            {
                //caller script, callbackmethod, fileextension
                fileBrowserScript.OpenFilePanel(this, "LoadFromPath", sFileExtension_inp, startPath);
            }
        }

        private void LoadFromPath(string sPath_inp)
        {
            if(sPath_inp.Contains(".dat"))
            {
                if (!sPath_inp.Contains("SaveFile"))
                {
                    return;
                }

                Util.DataLoadInfo._sourceDataPath = sPath_inp;
                Util.DataLoadInfo._accessMode = Util.AccesMode.Load;
            }
            else
            {
                Util.DataLoadInfo._sourceDataPath = sPath_inp;
                Util.DataLoadInfo._accessMode = Util.AccesMode.Create;
            }

            SceneManager.LoadScene("ApplicationScene");
        }
    }
}
