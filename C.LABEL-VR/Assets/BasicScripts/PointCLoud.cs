using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Runtime.InteropServices;
//using NUnit.Framework;

public class PointCLoud
{

	public List<LabelPoint_old> allPoints{ get; private set; }

//	public List<LabelPoint> UnlabeledPoints{ get; private set; }
//
//	public List<LabelPoint> LabeledPoints{ get; private set; }

	public PointCLoud (List<LabelPoint_old> points_inp)
	{
		allPoints = new List<LabelPoint_old> ();
		
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
		allPoints = new List<LabelPoint_old> ();

		foreach (var savepoint in saveData_inp.allPoints)
		{
			allPoints.Add (new LabelPoint_old (savepoint));
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
		allPoints = new List<LabelPoint_old> ();
		
		for (int i = 0; i < coordinates_inp.Count; i++)
		{
			LabelPoint_old lp = new LabelPoint_old (LabelPoint_old.CreateBlankLabelpointGameobject (), Labeler.LabelGroup.unlabeled, i, false);
			lp.SetPosition (coordinates_inp [i]);
			allPoints.Add (lp);
//			UnlabeledPoints.Add (lp);
		}
	}

	public PointCLoud ()
	{
		allPoints = new List<LabelPoint_old> ();
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
			if (allPoints [i]._point.transform.position == transform_inp.position)
			{
				if (allPoints [i]._labelingClassGroup != classification_inp)
				{
					//AllPoints [i].bIsLabeled = true;
					allPoints [i]._labelingClassGroup = classification_inp;
					allPoints [i]._point.GetComponent <Renderer> ().material.color = Labeler.GetGroupColor (allPoints [i]._labelingClassGroup);
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
			if (allPoints [i]._point.transform.position == transform_inp.position)
			{
				Debug.Log("Labeled found");
				//AllPoints [i].bIsLabeled = false;
				allPoints [i]._labelingClassGroup = Labeler.LabelGroup.unlabeled;
				allPoints [i]._point.GetComponent <Renderer> ().material.color = Labeler.GetGroupColor (allPoints [i]._labelingClassGroup);
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
	public void SetPointsAsUnlabeled (List<LabelPoint_old> LPoints_inp)
	{
		throw new NotImplementedException ("SetPointsAsUnlabeled");
	}

	public void EnableAllPoints ()
	{
		foreach (var Point in allPoints)
		{
			Point._point.SetActive (true);
		}
	}

	public void DisableAllPoints ()
	{
		foreach (var Point in allPoints)
		{
			Point._point.SetActive (false);
		}
	}

	public List<LabelPoint_old> GetLabeledPoints ()
	{
		List<LabelPoint_old> LabeledPoints = new  List<LabelPoint_old> ();
		foreach (var point in allPoints)
		{
			if (point._labelingClassGroup != Labeler.LabelGroup.unlabeled)
			{
				LabeledPoints.Add (point);
			}
		}
		return LabeledPoints;
	}

	public List<LabelPoint_old> GetUnlabeledPoints ()
	{
		List<LabelPoint_old> UnlabeledPoints = new  List<LabelPoint_old> ();
		foreach (var point in allPoints)
		{
			if (point._labelingClassGroup == Labeler.LabelGroup.unlabeled)
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
			List<LabelPoint_old> LabelPointList = new List<LabelPoint_old> ();
			for (int j = 0; j < coordinatesMultiFiles_inp [i].Value.Count; j++)
			{
				LabelPoint_old Point = new LabelPoint_old (LabelPoint_old.CreateBlankLabelpointGameobject (), Labeler.LabelGroup.unlabeled, j, false);
				Point.SetPosition (coordinatesMultiFiles_inp [i].Value [j]);
				LabelPointList.Add (Point);
			}
			PointCLoud Cloud = new PointCLoud (LabelPointList); 
			MultiFilesPointCloud.Add (Cloud);
		}
		return MultiFilesPointCloud;
	}
}
