﻿using System;
using UnityEngine;

public class CustomAttributes : MonoBehaviour
{
    public int _ID { get; set; }

    private uint _Label;
    public uint _label
    {
        get
        {
            return _Label;
        }
        set
        {
            _Label = value;
            try { GetComponent<MeshRenderer>().material = Labeling.GetLabelClassMaterial(_label); }
            catch { Debug.Log("PointAttribute: No Meshrenderer"); }
        }
    }

    private Vector3 _POINTPOSITION;
    public Vector3 _pointPosition
    {
        get
        {
            try { return transform.position; }
            catch { return _POINTPOSITION; }
        }
        set
        {
            try { transform.position = value; }
            catch
            {
                Debug.Log("CustomAttributes Set: Position Assignment not possible!");
                _POINTPOSITION = value;
            }
        }
    }

    public int _groundPoint { get; set; }
}