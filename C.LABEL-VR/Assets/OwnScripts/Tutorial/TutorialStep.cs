using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialStep {

    public string _caption
    {
        get
        {
            return _intern_caption.text;
        }
        set
        {
            _intern_caption.text = value;
        }
    }

    public string _explanation
    {
        get
        {
            return _intern_explanation.text;
        }
        set
        {
            _intern_explanation.text = value;
        }
    }

    public Texture _image
    {
        get
        {
            return _intern_image.texture;
        }
        set
        {
            _intern_image.texture = value;
        }
    }

    public bool _bButtonNextDisabled;
    public OVRInput.Button _buttonToContinue;

    private Text _intern_caption;
    private Text _intern_explanation;
    private RawImage _intern_image;

    public TutorialStep(GameObject uiPrefab_inp)
    {
        _intern_caption = uiPrefab_inp.transform.Find("Text_Caption").GetComponent<Text>();
        _intern_explanation = uiPrefab_inp.transform.Find("Text_Explanation").GetComponent<Text>();
        _intern_image = uiPrefab_inp.transform.Find("Image").GetComponent<RawImage>();
        _bButtonNextDisabled = false;
        _buttonToContinue = new OVRInput.Button();
    }
}
