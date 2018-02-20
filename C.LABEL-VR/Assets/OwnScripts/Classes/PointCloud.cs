using OpenCvSharp;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCloud
{
    public List<GameObject> _validPoints { get; private set; }
    public List<GameObject> _inValidPoints { get; private set; }

    //public List<GameObject> _points { get; private set; }
    public GameObject _origin { get; set; }

    public GameObject[,] _debugPointTable { get; private set; }

    private const float _alphaMax = 45;
    private const float _hMin = 0.1f;

    public PointCloud(List<Vector3> pcdCoordinates_inp, Util.Datatype type = Util.Datatype.pcd)
    {
        //Dictionary<Tuple<int, int>, GameObject> validPointsRef = new Dictionary<Tuple<int, int>, GameObject>();
        _validPoints = new List<GameObject>();
        _inValidPoints = new List<GameObject>();
        _origin = CreateOrigin();

        for (int i = 0; i < pcdCoordinates_inp.Count; i++)
        {
            GameObject point = Util.CreateDefaultLabelPoint();
            point.transform.position = new Vector3(pcdCoordinates_inp[i].x, pcdCoordinates_inp[i].y, pcdCoordinates_inp[i].z);
            point.SetActive(false);
            _validPoints.Add(point);
            //validPointsRef.Add(new Tuple<int, int>(i, 0), point);
        }

        //GroundPLaneFitting(ref validPointsRef, 5);

        //_validPoints = validPointsRef;
        //validPointsRef.Clear();
    }

    private void GroundPLaneFitting(ref Dictionary<Tuple<int, int>, GameObject> points_inp, int segments_inp)
    {
        float heightThresh = 0.15f;

        var cloudSegments = new List<Dictionary<Tuple<int, int>, GameObject>>();

        for (int i = 0; i < cloudSegments.Count; i++)
        {
            Dictionary<Tuple<int, int>, GameObject> pointCloudSegment = new Dictionary<Tuple<int, int>, GameObject>();
            Dictionary<Tuple<int, int>, GameObject> initialSeeds = new Dictionary<Tuple<int, int>, GameObject>();
            Mat covariance = new Mat(3, 3, MatType.CV_32F);
            float lprHeight = 0;

            pointCloudSegment = cloudSegments.ElementAt(i);
            lprHeight = CloudSegmentation.GetLprHeight(pointCloudSegment);
            initialSeeds = CloudSegmentation.GetLowestPoints(pointCloudSegment, lprHeight, heightThresh);
            covariance = CloudSegmentation.ComputeCovarianceMat(initialSeeds);

            SVD svd = new SVD(covariance);
            Debug.Log(svd.U);
            Debug.Log(svd.W);
            Debug.Log(svd.Vt);
        }
    }



    public PointCloud(HDF5Addon.Lidar_Daimler hdf5Container_inp)
    {
        _origin = CreateOrigin();

        int rows = hdf5Container_inp._sensorX.GetLength(0);
        int cols = hdf5Container_inp._sensorX.GetLength(1);

        CustomAttributes.CustomAttributesContainer[,] attributeTable = new CustomAttributes.CustomAttributesContainer[rows, cols];

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                CustomAttributes.CustomAttributesContainer attribute = new CustomAttributes.CustomAttributesContainer
                {
                    _hdf5TableIndex = new Tuple<int, int>(i, j),
                    _distance = hdf5Container_inp._distances[i, j],
                    _intensity = hdf5Container_inp._intensity[i, j],
                    _labelPropability = hdf5Container_inp._labelProbabilities[i, j],
                    _labelWorkingSet = hdf5Container_inp._labelWorkingSet,
                    _label = new Tuple<string, uint>("unlabeled", 0),
                    _pointValid = hdf5Container_inp._pointValid[i, j],
                    _position_Sensor = Quaternion.Euler(0, 180, 0) * new Vector3(hdf5Container_inp._sensorX[i, j], hdf5Container_inp._sensorZ[i, j], hdf5Container_inp._sensorY[i, j]),
                    _position_Vehicle = new Vector3(hdf5Container_inp._vehicleX[i, j], hdf5Container_inp._vehicleZ[i, j], hdf5Container_inp._vehicleY[i, j])
                };

                attributeTable[i, j] = attribute;
            }
        }


        //_validPoints = points;
    }


    //private void CalculateGroundPoints(Util.CustomAttributesContainer[,] pointTable_inp, ref List<GameObject> points_out)
    //{
    //    int rows = pointTable_inp.GetLength(0);
    //    int cols = pointTable_inp.GetLength(1);

    //    for (int i = 0; i < cols; i++)
    //    {
    //        //+1 because of the selfmade start ground point 
    //        Util.CustomAttributesContainer[] verticalLine = new Util.CustomAttributesContainer[rows + 1];

    //        //start grounbd point at sensors location
    //        var startGroundPoint = new Util.CustomAttributesContainer { _pointValid = 1 };
    //        verticalLine[rows] = startGroundPoint;

    //        for (int j = 0; j < rows; j++)
    //        {
    //            verticalLine[j] = pointTable_inp[j, i];
    //        }

    //        ProcessVerticalLine(verticalLine, ref points_out);
    //    }
    //}

    //private void ProcessVerticalLine(Util.CustomAttributesContainer[] verticalLine_inp, ref List<GameObject> points_out)
    //{
    //    //BestWayToGoToNextStep

    //    List<int> thresholdPoints = new List<int>();
    //    int prePoint = verticalLine_inp.Length - 1;
    //    int curPoint = prePoint - 1;

    //    while (curPoint >= 0)
    //    {
    //        if (verticalLine_inp[prePoint]._groundPoint == true)
    //        {
    //            if (verticalLine_inp[prePoint]._pointValid == 1 && verticalLine_inp[curPoint]._pointValid == 0)
    //            {
    //                int nextV = curPoint - 1;
    //                bool found = false;
    //                while (nextV > 0)
    //                {
    //                    if (verticalLine_inp[nextV]._pointValid == 1)
    //                    {
    //                        found = true;
    //                        break;
    //                    }

    //                    nextV--;
    //                }
    //                if (found)
    //                {
    //                    curPoint = nextV;
    //                }
    //                else
    //                {
    //                    break;
    //                }
    //            }

    //            float heightDif = Mathf.Abs(verticalLine_inp[prePoint]._position_Vehicle.y - verticalLine_inp[curPoint]._position_Vehicle.y);

    //            if (verticalLine_inp[curPoint]._position_Vehicle.y > verticalLine_inp[prePoint]._position_Vehicle.y)
    //            {
    //                verticalLine_inp[curPoint]._groundPoint = true;

    //                //if lost points between pre and cur
    //                if (prePoint - curPoint > 1)
    //                {
    //                    //Second Stage (Lost-PointQuery)
    //                    if (heightDif > _hMin)
    //                    {
    //                        verticalLine_inp[curPoint]._groundPoint = false;
    //                    }
    //                }
    //                else
    //                {
    //                    float distance = Vector3.Distance(verticalLine_inp[prePoint]._position_Vehicle, verticalLine_inp[curPoint]._position_Vehicle);
    //                    float alpha = Mathf.Rad2Deg * Mathf.Asin(heightDif / distance);

    //                    //first and third query
    //                    if (alpha >= _alphaMax || verticalLine_inp[prePoint]._distance > verticalLine_inp[curPoint]._distance)
    //                    {
    //                        verticalLine_inp[curPoint]._groundPoint = false;
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                verticalLine_inp[curPoint]._groundPoint = true;
    //            }
    //        }
    //        else
    //        {
    //            float heightDif = Mathf.Abs(verticalLine_inp[prePoint]._position_Vehicle.y - verticalLine_inp[curPoint]._position_Vehicle.y);

    //            if (verticalLine_inp[curPoint]._position_Vehicle.y > verticalLine_inp[prePoint]._position_Vehicle.y)
    //            {
    //                verticalLine_inp[curPoint]._groundPoint = true;
    //            }
    //            else
    //            {
    //                int nextGroundPoint = verticalLine_inp.Length - 1;
    //                for (int i = prePoint; i < verticalLine_inp.Length - 1; i++)
    //                {
    //                    if (verticalLine_inp[i]._groundPoint == true)
    //                    {
    //                        nextGroundPoint = i;
    //                        break;
    //                    }
    //                }

    //                float heightDifToGround = verticalLine_inp[curPoint]._position_Vehicle.y - verticalLine_inp[nextGroundPoint]._position_Vehicle.y;
    //                if (heightDifToGround > _hMin)
    //                {
    //                    verticalLine_inp[curPoint]._groundPoint = false;
    //                }
    //                else
    //                {
    //                    verticalLine_inp[curPoint]._groundPoint = true;
    //                }
    //            }
    //        }
    //        prePoint = curPoint;
    //        curPoint = prePoint - 1;
    //    }

    //    for (int i = 0; i < verticalLine_inp.Length - 2; i++)
    //    {
    //        if (verticalLine_inp[i]._pointValid == 1)
    //        {
    //            if (verticalLine_inp[i]._groundPoint)
    //            {
    //                verticalLine_inp[i]._groundPoint = true;
    //                GameObject point = Util.CreateDefaultLabelPoint();
    //                point.GetComponent<CustomAttributes>().CopyAttributes(verticalLine_inp[i]);

    //                point.GetComponent<CustomAttributes>()._label = Labeling.LabelGroup.motorcycle;

    //                point.SetActive(false);
    //                points_out.Add(point);
    //            }
    //            else
    //            {
    //                verticalLine_inp[i]._groundPoint = false;
    //                GameObject point = Util.CreateDefaultLabelPoint();
    //                point.GetComponent<CustomAttributes>().CopyAttributes(verticalLine_inp[i]);
    //                point.SetActive(false);
    //                points_out.Add(point);
    //            }
    //        }
    //    }
    //}


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
                _validPoints[i].SetActive(true);
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
