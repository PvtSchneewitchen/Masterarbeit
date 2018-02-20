using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OpenCvSharp;

public static class CloudSegmentation
{
    public static List<Dictionary<Tuple<int, int>, GameObject>> GetPointsInSegment(Dictionary<Tuple<int, int>, GameObject> points_inp, float sensorRange_inp, float segmentCount_inp)
    {
        var segmentDictionary = new Dictionary<Tuple<float, float>, Dictionary<Tuple<int, int>, GameObject>>();
        var boarders = new List<Tuple<float, float>>();
        float boarderStep = 2 * sensorRange_inp / segmentCount_inp;


        for (float i = 0; i < segmentCount_inp; i++)
        {
            boarders.Add(new Tuple<float, float>(-sensorRange_inp + boarderStep * i, (-sensorRange_inp + boarderStep * i) + boarderStep));
        }

        //for (int i = 0; i < points_inp.Count; i++)
        //{
        //    for (int j = 0; j < boarders.Count; j++)
        //    {
        //        float xVal = points_inp.ElementAt(i).Value.transform.position.x;
        //        float lBoarder = boarders.ElementAt(i).Item1;
        //        float uBoarder = boarders.ElementAt(i).Item2;
        //        if (xVal>=lBoarder && xVal < uBoarder)
        //        {
        //            segmentDictionary[new Tuple<float,float>(1,2)].ad
        //        }
        //    }
        //}

        return new List<Dictionary<Tuple<int, int>, GameObject>>();
    }

    public static float GetLprHeight(Dictionary<Tuple<int, int>, GameObject> pointCloud_inp)
    {
        float avgPointsCount = 100;
        float lprHeight = 0;
        Dictionary<Tuple<int, int>, float> lowestAvgPoints = new Dictionary<Tuple<int, int>, float>();


        for (int i = 0; i < avgPointsCount; i++)
        {
            lowestAvgPoints.Add(new Tuple<int, int>(-1, -1), float.MaxValue);
        }

        for (int i = 0; i < pointCloud_inp.Count; i++)
        {
            var element = pointCloud_inp.ElementAt(i);
            float height = element.Value.transform.position.y;

            if (height < lowestAvgPoints.Values.Max())
            {
                var keyWithHighestVal = lowestAvgPoints.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
                lowestAvgPoints.Remove(keyWithHighestVal);
                lowestAvgPoints.Add(element.Key, height);
            }
        }

        for (int i = 0; i < lowestAvgPoints.Count; i++)
        {
            lprHeight += lowestAvgPoints.ElementAt(i).Value;
        }

        lprHeight /= lowestAvgPoints.Count;

        return lprHeight;
    }

    public static Dictionary<Tuple<int, int>, GameObject> GetLowestPoints(Dictionary<Tuple<int, int>, GameObject> pointCLoudSegment, float heightRef, float heightThresh)
    {
        Dictionary<Tuple<int, int>, GameObject> outPoints = new Dictionary<Tuple<int, int>, GameObject>();
        float heightBoarder = heightRef + heightThresh;

        for (int i = 0; i < pointCLoudSegment.Count; i++)
        {
            var element = pointCLoudSegment.ElementAt(i);
            float height = element.Value.transform.position.y;
            if (height <= heightBoarder)
            {
                outPoints.Add(element.Key, element.Value);
            }
        }

        return outPoints;
    }

    public static Mat ComputeCovarianceMat(Dictionary<Tuple<int, int>, GameObject> initialSeeds)
    {
        Mat covariance_out = new Mat(3, 3, MatType.CV_32F);
        Mat avgSeedPos = new Mat(1, 3, MatType.CV_32F);
        Mat seedPos = new Mat(1, 3, MatType.CV_32F);
        Mat avgSeedPosT = new Mat(3, 1, MatType.CV_32F);
        Mat seedPosT = new Mat(3, 1, MatType.CV_32F);

        for (int i = 0; i < initialSeeds.Count; i++)
        {
            Vector3 position = initialSeeds.ElementAt(i).Value.transform.position;
            avgSeedPos.Set(0, 0, avgSeedPos.Get<float>(0, 0) + position.x);
            avgSeedPos.Set(0, 1, avgSeedPos.Get<float>(0, 1) + position.y);
            avgSeedPos.Set(0, 2, avgSeedPos.Get<float>(0, 2) + position.z);
        }
        avgSeedPos.Set(0, 0, avgSeedPos.Get<float>(0, 0) / initialSeeds.Count);
        avgSeedPos.Set(0, 1, avgSeedPos.Get<float>(0, 1) / initialSeeds.Count);
        avgSeedPos.Set(0, 2, avgSeedPos.Get<float>(0, 2) / initialSeeds.Count);

        for (int i = 0; i < initialSeeds.Count; i++)
        {
            Vector3 position = initialSeeds.ElementAt(i).Value.transform.position;

            Mat newCovar = new Mat(3, 3, MatType.CV_32F);
            Mat entryPos = new Mat(1, 3, MatType.CV_32F);
            Mat multiplier1 = new Mat(1, 3, MatType.CV_32F);
            Mat multiplier2 = new Mat(3, 1, MatType.CV_32F);

            entryPos.Set(0, 0, position.x);
            entryPos.Set(0, 0, position.y);
            entryPos.Set(0, 0, position.z);

            Cv2.Subtract(seedPos, avgSeedPos, multiplier1);
            Cv2.Subtract(seedPos.Transpose(), avgSeedPos.Transpose(), multiplier2);
            Cv2.Multiply(multiplier1, multiplier2, newCovar);
            Cv2.Add(covariance_out, newCovar, covariance_out);
        }

        return covariance_out;
    }

    private static Vec3f SubtractVec3f(Vec3f minuend_inp, Vec3f subtractor_inp)
    {
        minuend_inp[0] -= subtractor_inp[0];
        minuend_inp[1] -= subtractor_inp[1];
        minuend_inp[2] -= subtractor_inp[2];

        return minuend_inp;
    }
}
