using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step6_InGameMenu : TutorialStep<Step6_InGameMenu>
{
    private bool menuShown;
    private Canvas thisCanvas;

    public static void Show()
    {
        Open();
        LabelClassDisplayUpdate.Instance.enabled = false;
    }

    private void OnEnable()
    {
        menuShown = true;
        thisCanvas = GetComponent<Canvas>();
    }

    private void Update()
    {
        if (menuShown && OVRInput.GetDown(OVRInput.Button.Start))
        {
            menuShown = false;
            thisCanvas.enabled = false;
        }
        else if(!menuShown && OVRInput.GetDown(OVRInput.Button.Start))
        {
            menuShown = true;
            thisCanvas.enabled = true;
        }
    }

    public override void OnClickNext()
    {
        Step7_MovementFreeFly.Show();
    }
}
