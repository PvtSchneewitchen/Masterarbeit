using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using VRTK;

namespace GracesGames.SimpleFileBrowser.Scripts
{
    public class MainMenuController : MonoBehaviour
    {
        public GameObject _fileBrowserPrefab;
        public GameObject _mainWindow;
        public GameObject _cameraRig;

        private Vector3 _uiPosition;
        private Quaternion _uiRotation;
        private GameObject _currentWindow;
        private GameObject _parent;
        private float _cameraDistance;
        private Util.Datatype _dataTypeToLoad;


        private void Start()
        {

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
            Util.DataLoadInfo.StoreInfo(path, Util.Datatype.pcd);
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
            _cameraDistance = 5.0f;
            _currentWindow = GetNewWindow("MainMenu_LoadSessionBrowser");
        }

        public void OnButtonCLick_Quit()
        {
            Application.Quit();
        }

        public void OnButtonCLick_PCD()
        {
            _cameraDistance = 10.0f;
            _currentWindow = GetNewWindow("MainMenu_LoadTypeBrowser");
            _dataTypeToLoad = Util.Datatype.pcd;
            OpenBrowser("pcd");
        }

        public void OnButtonCLick_Lidar()
        {
            _cameraDistance = 10.0f;
            _currentWindow = GetNewWindow("MainMenu_LoadTypeBrowser");
            _dataTypeToLoad = Util.Datatype.lidar;
            //TODO find out data ending for lidar data and call OpenBrowser() with it
        }

        public void OnButtonCLick_HDF5_Daimler()
        {
            _cameraDistance = 10.0f;
            _currentWindow = GetNewWindow("MainMenu_LoadTypeBrowser");
            _dataTypeToLoad = Util.Datatype.hdf5_DaimlerLidar;
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

        private void OpenBrowser(string sFileExtension_inp)
        {
            OpenFileBrowser(FileBrowserMode.Load, GameObject.Find("MainMenu_LoadTypeBrowser").transform, sFileExtension_inp);
        }

        private void OpenFileBrowser(FileBrowserMode fileBrowserMode_inp, Transform parent_inp, string sFileExtension_inp)
        {
            // Create the file browser and name it
            GameObject fileBrowserObject = Instantiate(_fileBrowserPrefab, parent_inp);
            fileBrowserObject.name = "FileBrowser";
            // Set the mode to save or load
            FileBrowser fileBrowserScript = fileBrowserObject.GetComponent<FileBrowser>();
            fileBrowserScript.SetupFileBrowser(ViewMode.Landscape, parent_inp);
            if (fileBrowserMode_inp == FileBrowserMode.Save)
            {
                fileBrowserScript.SaveFilePanel(this, "SaveFileUsingPath", "DemoText", sFileExtension_inp);
            }
            else
            {
                //caller script, callbackmethod, fileextension
                fileBrowserScript.OpenFilePanel(this, "LoadFileUsingPath", sFileExtension_inp);
            }
        }

        private void LoadFromPath(string sPath_inp)
        {
            if (!File.Exists(sPath_inp))
            {
                if (!Directory.Exists(sPath_inp))
                {
                    Debug.Log("No such File or Directory: " + sPath_inp);
                    return;
                }
            }

            Util.DataLoadInfo.StoreInfo(sPath_inp, _dataTypeToLoad);
            SceneManager.LoadScene("ApplicationScene");
        }
    }
}
