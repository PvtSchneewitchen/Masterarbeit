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
    public bool PointerLabelingEnabled { get; set; }

    private bool pointerActivated;
    private bool pointerSelectionActivated;

    private VRTK_Pointer rightPointer;
    private VRTK_StraightPointerRenderer rightPointerRenderer;


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
        PointerLabelingEnabled = true;

        rightPointer = ReferenceHandler.Instance.GetRightPointer();
        rightPointerRenderer = ReferenceHandler.Instance.GetRightPointerRenderer();

        rightPointer.ActivationButtonPressed += SetPointerActivated;
        rightPointer.ActivationButtonReleased += SetPointerDeactivated;
        rightPointer.SelectionButtonPressed += SetPointerSelectionActivated;
        rightPointer.SelectionButtonReleased += SetPointerSelectionDeactivated;
    }

    // Update is called once per frame
    void Update()
    {
        if (pointerActivated && pointerSelectionActivated)
        {
            GameObject collidedObject = new GameObject();
            try
            {
                collidedObject = rightPointerRenderer.GetDestinationHit().collider.gameObject;
            }
            catch
            {
                return;
            }

            if (collidedObject != null && collidedObject.name.Contains("DefaultLabelpoint"))
            {
                CustomAttributes collidedObjectAttributes = collidedObject.GetComponent<CustomAttributes>();

                if (OVRInput.Get(OVRInput.Button.SecondaryThumbstick))
                {
                    //Clustering
                    if (collidedObjectAttributes._label != Labeling.currentLabelClassID && ClusterLabelingEnabled)
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
                else
                {
                    //simple single point labeling
                    if (PointerLabelingEnabled)
                    {
                        var objectsAroundTarget = Clustering.RadiusSearch(collidedObject, collidedObject.GetComponent<SphereCollider>().radius);

                        for (int i = 0; i < objectsAroundTarget.Count; i++)
                        {
                            var attr = objectsAroundTarget[i].GetComponent<CustomAttributes>();

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

    public void SetPointerActivated(object sender, ControllerInteractionEventArgs e)
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
}
