using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step7_MovementFreeFly : TutorialStep<Step7_MovementFreeFly>
{
    private bool menuShown;
    private Canvas thisCanvas;

    public static void Show()
    {
        OVRManager.instance.transform.Translate(-Vector3.forward*2);
        Movement.Instance.enabled = true;
        LabelClassDisplayUpdate.Instance.enabled = true;
        PointerLabeler.Instance.enabled = true;
        MovementOptions.MoveMode = Util.MovementMode.FreeFly;
        TutorialSessionHandler.Instance.Session.GetCurrentPointCloud().EnableAllPoints();
        Open();
    }

    private void OnEnable()
    {
        menuShown = true;
        thisCanvas = GetComponent<Canvas>();
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Start))
            TutorialMenuManager.Instance.AlignMenu(Instance);


        //if (menuShown && OVRInput.GetDown(OVRInput.Button.Start))
        //{
        //    menuShown = false;
        //    thisCanvas.enabled = false;
        //    Movement.Instance.enabled = true;
        //}
        //else if (!menuShown && OVRInput.GetDown(OVRInput.Button.Start))
        //{
        //    menuShown = true;
        //    thisCanvas.enabled = true;
        //    Movement.Instance.enabled = false;
        //    TutorialMenuManager.Instance.AlignMenu(Instance);
        //}
    }

    public override void OnClickNext()
    {
        Step8_MovementTeleport.Show();
    }
}
