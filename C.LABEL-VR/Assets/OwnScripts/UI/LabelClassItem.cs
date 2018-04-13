using System;
using UnityEngine;
using UnityEngine.UI;

public class LabelClassItem : MonoBehaviour
{
    public Text className;
    public Text classID;
    public Image classColor;

    public string ClassName
    {
        get { return className.text; }
        set { className.text = value; }
    }

    public uint ClassID
    {
        get { return Convert.ToUInt32(classID.text); }
        set { classID.text = Convert.ToString(value); }
    }

    public Color ClassColor
    {
        get { return classColor.color; }
        set { classColor.color = value; }
    }
}
