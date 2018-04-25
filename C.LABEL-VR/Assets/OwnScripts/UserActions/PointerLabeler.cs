using System.Collections.Generic;
using UnityEngine;
using VRTK;

/// <summary>
/// Attach this to Controller to annotate points with Pointer
/// </summary>
public class PointerLabeler : MonoBehaviour
{
    public static PointerLabeler Instance { get; private set; }

    public bool ClusterLabelingEnabled { get; set; }
    public bool LabelingEnabled { get; set; }

    private bool pointerActivated;
    private bool pointerSelectionActivated;

    private VRTK_Pointer rightPointer;
    private VRTK_StraightPointerRenderer rightPointerRenderer;

    private float startTime;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    // Use this for initialization
    void Start()
    {
        ClusterLabelingEnabled = true;
        LabelingEnabled = true;

        rightPointer = ReferenceHandler.Instance.GetRightPointer();
        rightPointerRenderer = ReferenceHandler.Instance.GetRightPointerRenderer();

        rightPointer.ActivationButtonPressed += SetPointerActivated;
        rightPointer.ActivationButtonReleased += SetPointerDeactivated;
        rightPointer.SelectionButtonPressed += SetPointerSelectionActivated;
        rightPointer.SelectionButtonReleased += SetPointerSelectionDeactivated;

        startTime = Time.realtimeSinceStartup;
    }



    // Update is called once per frame
    void Update()
    {
        if (pointerActivated && LabelingEnabled)
        {
            if (OVRInput.Get(OVRInput.Button.PrimaryThumbstick))
            {
                if (pointerSelectionActivated)
                {
                    var collidedObject = GetPointerCollisionObject();
                    if (!collidedObject)
                        return;

                    //Clustering
                    if (collidedObject.GetComponent<CustomAttributes>()._label != Labeling.currentLabelClassID && ClusterLabelingEnabled)
                    {
                        List<GameObject> clusteredObjects = Clustering.GetClusterByRadiusSearch(collidedObject, 5.5f, false);
                        for (int i = 0; i < clusteredObjects.Count; i++)
                        {
                            if (clusteredObjects[i].GetComponent<CustomAttributes>()._label != Labeling.currentLabelClassID)
                            {
                                clusteredObjects[i].GetComponent<CustomAttributes>()._label = Labeling.currentLabelClassID;
                            }
                        }
                    }
                }
            }
            else
            {
                if (pointerSelectionActivated)
                {

                    var collidedObject = GetPointerCollisionObject();
                    if (!collidedObject)
                        return;

                    var collider = Physics.OverlapSphere(collidedObject.transform.position, collidedObject.GetComponent<SphereCollider>().radius * collidedObject.transform.localScale.x);

                    for (int i = 0; i < collider.Length; i++)
                    {
                        var attr = collider[i].gameObject.GetComponent<CustomAttributes>();

                        if (attr)
                        {
                            if (attr._label != Labeling.currentLabelClassID)
                            {
                                attr._label = Labeling.currentLabelClassID;
                            }
                        }
                    }
                }
            }
        }
    }

    private void SetPointerActivated(object sender, ControllerInteractionEventArgs e)
    {
        pointerActivated = true;
    }

    private void SetPointerDeactivated(object sender, ControllerInteractionEventArgs e)
    {
        pointerActivated = false;
    }

    private void SetPointerSelectionActivated(object sender, ControllerInteractionEventArgs e)
    {
        pointerSelectionActivated = true;
    }

    private void SetPointerSelectionDeactivated(object sender, ControllerInteractionEventArgs e)
    {
        pointerSelectionActivated = false;
    }

    private GameObject GetPointerCollisionObject()
    {
        try
        {
            var collisionObject = rightPointerRenderer.GetDestinationHit().collider.gameObject;
            if (collisionObject.GetComponent<CustomAttributes>())
            {
                return collisionObject;
            }
            else
            {
                return null;
            }
        }
        catch
        {
            return null;
        }
    }
}
