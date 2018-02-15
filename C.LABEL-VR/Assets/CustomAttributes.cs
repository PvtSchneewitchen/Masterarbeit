using UnityEngine;

public class CustomAttributes : MonoBehaviour
{
    public Util.Tuple<int, int> _tableIndex { get; set; }

    public float _distance { get; set; }

    public float _intensity { get; set; }

    public float _labelPropability { get; set; }

    private Util.Labeling.LabelGroup _Label;
    public Util.Labeling.LabelGroup _label
    {
        get
        {
            return _Label;
        }
        set
        {
            _Label = value;
            try { GetComponent<MeshRenderer>().material = Util.Labeling.GetGroupMaterial(_label); }
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
            try { return transform.parent.transform.position; }
            catch { return _POSITION_VEHICLE; }
        }
        set
        {
            try { transform.parent.transform.position = value; }
            catch { _POSITION_VEHICLE = value; }
        }
    }

    public bool _groundPoint { get; set; }

    public void CopyAttributes(Util.CustomAttributesContainer container_inp)
    {
        _tableIndex = container_inp._tableIndex;
        _distance = container_inp._distance;
        _intensity = container_inp._intensity;
        _labelPropability = container_inp._labelPropability;
        _label = container_inp._label;
        _pointValid = container_inp._pointValid;
        _position_Sensor = container_inp._position_Sensor;
        _position_Vehicle = container_inp._position_Vehicle;
        _groundPoint = container_inp._groundPoint;
    }
}
