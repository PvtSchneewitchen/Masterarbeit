//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.UI;
//using VRTK;

//public class TutorialManager : MonoBehaviour
//{

//    public GameObject[] _uiCanvas;
//    public GameObject _cameraRig;
//    public GameObject _centerEyeAnchor;
//    public Camera _camera;
//    public Movement _movementController;
//    public VRTK_Pointer _rightControllerPointer;
//    public VRTK_Pointer _leftControllerPointer;
//    public PointerTeleport _pointerTeleport;
//    public GameObject _toucher;

//    private int _istepIndex = -1;

//    private GameObject _currentTutorialStep;
//    private LabelSession _session;
//    private List<GameObject> _generatedPoints;


//    private void Start()
//    {
//        _generatedPoints = new List<GameObject>();
//        InitPointCloud();
//        NextTutorialStep();
//    }


//    public void NextTutorialStep()
//    {
//        _istepIndex++;
//        print(_istepIndex);

//        if (_currentTutorialStep != null)
//            _currentTutorialStep.SetActive(false);

//        _currentTutorialStep = _uiCanvas[_istepIndex];
//        _cameraRig.transform.position = new Vector3(0, 7, -5);
//        Util.AlignToCamera(_centerEyeAnchor, _currentTutorialStep, 7);

//        _currentTutorialStep.SetActive(true);

//    }

//    private void InitPointCloud()
//    {
//        InGameOptions.LoadOptions();
//        InGameOptions.RestoreDefaultValues();

//        foreach (var camera in _cameraRig.GetComponentsInChildren<Camera>())
//        {
//            camera.clearFlags = CameraClearFlags.SolidColor;
//            camera.backgroundColor = Color.white;
//        }

//        Dictionary<int, PointCloud> pointcloud = Import.ImportPcd("C:\\Users\\gruepazu\\Desktop\\Masterarbeit\\C.LABEL-VR\\App\\PointClouds");

//        _session = new LabelSession(pointcloud, 0);
//    }

//    private void InitDisableCanvas()
//    {
//        //disable all but the first one
//        for (int i = 1; i < _uiCanvas.Length; i++)
//        {
//            _uiCanvas[i].SetActive(false);
//        }
//    }


//    #region Events

//    public void ButtonNextClicked()
//    {
//        NextTutorialStep();
//    }

//    public void FinishTutorial()
//    {
//        SceneManager.LoadScene("MainMenu");
//    }

//    public void Enable_RightPointer()
//    {
//        _rightControllerPointer.enabled = true;
//    }

//    public void Enable_LeftPointer()
//    {
//        _leftControllerPointer.enabled = true;
//    }

//    public void Disable_RightPointer()
//    {
//        _rightControllerPointer.enabled = false;
//    }

//    public void Disable_LeftPointer()
//    {
//        _leftControllerPointer.enabled = false;
//    }

//    public void Enable_PointerLabeling()
//    {
//        PointerLabeler lc = _toucher.GetComponent<PointerLabeler>();
//        lc.enabled = true;
//    }

//    public void Enable_TouchLabeling()
//    {
//        Collider c = _toucher.GetComponent<Collider>();
//        c.enabled = true;
//    }

//    public void Enable_MovementTeleportMode()
//    {
//        InGameOptions._movementMode = Util.MovementMode.TeleportMode;
//    }

//    public void Enable_MovementFreeFlyMode()
//    {
//        InGameOptions._movementMode = Util.MovementMode.FreeFly;
//    }

//    public void Create_LabelpointsForPointer()
//    {
//        for (int i = 0; i < 3; i++)
//        {
//            GameObject go = Util.CreateDefaultLabelPoint();
//            go.transform.localScale *= 3;
//            go.transform.position = _camera.transform.position + _camera.transform.forward * 3 + -_camera.transform.up * 0.5f + _camera.transform.right * i;
//            _generatedPoints.Add(go);
//        }
//    }

//    public void Create_LabelpointsForToucher()
//    {
//        Transform rightHand = _rightControllerPointer.transform.parent.transform;
//        for (int i = 0; i < 3; i++)
//        {
//            GameObject go = Util.CreateDefaultLabelPoint();
//            go.transform.position = rightHand.position + rightHand.forward* 0.15f + -_camera.transform.right * i * 0.2f;
//            _generatedPoints.Add(go);
//        }
//    }

//    public void ClearGeneratedPoints()
//    {
//        foreach (var point in _generatedPoints)
//        {
//            Destroy(point);
//        }
//        _generatedPoints.Clear();
//    }

//    public void Update_DisplayPointerToggleTime()
//    {
//        Text pointerToggleTime = GameObject.Find("PointerToggleTime").GetComponent<Text>();
//        ContinueEvent cEvent = _currentTutorialStep.GetComponent<ContinueEvent>();
//        pointerToggleTime.text = Convert.ToString((float)(Math.Round((double)cEvent._feventDuration, 1) - (float)(Math.Round((double)cEvent._fTimeSum, 1))) + " s");
//    }

//    public void Update_EnableDisableCanvas()
//    {
//        if (OVRInput.GetDown(OVRInput.Button.Start))
//        {
//            if (_currentTutorialStep.GetComponent<Canvas>().enabled)
//            {
//                _currentTutorialStep.GetComponent<Canvas>().enabled = false;
//            }
//            else
//            {
//                _currentTutorialStep.GetComponent<Canvas>().enabled = true;
//                Util.AlignToCamera(_centerEyeAnchor, _currentTutorialStep, 7);
//            }
//        }
//    }

//    public void Update_EnableDisableCanvasWithMovement()
//    {
//        if (OVRInput.GetDown(OVRInput.Button.Start))
//        {
//            if (_currentTutorialStep.GetComponent<Canvas>().enabled)
//            {
//                _currentTutorialStep.GetComponent<Canvas>().enabled = false;
//                _currentTutorialStep.GetComponent<BoxCollider>().enabled = false;
//                if (!_movementController.enabled)
//                    _movementController.enabled = true;
//            }
//            else
//            {
//                _currentTutorialStep.GetComponent<Canvas>().enabled = true;
//                _currentTutorialStep.GetComponent<BoxCollider>().enabled = true;
//                Util.AlignToCamera(_centerEyeAnchor, _currentTutorialStep, 7);
//                if (_movementController.enabled)
//                    _movementController.enabled = false;
//            }
//        }
//    }

//    public void Update_EnableDisableCanvasWithPointerTeleport()
//    {
//        if (OVRInput.GetDown(OVRInput.Button.Start))
//        {
//            if (_currentTutorialStep.GetComponent<Canvas>().enabled)
//            {
//                _currentTutorialStep.GetComponent<Canvas>().enabled = false;
//                _currentTutorialStep.GetComponent<BoxCollider>().enabled = false;
//                if (!_movementController.GetComponent<Movement>().enabled)
//                    _movementController.GetComponent<Movement>().enabled = true;
//                if (!_pointerTeleport.enabled)
//                    _pointerTeleport.enabled = true;
//            }
//            else
//            {
//                _currentTutorialStep.GetComponent<Canvas>().enabled = true;
//                _currentTutorialStep.GetComponent<BoxCollider>().enabled = true;
//                Util.AlignToCamera(_centerEyeAnchor, _currentTutorialStep, 7);
//                if (_movementController.GetComponent<Movement>().enabled)
//                    _movementController.GetComponent<Movement>().enabled = false;
//                if (_pointerTeleport.enabled)
//                    _pointerTeleport.enabled = false;
//            }
//        }
//    }

//    public void Enable_CloudPoints()
//    {
//        if(_session.GetCurrentPointCloud() != null)
//        {
//            _session.GetCurrentPointCloud().EnableAllPoints();
//        }
//    }
//    #endregion

//    //private void Recenter()
//    //{
//    //    _currentTutorialStep.transform.rotation = Quaternion.Euler(new Vector3(0, _centerEyeAnchor.transform.rotation.eulerAngles.y,0));
//    //    Vector3 forwardVector = _camera.transform.forward;
//    //    forwardVector.y = 0;
//    //    _currentTutorialStep.transform.position = _camera.transform.position + forwardVector * 7;
//    //}
//}
