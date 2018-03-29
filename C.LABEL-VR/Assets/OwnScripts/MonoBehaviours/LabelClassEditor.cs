using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LabelClassEditor : MonoBehaviour
{
    public KeyboardController _numpad;
    public GameObject _labelPanel;
    public LabelClassContent labelClassContent;

    private InputField _className;
    private InputField _classID;
    private Image _classColorImage;
    private AccessMode accessMode;

    private string _oldName;
    private uint _oldID;
    private Color _oldColor;

    void Start()
    {
        InputField[] inputFields = GetComponentsInChildren<InputField>();
        Image[] images = GetComponentsInChildren<Image>();

        _className = inputFields[0];
        _classID = inputFields[1];
        _classColorImage = images[3];

        gameObject.SetActive(false);
    }

    public void OnAddClick()
    {
        accessMode = AccessMode.Create;
        EnableLabelClassEditor();
    }

    public void OnEditClick()
    {
        accessMode = AccessMode.Edit;

        if(labelClassContent._currentSelectedItem)
        {
            var itemContent = labelClassContent._currentSelectedItem.GetComponent<ItemContent>();

             _oldName = itemContent._className.text;
            _oldID = Convert.ToUInt32(itemContent._classID.text);
            _oldColor = itemContent._classImage.color;

            EnableLabelClassEditor(_oldName, _oldID, _oldColor);
        }
    }

    public void OnConfirmClick()
    {
        if (accessMode == AccessMode.Create)
        {
            var labelClassInfos = Labeling.GetAllLabelClassInfos();
            if (labelClassInfos.ContainsKey(Convert.ToUInt32(_classID.text)))
            {
                Debug.Log("This key already exists!");
                return;
            }
            else
            {
                Labeling.AddNewLabelClass(Convert.ToUInt32(_classID.text), _className.text, _classColorImage.color);
                labelClassContent.UpdateContent();
                DisableLabelClassEditor();
            }
        }
        else
        {
            Labeling.EditLabelClass(_oldID, Convert.ToUInt32(_classID.text), _className.text, _classColorImage.color);
            labelClassContent.UpdateContent();
            DisableLabelClassEditor();
        }
    }

    private void DisableLabelClassEditor()
    {
        _className.text = "";
        _classID.text = "";
        _classColorImage.color = new Color(0, 0, 0, 255);
        gameObject.SetActive(false);
    }

    private void EnableLabelClassEditor()
    {
        print("EnableClassEditor");
        transform.rotation = _labelPanel.transform.rotation;
        transform.position = _labelPanel.transform.position - _labelPanel.transform.transform.forward - _labelPanel.transform.transform.right;
        gameObject.SetActive(true);
    }

    private void EnableLabelClassEditor(string name_inp, uint id_inp, Color color_inp)
    {
        print("EnableClassEditor");
        transform.rotation = _labelPanel.transform.rotation;
        transform.position = _labelPanel.transform.position - _labelPanel.transform.transform.forward - _labelPanel.transform.transform.right;
        gameObject.SetActive(true);

        _className.text = name_inp;
        _classID.text = id_inp.ToString() ;
        _classColorImage.color = color_inp;
    }

    private enum AccessMode
    {
        Create,
        Edit
    }
}
