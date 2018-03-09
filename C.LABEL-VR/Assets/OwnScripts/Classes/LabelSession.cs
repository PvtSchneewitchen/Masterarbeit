using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LabelSession
{
	public List<PointCloud> _pointClouds { get; set;}
	private int _currentCLoud{ get; set; }

	public LabelSession (List<PointCloud> pointClouds_inp, int iCurrentCloud)
	{
        _pointClouds = pointClouds_inp;
        _currentCLoud = iCurrentCloud;
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

    public int GetCurrentPointCloudIndex()
    {
        return _currentCLoud;
    }

    public PointCloud GetCurrentPointCloud ()
	{
        return _pointClouds.ElementAt(_currentCLoud);
	}

	public PointCloud GetPointcloud (int index_inp)
	{
        return _pointClouds.ElementAt(index_inp);
    }

	public PointCloud GetNextPointCloud ()
	{
        _currentCLoud++;
		if (_currentCLoud >= _pointClouds.Count)
            _currentCLoud = 0;

		return _pointClouds.ElementAt(_currentCLoud);
	}

	public PointCloud GetPreviousPointCloud ()
	{
        _currentCLoud--;
        if (_currentCLoud >= _pointClouds.Count)
            _currentCLoud = 0;

        return _pointClouds.ElementAt(_currentCLoud);
    }
}
