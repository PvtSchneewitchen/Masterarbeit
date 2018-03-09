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
        Quaternion pcdToUnityCS = Quaternion.Euler(-90, 0, 0);
        List<string> paths = new List<string>();

        listOfCoordinateLists = PcdAddon.ReadPcdFromPath(loadPath_inp, ref paths);
        listOfDataLists = PcdAddon.GetDataFromCoordinates(listOfCoordinateLists);

        //for (int i = 0; i < listOfDataLists.Count; i++)
        //{
        //    CloudSegmentation.SetGroundLabels(listOfDataLists.ElementAt(i));
        //}

        for (int i = 0; i < listOfDataLists.Count; i++)
        {
            pointClouds_out.Add(new PointCloud(listOfDataLists[i], paths[i], pcdToUnityCS));
        }
        
        return pointClouds_out;
    }

    public static List<PointCloud> ImportHdf5_DaimlerLidar(string loadPath_inp)
    {
        List<PointCloud> pointClouds_out = new List<PointCloud>();
        List<List<InternalDataFormat>> listOfDataLists = new List<List<InternalDataFormat>>();
        List<string> paths = new List<string>();
        Quaternion hdf5ToUnityCS = Quaternion.Euler(-90, 0, 0);

        listOfDataLists = HDF5Addon.ReadHdf5_DaimlerLidar(loadPath_inp, ref paths);

        for (int i = 0; i < listOfDataLists.Count; i++)
        {
            pointClouds_out.Add(new PointCloud(listOfDataLists[i], paths[i], hdf5ToUnityCS));
        }

        return pointClouds_out;
    }
}
