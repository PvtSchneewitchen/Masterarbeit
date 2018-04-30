using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class PointCloud
{
    public List<GameObject> _validPoints { get; private set; }
    public string _pathToPointCloudData { get; private set; }

    public GameObject _origin { get; set; }

    private const float _alphaMax = 45;
    private const float _hMin = 0.1f;
    private List<Tuple<Vector3, bool>> list;


    public PointCloud(List<InternalDataFormat> dataList_inp, string dataPath_inp, Quaternion rotationToUnityCs, Vector3 mirroring_inp)
    {
        _validPoints = new List<GameObject>();
        _pathToPointCloudData = dataPath_inp;
        _origin = CreateOrigin();

        for (int i = 0; i < dataList_inp.Count; i++)
        {
            GameObject point = Util.CreateDefaultLabelPoint();
            CustomAttributes attributes = point.GetComponent<CustomAttributes>();

            attributes._pointPosition = Util.MultiplyVectorValues(rotationToUnityCs * dataList_inp[i]._position , mirroring_inp);
            attributes._groundPoint = dataList_inp[i]._groundPointLabel;
            attributes._ID = dataList_inp[i]._ID;
            attributes._label = dataList_inp[i]._label;
            point.SetActive(false);
            _validPoints.Add(point);
        }
    }

    public void EnableAllPoints()
    {
        for (int i = 0; i < _validPoints.Count; i++)
        {
            _validPoints[i].SetActive(true);
        }
    }

    public void DisableAllPoints()
    {
        for (int i = 0; i < _validPoints.Count; i++)
        {
            _validPoints[i].SetActive(false);
        }
    }

    public void DecreasePoints()
    {
        for (int i = 0; i < _validPoints.Count; i++)
        {
            if (i % 2 == 0 || i % 3 == 0 || i % 5 == 0 || i % 7 == 0)
            {
                _validPoints[i].SetActive(false);
            }
            
        }
    }

    public void ResetLabels()
    {
        for (int i = 0; i < _validPoints.Count; i++)
        {
            _validPoints[i].GetComponent<CustomAttributes>()._label = 0;
        }
    }

    public void RefreshPointsOfLabelCLass(uint oldLabelId, uint newLabelId)
    {
        for (int i = 0; i < _validPoints.Count; i++)
        {
            var attr = _validPoints[i].GetComponent<CustomAttributes>();
            if(attr._label == oldLabelId)
            {
                attr._label = newLabelId;
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
