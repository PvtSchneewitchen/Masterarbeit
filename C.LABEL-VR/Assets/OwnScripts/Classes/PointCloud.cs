using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accord.MachineLearning;
using Accord.Statistics.Models.Regression.Linear;
using OpenCvSharp;

public class PointCloud
{

    public List<GameObject> _points { get; private set; }
    public GameObject _origin { get; set; }
    public List<GameObject> _groundPoints { get; private set; }
    public List<GameObject> _nonGroundPoints { get; private set; }
     public PointOctree<GameObject> _ocTree { get; set; }

    public PointCloud(List<Vector3> pcdCoordinates_inp, Util.Datatype type = Util.Datatype.pcd)
    {
        _points = new List<GameObject>();
        _origin = CreateOrigin();

        for (int i = 0; i < pcdCoordinates_inp.Count; i++)
        {
            GameObject point = Util.CreateDefaultLabelPoint();
            point.transform.position = new Vector3(pcdCoordinates_inp[i].x, pcdCoordinates_inp[i].y, pcdCoordinates_inp[i].z);
            point.SetActive(false);
            _points.Add(point);
        }

        _ocTree = new PointOctree<GameObject>(GetBiggestDistance(_points), _origin.transform.position, 0.001f);

        OrdinaryLeastSquares ols = new OrdinaryLeastSquares();
        KNearestNeighbors<GameObject> n = new KNearestNeighbors<GameObject>();
    }

    public void EnableAllPoints()
    {
        for (int i = 0; i < _points.Count; i++)
        {
            _points[i].SetActive(true);
        }
    }

    public void DisableAllPoints()
    {
        for (int i = 0; i < _points.Count; i++)
        {
            _points[i].SetActive(false);
        }
    }

    public void DecreasePoints()
    {
        for (int i = 0; i < _points.Count; i++)
        {
            if (i % 2 == 0 || i % 3 == 0 || i % 5 == 0 || i % 7 == 0)
            {
                _points[i].SetActive(false);
            }
        }
    }

    private GameObject CreateOrigin()
    {
        UnityEngine.Object o = Resources.Load("Origin");
        GameObject go = UnityEngine.Object.Instantiate(o) as GameObject;
        go.name = o.name;
        return go;
    }

    private float GetBiggestDistance(List<GameObject> points_inp)
    {
        List<GameObject> lowestValuePoints = new List<GameObject>(4);
        List<float> distances = new List<float>();

        //init list
        for (int i = 0; i < lowestValuePoints.Capacity; i++)
        {
            lowestValuePoints.Add(points_inp[0]);
        }

        //search for lowest values
        for (int i = 1; i < points_inp.Count; i++)
        {
            if(points_inp[i].transform.position.x < lowestValuePoints[0].transform.position.x)
            {
                lowestValuePoints[0] = points_inp[i];
            }
            if (points_inp[i].transform.position.z < lowestValuePoints[1].transform.position.z)
            {
                lowestValuePoints[1] = points_inp[i];
            }
            if (points_inp[i].transform.position.x > lowestValuePoints[2].transform.position.x)
            {
                lowestValuePoints[2] = points_inp[i];
            }
            if (points_inp[i].transform.position.z > lowestValuePoints[3].transform.position.z)
            {
                lowestValuePoints[3] = points_inp[i];
            }
        }

        //calculate distances
        for (int i = 0; i < lowestValuePoints.Count; i++)
        {
            for (int j = 0; j < lowestValuePoints.Count; j++)
            {
                distances.Add(Vector3.Distance(lowestValuePoints[i].transform.position, lowestValuePoints[j].transform.position));
            }
        }

        //sort descending
        distances.Sort((x, y) => -1*x.CompareTo(y));

        return distances[0];
    }
}
