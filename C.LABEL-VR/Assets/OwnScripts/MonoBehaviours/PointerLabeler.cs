using System.Collections.Generic;
using UnityEngine;
using VRTK;
using System.Linq;


/// <summary>
/// Attach this to Controller to annotate points with Pointer
/// </summary>
public class PointerLabeler : MonoBehaviour
{
    #region self pointer
    //private GameObject _pointerTracer;
    //private GameObject _pointerCursor;

    //public float _tracerLength
    //{
    //    get
    //    {
    //        return _pointerTracer.transform.localScale.z;
    //    }
    //    set
    //    {
    //        _pointerTracer.transform.localScale = new Vector3(0, 0, value);
    //        _pointerTracer.transform.localPosition = new Vector3(0, 0, value / 2);
    //        _pointerCursor.transform.localPosition = new Vector3(0, 0, value);
    //    }
    //}

    //private float _TracerScale { get; set; }
    //private float _tracerScale
    //{
    //    get
    //    {
    //        return _TracerScale;
    //    }
    //    set
    //    {
    //        _TracerScale = value;
    //        _pointerTracer.transform.localScale = new Vector3(value, value, _tracerLength);
    //    }
    //}

    //private float _CursorScale { get; set; }
    //private float _cursorScale
    //{
    //    get
    //    {
    //        return _CursorScale;
    //    }
    //    set
    //    {
    //        _CursorScale = value;
    //        _pointerCursor.transform.localScale = new Vector3(value, value, value);
    //    }
    //}

    //private float _cursorRadius { get { return _pointerCursor.GetComponent<SphereCollider>().radius; } }

    //private void Start()
    //{

    //    InitPointer();
    //}

    //private void InitPointer()
    //{
    //    Object tr = Resources.Load("DefaultTracer");
    //    Object cu = Resources.Load("DefaultCursor");

    //    _pointerTracer = Instantiate(tr) as GameObject;
    //    _pointerCursor = Instantiate(cu) as GameObject;

    //    _pointerTracer.transform.parent = gameObject.transform;
    //    _pointerCursor.transform.parent = gameObject.transform;

    //    _tracerLength = 10;
    //    _tracerScale = 0.002f;
    //    _cursorScale = 0.1f;
    //}

    //public void HandleTracerCollision(GameObject collisionObject_inp)
    //{
    //    float fDistance = Vector3.Distance(collisionObject_inp.transform.position, _pointerTracer.transform.position);
    //    _tracerLength = fDistance - _cursorRadius;
    //}
    #endregion

    public ControlScript _controlScript;

    private bool _bPointerActivated;
    private bool _bPointerSelectionActivated;

    private VRTK_Pointer _pointer;
    private VRTK_StraightPointerRenderer _pointerRenderer;
    private CustomAttributes _collidedObjectAttributes;

    // Use this for initialization
    void Start()
    {
        _pointer = GetComponent<VRTK_Pointer>();
        _pointerRenderer = GetComponent<VRTK_StraightPointerRenderer>();

        _pointer.ActivationButtonPressed += SetPointerActivated;
        _pointer.ActivationButtonReleased += SetPointerDeactivated;
        _pointer.SelectionButtonPressed += SetPointerSelectionActivated;
        _pointer.SelectionButtonReleased += SetPointerSelectionDeactivated;
    }

    // Update is called once per frame
    void Update()
    {
        if (_bPointerActivated)
        {
            if (_bPointerSelectionActivated)
            {
                Collider collidedObject = _pointerRenderer.GetDestinationHit().collider;
                if (collidedObject != null && collidedObject.name.Contains("Label"))
                {
                    _collidedObjectAttributes = collidedObject.gameObject.GetComponent<CustomAttributes>();

                    if (OVRInput.Get(OVRInput.Button.One))
                    {
                        //adjacent labeling
                        if (_collidedObjectAttributes._label != Util.Labeling._currentLabel)
                        {
                            float t = Time.realtimeSinceStartup;
                            List<GameObject> adjacentObjects = GetAdjacentLabelPoints(collidedObject.gameObject);

                            for (int i = 0; i < adjacentObjects.Count; i++)
                            {
                                if (adjacentObjects[i].GetComponent<CustomAttributes>()._label != Util.Labeling._currentLabel)
                                {
                                    adjacentObjects[i].GetComponent<CustomAttributes>()._label = Util.Labeling._currentLabel;
                                }
                            }

                            print(t - Time.realtimeSinceStartup + "s");
                        }
                    }
                    else if (OVRInput.Get(OVRInput.Button.Two))
                    {
                        if (_collidedObjectAttributes._label != Util.Labeling._currentLabel)
                        {
                            float t = Time.realtimeSinceStartup;
                            List<GameObject> adjacentObjects = GetAdjacentLabelPointsFromOcTree(collidedObject.gameObject);

                            for (int i = 0; i < adjacentObjects.Count; i++)
                            {
                                if (adjacentObjects[i].GetComponent<CustomAttributes>()._label != Util.Labeling._currentLabel)
                                {
                                    adjacentObjects[i].GetComponent<CustomAttributes>()._label = Util.Labeling._currentLabel;
                                }
                            }

                            print(t - Time.realtimeSinceStartup + "s");
                        }
                    }
                    else
                    {
                        //simple single point labeling
                        if (_collidedObjectAttributes._label != Util.Labeling._currentLabel)
                        {
                            _collidedObjectAttributes._label = Util.Labeling._currentLabel;
                        }
                    }
                }
            }
        }
    }

    public void SetPointerActivated(object sender, ControllerInteractionEventArgs e)
    {
        _bPointerActivated = true;
    }

    private void SetPointerDeactivated(object sender, ControllerInteractionEventArgs e)
    {
        _bPointerActivated = false;
    }

    private void SetPointerSelectionActivated(object sender, ControllerInteractionEventArgs e)
    {
        _bPointerSelectionActivated = true;
    }

    private void SetPointerSelectionDeactivated(object sender, ControllerInteractionEventArgs e)
    {
        _bPointerSelectionActivated = false;
    }

    private List<GameObject> GetAdjacentLabelPointsFromOcTree(GameObject startPoint_inp)
    {
        Dictionary<int, GameObject> adjacentObjects = new Dictionary<int, GameObject>();
        float radius = startPoint_inp.GetComponent<SphereCollider>().radius * startPoint_inp.transform.localScale.x;

        RecursiveRadiusSearch(ref adjacentObjects, new List<GameObject> { startPoint_inp }, radius);

        return adjacentObjects.Values.ToList();
    }

    private List<GameObject> GetAdjacentLabelPoints(GameObject startPoint_inp)
    {
        Dictionary<int, GameObject> adjacentObjects = new Dictionary<int, GameObject>();
        float radius = startPoint_inp.GetComponent<SphereCollider>().radius * startPoint_inp.transform.localScale.x;

        RecursiveRadiusSearch(ref adjacentObjects, new List<GameObject>{startPoint_inp}, radius);

        return adjacentObjects.Values.ToList();
    }

    private void RecursiveRadiusSearch(ref Dictionary<int, GameObject> outputObjects, List<GameObject> objectsToCheck_inp, float radius_inp)
    {
        List<GameObject> newObjectsToCheck = new List<GameObject>();

        for (int i = 0; i < objectsToCheck_inp.Count; i++)
        {
            Collider[] collidersInRadius = Physics.OverlapSphere(objectsToCheck_inp[i].transform.position, radius_inp);

            for (int j = 0; j < collidersInRadius.Length; j++)
            {
                if (!outputObjects.ContainsKey(collidersInRadius[j].gameObject.GetInstanceID()) && collidersInRadius[j].gameObject.name.Contains("Label"))
                {
                    outputObjects.Add(collidersInRadius[j].gameObject.GetInstanceID(), collidersInRadius[j].gameObject);
                    newObjectsToCheck.Add(collidersInRadius[j].gameObject);
                }
            }
        }

        if (newObjectsToCheck.Count > 0)
        {
            RecursiveRadiusSearch(ref outputObjects, newObjectsToCheck, radius_inp);
        }
    }
}
