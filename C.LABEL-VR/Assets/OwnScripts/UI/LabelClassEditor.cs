using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using VRTK;

public class LabelClassEditor : Menu<LabelClassEditor>
{
    public LabelClassItem labelClassItemPrefab;
    public InputField className;
    public InputField classID;
    public Image classColor;
    public Text userInfo;

    private LabelCLassItemStruct currentItemToEdit;
    private AccessMode currentAccessMode;
    private static Action<LabelCLassItemStruct> callBackMethod;

    public enum AccessMode
    {
        Create,
        Edit
    }

    public struct LabelCLassItemStruct
    {
        public uint ID { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }

        public LabelCLassItemStruct(uint id, string name, Color color)
        {
            ID = id;
            Name = name;
            Color = color;
        }
    }

    public static void Show(Action<LabelCLassItemStruct> callBackMethod_inp, AccessMode accessMode, LabelCLassItemStruct editInfos, string userInfo_inp)
    {
        callBackMethod = callBackMethod_inp;

        Open();
        Instance.userInfo.text = userInfo_inp;
        Instance.currentAccessMode = accessMode;

        if (accessMode == AccessMode.Edit)
        {
            Instance.InitEditor(editInfos);
        }
        else
        {
            Instance.InitEditor();
        }
    }

    public void OnInputFieldClick(object obj, UIPointerEventArgs args)
    {
        GameObject clickedObject = args.currentTarget;

        if (MenuManager.Instance.MenuStack.Peek() != Instance)
        {
            //this menu is not visible
            return;
        }

        InputField clickedInputField = clickedObject.GetComponent<InputField>();

        if (clickedInputField != null)
        {
            if (clickedInputField.name.Contains("ID"))
            {
                NumPad.Show(EditID, "Set desired ID!");
            }
            else if (clickedInputField.name.Contains("Name"))
            {
                KeyboardManager.Show(EditName, "Set desired Name!");
            }
        }
    }

    private void EditName(string name_inp)
    {
        Instance.className.text = name_inp;
    }

    private void EditID(string ID_inp)
    {
        Instance.classID.text = ID_inp;
    }

    private void InitEditor(LabelCLassItemStruct itemToEdit_inp)
    {
        Instance.className.text = itemToEdit_inp.Name;
        Instance.classID.text = itemToEdit_inp.ID.ToString();
        Instance.classColor.color = itemToEdit_inp.Color;

        Instance.currentItemToEdit = itemToEdit_inp;
    }

    private void InitEditor()
    {
        Instance.className.text = "";
        Instance.classID.text = "";
        Instance.classColor.color = Color.white;
    }

    public void OnConfirmClick()
    {
        LabelCLassItemStruct item_out = new LabelCLassItemStruct
        {
            Name = className.text,
            ID = Convert.ToUInt32(classID.text),
            Color = classColor.color
        };

        Close();
        callBackMethod(item_out);
    }
}
