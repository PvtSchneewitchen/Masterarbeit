using System.Collections.Generic;
using UnityEngine;

public class CustomAttributes : MonoBehaviour
{
    public Tuple<int, int> _hdf5TableIndex { get; set; }

    public float _distance { get; set; }

    public float _intensity { get; set; }

    public float _labelPropability { get; set; }

    private Tuple<string, uint> _Label;
    public Tuple<string, uint> _label
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

    private Vector3 _POSITION_VEHICLE;
    public Vector3 _position_Vehicle
    {
        get
        {
            try { return transform.position; }
            catch { return _POSITION_VEHICLE; }
        }
        set
        {
            try { transform.position = value; }
            catch
            {
                Debug.Log("CustomAttributes Set: Position Assignment not possible!");
                _POSITION_VEHICLE = value;
            }
        }
    }

    public int _groundPoint { get; set; }

    public void CopyAttributes(CustomAttributesContainer attrContainer_inp)
    {
        _hdf5TableIndex = attrContainer_inp._hdf5TableIndex;
        _distance = attrContainer_inp._distance;
        _intensity = attrContainer_inp._intensity;
        _labelPropability = attrContainer_inp._labelPropability;
        _label = attrContainer_inp._label;
        _pointValid = attrContainer_inp._pointValid;
        _position_Sensor = attrContainer_inp._position_Sensor;
        _position_Vehicle = attrContainer_inp._position_Vehicle;
        _groundPoint = attrContainer_inp._groundPoint;
    }

    public class CustomAttributesContainer
    {
        public Tuple<int, int> _hdf5TableIndex { get; set; }

        public float _distance { get; set; }

        public float _intensity { get; set; }

        public float _labelPropability { get; set; }

        public Dictionary<string, uint> _labelWorkingSet { get; set; }

        public Tuple<string, uint> _label;

        public int _pointValid { get; set; }

        public Vector3 _position_Sensor { get; set; }

        public Vector3 _position_Vehicle { get; set; }

        public int _groundPoint { get; set; }

        public CustomAttributesContainer()
        {
            _distance = 0;
            _intensity = 0;
            _labelPropability = 0;
            _labelWorkingSet = new Dictionary<string, uint>();
            _pointValid = 0;
            _position_Sensor = Vector3.zero;
            _position_Vehicle = Vector3.zero;
            _groundPoint = 2;
        }
    }
}
