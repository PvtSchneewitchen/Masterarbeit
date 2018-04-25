using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Labeling
{
    public static uint currentLabelClassID { get; private set; }

    private static Dictionary<uint, Tuple<string, Material>> labelClassInformations;
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

        labelClassInformations = new Dictionary<uint, Tuple<string, Material>>
            {
                {0 , new Tuple<string, Material>("unlabeled" ,unlabeledMaterial) }
            };

        _standardColors = new List<Color32>
        {
            new Color32(210, 240, 225, 225),
            //new Color32(255,15,15,255),
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
        SetCurrentLabelClassID(labelClassInformations.ElementAt(1).Key);
    }

    public static void SwitchToNextLabelClass()
    {
        List<string> nameList = new List<string>();
        for (int i = 0; i < labelClassInformations.Count; i++)
        {
            nameList.Add(labelClassInformations.ElementAt(i).Value.Item1);
        }
        nameList.Sort();

        var currentClassName = labelClassInformations[currentLabelClassID].Item1;
        var currentIndex = nameList.IndexOf(currentClassName);

        if (currentIndex <= 0)
        {
            currentLabelClassID = labelClassInformations.First(entry => entry.Value.Item1 == nameList.Last()).Key;
        }
        else
        {
            currentLabelClassID = labelClassInformations.First(entry => entry.Value.Item1 == nameList[currentIndex - 1]).Key;
        }
    }

    public static void SwitchToPreviousLabelClass()
    {
        List<string> nameList = new List<string>();
        for (int i = 0; i < labelClassInformations.Count; i++)
        {
            nameList.Add(labelClassInformations.ElementAt(i).Value.Item1);
        }
        nameList.Sort();

        var currentClassName = labelClassInformations[currentLabelClassID].Item1;
        var currentIndex = nameList.IndexOf(currentClassName);

        if (currentIndex + 1 >= nameList.Count)
        {
            currentLabelClassID = labelClassInformations.First(entry => entry.Value.Item1 == nameList.First()).Key;
        }
        else
        {
            currentLabelClassID = labelClassInformations.First(entry => entry.Value.Item1 == nameList[currentIndex + 1]).Key;
        }
    }

    public static void EditSingleLabelClass(uint oldID_inp, uint newID_inp, string newName_inp, Color newColor_inp)
    {
        if (oldID_inp != 0 && labelClassInformations.ContainsKey(oldID_inp))
        {
            labelClassInformations.Remove(oldID_inp);

            labelClassInformations.Add(newID_inp, new Tuple<string, Material>(newName_inp, CreateNewMaterial(newColor_inp)));
        }
    }

    public static string GetLabelClassName(uint ID_inp)
    {
        Tuple<string, Material> info;

        if (labelClassInformations.TryGetValue(ID_inp, out info))
        {
            return info.Item1;
        }
        else
        {
            Debug.Log("GetLabelClassName: No such key" + ID_inp);
            return null;
        }
    }

    public static Dictionary<uint, Tuple<string, SessionSave.SerializableColor>> GetAllIdsNamesAndSerializedColors()
    {
        Dictionary<uint, Tuple<string, SessionSave.SerializableColor>> infos_out = new Dictionary<uint, Tuple<string, SessionSave.SerializableColor>>();

        foreach (var item in labelClassInformations)
        {
            Color c = new Color(item.Value.Item2.color.r, item.Value.Item2.color.g, item.Value.Item2.color.b, item.Value.Item2.color.a);
            infos_out.Add(item.Key, new Tuple<string, SessionSave.SerializableColor>(item.Value.Item1, new SessionSave.SerializableColor(c.r, c.g, c.b, c.a)));
        }

        return infos_out;
    }

    public static Dictionary<uint, Tuple<string, Material>> GetAllIdsNamesAndMaterials()
    {
        return labelClassInformations;
    }

    public static Tuple<string, Material> GetLabelClassNameAndMaterial(uint ID_inp)
    {
        Tuple<string, Material> info;

        if (labelClassInformations.TryGetValue(ID_inp, out info))
        {
            return info;
        }
        else
        {
            Debug.Log("GetLabelClassName: No such key" + ID_inp);
            return null;
        }
    }

    public static Dictionary<uint, string> GetAllIdsAndNames()
    {
        Dictionary<uint, string> lws = new Dictionary<uint, string>();

        for (int i = 0; i < labelClassInformations.Count; i++)
        {
            uint id = labelClassInformations.ElementAt(i).Key;
            string name = labelClassInformations.ElementAt(i).Value.Item1;
            lws.Add(id, name);
        }

        return lws;
    }

    public static void AddSingleLabelClass(uint id_inp, string name_inp, Color color_inp)
    {
        if (!labelClassInformations.ContainsKey(id_inp))
        {
            Material mat = new Material(_standardMaterial)
            {
                color = color_inp
            };

            labelClassInformations.Add(id_inp, new Tuple<string, Material>(name_inp, mat));
        }
    }

    public static void SetSavedLabelClasses(Dictionary<uint, Tuple<string, SessionSave.SerializableColor>> labelWorkingSet_inp)
    {
        labelClassInformations.Clear();

        foreach (var entry in labelWorkingSet_inp)
        {
            Color c = new Color(entry.Value.Item2.R, entry.Value.Item2.G, entry.Value.Item2.B, entry.Value.Item2.A);
            Material m = CreateNewMaterial(c);
            labelClassInformations.Add(entry.Key, new Tuple<string, Material>(entry.Value.Item1, m));
        }

        if (labelClassInformations.Count > 1)
            SetCurrentLabelClassID(labelClassInformations.ElementAt(1).Key);
    }

    public static void SetNewLabelClasses(Dictionary<uint, string> labelWorkingSet_inp)
    {
        if (labelClassInformations.ContainsKey(1))
        {
            Tuple<string, Material> info;
            labelClassInformations.TryGetValue(1, out info);

            if (info.Item1 == _dummyClassName)
                labelClassInformations.Remove(1);
        }

        for (int i = 0; i < labelWorkingSet_inp.Count; i++)
        {
            uint key = labelWorkingSet_inp.ElementAt(i).Key;
            string value = labelWorkingSet_inp.ElementAt(i).Value;
            if (value != "unlabeled" && key != 0 && !labelClassInformations.ContainsKey(key))
            {
                try
                {
                    labelClassInformations.Add(key, new Tuple<string, Material>(value, CreateNewRandomMaterial()));
                }
                catch
                {
                    Debug.Log("SetNewLabelClasses(): Key probabably added yet");
                }
            }
        }

        if (labelClassInformations.Count > 1)
            SetCurrentLabelClassID(labelClassInformations.ElementAt(1).Key);
    }

    public static Material GetLabelClassMaterial(uint ID_inp)
    {
        Tuple<string, Material> info;

        if (labelClassInformations.TryGetValue(ID_inp, out info))
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
        if (labelClassInformations.ContainsKey(label_inp))
        {
            currentLabelClassID = label_inp;
            try
            {
                LabelClassDisplayUpdate.Instance.UpdatePointerDisplay();
            }
            catch
            {

                Debug.Log("LabelClassDisplayUpdate not ready");
            }
        }
        else
        {
            Debug.Log("LabelClass with ID " + label_inp + " does not exist!");
        }
    }

    public static Color32 GetLabelClassColor(uint ID_inp)
    {
        Tuple<string, Material> info;

        if (labelClassInformations.TryGetValue(ID_inp, out info))
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
        Material outMat = new Material(_standardMaterial)
        {
            color = color_inp
        };

        return outMat;
    }

    private static Material CreateNewRandomMaterial()
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
