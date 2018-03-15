using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using HDF.PInvoke;

public class Export
{
    private static ControlScript _ctrl;
    private static string _exportDataPath;

    public static void ExportHdf5_DaimlerLidar(string exportPath_inp)
    {
        _ctrl = GameObject.Find("AppController").GetComponent<ControlScript>();

        for (int i = 0; i < _ctrl._session._pointClouds.Count; i++)
        {
            var container = MetaData.Hdf5_DaimlerLidar._importedContainers[i];
            var indexToID = MetaData.Hdf5_DaimlerLidar._tableIndexToID[i];

            PointCloud cloud = _ctrl._session._pointClouds[i];
            List<GameObject> pointList = cloud._validPoints;

            string[] filePaths = Directory.GetFiles(exportPath_inp);
            string exportDatapath = Path.Combine(exportPath_inp, Path.GetFileName(cloud._pathToPointCloudData));

            if (filePaths.ToList().Contains(exportDatapath))
            {
                //overwrite
                HDF5Addon.OverwriteHdf5_DaimlerLidar(i, indexToID, container, pointList, exportDatapath);
            }
            else
            {
                //create new
                HDF5Addon.CreateNewHdf5File_DaimlerLidar(i, indexToID, container, pointList, exportDatapath);
            }

        }
        Debug.Log("hdf5-Files exported to " + exportPath_inp);
    }

    public static void ExportPcd(string exportPath_inp)
    {
        Quaternion UnityToPcdCs = Quaternion.Euler(-90, 90, 0);

        _ctrl = GameObject.Find("AppController").GetComponent<ControlScript>();

        for (int i = 0; i < _ctrl._session._pointClouds.Count; i++)
        {
            PointCloud cloud = _ctrl._session._pointClouds[i];
            List<GameObject> pointList = cloud._validPoints;

            string[] filePaths = Directory.GetFiles(exportPath_inp);

            if (filePaths.ToList().Contains(Path.Combine(exportPath_inp, Path.GetFileName(cloud._pathToPointCloudData))))
            {
                _exportDataPath = cloud._pathToPointCloudData;
            }
            else
            {
                _exportDataPath = Path.Combine(exportPath_inp, Path.GetFileName(cloud._pathToPointCloudData));
            }

            using (StreamWriter pcdFileWriter = new StreamWriter(_exportDataPath, false))
            {
                pcdFileWriter.WriteLine("FIELDS x y z label");
                pcdFileWriter.WriteLine("DATA ascii");

                for (int j = 0; j < pointList.Count; j++)
                {
                    CustomAttributes attr = pointList[j].GetComponent<CustomAttributes>();
                    Vector3 position = UnityToPcdCs * attr._pointPosition;
                    position.x *= -1;

                    string lineContent = position.y.ToString() + " "
                                            + position.z.ToString() + " "
                                                + position.x.ToString() + " "
                                                     + attr._label.ToString();

                    pcdFileWriter.WriteLine(lineContent);
                }
            }
        }
        Debug.Log("Pcd-Files exported to " + _exportDataPath);
    }
}
