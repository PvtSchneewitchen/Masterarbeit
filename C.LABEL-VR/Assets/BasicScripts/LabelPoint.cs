using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LabelPoint_old
{
	public GameObject _point {get;}
	public Labeler.LabelGroup _labelingClassGroup{get; set;}
	public int _iID {get;}
//	public bool bIsLabeled{get; set;}

	public LabelPoint_old (GameObject Point_inp, Labeler.LabelGroup Group_inp, int iId_inp, bool bIsLabeled_inp)
	{
		_point = Point_inp;
		_labelingClassGroup = Group_inp;
		_iID = iId_inp;
//		bIsLabeled = bIsLabeled_inp;
	}

	public LabelPoint_old (LabelPointSave savedpoint_inp)
	{	
		GameObject recreatedPoint = LabelPoint_old.CreateBlankLabelpointGameobject ();
		recreatedPoint.GetComponent <Renderer>().material.color = Labeler.GetGroupColor (savedpoint_inp.labelingClassGroup);
		recreatedPoint.transform.position = new Vector3(savedpoint_inp.x, savedpoint_inp.y, savedpoint_inp.z);

		_point = recreatedPoint;
		_labelingClassGroup = savedpoint_inp.labelingClassGroup;
		_iID = savedpoint_inp.iID;
//		bIsLabeled = Savedpoint_inp.bIsLabeled;
	}

	public void SetPosition (Vector3 Position_inp)
	{
		_point.transform.position = Position_inp;
	}

	public static GameObject CreateBlankLabelpointGameobject()
	{
		float scale = 0.1f;

		GameObject go = new GameObject("LabelPoint");
		go = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		go.transform.localScale = new Vector3(scale, scale, scale);
		go.GetComponent <Renderer>().material.color = Labeler.LabelColor.unlabeled;
		go.GetComponent <Renderer>().material.EnableKeyword ("_SPECULARHIGHLIGHTS_OFF");
		go.GetComponent <Renderer>().material.SetFloat ("_SpecularHighlights",0f);
		go.SetActive (false);

		return go;
	}

	public static GameObject CreateBlankLabelpointGameobject(Vector3 position_inp)
	{
		float scale = 0.1f;
		GameObject go = new GameObject("LabelPoint");
		go = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		go.transform.localScale = new Vector3(scale, scale, scale);
		go.transform.position = position_inp;
		go.GetComponent <Renderer>().material.color = Labeler.LabelColor.unlabeled;
		go.GetComponent <Renderer>().material.EnableKeyword ("_SPECULARHIGHLIGHTS_OFF");
		go.GetComponent <Renderer>().material.SetFloat ("_SpecularHighlights",0f);
		go.SetActive (false);

		return go;
	}
}

