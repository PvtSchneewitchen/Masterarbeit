using System;
using System.Collections.Generic;
using UnityEngine;

public static class MetaData
{
    public static class Hdf5_DaimlerLidar
    {
        public static List<Dictionary<Tuple<int, int>, int>> _tableIndexToID { get; set; }
        public static List<HDF5Addon.Hdf5Container_LidarDaimler> _importedContainer { get; set; }

        static Hdf5_DaimlerLidar()
        {
            _tableIndexToID = new List<Dictionary<Tuple<int, int>, int>>();
            _importedContainer = new List<HDF5Addon.Hdf5Container_LidarDaimler>();
        }

        public static int GetIdByTableIndex(int fileIndex_inp, int row_inp, int col_inp)
        {
            int ID_out;
            if(_tableIndexToID[fileIndex_inp].TryGetValue(new Tuple<int, int>(row_inp,col_inp),out ID_out))
            {
                return ID_out;
            }
            else
            {
                Debug.Log("GetIdByTableIndex: no index like "+  new Tuple<int, int>(row_inp, col_inp));
                return -1;
            }
        }

        //public static Tuple<int, int> _rowsCols { get; set; }
        //public static List<List<HDF5Addon.Hdf5MetaData_LidarDaimler>> _metaDataList { get; private set; }

        //public static void AddMetaData(int cloudIndex_inp, HDF5Addon.Hdf5MetaData_LidarDaimler data_inp)
        //{
        //    _metaDataList[cloudIndex_inp].Add(data_inp);
        //}

        //public static void SetMetaData(int cloudIndex_inp, List<HDF5Addon.Hdf5MetaData_LidarDaimler> list_inp)
        //{
        //    _metaDataList[cloudIndex_inp] = list_inp;
        //}
    }
}
