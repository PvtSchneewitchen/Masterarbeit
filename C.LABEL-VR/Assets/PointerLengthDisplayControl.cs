using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerLengthDisplayControl : MonoBehaviour {

    public GameObject _lenghtDisplay;
    public GameObject _leftController;
    private float _fUpperShift;

    void Start () {
        _fUpperShift = 0.1f;

        _lenghtDisplay.transform.position = _leftController.transform.position + _leftController.transform.up * _fUpperShift;
	}
	
	void Update () {
	}
}
