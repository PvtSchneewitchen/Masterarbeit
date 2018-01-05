using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
	public Text text_Filenumber{get; set;}
	public Dropdown dropdown_LabelGroup{get; private set;}
	public Button button_Save;
	public Button button_Load;
	public Button button_Quit;

	List<string> dropdown_LabelGroupList = new List<string>();

	void Awake ()
	{
		text_Filenumber = GameObject.Find ("Text_FileNumber").GetComponent <Text>();
		dropdown_LabelGroup = GameObject.Find ("Dropdown_LabelGroup").GetComponent <Dropdown>();
		button_Save = GameObject.Find ("Button_Save").GetComponent <Button>();
		button_Load = GameObject.Find ("Button_Load").GetComponent <Button>();
		button_Quit = GameObject.Find ("Button_Quit").GetComponent <Button>();


		CreateDropdownList();

		dropdown_LabelGroup.onValueChanged.AddListener (DropdownLabelGroupIndexChanged);
		button_Save.onClick.AddListener (OnCLickSave);
		button_Load.onClick.AddListener (OnClickLoad);
		button_Quit.onClick.AddListener (OnClickQuit);
 	}


	public void OnCLickSave ()
	{
		GameObject.Find ("AppController").GetComponent <AppController>().Save ();
		GameObject.Find ("AppController").GetComponent <AppController>().ShowFrameNumber ();
	}

	public void OnClickLoad ()
	{
		GameObject.Find ("AppController").GetComponent <AppController>().Load ();
		GameObject.Find ("AppController").GetComponent <AppController>().ShowFrameNumber ();
	}

	public void OnClickQuit ()
	{
		Application.Quit ();
	}

	public void DropdownLabelGroupIndexChanged (int iIndex_inp)
	{
		GameObject.Find ("AppController").GetComponent <AppController>().sessionLabeler.currentGroup = (Labeler.LabelGroup)iIndex_inp;
	}
	 
	private void CreateDropdownList()
	{
		foreach (var group in Enum.GetValues (typeof(Labeler.LabelGroup)))
		{
			dropdown_LabelGroupList.Add (Enum.GetName (typeof(Labeler.LabelGroup), group));
		}
		dropdown_LabelGroup.AddOptions (dropdown_LabelGroupList);
	}
}
