using System;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class ItemContent : MonoBehaviour
{
    public Text _className { get; set; }
    public Text _classID { get; set; }
    public Image _classImage { get; set; }

    private LabelClassContent _labelClassContent;
    private Button _item;
    private Color _normalColor;
    private Color _pressedColor;
    private Color _highlightedColor;

    void OnEnable()
    {
        _labelClassContent = transform.parent.GetComponent<LabelClassContent>();

        _item = GetComponent<Button>();
        _item.onClick.AddListener(OnItemClick); 
        _normalColor = _item.colors.normalColor;
        _pressedColor = _item.colors.pressedColor;
        _highlightedColor = _item.colors.highlightedColor;

        Text[] t = GetComponentsInChildren<Text>();
        Image[] i = GetComponentsInChildren<Image>();
        _className = t[0];
        _classID = t[1];
        _classImage = i[1];
    }

    private void OnItemClick()
    {
        Button currentSelected = _labelClassContent._currentSelectedItem;
        if(currentSelected)
        {
            ChangeButtonColorToDeselected(currentSelected);
        }

        ChangeButtonColorToSelected(_item);
        _labelClassContent._currentSelectedItem = _item;
    }

    private void ChangeButtonColorToSelected(Button button_inp)
    {
        var colors = button_inp.colors;
        colors.normalColor = _pressedColor;
        colors.highlightedColor = _pressedColor;
        button_inp.colors = colors;
    }

    private void ChangeButtonColorToDeselected(Button button_inp)
    {
        var colors = button_inp.colors;
        colors.normalColor = _normalColor;
        colors.highlightedColor = _highlightedColor;
        button_inp.colors = colors;
    }
}
