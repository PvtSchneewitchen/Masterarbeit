using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class PointAttributes : MonoBehaviour
{
    public Util.Labeling.LabelGroup _group { get; private set; }

    public Util.Labeling.LabelGroup GetGroup()
    {
        return _group;
    }

    public void ChangeGroup(Util.Labeling.LabelGroup group_inp)
    {
        _group = group_inp;
        GetComponent<MeshRenderer>().material = Util.Labeling.GetGroupMaterial(_group);
        //GetComponent<MeshRenderer>().material.color = Util.Labeling.GetGroupColor(_group);
        //GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Util.Labeling.GetGroupColor(_group));
    }
}
