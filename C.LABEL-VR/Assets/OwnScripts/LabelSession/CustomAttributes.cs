using System;
using UnityEngine;

public class CustomAttributes : MonoBehaviour
{
    //public int _ID { get; set; }
    [SerializeField]
    private int EditorID;
    public int ID { get { return EditorID; } set { EditorID = value; } }

    private uint _Label;
    public uint Label
    {
        get
        {
            return _Label;
        }
        set
        {
            _Label = value;
            try { GetComponent<MeshRenderer>().material = Labeling.GetLabelClassMaterial(_Label); }
            catch { Debug.Log("PointAttribute: No Meshrenderer"); }
        }
    }

    private Vector3 _PointPosition;
    public Vector3 PointPosition
    {
        get
        {
            try { return transform.position; }
            catch { return _PointPosition; }
        }
        set
        {
            try { transform.position = value; }
            catch
            {
                Debug.Log("CustomAttributes Set: Position Assignment not possible!");
                _PointPosition = value;
            }
        }
    }

    //0=nonGround, 1=ground, 2=notYetDefined
    public int GroundPoint { get; set; }
}
