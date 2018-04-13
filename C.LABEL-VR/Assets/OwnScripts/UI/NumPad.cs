using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumPad : Menu<NumPad>
{
    public Text userInfo;
    public Text inputText;

    private string Input
    {
        get { return inputText.text; }
        set { inputText.text = value; }
    }

    private static Action<string> callBackMethod;

    public static void Show(Action<string> callBackMehtod_inp, string userInfo_inp)
    {
        callBackMethod = callBackMehtod_inp;

        Open();
        Instance.userInfo.text = userInfo_inp;
        Instance.InitNumberButtons();
    }

    public void InitNumberButtons()
    {
        var buttons = Instance.GetComponentsInChildren<Button>();

        for (int i = 0; i < buttons.Length; i++)
        {
            Button button = buttons[i];

            if (button.name.Length == 1)
            {
                button.onClick.AddListener(delegate { OnNumberClick(button.name); });
            }
        }
    }

    //onclicks
    public void OnNumberClick(string number)
    {
        Input += Convert.ToInt32(number);
    }

    public void OnDeleteClick()
    {
        Input = Input.Remove(Input.Length - 1);
    }

    public void OnConfirmClick()
    {
        string callBackString = Instance.Input;

        Close();
        callBackMethod(callBackString);
    }
}
