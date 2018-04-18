using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using HDF.PInvoke;

/// <summary>
/// Class that calls all specific export functions from the Data Addons
/// </summary>
public class Export
{
    private static SessionHandler sessionHandler;
    private static string exportDataPath;

    /// <summary>
    /// Calls the Exportfunction of the Daimler hdf5 addon that all files that were imorted get exorted to the given path
    /// </summary>
    /// <param name="exportPath_inp">Export path inp.</param>
    public static void ExportHdf5_DaimlerLidar(string exportPath_inp)
    {
        sessionHandler = ReferenceHandler.Instance.GetSessionHandler();

        for (int i = 0; i < sessionHandler.Session._pointClouds.Count; i++)
        {
            var container = MetaData.Hdf5_DaimlerLidar._importedContainers[i];
            var indexToID = MetaData.Hdf5_DaimlerLidar._tableIndexToID[i];

            PointCloud cloud = sessionHandler.Session._pointClouds[i];
            List<GameObject> pointList = cloud._validPoints;

            string[] filePaths;
            try
            {
                filePaths = Directory.GetFiles(exportPath_inp);
            }
            catch
            {
                exportPath_inp = exportPath_inp + "//";
                Debug.Log(exportPath_inp + "is not a valid Directory => trying: " + exportPath_inp);
                filePaths = Directory.GetFiles(exportPath_inp);
            }

            exportDataPath = Path.Combine(exportPath_inp, Path.GetFileName(cloud._pathToPointCloudData));

            if (filePaths.ToList().Contains(exportDataPath))
            {
                //overwrite
                HDF5Addon.OverwriteHdf5_DaimlerLidar(i, indexToID, container, pointList, exportDataPath);
            }
            else
            {
                //create new
                HDF5Addon.CreateNewHdf5File_DaimlerLidar(i, indexToID, container, pointList, exportDataPath);
            }
            Debug.Log("hdf5-Files exported to " + exportDataPath);
        }
        
    }

	/// <summary>
    /// Calls the Exportfunction of the pcd addon that all files that were imorted get exorted to the given path
    /// </summary>
    /// <param name="exportPath_inp">Export path inp.</param>
    public static void ExportPcd(string exportPath_inp)
    {
        Quaternion UnityToPcdCs = Quaternion.Euler(-90, 90, 0);

        sessionHandler = ReferenceHandler.Instance.GetSessionHandler();

        for (int i = 0; i < sessionHandler.Session._pointClouds.Count; i++)
        {
            PointCloud cloud = sessionHandler.Session._pointClouds[i];
            List<GameObject> pointList = cloud._validPoints;

            string[] filePaths = Directory.GetFiles(exportPath_inp);

            if (filePaths.ToList().Contains(Path.Combine(exportPath_inp, Path.GetFileName(cloud._pathToPointCloudData))))
            {
                exportDataPath = cloud._pathToPointCloudData;
            }
            else
            {
                exportDataPath = Path.Combine(exportPath_inp, Path.GetFileName(cloud._pathToPointCloudData));
            }

            using (StreamWriter pcdFileWriter = new StreamWriter(exportDataPath, false))
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
        Debug.Log("Pcd-Files exported to " + exportDataPath);
    }
}
