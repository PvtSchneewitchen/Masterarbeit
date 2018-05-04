using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step4_Labeling_ClusterLabeling : TutorialStep<Step4_Labeling_ClusterLabeling>
{
    private static GameObject testPoints;

    public static void Show()
    {
        Open();
        testPoints = Instantiate(Resources.Load("Prefabs/TestPoints")) as GameObject;
        testPoints.transform.position = Instance.transform.position;
    }

    public override void OnClickNext()
    {
        Destroy(testPoints);
        Step5_Labeling_TouchLabeling.Show();
    }
}
