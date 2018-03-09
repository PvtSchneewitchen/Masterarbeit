using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System;

public static class CloudSegmentation
{
    public static void SetGroundLabels(List<InternalDataFormat> listOfDataLists)
    {
        int segmentCount = 5;
        float initialSeedsThresh = 0.4f;
        float seedDistanceThreshold = 0.2f;
        float covarIterations = 3;
        float seedDistanceToPlane;
        InternalDataFormat seed = new InternalDataFormat(0, Vector3.zero, 0, 2);
        Vector<float> seedPosition = CreateVector.Dense<float>(3);
        Matrix<float> covariance = CreateMatrix.Dense<float>(3, 3);
        Vector<float> estimatedPlaneNormal = CreateVector.Dense<float>(3);
        Svd<float> singleValueDecomposition;
        List<InternalDataFormat> seedPoints = new List<InternalDataFormat>();

        List<List<InternalDataFormat>> cloudSegments = GetCloudSegments(listOfDataLists, segmentCount);

        for (int i = 0; i < cloudSegments.Count; i++)
        {
            List<InternalDataFormat> segmentData = cloudSegments.ElementAt(i);
            seedPoints = GetInitialSeedPoints(segmentData, initialSeedsThresh);

            for (int j = 0; j < covarIterations; j++)
            {
                covariance = ComputeCovarianceMat(seedPoints);
                seedPoints.Clear();

                singleValueDecomposition = covariance.Svd();

                estimatedPlaneNormal = singleValueDecomposition.VT.Column(2);

                for (int k = 0; k < segmentData.Count; k++)
                {
                    seed = cloudSegments.ElementAt(i).ElementAt(k);
                    seedPosition[0] = seed._position.x;
                    seedPosition[1] = seed._position.y;
                    seedPosition[2] = seed._position.z;

                    var distanceMat = estimatedPlaneNormal.ToRowMatrix() * seedPosition.ToColumnMatrix();

                    seedDistanceToPlane = distanceMat[0, 0];

                    if (seedDistanceToPlane <= seedDistanceThreshold)
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

    private static Matrix<float> ComputeCovarianceMat(List<InternalDataFormat> segmentData_inp)
    {
        Matrix<float> covariance_out = CreateMatrix.Dense<float>(3, 3);
        Vector<float> avgSeedPosition = CreateVector.Dense<float>(3);
        Vector<float> seedPosition = CreateVector.Dense<float>(3);
        Vector<float> positionDifference = CreateVector.Dense<float>(3);
        Vector<float> sum = CreateVector.Dense<float>(3);

        for (int i = 0; i < segmentData_inp.Count; i++)
        {
            Vector3 pos = segmentData_inp.ElementAt(i)._position;
            sum[0] = pos.x;
            sum[1] = pos.y;
            sum[2] = pos.z;
        }
        avgSeedPosition = sum / segmentData_inp.Count;

        for (int i = 0; i < segmentData_inp.Count; i++)
        {
            Matrix<float> seedVariance = CreateMatrix.Dense<float>(3, 3);
            Vector3 position = segmentData_inp.ElementAt(i)._position;
            seedPosition[0] = position.x;
            seedPosition[1] = position.y;
            seedPosition[2] = position.z;

            positionDifference = seedPosition - avgSeedPosition;
            seedVariance = positionDifference.ToColumnMatrix() * positionDifference.ToRowMatrix();
            covariance_out += seedVariance;
        }

        return covariance_out;
    }

    private static List<InternalDataFormat> GetInitialSeedPoints(List<InternalDataFormat> segmentData_inp, float initialSeedsThresh_inp)
    {
        List<InternalDataFormat> initialSeeds_out = new List<InternalDataFormat>();

        float lprHeightValue = GetLprHeight(segmentData_inp);
        initialSeeds_out = GetLowestPoints(segmentData_inp, lprHeightValue, initialSeedsThresh_inp);

        return initialSeeds_out;
    }

    private static float GetLprHeight(List<InternalDataFormat> segmentData_inp)
    {
        float lprHeight_out;
        int avgPointsCount = (int)Mathf.Floor(segmentData_inp.Count / 3);
        List<float> lowestAvgPoints = new List<float>();

        lowestAvgPoints = Enumerable.Range(0, avgPointsCount).Select(n => float.MaxValue).ToList();

        for (int i = 0; i < segmentData_inp.Count; i++)
        {
            var segmentDataElement = segmentData_inp.ElementAt(i);
            float height = segmentDataElement._position.z;
            float maxValue = lowestAvgPoints.Max();

            if (height < maxValue)
            {
                int indexMax = lowestAvgPoints.Select((value, index) => new { Value = value, Index = index }).Aggregate((a, b) => (a.Value > b.Value) ? a : b).Index;
                lowestAvgPoints.RemoveAt(indexMax);
                lowestAvgPoints.Add(height);
            }
        }

        lprHeight_out = lowestAvgPoints.Sum() / lowestAvgPoints.Count;

        return lprHeight_out;
    }

    private static List<InternalDataFormat> GetLowestPoints(List<InternalDataFormat> segmentData_inp, float lprHeightValue_inp, float initialSeedsThresh_inp)
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


    private static List<List<InternalDataFormat>> GetCloudSegments(List<InternalDataFormat> dataList_inp, int segmentCount_inp)
    {
        List<List<InternalDataFormat>> listOfSegments_out = new List<List<InternalDataFormat>>();

        float range = Util.GetBiggestDistance(dataList_inp.Select(dataElement => dataElement._position).ToList()) / 2;
        var boarders = new List<Tuple<float, float>>();
        float boarderStep = 2 * range / segmentCount_inp;

        for (float i = 0; i < segmentCount_inp; i++)
        {
            float lowerBoundary = -range + boarderStep * i;
            boarders.Add(new Tuple<float, float>(lowerBoundary, lowerBoundary + boarderStep));
            listOfSegments_out.Add(new List<InternalDataFormat>());
        }

        for (int i = 0; i < dataList_inp.Count; i++)
        {
            float xVal = dataList_inp.ElementAt(i)._position.x;
            for (int j = 0; j < boarders.Count; j++)
            {
                float lBoarder = boarders.ElementAt(j).Item1;
                float uBoarder = boarders.ElementAt(j).Item2;
                if (xVal >= lBoarder && xVal < uBoarder)
                {
                    listOfSegments_out.ElementAt(j).Add(dataList_inp.ElementAt(i));
                    break;
                }
            }
        }

        return listOfSegments_out;
    }
}