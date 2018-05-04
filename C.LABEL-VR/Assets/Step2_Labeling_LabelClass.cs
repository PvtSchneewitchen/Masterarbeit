using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step2_Labeling_LabelClass : TutorialStep<Step2_Labeling_LabelClass>
{ 
    public static void Show()
    {
        Open();
        LabelClassDisplayUpdate.Instance.enabled = true;
    }

    public override void OnClickNext()
    {
        Step3_Labeling_PointerLabeler.Show();
    }
}
