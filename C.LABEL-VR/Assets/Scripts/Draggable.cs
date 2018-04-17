using UnityEngine;
using VRTK;

public class Draggable : MonoBehaviour
{
    
    public bool fixX;
    public bool fixY;
    public Transform thumb;
    bool dragging;

    private VRTK_UIPointer uiPointer;
    private VRTK_StraightPointerRenderer pointerRenderer;

    private void Start()
    {
        uiPointer = ReferenceHandler.Instance.GetRightUiPointer();
        pointerRenderer = ReferenceHandler.Instance.GetRightPointerRenderer();
    }

    void FixedUpdate()
    {
        bool gripHold = OVRInput.Get(OVRInput.Button.SecondaryHandTrigger);
        bool triggerClick = OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger);
        bool triggerHold = OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger);
        bool triggerUp = OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger);

        if (gripHold & triggerClick)
        {
            dragging = false;
            Ray ray = new Ray(uiPointer.GetOriginPosition(), uiPointer.GetOriginForward());
            RaycastHit hit;
            if (GetComponent<Collider>().Raycast(ray, out hit, 100))
            {
                dragging = true;
            }
        }

        if (triggerUp)
        {
            dragging = false;
        }

        if (dragging && triggerHold)
        {
            var point = pointerRenderer.actualCursor.transform.position;
            point = GetComponent<Collider>().ClosestPointOnBounds(point);
            SetThumbPosition(point);
            SendMessage("OnDrag", Vector3.one - (thumb.position - GetComponent<Collider>().bounds.min) / GetComponent<Collider>().bounds.size.x);
        }


        //if (Input.GetMouseButtonDown(0))
        //{
        //    dragging = false;
        //    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;
        //    if (GetComponent<Collider>().Raycast(ray, out hit, 100))
        //    {
        //        dragging = true;
        //    }
        //}
        //if (Input.GetMouseButtonUp(0)) dragging = false;
        //if (dragging && Input.GetMouseButton(0))
        //{
        //    var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    point = GetComponent<Collider>().ClosestPointOnBounds(point);
        //    SetThumbPosition(point);
        //    SendMessage("OnDrag", Vector3.one - (thumb.position - GetComponent<Collider>().bounds.min) / GetComponent<Collider>().bounds.size.x);
        //}
    }

    void SetDragPoint(Vector3 point)
    {
        point = (Vector3.one - point) * GetComponent<Collider>().bounds.size.x + GetComponent<Collider>().bounds.min;
        SetThumbPosition(point);
    }

    void SetThumbPosition(Vector3 point)
    {
        thumb.position = new Vector3(fixX ? thumb.position.x : point.x, fixY ? thumb.position.y : point.y, thumb.position.z);
    }
}
