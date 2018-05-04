using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step5_Labeling_TouchLabeling : TutorialStep<Step5_Labeling_TouchLabeling>
{
    private static GameObject testPoints;

    public static void Show()
    {
        Open();
        PointerLabeler.Instance.enabled = false;
        TouchLabeler.Instance.enabled = true;
        testPoints = Instantiate(Resources.Load("Prefabs/TestPointsTouch")) as GameObject;
        testPoints.transform.position = OVRManager.instance.transform.position - OVRManager.instance.transform.up * 0.3f;
    }

    public override void OnClickNext()
    {
        Destroy(testPoints);
        Step6_InGameMenu.Show();
    }
}
