using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerLengthDisplay : MonoBehaviour
{
    public static PointerLengthDisplay Instance { get; private set; }

    public string PointerLength
    {
        get { return pointerLenght.text; }
        set { pointerLenght.text = value; }
    }

    private TextMesh pointerLenght;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void Start()
    {
        pointerLenght = GetComponent<TextMesh>();
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }


    //public GameObject _lenghtDisplay;
    //public GameObject _leftController;
    //private float _fUpperShift;

    //void Start()
    //{
    //    _fUpperShift = 0.1f;

    //    _lenghtDisplay.transform.position = _leftController.transform.position + _leftController.transform.up * _fUpperShift;
    //}
}
