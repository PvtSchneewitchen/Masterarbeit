using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using HDF.PInvoke;
using System.Collections;
using System.Text;
using System;

public class HDF5Addon
{
    public static List<Lidar_Daimler> ReadDaimlerHdf(string sPath_inp)
    {
        string[] sFilePaths = new string[1];
        List<Lidar_Daimler> outContainers = new List<Lidar_Daimler>();

        if (sPath_inp.Substring(sPath_inp.Length - 5) == ".hdf5")
        {
            //single file
            sFilePaths[0] = sPath_inp;
        }
        else
        {
            //directory
            sFilePaths = Directory.GetFiles(sPath_inp);
        }

        for (int i = 0; i < sFilePaths.Length; i++)
        {
            var container = ReadContainer(sFilePaths[i]);
            Labeling.SetNewLabelClasses(container._labelWorkingSet);
            outContainers.Add(container);
        }

        return outContainers;
    }

    private static Lidar_Daimler ReadContainer(string sFilePath_inp)
    {
        Lidar_Daimler outContainer = new Lidar_Daimler();
        int rows = outContainer._distances.GetLength(0);
        int cols = outContainer._distances.GetLength(1);
        int status = 0;

        var file_id = H5F.open(sFilePath_inp, H5F.ACC_RDWR);

        outContainer._distances = GetFloatDataset(H5D.open(file_id, "distance"), rows, cols);
        outContainer._intensity = GetFloatDataset(H5D.open(file_id, "intensity"), rows, cols);
        outContainer._labelProbabilities = GetFloatDataset(H5D.open(file_id, "labelProbabilities"), rows, cols);
        outContainer._labelWorkingSet = GetLabelWorkingSet(H5G.open(file_id, "labelWorkingSet"));
        outContainer._labels = GetUintDataset(H5D.open(file_id, "labels"), rows, cols);
        outContainer._pointValid = GetIntDataset(H5D.open(file_id, "pointValid"), rows, cols);
        outContainer._sensorX = GetFloatDataset(H5D.open(file_id, "sensorX"), rows, cols);
        outContainer._sensorY = GetFloatDataset(H5D.open(file_id, "sensorY"), rows, cols);
        outContainer._sensorZ = GetFloatDataset(H5D.open(file_id, "sensorZ"), rows, cols);
        outContainer._vehicleX = GetFloatDataset(H5D.open(file_id, "vehicleX"), rows, cols);
        outContainer._vehicleY = GetFloatDataset(H5D.open(file_id, "vehicleY"), rows, cols);
        outContainer._vehicleZ = GetFloatDataset(H5D.open(file_id, "vehicleZ"), rows, cols);

        status = H5F.close(file_id);

        return outContainer;
    }

    private static Dictionary<string, uint> GetLabelWorkingSet(long group_id)
    {
        Dictionary<string, uint> labelWorkingSet = new Dictionary<string, uint>();
        H5A.operator_t callBackMethod = DelegateMethod;
        ArrayList attrNameArray = new ArrayList();
        GCHandle nameArrayAlloc = GCHandle.Alloc(attrNameArray);
        IntPtr ptrOnAllocArray = (IntPtr)nameArrayAlloc;
        int status;
        ulong beginAt = 0;

        status = H5A.iterate(group_id, H5.index_t.CRT_ORDER, H5.iter_order_t.INC, ref beginAt, callBackMethod, ptrOnAllocArray);

        for (int i = 0; i < attrNameArray.Count; i++)
        {
            string attr_name = Convert.ToString(attrNameArray[i]);
            long attr_id = H5A.open(group_id, attr_name);
            uint[] attr_value = { 0 };
            GCHandle valueAlloc = GCHandle.Alloc(attr_value, GCHandleType.Pinned);

            status = H5A.read(attr_id, H5T.NATIVE_UINT32, valueAlloc.AddrOfPinnedObject());
            status = H5A.close(attr_id);
            labelWorkingSet.Add(attr_name, attr_value[0]);
        }

        status = H5G.close(group_id);

        return labelWorkingSet;
    }

    private static int DelegateMethod(Int64 location_id, IntPtr attr_name, ref H5A.info_t ainfo, IntPtr op_data)
    {
        GCHandle hnd = (GCHandle)op_data;
        ArrayList al = (hnd.Target as ArrayList);
        int len = 0;
        while (Marshal.ReadByte(attr_name, len) != 0) { ++len; }
        byte[] buf = new byte[len];
        Marshal.Copy(attr_name, buf, 0, len);
        al.Add(Encoding.UTF8.GetString(buf));
        return 0;
    }

    private static float[,] GetFloatDataset(long dataset_id_inp, int rows_inp, int cols_inp)
    {
        int status;
        float[,] dataArray_out = new float[rows_inp, cols_inp];

        GCHandle buf = GCHandle.Alloc(dataArray_out, GCHandleType.Pinned);

        status = H5D.read(dataset_id_inp, H5T.NATIVE_FLOAT, H5S.ALL, H5S.ALL, H5P.DEFAULT, buf.AddrOfPinnedObject());
        status = H5D.close(dataset_id_inp);

        return dataArray_out;
    }

    private static uint[,] GetUintDataset(long dataset_id_inp, int rows_inp, int cols_inp)
    {
        int status;
        uint[,] dataArray_out = new uint[rows_inp, cols_inp];

        GCHandle buf = GCHandle.Alloc(dataArray_out, GCHandleType.Pinned);

        status = H5D.read(dataset_id_inp, H5T.NATIVE_UINT, H5S.ALL, H5S.ALL, H5P.DEFAULT, buf.AddrOfPinnedObject());
        H5D.close(dataset_id_inp);

        return dataArray_out;
    }

    private static int[,] GetIntDataset(long dataset_id_inp, int rows_inp, int cols_inp)
    {
        int status;
        int[,] dataArray_out = new int[rows_inp, cols_inp];

        GCHandle buf = GCHandle.Alloc(dataArray_out, GCHandleType.Pinned);

        status = H5D.read(dataset_id_inp, H5T.NATIVE_INT, H5S.ALL, H5S.ALL, H5P.DEFAULT, buf.AddrOfPinnedObject());
        H5D.close(dataset_id_inp);

        return dataArray_out;
    }

    public class Lidar_Daimler
    {
        public float[,] _distances { get; set; }
        public float[,] _intensity { get; set; }
        public float[,] _labelProbabilities { get; set; }
        public Dictionary<string, uint> _labelWorkingSet { get; set; }
        public uint[,] _labels { get; set; }
        public int[,] _pointValid { get; set; }
        public float[,] _sensorX { get; set; }
        public float[,] _sensorY { get; set; }
        public float[,] _sensorZ { get; set; }
        public float[,] _vehicleX { get; set; }
        public float[,] _vehicleY { get; set; }
        public float[,] _vehicleZ { get; set; }

        public Lidar_Daimler()
        {
            int rows = 16;
            int colums = 1800;

            _distances = new float[rows, colums];
            _intensity = new float[rows, colums];
            _labelProbabilities = new float[rows, colums];
            _labelWorkingSet = new Dictionary<string, uint>();
            _labels = new uint[rows, colums];
            _pointValid = new int[rows, colums];
            _sensorX = new float[rows, colums];
            _sensorY = new float[rows, colums];
            _sensorZ = new float[rows, colums];
            _vehicleX = new float[rows, colums];
            _vehicleY = new float[rows, colums];
            _vehicleZ = new float[rows, colums];
        }
    }
}