using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Step8_MovementTeleport : TutorialStep<Step8_MovementTeleport>
{
    private bool menuShown;
    private Canvas thisCanvas;

    public static void Show()
    {
        Open();
        Movement.Instance.enabled = true;
        MovementOptions.MoveMode = Util.MovementMode.TeleportMode;
        PointerTeleport.Instance.enabled = true; ;
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
        SceneManager.LoadScene(0);
    }
}
