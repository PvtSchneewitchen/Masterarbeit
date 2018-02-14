using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class Labeling
{
    public static LabelGroup _currentLabel;

    private static bool _bIsInitialized = false;
    //! the number and order of Labelgroup items and Groupcolor items must fit together!!

    static Labeling()
    {
        Materials.Unlabeled = Resources.Load("Materials/Unlabeled") as Material;
        Materials.Bicycle = Resources.Load("Materials/Bicycle") as Material;
        Materials.Building = Resources.Load("Materials/Building") as Material;
        Materials.Bus = Resources.Load("Materials/Bus") as Material;
        Materials.Car = Resources.Load("Materials/Car") as Material;
        Materials.Fence = Resources.Load("Materials/Fence") as Material;
        Materials.Motorcycle = Resources.Load("Materials/Motorcycle") as Material;
        Materials.Person = Resources.Load("Materials/Person") as Material;
        Materials.Pole = Resources.Load("Materials/Pole") as Material;
        Materials.Rider = Resources.Load("Materials/Rider") as Material;
        Materials.Road = Resources.Load("Materials/Road") as Material;
        Materials.Sidewalk = Resources.Load("Materials/Sidewalk") as Material;
        Materials.Sky = Resources.Load("Materials/Sky") as Material;
        Materials.Terrain = Resources.Load("Materials/Terrain") as Material;
        Materials.TrafficLight = Resources.Load("Materials/TrafficLight") as Material;
        Materials.TrafficSign = Resources.Load("Materials/TrafficSign") as Material;
        Materials.Train = Resources.Load("Materials/Train") as Material;
        Materials.Truck = Resources.Load("Materials/Truck") as Material;
        Materials.Void = Resources.Load("Materials/Void") as Material;
        Materials.Vegetation = Resources.Load("Materials/Vegetation") as Material;
        Materials.Wall = Resources.Load("Materials/Wall") as Material;
    }

    public static void SetCurrentGroup(LabelGroup newGroup_inp)
    {
        _currentLabel = newGroup_inp;
    }

    public static Material GetGroupMaterial(LabelGroup group_inp)
    {
        PropertyInfo[] properties = typeof(Materials).GetProperties();
        PropertyInfo p = properties[(int)group_inp];
        var v = properties[(int)group_inp].GetValue(typeof(Material), null);
        Material material_out = (Material)properties[(int)group_inp].GetValue(typeof(Materials), null);
        return material_out;
    }

    public enum LabelGroup
    {
        unlabeled,
        bicycle,
        building,
        bus,
        car,
        fence,
        motorcycle,
        person,
        pole,
        rider,
        road,
        sidewalk,
        sky,
        terrain,
        trafficLight,
        trafficSign,
        train,
        truck,
        vegetation,
        Void,
        wall
    }

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

    private struct Materials
    {
        public static Material Unlabeled { get; set; }
        public static Material Bicycle { get; set; }
        public static Material Building { get; set; }
        public static Material Bus { get; set; }
        public static Material Car { get; set; }
        public static Material Fence { get; set; }
        public static Material Motorcycle { get; set; }
        public static Material Person { get; set; }
        public static Material Pole { get; set; }
        public static Material Rider { get; set; }
        public static Material Road { get; set; }
        public static Material Sidewalk { get; set; }
        public static Material Sky { get; set; }
        public static Material Terrain { get; set; }
        public static Material TrafficLight { get; set; }
        public static Material TrafficSign { get; set; }
        public static Material Train { get; set; }
        public static Material Truck { get; set; }
        public static Material Void { get; set; }
        public static Material Vegetation { get; set; }
        public static Material Wall { get; set; }
    }
}
