using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Labeling
{
    public static uint currentLabelClassID;

    private static Dictionary<uint, Tuple<string, Material>> _labelClassInformations;
    private static Material _standardMaterial;
    private static List<Color32> _standardColors;
    private static string _dummyClassName = "Dummy Class";

    static Labeling()
    {
        Material unlabeledMaterial = Resources.Load("Materials/Unlabeled") as Material;
        _standardMaterial = unlabeledMaterial;

        _labelClassInformations = new Dictionary<uint, Tuple<string, Material>>
            {
                {0 , new Tuple<string, Material>("unlabeled" ,unlabeledMaterial) }
            };

        _standardColors = new List<Color32>
        {
            new Color32(210, 240, 225, 225),
            new Color32(190, 255, 255, 225),
            new Color32(0, 255, 0, 225),
            new Color32(150, 12, 150, 225),
            new Color32(255, 0, 255, 225),
            new Color32(120, 120, 200, 225),
            new Color32(40, 140, 50, 225),
            new Color32(24, 24, 147, 225),
            new Color32(120, 120, 2, 225),
            new Color32(170, 90, 50, 225),
            new Color32(200, 200, 200, 225),
            new Color32(255, 100, 0, 225),
            new Color32(100, 255, 100, 225),
            new Color32(255, 255, 0, 225),
            new Color32(90, 130, 140, 225),
            new Color32(0, 0, 255, 225),
            new Color32(200, 20, 135, 225),
            new Color32(0, 255, 255, 225),
            new Color32(255, 0, 0, 225),
            new Color32(10, 150, 150, 225)
        };

        SetNewLabelClasses(new Dictionary<uint, string> { { 1, _dummyClassName } });
        SetCurrentLabelClass(_labelClassInformations.ElementAt(1).Key);
    }

    public static string GetLabelClassName(uint ID_inp)
    {
        Tuple<string, Material> info;

        if(_labelClassInformations.TryGetValue(ID_inp, out info))
        {
            return info.Item1;
        }
        else
        {
            Debug.Log("GetLabelClassName: No such key" + ID_inp);
            return null;
        }
    }

    public static Dictionary<uint, Tuple<string, Material>> GetAllLabelClassInfos()
    {
        return _labelClassInformations;
    }

    public static Tuple<string, Material> GetLabelClassInfo(uint ID_inp)
    {
        Tuple<string, Material> info;

        if (_labelClassInformations.TryGetValue(ID_inp, out info))
        {
            return info;
        }
        else
        {
            Debug.Log("GetLabelClassName: No such key" + ID_inp);
            return null;
        }
    }

    public static void SetNewLabelClasses(Dictionary<uint, string> labelWorkingSet_inp)
    {
        if(_labelClassInformations.ContainsKey(1))
        {
            Tuple<string, Material> info;
            _labelClassInformations.TryGetValue(1, out info);

            if(info.Item1 == _dummyClassName)
                _labelClassInformations.Remove(1);
        }

        for (int i = 0; i < labelWorkingSet_inp.Count; i++)
        {
            uint key = labelWorkingSet_inp.ElementAt(i).Key;
            string value = labelWorkingSet_inp.ElementAt(i).Value;
            if (value != "unlabeled" && key != 0 && !_labelClassInformations.ContainsKey(key))
            {
                _labelClassInformations.Add(key, new Tuple<string, Material>(value, CreateNewMaterial()));
            }
        }

        if (_labelClassInformations.Count > 1)
            SetCurrentLabelClass(_labelClassInformations.ElementAt(1).Key);
    }

    public static Material GetLabelClassMaterial(uint ID_inp)
    {
        Tuple<string, Material> info;

        if (_labelClassInformations.TryGetValue(ID_inp, out info))
        {
            return info.Item2;
        }
        else
        {
            Debug.Log("GetLabelClassMaterial: No such key" + ID_inp);
            return null;
        }
    }

    public static void SetCurrentLabelClass(uint label_inp)
    {
        if (_labelClassInformations.ContainsKey(label_inp))
        {
            currentLabelClassID = label_inp;
        }
        else
        {
            Debug.Log("LabelClass with ID " + label_inp + " does not exist!");
        }
    }

    public static Color32 GetLabelClassColor(uint ID_inp)
    {
        Tuple<string, Material> info;

        if (_labelClassInformations.TryGetValue(ID_inp, out info))
        {
            return info.Item2.color;
        }
        else
        {
            Debug.Log("GetLabelClassMaterial: No such key" + ID_inp);
            return new Color32(0, 132, 198, 1);
        }
    }

    private static Material CreateNewMaterial()
    {
        Material outMat = new Material(_standardMaterial);

        if (_standardColors.Count > 0)
        {
            outMat.color = _standardColors[0];
            _standardColors.RemoveAt(0);
        }
        else
        {
            var r = UnityEngine.Random.Range(0f, 1f);
            var g = UnityEngine.Random.Range(0f, 1f);
            var b = UnityEngine.Random.Range(0f, 1f);
            outMat.color = new Color(r, g, b);
        }

        return outMat;
    }
}

//Materials.Unlabeled = Resources.Load("Materials/Unlabeled") as Material;
//Materials.Bicycle = Resources.Load("Materials/Bicycle") as Material;
//Materials.Building = Resources.Load("Materials/Building") as Material;
//Materials.Bus = Resources.Load("Materials/Bus") as Material;
//Materials.Car = Resources.Load("Materials/Car") as Material;
//Materials.Fence = Resources.Load("Materials/Fence") as Material;
//Materials.Motorcycle = Resources.Load("Materials/Motorcycle") as Material;
//Materials.Person = Resources.Load("Materials/Person") as Material;
//Materials.Pole = Resources.Load("Materials/Pole") as Material;
//Materials.Rider = Resources.Load("Materials/Rider") as Material;
//Materials.Road = Resources.Load("Materials/Road") as Material;
//Materials.Sidewalk = Resources.Load("Materials/Sidewalk") as Material;
//Materials.Sky = Resources.Load("Materials/Sky") as Material;
//Materials.Terrain = Resources.Load("Materials/Terrain") as Material;
//Materials.TrafficLight = Resources.Load("Materials/TrafficLight") as Material;
//Materials.TrafficSign = Resources.Load("Materials/TrafficSign") as Material;
//Materials.Train = Resources.Load("Materials/Train") as Material;
//Materials.Truck = Resources.Load("Materials/Truck") as Material;
//Materials.Void = Resources.Load("Materials/Void") as Material;
//Materials.Vegetation = Resources.Load("Materials/Vegetation") as Material;
//Materials.Wall = Resources.Load("Materials/Wall") as Material;

//private struct Materials
//{
//    public static Material Unlabeled { get; set; }
//    public static Material Bicycle { get; set; }
//    public static Material Building { get; set; }
//    public static Material Bus { get; set; }
//    public static Material Car { get; set; }
//    public static Material Fence { get; set; }
//    public static Material Motorcycle { get; set; }
//    public static Material Person { get; set; }
//    public static Material Pole { get; set; }
//    public static Material Rider { get; set; }
//    public static Material Road { get; set; }
//    public static Material Sidewalk { get; set; }
//    public static Material Sky { get; set; }
//    public static Material Terrain { get; set; }
//    public static Material TrafficLight { get; set; }
//    public static Material TrafficSign { get; set; }
//    public static Material Train { get; set; }
//    public static Material Truck { get; set; }
//    public static Material Void { get; set; }
//    public static Material Vegetation { get; set; }
//    public static Material Wall { get; set; }
//}

//public static void SetCurrentGroup(LabelGroup newGroup_inp)
//{
//    _currentLabel = newGroup_inp;
//}

//public static Material GetGroupMaterial(LabelGroup group_inp)
//{
//    PropertyInfo[] properties = typeof(Materials).GetProperties();
//    PropertyInfo p = properties[(int)group_inp];
//    var v = properties[(int)group_inp].GetValue(typeof(Material), null);
//    Material material_out = (Material)properties[(int)group_inp].GetValue(typeof(Materials), null);
//    return material_out;
//}



//public static void SetCurrentGroup(int newGroup_inp)
//{
//    _currentLabel = newGroup_inp;
//}

//public enum LabelGroup
//{
//    unlabeled,
//    bicycle,
//    building,
//    bus,
//    car,
//    fence,
//    motorcycle,
//    person,
//    pole,
//    rider,
//    road,
//    sidewalk,
//    sky,
//    terrain,
//    trafficLight,
//    trafficSign,
//    train,
//    truck,
//    vegetation,
//    Void,
//    wall
//}

//unlabeled,
//bicycle = 33,
//building = 11,
//bus = 28,
//car = 26,
//fence = 13,
//motorcycle = 32,
//person = 24,
//pole = 17,
//rider = 25,
//road = 7,
//sidewalk = 8,
//sky = 23,
//terrain = 22,
//trafficLight = 19,
//trafficSign = 20,
//train = 31,
//truck = 27,
//vegetation = 21,
//Void = 38,
//wall = 12
