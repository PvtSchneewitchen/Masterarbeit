
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

public class KeyboardManager : Menu<KeyboardManager>
{
    #region Public Variables
    [Header("User defined")]
    [Tooltip("If the character is uppercase at the initialization")]
    public bool isUppercase = false;
    public int maxInputLength;

    [Header("UI Elements")]
    public Text keyboardInputfield;
    //public InputField keyboardInputfield;


    [Header("Essentials")]
    public Transform characters;

    public Text userInfoText;
    #endregion

    #region Private Variables
    private string Input
    {
        get { return keyboardInputfield.text; }
        set { keyboardInputfield.text = value; }
    }

    private Dictionary<GameObject, Text> keysDictionary = new Dictionary<GameObject, Text>();

    private bool capslockFlag;

    private static Action<string> callBackMethod;

    private static string goalText;
    private static InputField goalInputField;
    private static bool isGoalInputfield;
    #endregion

    #region Monobehaviour Callbacks
    private void OnEnable()
    {
        //addlistener didnt work so the listeners were added in the editor

        for (int i = 0; i < characters.childCount; i++)
        {
            GameObject key = characters.GetChild(i).gameObject;
            Text _text = key.GetComponentInChildren<Text>();
            keysDictionary.Add(key, _text);

            key.GetComponent<Button>().onClick.AddListener(delegate { GenerateInput(_text.text); });

            //Debug.Log("key: " + key + " text: " + _text.text + " listenercount: " + key.GetComponent<Button>().onClick.GetPersistentEventCount());
        }

        capslockFlag = isUppercase;
        CapsLock();
    }

    public static void Show(Action<string> callBackMethod_inp, string userInfo_inp)
    {
        callBackMethod = callBackMethod_inp;
        Open();
        Instance.userInfoText.text = userInfo_inp;
    }
    #endregion

    #region Public Methods
    public void Backspace()
    {
        if (Input.Length > 0)
        {
            Input = Input.Remove(Input.Length - 1);
        }
        else
        {
            return;
        }
    }

    public void Clear()
    {
        Input = "";
    }

    public void CapsLock()
    {
        if (capslockFlag)
        {
            foreach (var pair in keysDictionary)
            {
                pair.Value.text = ToUpperCase(pair.Value.text);
            }
        }
        else
        {
            foreach (var pair in keysDictionary)
            {
                pair.Value.text = ToLowerCase(pair.Value.text);
            }
        }
        capslockFlag = !capslockFlag;
    }

    public void OnConfirmCLick()
    {
        Close();

        callBackMethod(Input);
    }
    #endregion

    #region Private Methods
    public void GenerateInput(string s)
    {
        if (Input.Length > maxInputLength) { return; }
        Input += s;
    }

    private string ToLowerCase(string s)
    {
        return s.ToLower();
    }

    private string ToUpperCase(string s)
    {
        return s.ToUpper();
    }
    #endregion
}
