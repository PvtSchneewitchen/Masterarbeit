using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using HDF.PInvoke;
using System.Collections;
using System.Text;
using System;
using UnityEngine;

public class HDF5Addon
{
    public static List<List<InternalDataFormat>> ReadHdf5_DaimlerLidar(string loadPath_inp, ref List<string> paths_ref)
    {
        string[] filePaths;

        List<List<InternalDataFormat>> listOfDataLists_out = new List<List<InternalDataFormat>>();

        if (loadPath_inp.Substring(loadPath_inp.Length - 5) == ".hdf5")
        {
            //single file
            string[] temp = new string[1];
            temp[0] = loadPath_inp;
            filePaths = temp;
            paths_ref.Add(loadPath_inp);
        }
        else
        {
            //directory
            filePaths = Directory.GetFiles(loadPath_inp + "\\", "*.hdf5");
            for (int i = 0; i < filePaths.Length; i++)
            {
                paths_ref.Add(filePaths[i]);
            }
        }

        Debug.Log("Loading FIles: ");
        Debug.Log(filePaths);

        for (int i = 0; i < filePaths.Length; i++)
        {
            Hdf5Container_LidarDaimler container = ReadContainer(filePaths[i]);
            List<InternalDataFormat> listOfData = Processhdf5Container_DaimlerLidar(i, container);

            Labeling.SetNewLabelClasses(container._labelWorkingSet);
            listOfDataLists_out.Add(listOfData);
        }

        return listOfDataLists_out;
    }

    public static void CreateNewHdf5File_DaimlerLidar(int fileIndex_inp, Dictionary<Tuple<int, int>, int> indexToID, Hdf5Container_LidarDaimler container, List<GameObject> pointList, string exportDatapath)
    {
        int status = 0;

        long file_id = H5F.create(exportDatapath, H5F.ACC_TRUNC);

        int rows = container._labels.GetLength(0);
        int cols = container._labels.GetLength(1);

        uint[,] labels = container._labels;

        pointList.OrderBy(x => x.GetComponent<CustomAttributes>()._ID);
        indexToID.OrderBy(x => x.Value);
        for (int i = 0; i < pointList.Count; i++)
        {
            var attr = pointList[i].GetComponent<CustomAttributes>();
            labels[indexToID.ElementAt(i).Key.Item1, indexToID.ElementAt(i).Key.Item2] = attr._label;
        }

        //for (int i = 0; i < pointList.Count; i++)
        //{
        //    var attr = pointList[i].GetComponent<CustomAttributes>();
        //    var index = indexToID.First(indx => indx.Value == attr._ID).Key;
        //    labels[index.Item1, index.Item2] = attr._label;
        //}

        Hdf5IO.WriteUIntDataset(file_id, "labels", labels, false);
        Hdf5IO.WriteLabelWorkingSet(file_id, false);

        //write unchangenged datasets
        Hdf5IO.WriteFloatDataset(file_id, "distance", container._distances, false);
        Hdf5IO.WriteFloatDataset(file_id, "intensity", container._intensity, false);
        Hdf5IO.WriteFloatDataset(file_id, "labelProbabilities", container._labelProbabilities, false);
        Hdf5IO.WriteIntDataset(file_id, "pointValid", container._pointValid, false);
        Hdf5IO.WriteFloatDataset(file_id, "sensorX", container._sensorX, false);
        Hdf5IO.WriteFloatDataset(file_id, "sensorY", container._sensorY, false);
        Hdf5IO.WriteFloatDataset(file_id, "sensorZ", container._sensorZ, false);
        Hdf5IO.WriteFloatDataset(file_id, "vehicleX", container._vehicleX, false);
        Hdf5IO.WriteFloatDataset(file_id, "vehicleY", container._vehicleY, false);
        Hdf5IO.WriteFloatDataset(file_id, "vehicleZ", container._vehicleZ, false);

        status = H5F.close(file_id);
    }

    public static void OverwriteHdf5_DaimlerLidar(int fileIndex_inp, Dictionary<Tuple<int, int>, int> indexToID, Hdf5Container_LidarDaimler container, List<GameObject> pointList, string pathToFile_inp)
    {
        var previousTime = Time.realtimeSinceStartup;

        var duration = Time.realtimeSinceStartup - previousTime;
        previousTime = Time.realtimeSinceStartup;
        Debug.Log("Start: " + duration);

        int status = 0;

        long file_id = H5F.open(pathToFile_inp, H5F.ACC_RDWR);

        int rows = container._labels.GetLength(0);
        int cols = container._labels.GetLength(1);

        uint[,] labels = new uint[rows, cols];
        var pointValid = container._pointValid;

        labels = container._labels;

        pointList.OrderBy(x => x.GetComponent<CustomAttributes>()._ID);
        indexToID.OrderBy(x => x.Value);

        for (int i = 0; i < pointList.Count; i++)
        {
            var attr = pointList[i].GetComponent<CustomAttributes>();
            var key = indexToID.ElementAt(i).Key;

            labels[key.Item1, key.Item2] = attr._label;
        }

        //for (int i = 0; i < pointList.Count; i++)
        //{
        //    var attr = pointList[i].GetComponent<CustomAttributes>();
        //    var index = indexToID.First(indx => indx.Value == attr._ID).Key;
        //    labels[index.Item1, index.Item2] = attr._label;
        //}

        Hdf5IO.WriteUIntDataset(file_id, "labels", labels, true);
        Hdf5IO.WriteLabelWorkingSet(file_id, true);

        status = H5F.close(file_id);
    }

    private static List<InternalDataFormat> Processhdf5Container_DaimlerLidar(int fileIndex, Hdf5Container_LidarDaimler hdf5Container_inp)
    {
        List<InternalDataFormat> listOfData_out = new List<InternalDataFormat>();
        MetaData.Hdf5_DaimlerLidar._importedContainers.Add(hdf5Container_inp);
        MetaData.Hdf5_DaimlerLidar._tableIndexToID.Add(new Dictionary<Tuple<int, int>, int>());


        int containerRows = hdf5Container_inp._distances.GetLength(0);
        int containerCols = hdf5Container_inp._distances.GetLength(1);

        for (int j = 0; j < containerRows; j++)
        {
            for (int k = 0; k < containerCols; k++)
            {
                InternalDataFormat data = CreateInternalDataFormat(hdf5Container_inp, j, k);
                MetaData.Hdf5_DaimlerLidar._tableIndexToID[fileIndex].Add(new Tuple<int, int>(j, k), data._ID);

                if (hdf5Container_inp._pointValid[j, k] == 1)
                    listOfData_out.Add(data);
            }
        }
        return listOfData_out;
    }

    

    private static InternalDataFormat CreateInternalDataFormat(Hdf5Container_LidarDaimler hdf5Container_input, int rowIndex_inp, int columIndex)
    {

        int ID = int.Parse(Convert.ToString(rowIndex_inp + 1) + Convert.ToString(columIndex + 1));
        Vector3 position = new Vector3(hdf5Container_input._vehicleX[rowIndex_inp, columIndex], hdf5Container_input._vehicleY[rowIndex_inp, columIndex], hdf5Container_input._vehicleZ[rowIndex_inp, columIndex]);
        uint label = hdf5Container_input._labels[rowIndex_inp, columIndex];

        return new InternalDataFormat(ID, position, label, 2);
    }

    private static Hdf5Container_LidarDaimler ReadContainer(string sFilePath_inp)
    {
        int status = 0;

        long file_id = H5F.open(sFilePath_inp, H5F.ACC_RDWR);
        long testDataset_id = H5D.open(file_id, "distance");
        long testDataspace_id = H5D.get_space(testDataset_id);
        ulong[] dims = new ulong[2];
        status = H5S.get_simple_extent_dims(testDataspace_id, dims, null);

        int rows = Convert.ToInt32(dims[0]);
        int cols = Convert.ToInt32(dims[1]);

        Hdf5Container_LidarDaimler outContainer = new Hdf5Container_LidarDaimler(rows, cols)
        {
            _distances = Hdf5IO.GetFloatDataset(H5D.open(file_id, "distance"), rows, cols),
            _intensity = Hdf5IO.GetFloatDataset(H5D.open(file_id, "intensity"), rows, cols),
            _labelProbabilities = Hdf5IO.GetFloatDataset(H5D.open(file_id, "labelProbabilities"), rows, cols),
            _labelWorkingSet = Hdf5IO.GetLabelWorkingSet(H5G.open(file_id, "labelWorkingSet")),
            _labels = Hdf5IO.GetUintDataset(H5D.open(file_id, "labels"), rows, cols),
            _pointValid = Hdf5IO.GetIntDataset(H5D.open(file_id, "pointValid"), rows, cols),
            _sensorX = Hdf5IO.GetFloatDataset(H5D.open(file_id, "sensorX"), rows, cols),
            _sensorY = Hdf5IO.GetFloatDataset(H5D.open(file_id, "sensorY"), rows, cols),
            _sensorZ = Hdf5IO.GetFloatDataset(H5D.open(file_id, "sensorZ"), rows, cols),
            _vehicleX = Hdf5IO.GetFloatDataset(H5D.open(file_id, "vehicleX"), rows, cols),
            _vehicleY = Hdf5IO.GetFloatDataset(H5D.open(file_id, "vehicleY"), rows, cols),
            _vehicleZ = Hdf5IO.GetFloatDataset(H5D.open(file_id, "vehicleZ"), rows, cols)
        };

        status = H5F.close(file_id);

        return outContainer;
    }



    public class Hdf5IO
    {
        public static float[,] GetFloatDataset(long dataset_id_inp, int rows_inp, int cols_inp)
        {
            int status;
            float[,] dataArray_out = new float[rows_inp, cols_inp];

            GCHandle buf = GCHandle.Alloc(dataArray_out, GCHandleType.Pinned);

            status = H5D.read(dataset_id_inp, H5T.NATIVE_FLOAT, H5S.ALL, H5S.ALL, H5P.DEFAULT, buf.AddrOfPinnedObject());
            status = H5D.close(dataset_id_inp);

            return dataArray_out;
        }

        public static uint[,] GetUintDataset(long dataset_id_inp, int rows_inp, int cols_inp)
        {
            int status;
            uint[,] dataArray_out = new uint[rows_inp, cols_inp];

            GCHandle buf = GCHandle.Alloc(dataArray_out, GCHandleType.Pinned);

            status = H5D.read(dataset_id_inp, H5T.NATIVE_UINT, H5S.ALL, H5S.ALL, H5P.DEFAULT, buf.AddrOfPinnedObject());
            status = H5D.close(dataset_id_inp);

            return dataArray_out;
        }

        public static int[,] GetIntDataset(long dataset_id_inp, int rows_inp, int cols_inp)
        {
            int status;
            int[,] dataArray_out = new int[rows_inp, cols_inp];

            GCHandle buf = GCHandle.Alloc(dataArray_out, GCHandleType.Pinned);

            status = H5D.read(dataset_id_inp, H5T.NATIVE_INT, H5S.ALL, H5S.ALL, H5P.DEFAULT, buf.AddrOfPinnedObject());
            H5D.close(dataset_id_inp);

            return dataArray_out;
        }

        public static void WriteFloatDataset(long file_id_inp, string datasetName_inp, float[,] datasetValues_inp, bool overwrite_inp)
        {
            int status = 0;
            long dataset_id = 0;

            if (overwrite_inp)
            {
                dataset_id = H5D.open(file_id_inp, datasetName_inp);
            }
            else
            {
                ulong[] dims = new ulong[2];
                dims[0] = (ulong)datasetValues_inp.GetLength(0);
                dims[1] = (ulong)datasetValues_inp.GetLength(1);
                dataset_id = H5D.create(file_id_inp, datasetName_inp, H5T.NATIVE_FLOAT, H5S.create_simple(dims.Count(), dims, null));
            }

            GCHandle buf = GCHandle.Alloc(datasetValues_inp, GCHandleType.Pinned);

            status = H5D.write(dataset_id, H5T.NATIVE_FLOAT, H5S.ALL, H5S.ALL, H5P.DEFAULT, buf.AddrOfPinnedObject());

            status = H5D.close(dataset_id);
        }

        public static void WriteIntDataset(long file_id_inp, string datasetName_inp, int[,] datasetValues_inp, bool overwrite_inp)
        {
            int status = 0;
            long dataset_id = 0;

            if (overwrite_inp)
            {
                dataset_id = H5D.open(file_id_inp, datasetName_inp);
            }
            else
            {
                ulong[] dims = new ulong[2];
                dims[0] = (ulong)datasetValues_inp.GetLength(0);
                dims[1] = (ulong)datasetValues_inp.GetLength(1);
                dataset_id = H5D.create(file_id_inp, datasetName_inp, H5T.NATIVE_INT, H5S.create_simple(dims.Count(), dims, null));
            }

            GCHandle buf = GCHandle.Alloc(datasetValues_inp, GCHandleType.Pinned);

            status = H5D.write(dataset_id, H5T.NATIVE_INT, H5S.ALL, H5S.ALL, H5P.DEFAULT, buf.AddrOfPinnedObject());

            status = H5D.close(dataset_id);
        }

        public static void WriteUIntDataset(long file_id_inp, string datasetName_inp, uint[,] datasetValues_inp, bool overwrite_inp)
        {
            int status = 0;
            long dataset_id = 0;

            if (overwrite_inp)
            {
                dataset_id = H5D.open(file_id_inp, datasetName_inp);
            }
            else
            {
                ulong[] dims = new ulong[2];
                dims[0] = (ulong)datasetValues_inp.GetLength(0);
                dims[1] = (ulong)datasetValues_inp.GetLength(1);
                dataset_id = H5D.create(file_id_inp, datasetName_inp, H5T.NATIVE_UINT, H5S.create_simple(dims.Count(),dims,null));
            }

            GCHandle buf = GCHandle.Alloc(datasetValues_inp, GCHandleType.Pinned);

            status = H5D.write(dataset_id, H5T.NATIVE_UINT32, H5S.ALL, H5S.ALL, H5P.DEFAULT, buf.AddrOfPinnedObject());

            status = H5D.close(dataset_id);
        }

        public static void WriteLabelWorkingSet(long fileId_inp, bool overwrite_inp)
        {
            var LabelClassInfos = Labeling.GetAllIdsNamesAndMaterials();

            int status = 0;
            ulong[] dims = new ulong[2];
            dims[0] = 2;
            dims[1] = (ulong)LabelClassInfos.Count;

            long group_id = 0;

            if (overwrite_inp)
            {
                group_id = H5G.open(fileId_inp, "labelWorkingSet");
            }
            else
            {
                group_id = H5G.create(fileId_inp, "labelWorkingSet");
            }

            for (int i = 0; i < LabelClassInfos.Count; i++)
            {
                GCHandle labelID = GCHandle.Alloc(LabelClassInfos.ElementAt(i).Key, GCHandleType.Pinned);

                string labelClassName = LabelClassInfos.ElementAt(i).Value.Item1;

                long attr_id = H5A.open(group_id, labelClassName);

                if (attr_id >= 0)
                {
                    //attribute exists
                    status = H5A.write(attr_id, H5T.NATIVE_UINT32, labelID.AddrOfPinnedObject());
                }
                else
                {
                    //attribute has to be created
                    attr_id = H5A.create(group_id, LabelClassInfos.ElementAt(i).Value.Item1, H5T.NATIVE_UINT32, H5S.create(H5S.class_t.SCALAR));
                    status = H5A.write(attr_id, H5T.NATIVE_UINT32, labelID.AddrOfPinnedObject());
                }

                status = H5A.close(attr_id);
            }

            status = H5G.close(group_id);
        }

        public static Dictionary<uint, string> GetLabelWorkingSet(long group_id)
        {
            Dictionary<uint, string> labelWorkingSet = new Dictionary<uint, string>();
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
                labelWorkingSet.Add(attr_value[0], attr_name);
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
    }

    public class Hdf5Container_LidarDaimler
    {
        public float[,] _distances { get; set; }
        public float[,] _intensity { get; set; }
        public float[,] _labelProbabilities { get; set; }
        public Dictionary<uint, string> _labelWorkingSet { get; set; }
        public uint[,] _labels { get; set; }
        public int[,] _pointValid { get; set; }
        public float[,] _sensorX { get; set; }
        public float[,] _sensorY { get; set; }
        public float[,] _sensorZ { get; set; }
        public float[,] _vehicleX { get; set; }
        public float[,] _vehicleY { get; set; }
        public float[,] _vehicleZ { get; set; }

        public Hdf5Container_LidarDaimler(int rows, int colums)
        {
            _distances = new float[rows, colums];
            _intensity = new float[rows, colums];
            _labelProbabilities = new float[rows, colums];
            _labelWorkingSet = new Dictionary<uint, string>();
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

    public class Hdf5MetaData_LidarDaimler
    {
        public int _ID { get; private set; }
        public Tuple<int, int> _tableIndex { get; private set; }
        public float _distance { get; private set; }
        public float _intensity { get; private set; }
        public float _labelprobability { get; private set; }
        public int _pointvalid { get; private set; }
        public Vector3 _sensor { get; private set; }
        public Vector3 _vehicle { get; private set; }

        public Hdf5MetaData_LidarDaimler(int ID_inp, Hdf5Container_LidarDaimler container_inp, int rowIndex_inp, int columIndex_inp)
        {
            _ID = ID_inp;
            _tableIndex = new Tuple<int, int>(rowIndex_inp, columIndex_inp);
            _distance = container_inp._distances[rowIndex_inp, columIndex_inp];
            _intensity = container_inp._intensity[rowIndex_inp, columIndex_inp];
            _labelprobability = container_inp._labelProbabilities[rowIndex_inp, columIndex_inp];
            _pointvalid = container_inp._pointValid[rowIndex_inp, columIndex_inp];
            _sensor = new Vector3(container_inp._sensorX[rowIndex_inp, columIndex_inp],
                                     container_inp._sensorY[rowIndex_inp, columIndex_inp],
                                        container_inp._sensorZ[rowIndex_inp, columIndex_inp]);
            _vehicle = new Vector3(container_inp._vehicleX[rowIndex_inp, columIndex_inp],
                                     container_inp._vehicleY[rowIndex_inp, columIndex_inp],
                                        container_inp._vehicleZ[rowIndex_inp, columIndex_inp]);
        }
    }
}