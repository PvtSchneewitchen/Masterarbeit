using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Accord.MachineLearning;
using Accord.Math;


public static class Clustering
{
    public static List<GameObject> ClusterByRadiusSearch(GameObject startPoint_inp, float radiusInMeter_inp, bool includeGround)
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



    public static List<List<GameObject>> GetClustersByKmeans(List<GameObject> nonGroundPoints_inp, int k_inp)
    {
        List<List<GameObject>> clusters_out = new List<List<GameObject>>();
        int numberOfPoints = nonGroundPoints_inp.Count;
        double[][] coordinatesOfPoints = new double[numberOfPoints][];
        List<CustomAttributes> attributes = new List<CustomAttributes>();


        for (int i = 0; i < numberOfPoints; i++)
        {
            attributes.Add(nonGroundPoints_inp[i].GetComponent<CustomAttributes>());
            coordinatesOfPoints[i] = new double[] { attributes[i]._pointPosition.x, attributes[i]._pointPosition.y, attributes[i]._pointPosition.z };
        }
        Accord.Math.Random.Generator.Seed = 0;

        // Create a new K-Means algorithm
        KMeans kmeans = new KMeans(k_inp);
        // Compute and retrieve the data centroids
        var clusters = kmeans.Learn(coordinatesOfPoints);
        // Use the centroids to parition all the data
        int[] labels = clusters.Decide(coordinatesOfPoints);

        for (int i = 0; i < numberOfPoints; i++)
        {
            clusters_out[labels[i]].Add(nonGroundPoints_inp[i]);
            attributes[i]._clusterLabel = labels[i];
        }

        return clusters_out;
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
}

