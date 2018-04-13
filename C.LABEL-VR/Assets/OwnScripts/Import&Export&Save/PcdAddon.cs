﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public static class PcdAddon
{
    /// <summary>
    /// Reads one or multiple pcd-files and delivers the coordinates from it. 
    /// It adds a label class to the global label classes
    /// </summary>
    /// <param name="loadPath"> Path of a pcd file or a directory with pcd-files in it</param>
    /// <returns>coordinates of the pcd files</returns>
    public static List<List<Vector3>> ReadPcdFromPath(string loadPath, ref List<string> paths_ref)
    {
        List<List<Vector3>> outputCoordinates = new List<List<Vector3>>();
        List<Vector3> pcdCoordinates = new List<Vector3>();
        List<StreamReader> pcdFileStreams = new List<StreamReader>();

        CultureInfo cultureInfo = (CultureInfo)CultureInfo.CurrentCulture.Clone();
        cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";

        if (loadPath.Substring(loadPath.Length - 4) == ".pcd")
        {
            //single file

            pcdFileStreams.Add(new StreamReader(loadPath));
            paths_ref.Add(loadPath);
        }
        else
        {
            //directory

            string[] filePaths = Directory.GetFiles(loadPath, "*.pcd");
            foreach (var path in filePaths)
            {
                pcdFileStreams.Add(new StreamReader(path));
                paths_ref.Add(path);
            }
        }

        for (int i = 0; i < pcdFileStreams.Count; i++)
        {
            List<Vector3> coordinatesFromFile = new List<Vector3>();
            bool firstLineSkipped = false;

            while (!pcdFileStreams[i].EndOfStream)
            {
                var line = pcdFileStreams[i].ReadLine();
                if (firstLineSkipped)
                {
                    var coordinates = line.Split(' ');

                    float x = 0;
                    float y = 0;
                    float z = 0;

                    try
                    {
                        x = float.Parse(coordinates[0], NumberStyles.Any, cultureInfo);
                        y = float.Parse(coordinates[1], NumberStyles.Any, cultureInfo);
                        z = float.Parse(coordinates[2], NumberStyles.Any, cultureInfo);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Source + " " + e.Message);
                    }
                    coordinatesFromFile.Add(new Vector3(-x, y, z));
                }
                else
                {
                    firstLineSkipped = true;
                }
            }
            outputCoordinates.Add(coordinatesFromFile);
        }

        //InitializeLabelClasses();
        return outputCoordinates;
    }

    public static List<List<InternalDataFormat>> GetDataFromCoordinates(List<List<Vector3>> listOfCoordinateLists)
    {
        List<List<InternalDataFormat>> dataList_out = new List<List<InternalDataFormat>>();

        for (int i = 0; i < listOfCoordinateLists.Count; i++)
        {
            List<Vector3> singleCoordinateList = listOfCoordinateLists.ElementAt(i);
            List<InternalDataFormat> singleDataList = new List<InternalDataFormat>();

            for (int j = 0; j < singleCoordinateList.Count; j++)
            {
                singleDataList.Add(new InternalDataFormat(j, singleCoordinateList.ElementAt(j), 0, 2));
            }
            dataList_out.Add(singleDataList);
        }

        return dataList_out;
    }
}