using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HDF5Addon
{
    public static Dictionary<int, List<Vector3>> ReadHdf5FromPath(string sPath_inp)
    {
        Dictionary<int, List<Vector3>> outputCoordinates = new Dictionary<int, List<Vector3>>();
        List<Vector3> pcdCoordinates = new List<Vector3>();

        if (sPath_inp.Substring(sPath_inp.Length - 5) == ".hdf5")
        {
            //single file


        }
        else
        {
            //directory


        }

        return outputCoordinates;
    }
}