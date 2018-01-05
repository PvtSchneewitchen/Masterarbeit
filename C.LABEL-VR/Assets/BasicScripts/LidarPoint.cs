using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LidarPoint
{
	//: MonoBehaviour

	public float x;
	public float y;
	public float z;

	public LidarPoint (string xInp, string yInp, string zInp)
	{
		if (!float.TryParse (xInp, out x))
			Debug.Log ("Lidar x Parsing Error!\t" + x);
		if (!float.TryParse (yInp, out y))
			Debug.Log ("Lidar y Parsing Error!\t" + y);
		if (!float.TryParse (zInp, out z))
			Debug.Log ("Lidar z Parsing Error!\t" + z);

		x *= -1;
	}
}
