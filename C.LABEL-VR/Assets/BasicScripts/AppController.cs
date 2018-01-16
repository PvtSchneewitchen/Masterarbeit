using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using UnityEngine.Experimental.UIElements;
using System.Runtime.Remoting.Messaging;
//using UnityEditor;

public class AppController : MonoBehaviour
{

	//public static AppController StaticReference;

	public LabelSession session{ get; private set; }

	public PointCLoud currentCLoud{ get; private set; }

	public Labeler sessionLabeler{ get; set; }

	public bool bMultipleFiles = false;
    private bool _bOptionMode;
    //
    Text text_FileNumber;
	float fClickTime;
	bool bHoldDown = false;
	GameObject selectionBox;
	Vector3 startMousePosition;
	//

	void Start ()
	{
		session = new LabelSession ();
		currentCLoud = new PointCLoud ();
		sessionLabeler = new Labeler ();

        _bOptionMode = false;

        List<KeyValuePair<int, List<Vector3>>> Coordinates = new List<KeyValuePair<int, List<Vector3>>>();
        if (bMultipleFiles)
        {
            if (Application.isEditor)
            {
                Coordinates = PcdReader.GetCoordinatesFromMultiPcd("C:\\Users\\gruepazu\\Desktop\\Masterarbeit\\C.LABEL-VR\\App\\PointClouds");
            }
            else
            {
                string sFolderToCut = "/C.LABEL-VR_Data/";
                char[] cCharsToCut = sFolderToCut.ToCharArray();

                Coordinates = PcdReader.GetCoordinatesFromMultiPcd(Application.dataPath.TrimEnd(cCharsToCut) + "/PointClouds/");
            }

        }
        else
        {
            Coordinates = PcdReader.GetCoordinatesFromSinglePcd("C:\\Users\\gruepazu\\Desktop\\Masterarbeit\\C.LABEL-VR\\App\\PointClouds\\000000000_LidarImage_000000002.pcd");
        }


        session = new LabelSession (PointCLoud.CreateListOfPointclouds (Coordinates));
		currentCLoud = session.pointClouds [0];
		currentCLoud.EnableAllPoints ();


		//
		//ShowFrameNumber ();

		selectionBox = GameObject.CreatePrimitive (PrimitiveType.Plane);
		selectionBox.GetComponent <Renderer> ().material.color = new Color (1.0f, 0.0f, 0.0f, 1.0f);
		selectionBox.SetActive (false);
		//


	}

	void Update ()
	{
		
		if (Input.GetKeyDown (KeyCode.RightArrow))
		{
			SwitchToNextPointcloud ();
			//ShowFrameNumber ();
		}
		else if (Input.GetKeyDown (KeyCode.LeftArrow))
		{
			SwitchToPreviousPointcloud ();
			//ShowFrameNumber ();
		}

		if (Input.GetMouseButtonDown ((int)MouseButton.LeftMouse))
		{
			bHoldDown = true;
			startMousePosition = Input.mousePosition;
		}
		if (Input.GetMouseButtonUp ((int)MouseButton.LeftMouse))
		{
			bHoldDown = false;
		}
		if (bHoldDown)
		{
			sessionLabeler.TagObjectOnMouseClick (); 
//			foreach (LabelPoint labelPoint in currentCLoud.allPoints)
//			{
//				if (IsWithinSelectionBox (labelPoint.point))
//				{
//					currentCLoud.LabelTransform (labelPoint.point.transform, sessionLabeler.currentGroup);
//				}
//			}
		}

        if (!_bOptionMode)
        {
            if (OVRInput.GetDown(OVRInput.Button.Start))
            {
                GameObject.Find("InGameOptions").GetComponent<InGameOptionsController>().EnableOptionMenu();
                _bOptionMode = true;
            }
        }
        else
        {
            if (OVRInput.GetDown(OVRInput.Button.Start))
            {
                GameObject.Find("InGameOptions").GetComponent<InGameOptionsController>().DisableOptionMenu();
                _bOptionMode = false;
            }
        }
    }

//	void OnGUI ()
//	{
//		if (bHoldDown)
//		{
//			var rect = Labeler.SelectionBox.GetScreenRect (startMousePosition, Input.mousePosition);
//			Labeler.SelectionBox.DrawScreenRectBordered (rect, new Color(0.8f, 0.8f, 0.95f));
//		}
//	}

	public bool IsWithinSelectionBox (GameObject gameObject_inp)
		{
			if(!bHoldDown)
				return false;

			var viewPortBounds = Labeler.SelectionBox.GetViewportBounds (startMousePosition, Input.mousePosition);

			return  viewPortBounds.Contains (Camera.main.WorldToViewportPoint (gameObject_inp.transform.position));
		}


	//testzwecke//
	//public void ShowFrameNumber ()
	//{
	//	GameObject.Find ("UIController").GetComponent <UIController> ().text_Filenumber.text = "FrameNumber: " + session.iCurrentCLoud;
	//}

	/// ////////////////////////////////////////

	public void LabelTransform (Transform inp, Labeler.LabelGroup group)
	{
		currentCLoud.LabelTransform (inp, group);
	}


	public void Save ()
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/exampleSave.dat");
		SessionSave sessionToSave = new SessionSave (session);

		binaryFormatter.Serialize (file, sessionToSave);
		file.Close ();
		Debug.Log ("Session Saved");
	}

	public void Load ()
	{
		if (File.Exists (Application.persistentDataPath + "/exampleSave.dat"))
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/exampleSave.dat", FileMode.Open);

			SessionSave RecreatedSession = (SessionSave)binaryFormatter.Deserialize (file);
			file.Close ();

			session = new LabelSession (RecreatedSession);

			SwitchToPointcloud (session.iCurrentCLoud);

			Debug.Log ("Session Loaded");
		}
		else
		{
			Debug.Log ("No Save Data!"); 
		}
	}

	public void SwitchToNextPointcloud ()
	{
		currentCLoud.DisableAllPoints ();
		currentCLoud = session.GetNextPointcloud ();
		currentCLoud.EnableAllPoints ();
	}

	public void SwitchToPreviousPointcloud ()
	{
		currentCLoud.DisableAllPoints ();
		currentCLoud = session.GetPreviousPointcloud ();
		currentCLoud.EnableAllPoints ();
	}

	public void SwitchToPointcloud (int iIndex_inp)
	{
		currentCLoud.DisableAllPoints ();
		currentCLoud = session.GetPointcloud (iIndex_inp);
		currentCLoud.EnableAllPoints ();
	}

}
