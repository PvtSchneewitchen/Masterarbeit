using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Labeling
{
    //public static LabelGroup _currentLabel;
    public static Tuple<string, uint> _currentLabelClass;

    private static Dictionary<Tuple<string, uint>, Material> _labelClasses;
    private static Material _standardMaterial;
    private static List<Color> _standardColors;

    static Labeling()
    {
        Material unlabeledMaterial = Resources.Load("Materials/Unlabeled") as Material;
        _standardMaterial = unlabeledMaterial;

        _labelClasses = new Dictionary<Tuple<string, uint>, Material>
            {
                { new Tuple<string, uint>("unlabeled", 0), unlabeledMaterial }
            };

        _standardColors = new List<Color>
            {
                Color.yellow,
                Color.red,
                Color.blue,
                Color.green
            };

        SetCurrentLabelClass(new Tuple<string, uint>("unlabeled", 0)); 
    }

    public static void SetNewLabelClasses(Dictionary<string, uint> labelWorkingSet_inp)
    {
        for (int i = 0; i < labelWorkingSet_inp.Count; i++)
        {
            string key = labelWorkingSet_inp.ElementAt(i).Key;
            uint value = labelWorkingSet_inp.ElementAt(i).Value;
            if (key != "unlabeled" && value != 0 && !_labelClasses.ContainsKey(new Tuple<string, uint>(key, value)))
            {
                _labelClasses.Add(new Tuple<string, uint>(key, value), CreateNewMaterial());
            }
        }
    }

    public static Material GetGroupMaterial(Tuple<string, uint> label_inp)
    {
        Material outMat = new Material(_standardMaterial);

        _labelClasses.TryGetValue(label_inp, out outMat);

        return outMat;
    }

    public static void SetCurrentLabelClass(Tuple<string, uint> label_inp)
    {
        if (_labelClasses.ContainsKey(label_inp))
        {
            _currentLabelClass = label_inp;
        }
        else
        {
            UnityEngine.Debug.Log("LabelClass " + label_inp.Item1 + " with ID " + label_inp.Item2 + " does not exist!");
            UnityEngine.Debug.Log("Class tried to set: " + label_inp.Item1 + " " + label_inp.Item2);
            UnityEngine.Debug.Log("available:");
            for (int i = 0; i < _labelClasses.Count; i++)
            {
                UnityEngine.Debug.Log(_labelClasses.ElementAt(i).Key.Item1 + " " + _labelClasses.ElementAt(i).Key.Item2);
            }
        }
    }

    private static Material CreateNewMaterial()
    {
        Material outMat = new Material(_standardMaterial);

        if (_standardColors.Count > 0)
        {
            outMat.color = _standardColors[0];
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
