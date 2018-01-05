using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class SessionSave
{
	public List<PointCLoudSave> pointClouds{ get; private set; }

	public int iCurrentCLoud{ get; private set; }

	public SessionSave (LabelSession session_inp)
	{
		pointClouds = new List<PointCLoudSave> ();

		foreach (var pointloud in session_inp.pointClouds)
		{ 
			pointClouds.Add (new PointCLoudSave (pointloud));
		}
		iCurrentCLoud = session_inp.iCurrentCLoud;
	}

	public static LabelSession RecreateSession (SessionSave saveData_inp)
	{
		List<PointCLoud> RecreatedClouds = new List<PointCLoud> ();

		foreach (var cloud in saveData_inp.pointClouds)
		{
			RecreatedClouds.Add (new PointCLoud (cloud));
		}
		return new LabelSession (RecreatedClouds, saveData_inp.iCurrentCLoud);
	}
}

[Serializable]
public class PointCLoudSave
{
	public List<LabelPointSave> allPoints{ get; private set; }

	//	public List<LabelPointSave> UnlabeledPoints{ get; private set; }
	//
	//	public List<LabelPointSave> LabeledPoints{ get; private set; }

	public PointCLoudSave (PointCLoud cloud_inp)
	{
		allPoints = new List<LabelPointSave> ();
//		UnlabeledPoints = new List<LabelPointSave>();
//		LabeledPoints = new List<LabelPointSave>();

		foreach (var point in cloud_inp.allPoints)
		{
			allPoints.Add (new LabelPointSave (point));
		}

//		foreach (var point in Cloud_inp.UnlabeledPoints)
//		{
//			UnlabeledPoints.Add (new LabelPointSave(point));
//		}
//
//		foreach (var point in Cloud_inp.LabeledPoints)
//		{
//			LabeledPoints.Add (new LabelPointSave(point));
//		}
	}
}

[Serializable]
public class LabelPointSave
{
	public float x{ get; private set; }

	public float y{ get; private set; }

	public float z{ get; private set; }

	public Labeler.LabelGroup labelingClassGroup{ get; private set; }

	public int iID { get; private set; }

	public bool bIsLabeled { get; private set; }

	public LabelPointSave (LabelPoint_old Point_inp)
	{
		x = Point_inp._point.transform.position.x;
		y = Point_inp._point.transform.position.y;
		z = Point_inp._point.transform.position.z;

		labelingClassGroup = Point_inp._labelingClassGroup;

		iID = Point_inp._iID;

//		bIsLabeled = Point_inp.bIsLabeled;
	}
}