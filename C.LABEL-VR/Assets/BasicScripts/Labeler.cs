using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net;

public class Labeler
{
	//public static Labeler StaticReference;

	public LabelGroup currentGroup{ get; set; }

	public Labeler ()
	{
		//Classification = LabelGroup.unlabeled;

		currentGroup = LabelGroup.bicycle;
	}

	public void TagObjectOnMouseClick ()
	{
//		RaycastHit ClickHitInfo = new RaycastHit ();
//		bool bIsHit = Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out ClickHitInfo);

		RaycastHit[] hittedObjects = Physics.RaycastAll (Camera.main.ScreenPointToRay (Input.mousePosition));

		if (hittedObjects.Length > 0)
		{
			foreach (var Object in hittedObjects)
			{
				if(Object.distance - hittedObjects[0].distance < 0.5f)
					LabelGameobject (Object.transform);
			}
		}
	}



	private void LabelGameobject (Transform transform_inp)
	{
		if (currentGroup != LabelGroup.unlabeled)
		{
			GameObject.Find ("AppController").GetComponent <AppController>().currentCLoud.LabelTransform (transform_inp, currentGroup);
		}
		else
		{
			GameObject.Find ("AppController").GetComponent <AppController>().currentCLoud.UnlabelTransform (transform_inp);
		}
	}

	public static Color GetGroupColor (LabelGroup group_inp)
	{
		if (group_inp == LabelGroup.unlabeled)
			return LabelColor.unlabeled;
		else if (group_inp == LabelGroup.bicycle)
			return LabelColor.bicycle;
		else if (group_inp == LabelGroup.building)
			return LabelColor.building;
		else if (group_inp == LabelGroup.bus)
			return LabelColor.bus;
		else if (group_inp == LabelGroup.car)
			return LabelColor.car;
		else if (group_inp == LabelGroup.fence)
			return LabelColor.fence;
		else if (group_inp == LabelGroup.motorcycle)
			return LabelColor.motorcycle;
		else if (group_inp == LabelGroup.person)
			return LabelColor.person;
		else if (group_inp == LabelGroup.pole)
			return LabelColor.pole;
		else if (group_inp == LabelGroup.rider)
			return LabelColor.rider;
		else if (group_inp == LabelGroup.road)
			return LabelColor.road;
		else if (group_inp == LabelGroup.sidewalk)
			return LabelColor.sidewalk;
		else if (group_inp == LabelGroup.sky)
			return LabelColor.sky;
		else if (group_inp == LabelGroup.terrain)
			return LabelColor.terrain;
		else if (group_inp == LabelGroup.trafficLight)
			return LabelColor.trafficLight;
		else if (group_inp == LabelGroup.trafficSign)
			return LabelColor.trafficSign;
		else if (group_inp == LabelGroup.train)
			return LabelColor.train;
		else if (group_inp == LabelGroup.truck)
			return LabelColor.truck;
		else if (group_inp == LabelGroup.Void)
			return LabelColor.Void;
		else if (group_inp == LabelGroup.vegetation)
			return LabelColor.vegetation;
		else if (group_inp == LabelGroup.wall)
			return LabelColor.wall;
		else
		{
			Debug.Log ("LabelColors.GetGroupColor: Wrong Labelgroup passed. Color.Unlabeled returned");
			return LabelColor.unlabeled;
		}
	}

	public enum LabelGroup
	{
		bicycle,
		building,
		bus,
		car,
		fence,
		motorcycle,
		person,
		pole,
		rider,
		road,
		sidewalk,
		sky,
		terrain,
		trafficLight,
		trafficSign,
		train,
		truck,
		vegetation,
		Void,
		wall,
		unlabeled
	}

//	public struct LabelGroupStrings
//	{
//		public static readonly string bicycle = "bicycle";
//		public static readonly string building = "building";
//		public static readonly string bus = "bus";
//		public static readonly string car = "car";
//		public static readonly string fence = "fence";
//		public static readonly string motorcycle = "motorcycle";
//		public static readonly string person = "person";
//		public static readonly string pole = "pole";
//		public static readonly string rider = "rider";
//		public static readonly string road = "road";
//		public static readonly string sidewalk = "sidewalk";
//		public static readonly string sky = "sky";
//		public static readonly string terrain = "terrain";
//		public static readonly string trafficLight = "trafficLight";
//		public static readonly string trafficSign = "trafficSign";
//		public static readonly string train = "train";
//		public static readonly string truck = "truck";
//		public static readonly string vegetation = "vegetation";
//		public static readonly string Void = "Void";
//		public static readonly string wall = "wall";
//		public static readonly string unlabeled = "unlabeled";
//	}

	public struct LabelColor
	{
 		public static readonly Color bicycle = Color.red;
 		public static readonly Color building = Color.green;
 		public static readonly Color bus = Color.blue;
 		public static readonly Color car = Color.yellow;
 		public static readonly Color fence = Color.magenta;
 		public static readonly Color motorcycle = Color.cyan;
 		public static readonly Color person = new Color32 (255, 100, 0, 1);//orange
 		public static readonly Color pole = new Color32 (120, 120, 200, 1);//pale blue
 		public static readonly Color rider = new Color32 (100, 255, 100, 1);//pale green
 		public static readonly Color road = new Color32 (210, 240, 225, 1);//pale cyan
 		public static readonly Color sidewalk = new Color32 (190, 135, 195, 1);//pale magenta
		public static readonly Color sky = new Color32 (200, 200, 200, 1);//light grey
		public static readonly Color terrain = new Color32 (170, 90, 50, 1);//brown
		public static readonly Color trafficLight = new Color32 (40, 140, 50, 1);//dark green
		public static readonly Color trafficSign = new Color32 (30, 20, 130, 1);//dark blue
		public static readonly Color train = new Color32 (200, 20, 135, 1);//dark magenta
		public static readonly Color truck = new Color32 (90, 130, 140, 1);//pale dark green
		public static readonly Color Void = new Color32 (10, 150, 150, 1);//dark cyan
		public static readonly Color vegetation = new Color32 (120, 120, 5, 1);//pale brown/green
		public static readonly Color wall = new Color32 (150, 15, 150, 1);//very dark magenta
 		public static readonly Color unlabeled = Color.black;
	}

	public static class SelectionBox
	{
		static Texture2D whiteTexture;
		public static Texture2D WhiteTexture {
			get 
			{
				if (whiteTexture == null)
				{
					whiteTexture = new Texture2D(1,1);
					whiteTexture.SetPixel (0,0,Color.white);
					whiteTexture.Apply ();
				}
				return whiteTexture;
			}
		}

		public static void DrawScreenRect (Rect rect_inp, Color color_inp)
		{
			GUI.color = color_inp;
			GUI.DrawTexture (rect_inp, WhiteTexture);
			GUI.color = Color.white;
		}

		public static void DrawScreenRectBordered (Rect rect_inp, Color color_inp)
		{
			float fThicknes = 2;
			//Top
			DrawScreenRect (new Rect(rect_inp.xMin, rect_inp.yMin, rect_inp.width, fThicknes), color_inp);
			//Left
			DrawScreenRect (new Rect(rect_inp.xMin, rect_inp.yMin, fThicknes, rect_inp.height), color_inp);
			//Right
			DrawScreenRect (new Rect(rect_inp.xMax - fThicknes, rect_inp.yMin, fThicknes, rect_inp.height), color_inp);
			//Botom
			DrawScreenRect (new Rect(rect_inp.xMin, rect_inp.yMax - fThicknes, rect_inp.width, fThicknes), color_inp);
			//Fill
			DrawScreenRect (rect_inp, new Color(color_inp.r, color_inp.g, color_inp.b, 0.25f));
		}

		public static Rect GetScreenRect(Vector3 screenPosition1_inp, Vector3 screenPosition2_inp)
		{
			//Convert position to upper left origin
			screenPosition1_inp.y = Screen.height - screenPosition1_inp.y;
			screenPosition2_inp.y = Screen.height - screenPosition2_inp.y;

			//Calculate corners
			 var topLeft = Vector3.Min (screenPosition1_inp, screenPosition2_inp);
			 var bottomRight = Vector3.Max (screenPosition1_inp, screenPosition2_inp);

			 //create rect
			 return Rect.MinMaxRect (topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
		}

		public static Bounds GetViewportBounds (Vector3 screenPosition1_inp, Vector3 screenPosition2_inp)
		{
			var v1 = Camera.main.ScreenToViewportPoint (screenPosition1_inp);
			var v2 = Camera.main.ScreenToViewportPoint (screenPosition2_inp);
			var min = Vector3.Min (v1,v2);
			var max = Vector3.Max (v1,v2);
			min.z = Camera.main.nearClipPlane;
			max.z = Camera.main.farClipPlane;

			var bounds = new Bounds();
			bounds.SetMinMax (min, max);

			return bounds;
		}
	} 
}

