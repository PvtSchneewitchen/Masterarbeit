using System;
using UnityEngine;
using UnityEngine.UI;
using VRTK;

public class LabelClassDisplayUpdate : MonoBehaviour
{
    public Text displayText;

    public static LabelClassDisplayUpdate Instance { get; set; }

    public bool DisplayEnabled { get; set; }

    private VRTK_Pointer rightPointer;
    private VRTK_StraightPointerRenderer rightPointerRenderer;
    private RectTransform _displayTransform;
    private Color32 _oldPointerColor;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    void Start()
    {
        rightPointer = ReferenceHandler.Instance.GetRightPointer();
        rightPointerRenderer = ReferenceHandler.Instance.GetRightPointerRenderer();

        rightPointer.ActivationButtonPressed += PointerEnabled;
        rightPointer.ActivationButtonReleased += PointerDisabled;

        _displayTransform = GetComponent<RectTransform>();
        Transform parent = _displayTransform.parent.transform;
        _displayTransform.position = parent.position + (parent.up * 0.1f);
        gameObject.SetActive(false);
        DisplayEnabled = true;
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
        var info = Labeling.GetLabelClassNameAndMaterial(Labeling.currentLabelClassID);

        displayText.text = info.Item1;
        _oldPointerColor = rightPointerRenderer.validCollisionColor;
        rightPointerRenderer.validCollisionColor = info.Item2.color;
    }

    private void PointerEnabled(object sender, ControllerInteractionEventArgs e)
    {
        if(DisplayEnabled)
        {
            UpdatePointerDisplay();
            gameObject.SetActive(true);
        }
        
    }

    private void PointerDisabled(object sender, ControllerInteractionEventArgs e)
    {
        if (DisplayEnabled)
        {
            rightPointerRenderer.validCollisionColor = _oldPointerColor;
            gameObject.SetActive(false);
        }
    }
}
