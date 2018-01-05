using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class PcdReader
{
	public static List<KeyValuePair<int, List<Vector3>>> GetCoordinatesFromSinglePcd (string sPath_inp)
	{
		List<KeyValuePair<int, List<Vector3>>> CoordinatesSingleFile = new List<KeyValuePair<int, List<Vector3>>> ();
		List<Vector3> Coordinates = new List<Vector3>(); 
		bool firstLineSkipped = false;
		var pcdReader = new StreamReader (sPath_inp);

		while (!pcdReader.EndOfStream)
		{
			var line = pcdReader.ReadLine ();
			if (firstLineSkipped)
			{
				var values = line.Split (' ');

				//TODO add tryparse
				float x = float.Parse (values [0]);
				float y = float.Parse (values [1]);
				float z = float.Parse (values [2]);

				//pcd data is not compatible with unity coordinate system
				//the pointcloud is displayed vertical and is mirrored
				//so one has to totate it around the x-axis by -90° and negate the x-coordinate
				Coordinates.Add (Quaternion.Euler (-90, 0, 0) * new Vector3 (-x, y, z));
			}
			else
			{
				firstLineSkipped = true;
			}
		}
		CoordinatesSingleFile.Add (new KeyValuePair<int, List<Vector3>>(0, Coordinates));
		return CoordinatesSingleFile;
	}


	public static List<KeyValuePair<int, List<Vector3>>> GetCoordinatesFromMultiPcd (string sDirectory_inp)
	{
		List<KeyValuePair<int, List<Vector3>>> CoordinatesMultiFiles = new List<KeyValuePair<int, List<Vector3>>>();
		string[] pcdFiles = Directory.GetFiles (sDirectory_inp, "*.pcd");

		for (int i = 0; i < pcdFiles.Length; i++)
		{
			List<Vector3> CoordinatesSingleFile = new List<Vector3> ();
			bool firstLineSkipped = false;
			var pcdReader = new StreamReader (pcdFiles[i]);

			while (!pcdReader.EndOfStream)
			{
				var line = pcdReader.ReadLine ();
				if (firstLineSkipped)
				{
					var values = line.Split (' ');

					//TODO add tryparse
					float x = float.Parse (values [0]);
					float y = float.Parse (values [1]);
					float z = float.Parse (values [2]);

					//pcd data is not compatible with unity coordinate system
					//the pointcloud is displayed vertical and is mirrored
					//so one has to totate it around the x-axis by -90° and negate the x-coordinate
					CoordinatesSingleFile.Add (Quaternion.Euler (-90, 0, 0) * new Vector3 (-x, y, z));
				}
				else
				{
					firstLineSkipped = true;
				}
			}
			CoordinatesMultiFiles.Add (new KeyValuePair<int, List<Vector3>>(i, CoordinatesSingleFile));
		}
		return CoordinatesMultiFiles;
	}
}
