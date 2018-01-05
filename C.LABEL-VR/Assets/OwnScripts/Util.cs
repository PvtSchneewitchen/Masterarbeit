using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static void DisableAllChildren(GameObject parent_inp)
    {
        for (int i = 0; i < parent_inp.transform.childCount; i++)
        {
            var child = parent_inp.transform.GetChild(i).gameObject;
            if (child != null)
                child.SetActive(false);
        }
    }

    public static GameObject FindInactiveGameobject(GameObject parent_inp, string sName_inp)
    {
        var children = parent_inp.GetComponentsInChildren<Transform>(true);
        foreach (var child in children)
        {
            if (child.name == sName_inp)
            {
                return child.gameObject;
            }
        }
        return null;
    }

    public enum Datatype
    {
        pcd,
        lidar,
        hdf5
    }

    public enum MovementMode
    {
        FreeFlyFast,
        FreeFlySlow,
        SicknessPrevention
    }

    public static class DataLoadInfo
    {
        private static string sDataPath;
        private static Datatype dataType;
        private static bool bIsSingleFile;

        public static void StoreInfo(string sPath_inp, Datatype dataType_inp, bool bIsSingleFile_inp)
        {
            sDataPath = sPath_inp;
            dataType = dataType_inp;
            bIsSingleFile = bIsSingleFile_inp;
        }

        public static string LoadDataPath()
        {
            return sDataPath;
        }

        public static Datatype LoadDataType()
        {
            return dataType;
        }

        public static bool LoadFileQuantityInfo()
        {
            return bIsSingleFile;
        }
    }
}
