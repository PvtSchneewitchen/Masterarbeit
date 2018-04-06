 using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Import
{
    public static List<PointCloud> ImportPcd(string loadPath_inp)
    {
        List<PointCloud> pointClouds_out = new List<PointCloud>();
        List<List<Vector3>> listOfCoordinateLists = new List<List<Vector3>>();
        List<List<InternalDataFormat>> listOfDataLists = new List<List<InternalDataFormat>>();
        Quaternion pcdToUnityCS_Rotation = Quaternion.Euler(-90, 90, 0);
        Vector3 pcdToUnityCS_Mirroring = Vector3.one;
        List<string> paths = new List<string>();

        listOfCoordinateLists = PcdAddon.ReadPcdFromPath(loadPath_inp, ref paths);
        listOfDataLists = PcdAddon.GetDataFromCoordinates(listOfCoordinateLists);

        for (int i = 0; i < listOfDataLists.Count; i++)
        {
            GroundPointSegmentation.SetGroundLabels(listOfDataLists.ElementAt(i));
        }

        for (int i = 0; i < listOfDataLists.Count; i++)
        {
            pointClouds_out.Add(new PointCloud(listOfDataLists[i], paths[i], pcdToUnityCS_Rotation, pcdToUnityCS_Mirroring));
            //pointClouds_out.Add(new PointCloud(listOfDataLists[i], paths[i], Quaternion.Euler(0,0,0), pcdToUnityCS_Mirroring));
        }

        return pointClouds_out;
    }

    public static List<PointCloud> ImportHdf5_DaimlerLidar(string loadPath_inp)
    {
        List<PointCloud> pointClouds_out = new List<PointCloud>();
        List<List<InternalDataFormat>> listOfDataLists = new List<List<InternalDataFormat>>();
        List<string> paths = new List<string>();
        Quaternion hdf5ToUnityCS_Rotation = Quaternion.Euler(-90, -90, 0);
        Vector3 hdf5ToUnityCS_Mirroring = new Vector3(-1, 1, 1);

        listOfDataLists = HDF5Addon.ReadHdf5_DaimlerLidar(loadPath_inp, ref paths);
        Debug.Log("List of read Data: " + listOfDataLists.Count);

        for (int i = 0; i < listOfDataLists.Count; i++)
        {
            GroundPointSegmentation.SetGroundLabels(listOfDataLists.ElementAt(i));
        }

        for (int i = 0; i < listOfDataLists.Count; i++)
        {
            pointClouds_out.Add(new PointCloud(listOfDataLists[i], paths[i], hdf5ToUnityCS_Rotation, hdf5ToUnityCS_Mirroring));
            //pointClouds_out.Add(new PointCloud(listOfDataLists[i], paths[i], Quaternion.Euler(0, 0, 0), Vector3.one));
        }

        return pointClouds_out;
    }
}
