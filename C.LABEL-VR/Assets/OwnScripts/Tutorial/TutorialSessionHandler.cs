using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TutorialSessionHandler : MonoBehaviour
{
    public static TutorialSessionHandler Instance { get; private set; }

    public LabelSession Session { get; set; }

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
        List<PointCloud> pointClouds;

#if UNITY_EDITOR
        pointClouds= Import.ImportPcd("C:\\Users\\gruepazu\\Desktop\\MainBuild\\TutorialData");
#else
        pointClouds = Import.ImportPcd(Path.Combine(Directory.GetParent(Application.dataPath).FullName,"TutorialData\\TutorialCloud.pcd"));
#endif
        Session = new LabelSession(pointClouds, 0);

        foreach (var cloud in Session.PointClouds)
        {
            cloud.Origin.SetActive(false);
        }

        DisableFunctions();

        Step1_UiInteraction.Show();
    }

    private void DisableFunctions()
    {
        Movement.Instance.enabled = false;
        PointerLabeler.Instance.enabled = false;
        PointerTeleport.Instance.enabled = false;
        PointerLengthDisplay.Instance.enabled = false;
        LabelClassDisplayUpdate.Instance.enabled = false;
        TouchLabeler.Instance.enabled = false;
    }
}