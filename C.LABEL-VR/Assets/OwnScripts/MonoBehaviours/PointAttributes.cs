using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class PointAttributes : MonoBehaviour
{
    public Tuple<int, int> _tableIndex { get; set; }

    public float _distance { get; set; }

    public float _intensity { get; set; }

    public float _labelPropability { get; set; }

    private Labeling.LabelGroup _Label;
    public Labeling.LabelGroup _label
    {
        get
        {
            return _Label;
        }
        set
        {
            _Label = value;
            try { GetComponent<MeshRenderer>().material = Labeling.GetGroupMaterial(_label); }
            catch { Debug.Log("PointAttribute: No Meshrenderer"); }
        }
    }

    public int _pointValid { get; set; }

    public Vector3 _position_Sensor { get; set; }

    public Vector3 _position_Vehicle
    {
        get
        {
            try { return transform.parent.transform.position; }
            catch { return _position_Vehicle; }
        }
        set
        {
            try { transform.parent.transform.position = value; }
            catch { _position_Vehicle = value; }
        }
    }

    public bool _groundPoint { get; set; }

    public PointAttributes()
    {
        _tableIndex = new Tuple<int, int>(0, 0);
        _distance = 0;
        _intensity = 0;
        _labelPropability = 0;
        _label = Labeling.LabelGroup.unlabeled;
        _pointValid = 0;
        _position_Sensor = Vector3.zero;
        _position_Vehicle = Vector3.zero;
        _groundPoint = true;
    }

    public void CopyAttributes(PointAttributes attributes_inp)
    {
        _tableIndex = attributes_inp._tableIndex;
        _distance = attributes_inp._distance;
        _intensity = attributes_inp._intensity;
        _labelPropability = attributes_inp._labelPropability;
        _label = attributes_inp._label;
        _pointValid = attributes_inp._pointValid;
        _position_Sensor = attributes_inp._position_Sensor;
        _position_Vehicle = attributes_inp._position_Vehicle;
        _groundPoint = attributes_inp._groundPoint;
    }

    private void Reset()
    {
        transform.parent.transform.position = _position_Vehicle;
    }

    public class Tuple<T1>
    {
        public Tuple(T1 item1)
        {
            Item1 = item1;
        }

        public T1 Item1 { get; set; }
    }

    public class Tuple<T1, T2> : Tuple<T1>
    {
        public Tuple(T1 item1, T2 item2) : base(item1)
        {
            Item2 = item2;
        }

        public T2 Item2 { get; set; }
    }
}
