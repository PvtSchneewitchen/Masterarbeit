using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardController : MonoBehaviour
{
    public InputField _inputField;

    private Button button1;
    private Button button2;
    private Button button3;
    private Button button4;
    private Button button5;
    private Button button6;
    private Button button7;
    private Button button8;
    private Button button9;
    private Button button0;
    private Button buttonErase;
    private Button buttonConfirm;

    void Start()
    {
        InitButtons();
        AddButtonListeners();
        this.gameObject.SetActive(false);
    }

    public void EnableKeyboard(InputField inputField_inp)
    {
        _inputField = inputField_inp;
        this.gameObject.SetActive(true);

        //todo position
        Vector3 position = inputField_inp.transform.position + new Vector3(0,0,0.01f);//+ Camera.main.ScreenToWorldPoint(new Vector3(0,gameObject.GetComponent<RectTransform>().rect.height/2, 0.01f));
        this.transform.rotation = inputField_inp.transform.rotation;
        this.transform.position = position;
    }

    public void DisableKeyboard()
    {
        //_inputField.isFocused = false;
        this.gameObject.SetActive(false);
    }

    private void InitButtons()
    {
        button1 = GameObject.Find("Keyborad_Button_1").GetComponent<Button>();
        button2 = GameObject.Find("Keyborad_Button_2").GetComponent<Button>();
        button3 = GameObject.Find("Keyborad_Button_3").GetComponent<Button>();
        button4 = GameObject.Find("Keyborad_Button_4").GetComponent<Button>();
        button5 = GameObject.Find("Keyborad_Button_5").GetComponent<Button>();
        button6 = GameObject.Find("Keyborad_Button_6").GetComponent<Button>();
        button7 = GameObject.Find("Keyborad_Button_7").GetComponent<Button>();
        button8 = GameObject.Find("Keyborad_Button_8").GetComponent<Button>();
        button9 = GameObject.Find("Keyborad_Button_9").GetComponent<Button>();
        button0 = GameObject.Find("Keyborad_Button_0").GetComponent<Button>();
        buttonErase = GameObject.Find("Keyborad_Button_Erase").GetComponent<Button>();
        buttonConfirm = GameObject.Find("Keyborad_Button_OK").GetComponent<Button>();
    }

    private void AddButtonListeners()
    {
        button1.onClick.AddListener(Button1Click);
        button2.onClick.AddListener(Button2Click);
        button3.onClick.AddListener(Button3Click);
        button4.onClick.AddListener(Button4Click);
        button5.onClick.AddListener(Button5Click);
        button6.onClick.AddListener(Button6Click);
        button7.onClick.AddListener(Button7Click);
        button8.onClick.AddListener(Button8Click);
        button9.onClick.AddListener(Button9Click);
        button0.onClick.AddListener(Button0Click);
        buttonErase.onClick.AddListener(ButtonEraseClick);
        buttonConfirm.onClick.AddListener(ButtonConfirmClick);
    }

    private void Button1Click()
    {
        _inputField.text = _inputField.text + "1";
        print("1");
    }

    private void Button2Click()
    {
        _inputField.text = _inputField.text + "2";
    }

    private void Button3Click()
    {
        _inputField.text = _inputField.text + "3";
    }

    private void Button4Click()
    {
        _inputField.text = _inputField.text + "4";
    }

    private void Button5Click()
    {
        _inputField.text = _inputField.text + "5";
    }

    private void Button6Click()
    {
        _inputField.text = _inputField.text + "6";
    }

    private void Button7Click()
    {
        _inputField.text = _inputField.text + "7";
    }

    private void Button8Click()
    {
        _inputField.text = _inputField.text + "8";
    }

    private void Button9Click()
    {
        _inputField.text = _inputField.text + "9";
    }

    private void Button0Click()
    {
        _inputField.text = _inputField.text + "0";
    }

    private void ButtonEraseClick()
    {
        _inputField.text = _inputField.text.Remove(_inputField.text.Length - 1);
    }

    private void ButtonConfirmClick()
    {
        _inputField.OnDeselect(null);
        DisableKeyboard();
    }


}
