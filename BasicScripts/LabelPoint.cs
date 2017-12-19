using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LabelPoint
{
	public GameObject point {get;}
	public Labeler.LabelGroup labelingClassGroup{get; set;}
	public int iID {get;}
//	public bool bIsLabeled{get; set;}

	public LabelPoint (GameObject Point_inp, Labeler.LabelGroup Group_inp, int iId_inp, bool bIsLabeled_inp)
	{
		point = Point_inp;
		labelingClassGroup = Group_inp;
		iID = iId_inp;
//		bIsLabeled = bIsLabeled_inp;
	}

	public LabelPoint (LabelPointSave savedpoint_inp)
	{	
		GameObject RecreatedPoint = LabelPoint.CreateBlankLabelpointGameobject ();
		RecreatedPoint.GetComponent <Renderer>().material.color = Labeler.GetGroupColor (savedpoint_inp.labelingClassGroup);
		RecreatedPoint.transform.position = new Vector3(savedpoint_inp.x, savedpoint_inp.y, savedpoint_inp.z);

		point = RecreatedPoint;
		labelingClassGroup = savedpoint_inp.labelingClassGroup;
		iID = savedpoint_inp.iID;
//		bIsLabeled = Savedpoint_inp.bIsLabeled;
	}

	public void SetPosition (Vector3 Position_inp)
	{
		point.transform.position = Position_inp;
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

