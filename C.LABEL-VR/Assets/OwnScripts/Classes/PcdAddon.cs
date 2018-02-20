using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class PcdAddon
{
    /// <summary>
    /// Reads one or multiple pcd-files and delivers the coordinates from it. 
    /// It adds a label class to the global label classes
    /// </summary>
    /// <param name="sPath_inp"> Path of a pcd file or a directory with pcd-files in it</param>
    /// <returns>coordinates of the pcd files</returns>
    public static Dictionary<int, List<Vector3>> ReadPcdFromPath(string sPath_inp)
    {
        Dictionary<int, List<Vector3>> outputCoordinates = new Dictionary<int, List<Vector3>>();
        List<Vector3> pcdCoordinates = new List<Vector3>();
        List<StreamReader> pcdFiles = new List<StreamReader>();

        if (sPath_inp.Substring(sPath_inp.Length - 4) == ".pcd") 
        {
            //single file

            pcdFiles.Add(new StreamReader(sPath_inp));
        }
        else
        {
            //directory

            string[] files = Directory.GetFiles(sPath_inp, "*.pcd");
            foreach (var file in files)
            {
                pcdFiles.Add(new StreamReader(file));
            }
        }

        for (int i = 0; i < pcdFiles.Count; i++)
        {
            List<Vector3> coordinatesFromFile = new List<Vector3>();
            bool firstLineSkipped = false;

            while (!pcdFiles[i].EndOfStream)
            {
                var line = pcdFiles[i].ReadLine();
                if (firstLineSkipped)
                {
                    var coordinates = line.Split(' ');

                    float x = 0;
                    float y = 0;
                    float z = 0;

                    try
                    {
                        x = float.Parse(coordinates[0]);
                        y = float.Parse(coordinates[1]);
                        z = float.Parse(coordinates[2]);
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Source + " " + e.Message);
                    }

                    //pcd data is not compatible with unity coordinate system
                    //the pointcloud is displayed vertical and is mirrored
                    //so one has to rotate it around the x-axis by -90° and negate the x-coordinate
                    coordinatesFromFile.Add(Quaternion.Euler(-90, 0, 0) * new Vector3(-x, y, z));
                }
                else
                {
                    firstLineSkipped = true;
                }
            }
            outputCoordinates.Add(i, coordinatesFromFile);
        }

        InitializeLabelClasses();
        return outputCoordinates;
    }

    private static void InitializeLabelClasses()
    {
        Dictionary<string, uint> classes = new Dictionary<string, uint>
        {
            { "label1", 1 },
            { "label2", 2 },
            { "label3", 3 },
            { "label4", 4 }
        };

        Labeling.SetNewLabelClasses(classes);
    }
}
