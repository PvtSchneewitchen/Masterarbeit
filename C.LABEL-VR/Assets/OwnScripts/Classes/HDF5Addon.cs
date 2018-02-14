using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HDF.PInvoke;
using System.IO;
using System.Runtime.InteropServices;

public static class HDF5Addon
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
            outContainers.Add(ReadContainer(sFilePaths[i]));
        }

        return outContainers;
    }

    private static Lidar_Daimler ReadContainer(string sFilePath_inp)
    {
        Lidar_Daimler outContainer = new Lidar_Daimler();
        int status = 0;

        var file_id = H5F.open(sFilePath_inp, H5F.ACC_RDWR);

        outContainer._distances = GetFloatDataset(H5D.open(file_id, "/distance"), outContainer._distances);
        outContainer._intensity = GetFloatDataset(H5D.open(file_id, "intensity"), outContainer._intensity);
        outContainer._labelProbabilities = GetFloatDataset(H5D.open(file_id, "labelProbabilities"), outContainer._labelProbabilities);
        outContainer._labels = GetUintDataset(H5D.open(file_id, "labels"), outContainer._labels);
        outContainer._pointValid = GetIntDataset(H5D.open(file_id, "pointValid"), outContainer._pointValid);
        outContainer._sensorX = GetFloatDataset(H5D.open(file_id, "sensorX"), outContainer._sensorX);
        outContainer._sensorY = GetFloatDataset(H5D.open(file_id, "sensorY"), outContainer._sensorY);
        outContainer._sensorZ = GetFloatDataset(H5D.open(file_id, "sensorZ"), outContainer._sensorZ);
        outContainer._vehicleX = GetFloatDataset(H5D.open(file_id, "vehicleX"), outContainer._vehicleX);
        outContainer._vehicleY = GetFloatDataset(H5D.open(file_id, "vehicleY"), outContainer._vehicleY);
        outContainer._vehicleZ = GetFloatDataset(H5D.open(file_id, "vehicleZ"), outContainer._vehicleZ);

        status = H5F.close(file_id);

        return outContainer;
    }

    private static float[,] GetFloatDataset(long dataset_id_inp, float[,] dataArray_inp)
    {
        int status;
        var dataArray_out = dataArray_inp;

        GCHandle buf = GCHandle.Alloc(dataArray_out, GCHandleType.Pinned);

        status = H5D.read(dataset_id_inp, H5T.NATIVE_FLOAT, H5S.ALL, H5S.ALL, H5P.DEFAULT, buf.AddrOfPinnedObject());

        return dataArray_inp;
    }

    private static uint[,] GetUintDataset(long dataset_id_inp, uint[,] dataArray_inp)
    {
        int status;

        GCHandle buf = GCHandle.Alloc(dataArray_inp, GCHandleType.Pinned);

        status = H5D.read(dataset_id_inp, H5T.NATIVE_UINT, H5S.ALL, H5S.ALL, H5P.DEFAULT, buf.AddrOfPinnedObject());

        return dataArray_inp;
    }

    private static int[,] GetIntDataset(long dataset_id_inp, int[,] dataArray_inp)
    {
        int status;

        GCHandle buf = GCHandle.Alloc(dataArray_inp, GCHandleType.Pinned);

        status = H5D.read(dataset_id_inp, H5T.NATIVE_INT, H5S.ALL, H5S.ALL, H5P.DEFAULT, buf.AddrOfPinnedObject());

        return dataArray_inp;
    }

    public class Lidar_Daimler
    {
        public float[,] _distances { get; set; }
        public float[,] _intensity { get; set; }
        public float[,] _labelProbabilities { get; set; }
        public enum _classifications { };
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