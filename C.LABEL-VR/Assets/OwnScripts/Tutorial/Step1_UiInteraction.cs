using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step1_UiInteraction : TutorialStep<Step1_UiInteraction>
{
    public static void Show()
    {
        Open();
    }

    public override void OnClickNext()
    {
        Step2_Labeling_LabelClass.Show();      
    }
}
