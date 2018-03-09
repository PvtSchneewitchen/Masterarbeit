using System;
using UnityEngine;

public class InternalDataFormat
{ 
    public int _ID { get; set; }
    public uint _label { get; set; }
    public Vector3 _position { get; set; }
    public int _groundPointLabel { get; set; }

    public InternalDataFormat(int id_inp, Vector3 position_inp, uint label_inp, int groundPointLabel_inp)
    {
        _ID = id_inp;
        _label = label_inp;
        _position = position_inp;
        _groundPointLabel = groundPointLabel_inp;
    }
}
