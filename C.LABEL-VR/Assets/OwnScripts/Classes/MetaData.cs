using System;
using System.Collections.Generic;
using UnityEngine;

public static class MetaData
{
    public static class Hdf5_DaimlerLidar
    {
        public static List<Dictionary<Tuple<int, int>, int>> _tableIndexToID { get; set; }
        public static List<HDF5Addon.Hdf5Container_LidarDaimler> _importedContainers { get; set; }

        static Hdf5_DaimlerLidar()
        {
            _tableIndexToID = new List<Dictionary<Tuple<int, int>, int>>();
            _importedContainers = new List<HDF5Addon.Hdf5Container_LidarDaimler>();
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
    }
}
