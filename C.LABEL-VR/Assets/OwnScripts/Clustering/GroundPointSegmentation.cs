using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using g3;

/// <summary>
/// Class for detecting ground points
/// </summary>
public static class GroundPointSegmentation
{

/// <summary>
/// this method sets the ground point attribute of the given internal data list to the right value
/// </summary>
/// <param name="listOfDataLists">List of internal data</param>
    public static void SetGroundLabels(List<InternalDataFormat> listOfDataLists)
    {
        int segmentCount = 3;
        float initialSeedDistanceThreshold = 0.3f;
        float seedDistanceThreshold = 0.2f;
        float covarIterations = 1;
        InternalDataFormat seed = new InternalDataFormat(0, Vector3.zero, 0, 2);
        List<InternalDataFormat> seedPoints = new List<InternalDataFormat>();

        //List<List<InternalDataFormat>> cloudSegments = GetSimpleCloudSegments(listOfDataLists);
        //List<List<InternalDataFormat>> cloudSegments = GetCloudSegmentsSegmentedByYAxis(listOfDataLists, segmentCount);
        List<List<InternalDataFormat>> cloudSegments = GetCloudSegmentsSegmentedByYAndXAxis(listOfDataLists, segmentCount);

        for (int i = 0; i < cloudSegments.Count; i++)
        {
            List<InternalDataFormat> segmentData = cloudSegments.ElementAt(i);
            seedPoints = GetInitialSeedPoints(segmentData, initialSeedDistanceThreshold);

            for (int j = 0; j < covarIterations; j++)
            {
                List<Vector3d> points = new List<Vector3d>();
                for (int m = 0; m < seedPoints.Count; m++)
                {
                    var pos = seedPoints[m]._position;
                    points.Add(new Vector3d(pos.x, pos.y, pos.z));
                }

                OrthogonalPlaneFit3 estimatedGroundPlane = new OrthogonalPlaneFit3(points);

                Vector3 planeOriginPoint = new Vector3((float)estimatedGroundPlane.Origin.x, (float)estimatedGroundPlane.Origin.y, (float)estimatedGroundPlane.Origin.z);
                Vector3 planeNormal = new Vector3((float)estimatedGroundPlane.Normal.x, (float)estimatedGroundPlane.Normal.y, (float)estimatedGroundPlane.Normal.z);
                Plane estimatedGroundPlaneUnity = new Plane(planeNormal, planeOriginPoint);

                if (j == covarIterations - 1)
                {
                    Util.DrawPlane(planeOriginPoint, planeNormal, planeNormal.magnitude * 2, i);
                }


                for (int k = 0; k < segmentData.Count; k++)
                {
                    seed = cloudSegments.ElementAt(i).ElementAt(k);

                    float distancePointToPlane = estimatedGroundPlaneUnity.GetDistanceToPoint(seed._position);

                    if (distancePointToPlane <= seedDistanceThreshold)
                    {
                        seed._groundPointLabel = 1;
                        seedPoints.Add(seed);
                    }
                    else
                    {
                        seed._groundPointLabel = 0;
                    }
                }
            }
        }
    }

    /// <summary>
    /// This method delivers the startpoints for the ground plane fittin algorithm 
    /// </summary>
    /// <returns>The initial seed points</returns>
    /// <param name="segmentData_inp">Segment data inp.</param>
    /// <param name="initialSeedsThresh_inp">Initial seeds thresh inp.</param>
    private static List<InternalDataFormat> GetInitialSeedPoints(List<InternalDataFormat> segmentData_inp, float initialSeedsThresh_inp)
    {
        List<InternalDataFormat> initialSeeds_out = new List<InternalDataFormat>();

        float lprHeightValue = GetLprHeight(segmentData_inp);
        initialSeeds_out = GetPointsInLprThresh(segmentData_inp, lprHeightValue, initialSeedsThresh_inp);

        return initialSeeds_out;
    }

    /// <summary>
    /// Gets the average height of all points within a certain height range
    /// </summary>
    /// <returns>The lpr height (lowest point represantative)</returns>
    /// <param name="segmentData_inp">Segment data inp.</param>
    private static float GetLprHeight(List<InternalDataFormat> segmentData_inp)
    {
        float lprHeight_out;
        List<float> lowestAvgPoints = new List<float>();

        for (int i = 0; i < segmentData_inp.Count; i++)
        {
            float height = segmentData_inp[i]._position.z;

            if (height < 0.5 && height > -0.5)
            {
                lowestAvgPoints.Add(height);
            }
        }

        //for (int i = 0; i < segmentData_inp.Count; i++)
        //{
        //    if (i < 100)
        //    {
        //        lowestAvgPoints.Add(segmentData_inp[i]._position.z);
        //    }
        //    else
        //    {
        //        var min = lowestAvgPoints.Min();
        //        if (segmentData_inp[i]._position.z < min)
        //        {
        //            var index = lowestAvgPoints.IndexOf(min);
        //            lowestAvgPoints.RemoveAt(index);
        //            lowestAvgPoints.Add(segmentData_inp[i]._position.z);
        //        }
        //    }
        //}

        lprHeight_out = lowestAvgPoints.Sum() / lowestAvgPoints.Count;

        return lprHeight_out;
    }

    /// <summary>
    /// Gets the points within the lpr height and a threshold
    /// </summary>
    /// <returns>The points in lpr thresh.</returns>
    /// <param name="segmentData_inp">Segment data inp.</param>
    /// <param name="lprHeightValue_inp">Lpr height value inp.</param>
    /// <param name="initialSeedsThresh_inp">Initial seeds thresh inp.</param>
    private static List<InternalDataFormat> GetPointsInLprThresh(List<InternalDataFormat> segmentData_inp, float lprHeightValue_inp, float initialSeedsThresh_inp)
    {
        List<InternalDataFormat> lowestPoints_out = new List<InternalDataFormat>();
        float heightBoarder = lprHeightValue_inp + initialSeedsThresh_inp;

        for (int i = 0; i < segmentData_inp.Count; i++)
        {
            var segmentDataElement = segmentData_inp.ElementAt(i);
            float height = segmentDataElement._position.z;
            if (height <= heightBoarder)
            {
                lowestPoints_out.Add(segmentDataElement);
            }
        }
        return lowestPoints_out;
    }


//    private static List<List<InternalDataFormat>> GetSimpleCloudSegments(List<InternalDataFormat> dataList_inp)
//    {
//        List<List<InternalDataFormat>> listOfSegments_out = new List<List<InternalDataFormat>>(3)
//        {
//            new List<InternalDataFormat>(),
//            new List<InternalDataFormat>(),
//            new List<InternalDataFormat>()
//        };
//
//        var biggestdistance = GetBiggestDistance(dataList_inp);
//        var boarder = 25;
//
//        Debug.DrawLine(new Vector3(-boarder, -100, 0), new Vector3(-boarder, 100, 0), Color.blue, 300);
//        Debug.DrawLine(new Vector3(boarder, -100, 0), new Vector3(boarder, 100, 0), Color.blue, 300);
//
//        for (int i = 0; i < dataList_inp.Count; i++)
//        {
//            var xval = dataList_inp[i]._position.x;
//            if (xval < -boarder)
//            {
//                listOfSegments_out.ElementAt(0).Add(dataList_inp[i]);
//            }
//            else if (xval < boarder && xval >= -boarder)
//            {
//                listOfSegments_out.ElementAt(1).Add(dataList_inp[i]);
//            }
//            else
//            {
//                listOfSegments_out.ElementAt(2).Add(dataList_inp[i]);
//            }
//
//
//        }
//
//        return listOfSegments_out;
//    }


//    private static List<List<InternalDataFormat>> GetCloudSegmentsSegmentedByYAxis(List<InternalDataFormat> dataList_inp, int segmentCount_inp)
//    {
//        List<List<InternalDataFormat>> listOfSegments_out = new List<List<InternalDataFormat>>();
//
//        dataList_inp.Sort((a, b) => a._position.x.CompareTo(b._position.x));
//
//        int potentialGroundPointCounter = 0;
//        for (int i = 0; i < dataList_inp.Count; i++)
//        {
//            float height = dataList_inp[i]._position.z;
//            if (height < 0.5 && height > -0.5)
//            {
//                potentialGroundPointCounter++;
//            }
//        }
//
//        int potentialGroundPointsPerSegment = (int)Mathf.Floor(potentialGroundPointCounter / (segmentCount_inp)) + 1;
//
//        int pointsInSegmentCounter = 0;
//        int currentSegment = 0;
//
//        Debug.DrawLine(new Vector3(dataList_inp[0]._position.x, -100, 0), new Vector3(dataList_inp[0]._position.x, 100, 0), Color.blue, 300);
//
//        listOfSegments_out.Add(new List<InternalDataFormat>());
//        for (int i = 0; i < dataList_inp.Count; i++)
//        {
//            listOfSegments_out.ElementAt(currentSegment).Add(dataList_inp[i]);
//
//            float height = dataList_inp[i]._position.z;
//            if (height < 0.5 && height > -0.5)
//            {
//                pointsInSegmentCounter++;
//            }
//
//            if (pointsInSegmentCounter >= potentialGroundPointsPerSegment)
//            {
//                Debug.DrawLine(new Vector3(dataList_inp[i]._position.x, -100, 0), new Vector3(dataList_inp[i]._position.x, 100, 0), Color.blue, 300);
//
//                pointsInSegmentCounter = 0;
//                currentSegment++;
//                listOfSegments_out.Add(new List<InternalDataFormat>());
//            }
//        }
//        return listOfSegments_out;
//    }

/// <summary>
/// Devides a list of given points in segments that are devided by the x and y axis. each segments has the same amount of points that are within a certain height range (potential ground points)
/// </summary>
/// <returns>The cloud segments segmented by Y and X axis.</returns>
/// <param name="dataList_inp">Data list inp.</param>
/// <param name="segmentCount_inp">Segment count inp.</param>
    private static List<List<InternalDataFormat>> GetCloudSegmentsSegmentedByYAndXAxis(List<InternalDataFormat> dataList_inp, int segmentCount_inp)
    {
        List<List<InternalDataFormat>> listOfSegments_out = new List<List<InternalDataFormat>>();

        dataList_inp.Sort((a, b) => a._position.x.CompareTo(b._position.x));

        int potentialGroundPointCounter = 0;
        for (int i = 0; i < dataList_inp.Count; i++)
        {
            float height = dataList_inp[i]._position.z;
            if (height < 0.5 && height > -0.5)
            {
                potentialGroundPointCounter++;
            }
        }

        int potentialGroundPointsPerSegment = (int)Mathf.Ceil(potentialGroundPointCounter / (segmentCount_inp));

        int pointsInSegmentCounter = 0;
        int currentSegment = 0;

        //Debug.DrawLine(new Vector3(dataList_inp[0]._position.x, -100, 0), new Vector3(dataList_inp[0]._position.x, 100, 0), Color.blue, 300);
        //Debug.DrawLine(new Vector3(-100, 0, 0), new Vector3(100, 0, 0), Color.blue, 300);

        listOfSegments_out.Add(new List<InternalDataFormat>());
        listOfSegments_out.Add(new List<InternalDataFormat>());
        for (int i = 0; i < dataList_inp.Count; i++)
        {
            if (dataList_inp[i]._position.y >= 0)
            {
                listOfSegments_out.ElementAt(currentSegment).Add(dataList_inp[i]);
            }
            else
            {
                listOfSegments_out.ElementAt(currentSegment + 1).Add(dataList_inp[i]);
            }

            float height = dataList_inp[i]._position.z;
            if (height < 0.5 && height > -0.5)
            {
                pointsInSegmentCounter++;
            }

            if (pointsInSegmentCounter >= potentialGroundPointsPerSegment)
            {
                //Debug.DrawLine(new Vector3(dataList_inp[i]._position.x, -100, 0), new Vector3(dataList_inp[i]._position.x, 100, 0), Color.blue, 300);

                pointsInSegmentCounter = 0;
                currentSegment += 2;
                listOfSegments_out.Add(new List<InternalDataFormat>());
                listOfSegments_out.Add(new List<InternalDataFormat>());
            }

        }
        return listOfSegments_out;
    }


    /// <summary>
    /// Gets the biggest distance of two points within a list of points
    /// </summary>
    /// <returns>The biggest distance.</returns>
    /// <param name="data_inp">Data inp.</param>
    private static float GetBiggestDistance(List<InternalDataFormat> data_inp)
    {
        List<Vector3> lowestValuePoints = new List<Vector3>(4);
        List<float> distances = new List<float>();

        //init list
        for (int i = 0; i < lowestValuePoints.Capacity; i++)
        {
            lowestValuePoints.Add(data_inp[0]._position);
        }

        //search for lowest values
        for (int i = 1; i < data_inp.Count; i++)
        {
            if (data_inp[i]._position.x < lowestValuePoints[0].x)
            {
                lowestValuePoints[0] = data_inp[i]._position;
            }
            if (data_inp[i]._position.z < lowestValuePoints[1].z)
            {
                lowestValuePoints[1] = data_inp[i]._position;
            }
            if (data_inp[i]._position.x > lowestValuePoints[2].x)
            {
                lowestValuePoints[2] = data_inp[i]._position;
            }
            if (data_inp[i]._position.z > lowestValuePoints[3].z)
            {
                lowestValuePoints[3] = data_inp[i]._position;
            }
        }

        //calculate distances
        for (int i = 0; i < lowestValuePoints.Count; i++)
        {
            for (int j = 0; j < lowestValuePoints.Count; j++)
            {
                distances.Add(Vector3.Distance(lowestValuePoints[i], lowestValuePoints[j]));
            }
        }

        //sort descending
        distances.Sort((x, y) => -1 * x.CompareTo(y));

        return distances[0];

    }
}