using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Accord.MachineLearning;
using Accord.Math;
using Accord.Statistics;

public static class Clustering
{
    public static List<GameObject> GetClusterByRadiusSearch(GameObject startPoint_inp, float radiusInMeter_inp, bool includeGround)
    {
        Dictionary<int, GameObject> foundObjects = new Dictionary<int, GameObject>();
        float realRadius = radiusInMeter_inp * startPoint_inp.transform.localScale.x;

        RecursiveRadiusSearch(ref foundObjects, new List<GameObject> { startPoint_inp }, realRadius, includeGround);

        return foundObjects.Values.ToList();
    }

    public static List<GameObject> RadiusSearch(GameObject startPoint_inp, float radiusInMeter_inp)
    {
        List<GameObject> foundPoints_out = new List<GameObject>() { startPoint_inp };

        float realRadius = radiusInMeter_inp * startPoint_inp.transform.localScale.x;

        Collider[] collidersInRadius = Physics.OverlapSphere(startPoint_inp.transform.position, realRadius);

        for (int i = 0; i < collidersInRadius.Length; i++)
        {
            GameObject currentObject = collidersInRadius[i].gameObject;
            if (currentObject.name.Contains("Label"))
                foundPoints_out.Add(currentObject);
        }

        return foundPoints_out;
    }

    //public static List<List<GameObject>> GetClustersByGmeans(List<GameObject> nonGroundPoints_inp)
    //{
    //    List<List<GameObject>> clusters_out = new List<List<GameObject>>();
    //    double criticalValue = 1.8692;
    //    int k = 10;
    //    bool allClustersGaussian = false;

    //    while (!allClustersGaussian)
    //    {
    //        var kmeansClusters = GetClustersByKmeans(nonGroundPoints_inp, k);
    //        int kOld = k;

    //        for (int i = 0; i < kmeansClusters.Count; i++)
    //        {
    //            if (!IsCentroidDataGaussian(kmeansClusters[i], criticalValue))
    //            {
    //                k++;
    //            }
    //        }

    //        if (kOld == k)
    //        {
    //            allClustersGaussian = true;
    //        }
    //    }


    //    return clusters_out;
    //}

    private static bool IsCentroidDataGaussian(List<GameObject> nonGroundPoints_inp, double criticalValue_inp)
    {

        bool centroidDataGaussian = true;
        double[][] X = GetCoordinatesOfPoints(nonGroundPoints_inp);

        //split up centroid into 2 centroids
        var clusters = Kmeans(nonGroundPoints_inp, 2);

        //compute v => This is the direction that k-means believes to be important for clustering
        double[] v = new double[]
        {
            clusters.Centroids[0][0] - clusters.Centroids[1][0],
            clusters.Centroids[0][1] - clusters.Centroids[1][1],
            clusters.Centroids[0][2] - clusters.Centroids[1][2]
        };

        //project X onto v
        double[] X_ = new double[X.Length];
        for (int i = 0; i < X_.Length; i++)
        {
            X_[i] = Matrix.Dot(X[i], v) / GetMagnitude(v);
        }

        //Transform X' so that it has mean 0 and variance 1.
        double mean = Measures.Mean(X_);
        double deviation = Measures.StandardDeviation(X_);
        for (int i = 0; i < X_.Length; i++)
        {
            X_[i] = X_[i] - mean / deviation;
        }

        double testResult = ComputeResultOfA2ZStar(X_);

        if (testResult > criticalValue_inp)
        {
            centroidDataGaussian = false;
        }

        return centroidDataGaussian;
    }

    private static double ComputeResultOfA2ZStar(double[] Z)
    {
        int n = Z.Length;
        double A2Z = 0;

        //special case when i = 0
        double log0 = Math.Log(Z[0]) + Math.Log(1 - Z[(n - 1)]);
        A2Z += log0 - n;

        for (int i = 1; i < n; i++)
        {

            double log = Math.Log(Z[i]) + Math.Log(1 - Z[(n + 1 - i) - 1]);
            A2Z += (2 * i - 1) * log - n;
        }

        return -(1 / n) * A2Z * (1 + 4 / n - 25 / Math.Pow(n, 2));
    }

    public static List<List<GameObject>> GetClustersByKmeans(List<GameObject> nonGroundPoints_inp, int k_inp)
    {
        double[][] coordinatesOfPoints = GetCoordinatesOfPoints(nonGroundPoints_inp);

        var clusters = Kmeans(nonGroundPoints_inp, k_inp);
        // Use the centroids to parition all the data
        int[] labels = clusters.Decide(coordinatesOfPoints);

        List<List<GameObject>> clusters_out = new List<List<GameObject>>();
        for (int i = 0; i < clusters.Centroids.Length; i++)
        {
            clusters_out.Add(new List<GameObject>());
        }

        for (int i = 0; i < nonGroundPoints_inp.Count; i++)
        {

            clusters_out[labels[i]].Add(nonGroundPoints_inp[i]);

            //attributes[i]._clusterLabel = labels[i];
        }

        return clusters_out;
    }

    private static KMeansClusterCollection Kmeans(List<GameObject> nonGroundPoints_inp, int k_inp)
    {
        double[][] coordinatesOfPoints = GetCoordinatesOfPoints(nonGroundPoints_inp);

        Accord.Math.Random.Generator.Seed = 0;

        // Create a new K-Means algorithm
        KMeans kmeans = new KMeans(k_inp);
        // Compute and retrieve the data centroids
        return kmeans.Learn(coordinatesOfPoints);
    }

    private static void RecursiveRadiusSearch(ref Dictionary<int, GameObject> foundObjects_ref, List<GameObject> startPoints_inp, float radius_inp, bool includeGround)
    {
        List<GameObject> newStartPoints = new List<GameObject>();

        for (int i = 0; i < startPoints_inp.Count; i++)
        {
            Collider[] collidersInRadius = Physics.OverlapSphere(startPoints_inp[i].transform.position, radius_inp);

            for (int j = 0; j < collidersInRadius.Length; j++)
            {
                GameObject currentPoint = collidersInRadius[j].gameObject;
                if (!currentPoint.name.Contains("Label") || !includeGround && currentPoint.GetComponent<CustomAttributes>()._groundPoint == 1)
                {
                    continue;
                }
                else
                {
                    if (!foundObjects_ref.ContainsKey(currentPoint.gameObject.GetInstanceID()))
                    {
                        foundObjects_ref.Add(currentPoint.gameObject.GetInstanceID(), currentPoint.gameObject);
                        newStartPoints.Add(currentPoint.gameObject);
                    }
                }
            }
        }

        if (newStartPoints.Count > 0)
        {
            RecursiveRadiusSearch(ref foundObjects_ref, newStartPoints, radius_inp, includeGround);
        }
    }

    private static double GetMagnitude(double[] vector_inp)
    {
        double squareRootVal = 0;

        for (int i = 0; i < vector_inp.Length; i++)
        {
            squareRootVal += Math.Pow(vector_inp[i], 2);
        }

        return Math.Sqrt(squareRootVal);
    }

    private static double[][] GetCoordinatesOfPoints(List<GameObject> points_inp)
    {
        int numberOfPoints = points_inp.Count;
        double[][] coordinatesOfPoints = new double[numberOfPoints][];
        List<CustomAttributes> attributes = new List<CustomAttributes>();

        for (int i = 0; i < numberOfPoints; i++)
        {
            attributes.Add(points_inp[i].GetComponent<CustomAttributes>());
            coordinatesOfPoints[i] = new double[] { attributes[i]._pointPosition.x, attributes[i]._pointPosition.y, attributes[i]._pointPosition.z };
        }

        return coordinatesOfPoints;
    }
}

