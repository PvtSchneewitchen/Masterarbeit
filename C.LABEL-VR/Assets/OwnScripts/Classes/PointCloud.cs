using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCloud
{
    public List<GameObject> _points { get; private set; }
    public GameObject _origin { get; set; }

    private const float _alphaMax = 45;
    private const float _hMin = 0.1f;

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
    }

    //public PointCloud(HDF5Addon.Lidar_Daimler hdf5Container_inp)
    //{
    //    List<GameObject> points = new List<GameObject>();
    //    _origin = CreateOrigin();

    //    int rows = hdf5Container_inp._sensorX.GetLength(0);
    //    int cols = hdf5Container_inp._sensorX.GetLength(1);

    //    PointAttributes[,] attributeTable = new PointAttributes[rows, cols];

    //    for (int i = 0; i < rows; i++)
    //    {
    //        for (int j = 0; j < cols; j++)
    //        {
    //            PointAttributes attribute = new PointAttributes
    //            {
    //                _tableIndex = new PointAttributes.Tuple<int, int>(i, j),
    //                _distance = hdf5Container_inp._distances[i, j],
    //                _intensity = hdf5Container_inp._intensity[i, j],
    //                _labelPropability = hdf5Container_inp._labelProbabilities[i, j],
    //                //TODO import labelWorkingSet the right way
    //                _label = Util.Labeling.LabelGroup.unlabeled,
    //                _pointValid = hdf5Container_inp._pointValid[i, j],
    //                _position_Sensor = Quaternion.Euler(0, 180, 0) * new Vector3(hdf5Container_inp._sensorX[i, j], hdf5Container_inp._sensorZ[i, j], hdf5Container_inp._sensorY[i, j]),
    //                _position_Vehicle = new Vector3(hdf5Container_inp._vehicleX[i, j], hdf5Container_inp._vehicleZ[i, j], hdf5Container_inp._vehicleY[i, j])
    //            };

    //            attributeTable[i, j] = attribute;
    //        }
    //    }

    //    CalculateGroundPoints(attributeTable, ref points);

    //    _points = points;
    //}


    //private void CalculateGroundPoints(PointAttributes[,] pointTable_inp, ref List<GameObject> points_out)
    //{
    //    int rows = pointTable_inp.GetLength(0);
    //    int cols = pointTable_inp.GetLength(1);

    //    for (int i = 0; i < cols; i++)
    //    {
    //        //+1 because of the selfmade start ground point 
    //        PointAttributes[] verticalLine = new PointAttributes[rows+1];

    //        for (int j = 0; j < rows; j++)
    //        {
    //            verticalLine[j] = pointTable_inp[j, i];
    //        }

    //        ProcessVerticalLine(verticalLine, ref points_out);
    //    }
    //}

    //private void ProcessVerticalLine(PointAttributes[] verticalLine_inp, ref List<GameObject> points_out)
    //{
    //    int thresholdPoint = -1;

    //    //start grounbd point at sensors location
    //    verticalLine_inp[verticalLine_inp.Length] = new PointAttributes();

    //    for (int i = verticalLine_inp.Length; i > 0; i--)
    //    {
    //        int prePoint = i;
    //        int curPoint = i - 1;

    //        //First stage (Gradient-Query)
    //        float height = Mathf.Abs(verticalLine_inp[prePoint]._position_Vehicle.y - verticalLine_inp[curPoint]._position_Vehicle.y);
    //        float distance = Vector3.Distance(verticalLine_inp[prePoint]._position_Vehicle, verticalLine_inp[curPoint]._position_Vehicle);
    //        float alpha = Mathf.Asin(height / distance);

    //        if (alpha >= _alphaMax)
    //        {
    //            thresholdPoint = prePoint;
    //            break;
    //        }

    //        //Second Stage (Lost-PointQuery)
    //        if (verticalLine_inp[curPoint]._pointValid == 0)
    //        {
    //            //find next valid point
    //            int nextV = curPoint - 1;
    //            bool found = false;
    //            while (nextV < verticalLine_inp.Length)
    //            {
    //                if (verticalLine_inp[nextV]._pointValid == 1)
    //                {
    //                    found = true;
    //                    break;
    //                }

    //                nextV++;
    //            }

    //            if (found)
    //            {
    //                float tempHeight = Mathf.Abs(verticalLine_inp[prePoint]._position_Vehicle.y - verticalLine_inp[nextV]._position_Vehicle.y);
    //                if (tempHeight > _hMin)
    //                {
    //                    thresholdPoint = prePoint;
    //                    break;
    //                }
    //            }
    //            else
    //            {
    //                thresholdPoint = prePoint;
    //                break;
    //            }
    //        }

    //        //Third stage (Abnormality-Query)
    //        if(verticalLine_inp[prePoint]._distance > verticalLine_inp[curPoint]._distance)
    //        {
    //            thresholdPoint = prePoint;
    //            break;
    //        }

    //        //No query was valid -> next Step
    //    }

    //    if (thresholdPoint >= 0)
    //    {
    //        //found
    //        for (int i = verticalLine_inp.Length; i > 1; i--)
    //        {
    //            if (i >= thresholdPoint)
    //            {
    //                //is ground
    //                if (verticalLine_inp[i]._pointValid == 1)
    //                {
    //                    verticalLine_inp[i]._groundPoint = true;
    //                    GameObject point = Util.CreateDefaultLabelPoint();
    //                    point.GetComponent<PointAttributes>().CopyAttributes(verticalLine_inp[i]);
    //                    point.SetActive(false);
    //                    points_out.Add(point);
    //                }
    //            }
    //            else
    //            {
    //                //nonGround
    //                if (verticalLine_inp[i]._pointValid == 1)
    //                {
    //                    verticalLine_inp[i]._groundPoint = false;
    //                    GameObject point = Util.CreateDefaultLabelPoint();
    //                    point.GetComponent<PointAttributes>().CopyAttributes(verticalLine_inp[i]);
    //                    point.SetActive(false);
    //                    points_out.Add(point);
    //                }
    //            }
    //        }
    //    }
    //    else
    //    {
    //        //not found -> all points are ground points
    //        for (int i = verticalLine_inp.Length; i > 1; i--)
    //        {
    //            if (verticalLine_inp[i]._pointValid == 1)
    //            {
    //                verticalLine_inp[i]._groundPoint = true;
    //                GameObject point = Util.CreateDefaultLabelPoint();
    //                point.GetComponent<PointAttributes>().CopyAttributes(verticalLine_inp[i]);
    //                point.SetActive(false);
    //                points_out.Add(point);
    //            }
    //        }
    //    }
    //}

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
            if (points_inp[i].transform.position.x < lowestValuePoints[0].transform.position.x)
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
        distances.Sort((x, y) => -1 * x.CompareTo(y));

        return distances[0];
    }
}
