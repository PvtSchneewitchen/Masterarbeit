using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class MovementOptions
{
    public static Util.MovementMode MoveMode { get; set; }
    public static bool ReducePoints { get; set; }
    public static float TransSpeed { get; set; }
    public static float TransAcceleration { get; set; }
    public static float RotSpeed { get; set; }
    public static float RotAcceleration { get; set; }
    public static float TeleportDistance { get; set; }
    public static float TeleportAngle { get; set; }
    public static bool Twinkle { get; set; }

    private static void SetDefaultValues()
    {
        MoveMode = (int)Util.MovementMode.FreeFly;
        ReducePoints = false;
        TransSpeed = 50;
        TransAcceleration = 40;
        RotSpeed = 50;
        RotAcceleration = 40;
        TeleportDistance = 400;
        TeleportAngle = 20;
        Twinkle = false;
    }

    public static void LoadFromSessionPath(string sessionPath)
    {
        string userOptionsPath = sessionPath + "/UserOptions.dat";
        string defaultOptionsPath = sessionPath + "/DefaultOptions.dat";

        if (File.Exists(userOptionsPath))
        {
            LoadFrom(userOptionsPath);
            Debug.Log("Options Loaded from " + userOptionsPath);
        }
        else if (File.Exists(defaultOptionsPath))
        {
            Debug.Log("No Save Data in " + defaultOptionsPath + " || default values used");
            LoadFrom(defaultOptionsPath);
        }
        else
        {
            Debug.Log("No Save or Default Data " + defaultOptionsPath + " ||  editor values used");
            SetDefaultValues();
            SaveOptions();
        }

    }

    public static void SaveOptions()
    {
        string userOptionsPath = Util.DataLoadInfo._sessionFolderPath + "/UserOptions.dat";
        string defaultOptionsPath = Util.DataLoadInfo._sessionFolderPath + "/DefaultOptions.dat";

        Stream stream;

        if (!File.Exists(defaultOptionsPath))
        {
            stream = File.OpenWrite(defaultOptionsPath);
        }
        else
        {
            stream = File.OpenWrite(userOptionsPath);
        }

        MovementData dataToSave = new MovementData
        {
            MoveMode = MoveMode,
            ReducePoints = ReducePoints,
            TransSpeed = TransSpeed,
            TransAcceleration = TransAcceleration,
            RotSpeed = RotSpeed,
            RotAcceleration = RotAcceleration,
            TeleportDistance = TeleportDistance,
            TeleportAngle = TeleportAngle,
            Twinkle = Twinkle
        };

        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, dataToSave);
        stream.Flush();
        stream.Close();
        stream.Dispose();
    }

    public static void RestoreDefaultValues()
    {
        string defaultOptionsPth = Util.DataLoadInfo._sessionFolderPath + "/DefaultOptions.dat";

        if (File.Exists(defaultOptionsPth))
        {
            LoadFrom(defaultOptionsPth);
        }
        else
        {
            UnityEngine.Debug.Log("Something strange happened, no default values");
        }
    }

    private static void LoadFrom(string path_inp)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream fileStream = File.Open(path_inp, FileMode.Open);
        object obj = formatter.Deserialize(fileStream);
        MovementData loadedData = (MovementData)obj;
        fileStream.Flush();
        fileStream.Close();
        fileStream.Dispose();

        MoveMode = loadedData.MoveMode;
        ReducePoints = loadedData.ReducePoints;
        TransSpeed = loadedData.TransSpeed;
        TransAcceleration = loadedData.TransAcceleration;
        RotSpeed = loadedData.RotSpeed;
        RotAcceleration = loadedData.RotAcceleration;
        TeleportDistance = loadedData.TeleportDistance;
        TeleportAngle = loadedData.TeleportAngle;
        Twinkle = loadedData.Twinkle;
    }
}
