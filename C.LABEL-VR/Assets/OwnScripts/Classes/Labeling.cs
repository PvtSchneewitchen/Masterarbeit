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
        Init();
    }

    public static void Reset()
    {
        Init();
    }

    private static void Init()
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
            new Color32(255,15,15,255),
            new Color32(0, 213, 43, 225),
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
        SetCurrentLabelClassID(_labelClassInformations.ElementAt(1).Key);
    }

    public static void SwitchToPreviousLabelClass()
    {
        var keyList = _labelClassInformations.Keys.ToList();
        keyList.Sort();
        var currentListIndex = keyList.IndexOf(currentLabelClassID);

        if(currentListIndex <= 0)
        {
            currentLabelClassID = keyList.Last();
        }
        else
        {
            currentLabelClassID = keyList[currentListIndex-1];
        }
    }

    public static void SwitchToNextLabelClass()
    {
        var keyList = _labelClassInformations.Keys.ToList();
        keyList.Sort();
        var currentListIndex = keyList.IndexOf(currentLabelClassID);

        if (currentListIndex+1 >= keyList.Count)
        {
            currentLabelClassID = keyList[0];
        }
        else
        {
            currentLabelClassID = keyList[currentListIndex + 1];
        }
    }

    public static void EditLabelClass(uint oldID_inp, uint newID_inp, string newName_inp, Color newColor_inp)
    {
        if (oldID_inp != 0)
        {
            _labelClassInformations.Remove(oldID_inp);

            _labelClassInformations.Add(newID_inp, new Tuple<string, Material>(newName_inp, CreateNewMaterial(newColor_inp)));
        }
    }

    public static string GetLabelClassName(uint ID_inp)
    {
        Tuple<string, Material> info;

        if (_labelClassInformations.TryGetValue(ID_inp, out info))
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

    public static Tuple<string, Material> GetLabelClassNameAndColor(uint ID_inp)
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

    public static Dictionary<uint, string> GetLabelWorkingSet()
    {
        Dictionary<uint, string> lws = new Dictionary<uint, string>();

        for (int i = 0; i < _labelClassInformations.Count; i++)
        {
            uint id = _labelClassInformations.ElementAt(i).Key;
            string name = _labelClassInformations.ElementAt(i).Value.Item1;
            lws.Add(id, name);
        }

        return lws;
    }

    public static void AddNewLabelClass(uint id_inp, string name_inp, Color color_inp)
    {
        Material mat = new Material(_standardMaterial);
        mat.color = color_inp;

        _labelClassInformations.Add(id_inp, new Tuple<string, Material>(name_inp, mat));
    }

    public static void SetNewLabelClasses(Dictionary<uint, string> labelWorkingSet_inp)
    {
        if (_labelClassInformations.ContainsKey(1))
        {
            Tuple<string, Material> info;
            _labelClassInformations.TryGetValue(1, out info);

            if (info.Item1 == _dummyClassName)
                _labelClassInformations.Remove(1);
        }

        for (int i = 0; i < labelWorkingSet_inp.Count; i++)
        {
            uint key = labelWorkingSet_inp.ElementAt(i).Key;
            string value = labelWorkingSet_inp.ElementAt(i).Value;
            if (value != "unlabeled" && key != 0 && !_labelClassInformations.ContainsKey(key))
            {
                try
                {
                    _labelClassInformations.Add(key, new Tuple<string, Material>(value, CreateNewMaterial()));
                }
                catch
                {
                    Debug.Log("SetNewLabelClasses(): Key probabably added yet");
                }
            }
        }

        if (_labelClassInformations.Count > 1)
            SetCurrentLabelClassID(_labelClassInformations.ElementAt(1).Key);
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

    public static void SetCurrentLabelClassID(uint label_inp)
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

    private static Material CreateNewMaterial(Color color_inp)
    {
        Material outMat = new Material(_standardMaterial);

        outMat.color = color_inp;

        return outMat;
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
