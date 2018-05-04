using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step3_Labeling_PointerLabeler : TutorialStep<Step3_Labeling_PointerLabeler>
{
    private static GameObject testPoints;

    public static void Show()
    {
        Open();
        PointerLabeler.Instance.enabled = true;
        testPoints = Instantiate(Resources.Load("Prefabs/TestPoints")) as GameObject;
        testPoints.transform.position = Instance.transform.position;
    }

    public override void OnClickNext()
    {
        Destroy(testPoints);
        Step4_Labeling_ClusterLabeling.Show();
    }
}
