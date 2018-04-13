using System;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class LabelClassDisplayUpdate : MonoBehaviour
{
    public VRTK_Pointer _rightControllerPointer;
    public VRTK_StraightPointerRenderer _rightControllerPointerRenderer;
    public bool _enabled;

    private RectTransform _displayTransform;
    private Text _displayText;
    private Color32 _oldPointerColor;
    

    void Start()
    {
        _rightControllerPointer.ActivationButtonPressed += PointerEnabled;
        _rightControllerPointer.ActivationButtonReleased += PointerDisabled;

        _displayText = GetComponentInChildren<Text>();

        _displayTransform = GetComponent<RectTransform>();
        Transform parent = _displayTransform.parent.transform;
        _displayTransform.position = parent.position + (parent.up * 0.1f);
        gameObject.SetActive(false);
        _enabled = true;
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            Labeling.SwitchToNextLabelClass();
            UpdatePointerDisplay();
        }
        else if (OVRInput.GetDown(OVRInput.Button.One))
        {
            Labeling.SwitchToPreviousLabelClass();
            UpdatePointerDisplay();
        }
    }

    public void UpdatePointerDisplay()
    {
        var info = Labeling.GetLabelClassNameAndColor(Labeling.currentLabelClassID);

        _displayText.text = info.Item1;
        _oldPointerColor = _rightControllerPointerRenderer.validCollisionColor;
        _rightControllerPointerRenderer.validCollisionColor = info.Item2.color;
    }

    private void PointerEnabled(object sender, ControllerInteractionEventArgs e)
    {
        if(_enabled)
        {
            UpdatePointerDisplay();
            gameObject.SetActive(true);
        }
        
    }

    private void PointerDisabled(object sender, ControllerInteractionEventArgs e)
    {
        if (_enabled)
        {
            _rightControllerPointerRenderer.validCollisionColor = _oldPointerColor;
            gameObject.SetActive(false);
        }
    }
}
