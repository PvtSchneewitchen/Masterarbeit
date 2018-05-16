using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PointCloud
{
    public List<GameObject> ValidPoints { get; private set; }
    public string PathToInputFiles { get; private set; }

    public GameObject Origin { get; set; }


    public PointCloud(List<InternalDataFormat> dataList_inp, string dataPath_inp, Quaternion rotationToUnityCs, Vector3 mirroring_inp)
    {
        ValidPoints = new List<GameObject>();
        PathToInputFiles = dataPath_inp;
        Origin = CreateOrigin();

        for (int i = 0; i < dataList_inp.Count; i++)
        {
            GameObject point = Util.CreateDefaultLabelPoint();
            CustomAttributes attributes = point.GetComponent<CustomAttributes>();

            attributes.PointPosition = Util.MultiplyVectorValues(rotationToUnityCs * dataList_inp[i]._position , mirroring_inp);
            attributes.GroundPoint = dataList_inp[i]._groundPointLabel;
            attributes.ID = dataList_inp[i]._ID;
            attributes.Label = dataList_inp[i]._label;
            point.SetActive(false);
            ValidPoints.Add(point);
        }
    }

    public void EnableAllPoints()
    {
        for (int i = 0; i < ValidPoints.Count; i++)
        {
            ValidPoints[i].SetActive(true);
        }
    }

    public void DisableAllPoints()
    {
        for (int i = 0; i < ValidPoints.Count; i++)
        {
            ValidPoints[i].SetActive(false);
        }
    }

    public void DecreasePoints()
    {
        for (int i = 0; i < ValidPoints.Count; i++)
        {
            if (i % 2 == 0 || i % 3 == 0 || i % 5 == 0 || i % 7 == 0)
            {
                ValidPoints[i].SetActive(false);
            }
            
        }
    }

    public void ResetLabels()
    {
        for (int i = 0; i < ValidPoints.Count; i++)
        {
            ValidPoints[i].GetComponent<CustomAttributes>().Label = 0;
        }
    }

    public void RefreshPointsOfLabelCLass(uint oldLabelId, uint newLabelId)
    {
        for (int i = 0; i < ValidPoints.Count; i++)
        {
            var attr = ValidPoints[i].GetComponent<CustomAttributes>();
            if(attr.Label == oldLabelId)
            {
                attr.Label = newLabelId;
            }
        }
    }

    private GameObject CreateOrigin()
    {
        UnityEngine.Object o = Resources.Load("Prefabs/Origin");
        GameObject go = UnityEngine.Object.Instantiate(o) as GameObject;
        go.name = o.name;
        return go;
    }

    
}
