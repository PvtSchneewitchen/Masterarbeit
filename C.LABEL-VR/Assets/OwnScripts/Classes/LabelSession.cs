using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Session
{
	public List<PointCloud> pointClouds{ get; set;}
	private int iCurrentCLoud{ get; set; }

	public Session (List<PointCloud> pointClouds_inp, int iCurrentCloud)
	{
		pointClouds = pointClouds_inp;
		iCurrentCLoud = iCurrentCloud;
	}

	//public Session (SessionSave saveData_inp)
	//{
	//	List<PointCloud> RecreatedClouds = new List<PointCloud>();

	//	foreach (var cloud in saveData_inp.pointClouds)
	//	{
	//		RecreatedClouds.Add (new PointCloud(cloud));
	//	}

	//	pointClouds = RecreatedClouds;
	//	iCurrentCLoud = saveData_inp.iCurrentCLoud;
	//}

	public PointCloud GetCurrentPointCloud ()
	{
		return pointClouds [iCurrentCLoud];
	}

	public PointCloud GetPointcloud (int iIndex_inp)
	{
		return pointClouds [iIndex_inp];
	}

	public PointCloud GetNextPointCloud ()
	{
		iCurrentCLoud++;
		if (iCurrentCLoud >= pointClouds.Count)
			iCurrentCLoud = 0;

		return pointClouds [iCurrentCLoud];
	}

	public PointCloud GetPreviousPointCloud ()
	{
		iCurrentCLoud--;
		if (iCurrentCLoud < 0)
			iCurrentCLoud = pointClouds.Count-1;

		return pointClouds [iCurrentCLoud];
	}
}
