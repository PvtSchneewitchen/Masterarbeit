using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRTK;

class LabelClassPipette : MonoBehaviour
{
    public static LabelClassPipette Instance { get; set; }

    private VRTK_Pointer rightPointer;
    private VRTK_StraightPointerRenderer rightPointerRenderer;
    private bool pointerActivated;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void Start()
    {
        rightPointer = ReferenceHandler.Instance.GetRightPointer();
        rightPointerRenderer = ReferenceHandler.Instance.GetRightPointerRenderer();

        rightPointer.ActivationButtonPressed += SetPointerActivated;
        rightPointer.ActivationButtonReleased += SetPointerDeactivated;
    }

    private void Update()
    {
        if (pointerActivated)
        {
            if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick))
            {
                GameObject collidedObject= rightPointerRenderer.GetDestinationHit().collider.gameObject;
                var attr = collidedObject.GetComponent<CustomAttributes>();
                if(attr)
                {
                    if(Labeling.currentLabelClassID != attr._label)
                        Labeling.SetCurrentLabelClassID(attr._label);
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
}

