using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KeyboardController : MonoBehaviour
{
    private GameObject _currentWindow;
    private InputField _inputField;

    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;
    public Button button5;
    public Button button6;
    public Button button7;
    public Button button8;
    public Button button9;
    public Button button0;
    public Button buttonErase;
    public Button buttonConfirm;

    void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void EnableKeyboard(InputField inputField_inp, GameObject window_inp)
    {
        print("EnableKeyboard");
        _currentWindow = window_inp;
        _inputField = inputField_inp;
        transform.rotation = _currentWindow.transform.rotation;
        transform.position = _currentWindow.transform.position - _currentWindow.transform.transform.forward;
        gameObject.SetActive(true);
    }

    public void DisableKeyboard()
    {
        gameObject.SetActive(false);
    }


    public void Button1Click()
    {
        _inputField.text = _inputField.text + "1";
        print("1");
    }

    public void Button2Click()
    {
        _inputField.text = _inputField.text + "2";
    }

    public void Button3Click()
    {
        _inputField.text = _inputField.text + "3";
    }

    public void Button4Click()
    {
        _inputField.text = _inputField.text + "4";
    }

    public void Button5Click()
    {
        _inputField.text = _inputField.text + "5";
    }

    public void Button6Click()
    {
        _inputField.text = _inputField.text + "6";
    }

    public void Button7Click()
    {
        _inputField.text = _inputField.text + "7";
    }

    public void Button8Click()
    {
        _inputField.text = _inputField.text + "8";
    }

    public void Button9Click()
    {
        _inputField.text = _inputField.text + "9";
    }

    public void Button0Click()
    {
        _inputField.text = _inputField.text + "0";
    }

    public void ButtonEraseClick()
    {
        _inputField.text = _inputField.text.Remove(_inputField.text.Length - 1);
    }

    public void ButtonConfirmClick()
    {
        _inputField.OnDeselect(new BaseEventData(EventSystem.current));
        DisableKeyboard();
    }


}
