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

        private Vector3 _uiPosition;
        private Quaternion _uiRotation;
        private GameObject _currentWindow;
        private GameObject _mainCamera;
        private GameObject _parent;
        private bool _bFirstInit;
        private float _cameraDistance;
        private Util.Datatype _dataTypeToLoad;


        private void Start()
        {
            InitListeners();
            _cameraDistance = 5.0f;

            _mainCamera = GameObject.Find("Headset");
            _uiPosition = _mainCamera.transform.position + Vector3.forward * _cameraDistance;
            _uiRotation = _mainCamera.transform.rotation;

            _bFirstInit = false;

            _parent = GameObject.Find("MainMenu");
            Util.DisableAllChildren(_parent);
        }

        private void Update()
        {
            if (!_bFirstInit)
            {
                //this has to be done in the update method because disabling and reenabing of the VRTK_UICanvas in the start mehtod leads to unexpected behavior
                FirstInit();
            }

            _uiPosition = _mainCamera.transform.position + Vector3.forward * _cameraDistance;
            _currentWindow.transform.position = _uiPosition;
        }

        private void FirstInit()
        {
            _currentWindow = Util.FindInactiveGameobject(_parent, "MainMenu_Main");
            _currentWindow.SetActive(true);
            _currentWindow.transform.position = _uiPosition;
            _currentWindow.transform.rotation = _uiRotation;

            _bFirstInit = true;
        }

        private void InitListeners()
        {
            Button createNewSession = GameObject.Find("Button_CreateNewSession").GetComponent<Button>();
            Button loadSession = GameObject.Find("Button_LoadSession").GetComponent<Button>();
            Button quit = GameObject.Find("Button_Quit").GetComponent<Button>();
            Button pcd = GameObject.Find("Button_PCD").GetComponent<Button>();
            Button lidar = GameObject.Find("Button_Lidar").GetComponent<Button>();
            Button hdf5 = GameObject.Find("Button_HDF5").GetComponent<Button>();
            Button backFromNewSession = GameObject.Find("Button_BackFromNewSession").GetComponent<Button>();
            Button backFromLoadSessionBrowser = GameObject.Find("Button_BackFromLoadSessionBrowser").GetComponent<Button>();
            Button backFromLoadTypeBrowser = GameObject.Find("Button_BackFromLoadTypeBrowser").GetComponent<Button>();

            createNewSession.onClick.AddListener(OnButtonCLick_CreateNewSession);
            loadSession.onClick.AddListener(OnButtonCLick_LoadSession);
            quit.onClick.AddListener(OnButtonCLick_Quit);
            pcd.onClick.AddListener(OnButtonCLick_PCD);
            lidar.onClick.AddListener(OnButtonCLick_Lidar);
            hdf5.onClick.AddListener(OnButtonCLick_HDF5);
            backFromNewSession.onClick.AddListener(OnButtonCLick_BackFromNewSession);
            backFromLoadSessionBrowser.onClick.AddListener(OnButtonCLick_BackFromLoadSessionBrowser);
            backFromLoadTypeBrowser.onClick.AddListener(OnButtonCLick_BackFromLoadTypeBrowser);


            ///////////////////////////////////////////////////////////////////Test
            Button testSession = GameObject.Find("Button_TestSession").GetComponent<Button>();
            testSession.onClick.AddListener(loadtestsecene);
        }

        public void loadtestsecene()
        {
            Application.LoadLevel("ReaderViewerSceneVR");
        }

        /// ///////////////////////////////////////////////////////////////////

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

        public void OnButtonCLick_HDF5()
        {
            _cameraDistance = 10.0f;
            _currentWindow = GetNewWindow("MainMenu_LoadTypeBrowser");
            _dataTypeToLoad = Util.Datatype.hdf5;
            OpenBrowser("hdf5");
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
            fileBrowserScript.SetupFileBrowser(ViewMode.Landscape, parent_inp.name);
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

        private void LoadFileUsingPath(string path_inp)
        {
            Debug.Log("LoadFileUsingPath: " + path_inp);

            if (File.Exists(path_inp))
            {
                // path is a file.
                Util.DataLoadInfo.StoreInfo(path_inp, _dataTypeToLoad, true);
            }
            else if (Directory.Exists(path_inp))
            {
                // path is a directory.
                Util.DataLoadInfo.StoreInfo(path_inp, _dataTypeToLoad, false);
            }

            SceneManager.LoadScene("ApplicationScene");
        }
    }
}
