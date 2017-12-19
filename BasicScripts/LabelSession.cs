using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LabelSession
{
	public List<PointCLoud> pointClouds{ get; set;}

	public int iCurrentCLoud{ get; set; }

	public LabelSession (List<PointCLoud> pointClouds_inp)
	{
		pointClouds = pointClouds_inp;
		iCurrentCLoud = 0;
	}

	public LabelSession (List<PointCLoud> pointClouds_inp, int iCurrentCloud)
	{
		pointClouds = pointClouds_inp;
		iCurrentCLoud = iCurrentCloud;
	}

	public LabelSession (SessionSave saveData_inp)
	{
		List<PointCLoud> RecreatedClouds = new List<PointCLoud>();

		foreach (var cloud in saveData_inp.pointClouds)
		{
			RecreatedClouds.Add (new PointCLoud(cloud));
		}

		pointClouds = RecreatedClouds;
		iCurrentCLoud = saveData_inp.iCurrentCLoud;
	}

	public LabelSession ()
	{
		pointClouds = new List<PointCLoud> ();
		iCurrentCLoud = 0;
	}

	public PointCLoud GetCurrentPointcloud ()
	{
		return pointClouds [iCurrentCLoud];
	}

	public PointCLoud GetPointcloud (int iIndex_inp)
	{
		return pointClouds [iIndex_inp];
	}

	public PointCLoud GetNextPointcloud ()
	{
		iCurrentCLoud++;
		if (iCurrentCLoud >= pointClouds.Count)
			iCurrentCLoud = 0;

		return pointClouds [iCurrentCLoud];
	}

	public PointCLoud GetPreviousPointcloud ()
	{
		iCurrentCLoud--;
		if (iCurrentCLoud < 0)
			iCurrentCLoud = pointClouds.Count-1;

		return pointClouds [iCurrentCLoud];
	}
}
