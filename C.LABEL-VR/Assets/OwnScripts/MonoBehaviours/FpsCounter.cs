using System;
using UnityEngine;
using UnityEngine.UI;

public class FpsCounter : MonoBehaviour
{

    public Text fpsDisplay;
    private float fST;
    private float fAT;

    float fUpdateInSeconds = 0.5f;

    private void Start()
    {
        fST = Time.realtimeSinceStartup;
    }

    void Update()
    {
        fAT = Time.realtimeSinceStartup;

        if (fAT - fST > fUpdateInSeconds)
        {
            fpsDisplay.text = Convert.ToString(1.0f / Time.deltaTime);
            fST = fAT;
        }
    }
}
