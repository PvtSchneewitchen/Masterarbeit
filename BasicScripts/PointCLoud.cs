using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Runtime.InteropServices;
//using NUnit.Framework;

public class PointCLoud
{

	public List<LabelPoint> allPoints{ get; private set; }

//	public List<LabelPoint> UnlabeledPoints{ get; private set; }
//
//	public List<LabelPoint> LabeledPoints{ get; private set; }

	public PointCLoud (List<LabelPoint> points_inp)
	{
		allPoints = new List<LabelPoint> ();
		
		foreach (var labelpoint in points_inp)
		{
			allPoints.Add (labelpoint);
//
//			if (labelpoint.LabelingClassGroup == Labeler.LabelGroup.unlabeled)
//			{
//				UnlabeledPoints.Add (labelpoint);
//			}
//			else
//			{
//				LabeledPoints.Add (labelpoint);
//			}
		}
	}

	public PointCLoud (PointCLoudSave saveData_inp)
	{
		allPoints = new List<LabelPoint> ();

		foreach (var savepoint in saveData_inp.allPoints)
		{
			allPoints.Add (new LabelPoint (savepoint));
		}
//
//		foreach (var savepoint in SaveData_inp.UnlabeledPoints)
//		{
//			UnlabeledPoints.Add (new LabelPoint (savepoint));
//		}
//
//		foreach (var savepoint in SaveData_inp.LabeledPoints)
//		{
//			LabeledPoints.Add (new LabelPoint (savepoint));
//		}
	}

	public PointCLoud (List<Vector3> coordinates_inp)
	{
		allPoints = new List<LabelPoint> ();
		
		for (int i = 0; i < coordinates_inp.Count; i++)
		{
			LabelPoint lp = new LabelPoint (LabelPoint.CreateBlankLabelpointGameobject (), Labeler.LabelGroup.unlabeled, i, false);
			lp.SetPosition (coordinates_inp [i]);
			allPoints.Add (lp);
//			UnlabeledPoints.Add (lp);
		}
	}

	public PointCLoud ()
	{
		allPoints = new List<LabelPoint> ();
	}

//	private void Init ()
//	{
//		AllPoints = new List<LabelPoint> ();
//		UnlabeledPoints = new List<LabelPoint> ();
//		LabeledPoints = new List<LabelPoint> ();
//	}

	public void LabelTransform (Transform transform_inp, Labeler.LabelGroup classification_inp)
	{
		for (int i = 0; i < allPoints.Count; i++)
		{
			if (allPoints [i].point.transform.position == transform_inp.position)
			{
				if (allPoints [i].labelingClassGroup != classification_inp)
				{
					//AllPoints [i].bIsLabeled = true;
					allPoints [i].labelingClassGroup = classification_inp;
					allPoints [i].point.GetComponent <Renderer> ().material.color = Labeler.GetGroupColor (allPoints [i].labelingClassGroup);
				}
				break;
			}
		}
	}


	public void UnlabelTransform (Transform transform_inp)
	{
		Debug.Log("Unlabel try");
		for (int i = 0; i < allPoints.Count; i++)
		{
			if (allPoints [i].point.transform.position == transform_inp.position)
			{
				Debug.Log("Labeled found");
				//AllPoints [i].bIsLabeled = false;
				allPoints [i].labelingClassGroup = Labeler.LabelGroup.unlabeled;
				allPoints [i].point.GetComponent <Renderer> ().material.color = Labeler.GetGroupColor (allPoints [i].labelingClassGroup);
				break;
			}
		}
	}

	/// <summary>
	/// //TODO
	/// </summary>
	/// <param name="LPoints_inp">L points inp.</param>
	public void SetPointsAsLabeled (List<Transform> LPoints_inp)
	{
		throw new NotImplementedException ("SetPointsAsLabeled");
	}

	/// <summary>
	/// //TODO
	/// </summary>
	/// <param name="LPoint_inp">L point inp.</param>
	public void SetPointsAsUnlabeled (List<LabelPoint> LPoints_inp)
	{
		throw new NotImplementedException ("SetPointsAsUnlabeled");
	}

	public void EnableAllPoints ()
	{
		foreach (var Point in allPoints)
		{
			Point.point.SetActive (true);
		}
	}

	public void DisableAllPoints ()
	{
		foreach (var Point in allPoints)
		{
			Point.point.SetActive (false);
		}
	}

	public List<LabelPoint> GetLabeledPoints ()
	{
		List<LabelPoint> LabeledPoints = new  List<LabelPoint> ();
		foreach (var point in allPoints)
		{
			if (point.labelingClassGroup != Labeler.LabelGroup.unlabeled)
			{
				LabeledPoints.Add (point);
			}
		}
		return LabeledPoints;
	}

	public List<LabelPoint> GetUnlabeledPoints ()
	{
		List<LabelPoint> UnlabeledPoints = new  List<LabelPoint> ();
		foreach (var point in allPoints)
		{
			if (point.labelingClassGroup == Labeler.LabelGroup.unlabeled)
			{
				UnlabeledPoints.Add (point);
			}
		}
		return UnlabeledPoints;
	}

	public static List<PointCLoud> CreateListOfPointclouds (List<KeyValuePair<int, List<Vector3>>> coordinatesMultiFiles_inp)
	{
		List<PointCLoud> MultiFilesPointCloud = new List<PointCLoud> ();

		for (int i = 0; i < coordinatesMultiFiles_inp.Count; i++)
		{
			List<LabelPoint> LabelPointList = new List<LabelPoint> ();
			for (int j = 0; j < coordinatesMultiFiles_inp [i].Value.Count; j++)
			{
				LabelPoint Point = new LabelPoint (LabelPoint.CreateBlankLabelpointGameobject (), Labeler.LabelGroup.unlabeled, j, false);
				Point.SetPosition (coordinatesMultiFiles_inp [i].Value [j]);
				LabelPointList.Add (Point);
			}
			PointCLoud Cloud = new PointCLoud (LabelPointList); 
			MultiFilesPointCloud.Add (Cloud);
		}
		return MultiFilesPointCloud;
	}
}
