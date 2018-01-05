using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class InGameOptionsController : MonoBehaviour
{


    private bool _bIsEnabled;
    private float _fCameraDistance;
    private Util.Datatype _dataTypeToLoad;
    private GameObject _currentWindow;
    private GameObject _mainCamera;
    private GameObject _parent;
    private Color _invalidCollisionColor;
    private Color _validCollisionColor;
    private Color _invalidCollisionColor_old;
    private Color _validCollisionColor_old;

    private Dropdown _dropdown_MovementMode;
    private InputField _inputField_MovementDistance;

    private void Start()
    {
        InitListeners();

        _invalidCollisionColor = new Color32(99, 109, 115, 50);
        _validCollisionColor = new Color32(255, 255, 255, 150);

        _fCameraDistance = 5.0f;
        _mainCamera = GameObject.Find("Headset");
        _parent = this.gameObject;

        Util.DisableAllChildren(_parent);
    }

    public void EnableOptionMenu()
    {
        _currentWindow = Util.FindInactiveGameobject(_parent, "InGameOptions_Main");
        _currentWindow.SetActive(true);
        _currentWindow.transform.rotation = _mainCamera.transform.rotation;
        _currentWindow.transform.position = _mainCamera.transform.position + _mainCamera.transform.forward * _fCameraDistance;

        _invalidCollisionColor_old = GameObject.Find("RightController").GetComponent<VRTK_StraightPointerRenderer>().invalidCollisionColor;
        _validCollisionColor_old = GameObject.Find("RightController").GetComponent<VRTK_StraightPointerRenderer>().validCollisionColor;
        GameObject.Find("RightController").GetComponent<VRTK_StraightPointerRenderer>().invalidCollisionColor = _invalidCollisionColor;
        GameObject.Find("RightController").GetComponent<VRTK_StraightPointerRenderer>().validCollisionColor = _validCollisionColor;
    }

    public void DisableOptionMenu()
    {
        GameObject.Find("RightController").GetComponent<VRTK_StraightPointerRenderer>().invalidCollisionColor = _invalidCollisionColor_old;
        GameObject.Find("RightController").GetComponent<VRTK_StraightPointerRenderer>().validCollisionColor = _validCollisionColor_old;
        Util.DisableAllChildren(_parent);
    }

    private void InitListeners()
    {
        _dropdown_MovementMode = GameObject.Find("Dropdown_MovementMode").GetComponent<Dropdown>();
        _inputField_MovementDistance = GameObject.Find("InputField_MovementDistance").GetComponent<InputField>();

        InitUiComponents();

        _dropdown_MovementMode.onValueChanged.AddListener(MovementModeChanged);
        _inputField_MovementDistance.onValueChanged.AddListener(MovementDistanceChanged);
    }



    private void InitUiComponents()
    {
        //Dropdown MovementMode
        List<string> dropdown_MovementModeList = new List<string>();
        foreach (var mode in Enum.GetValues(typeof(Util.MovementMode)))
        {
            dropdown_MovementModeList.Add(Enum.GetName(typeof(Util.MovementMode), mode));
        }
        _dropdown_MovementMode.AddOptions(dropdown_MovementModeList);
        _dropdown_MovementMode.value = (int)GameObject.Find("MovementController").GetComponent<Movement>()._movementMode;
    }

    private void MovementModeChanged(int iModeIndex_inp)
    {
        GameObject.Find("MovementController").GetComponent<Movement>()._movementMode = (Util.MovementMode)iModeIndex_inp;
    }

    private void MovementDistanceChanged(string sDistance_input)
    {
        GameObject.Find("MovementController").GetComponent<Movement>()._fMovementDistance = float.Parse(sDistance_input);
    }

    public void OnMovementDistanceClicked()
    {
        Util.FindInactiveGameobject(this.gameObject, "IngameOptions_KeyBoard").GetComponent<KeyboardController>().EnableKeyboard(_inputField_MovementDistance);
    }
}
