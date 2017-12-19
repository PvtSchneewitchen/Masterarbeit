using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class FileBrowser
{
	BrowserWindow currentBrowserWindow;

	List<FolderObject> currentDirectoryFolderObjects;
	List<FileObject> currentDirectoryFileObjects;


	public FileBrowser (string sStartPath)
	{
		DirectoryInfo currentDirectory = new DirectoryInfo(sStartPath);
		DirectoryInfo[] currentDirectoryFolderInfos = currentDirectory.GetDirectories ();
		FileInfo[] currentDirectoryFileInfos = currentDirectory.GetFiles ();

		SHowCurrentDirectoryContent(currentDirectoryFolderInfos, currentDirectoryFileInfos);
	}

	private void SHowCurrentDirectoryContent (DirectoryInfo[] folderInfo_inp, FileInfo[] fileInfo_inp)
	{
		foreach (var folderInfo in folderInfo_inp)
		{
			currentDirectoryFolderObjects.Add (new FolderObject(folderInfo));
		}

		foreach (var fileInfo in fileInfo_inp)
		{
			currentDirectoryFileObjects.Add (new FileObject(fileInfo)); 
		}

		currentBrowserWindow = new BrowserWindow(currentDirectoryFolderObjects, currentDirectoryFileObjects);
	}

	private class BrowserWindow
	{
		Vector3 startPosition = Camera.main.transform.position + new Vector3(-2.5f, 1.5f, 5.0f);
		int iRows = 5;
		int iColoums;
		Vector3[] positions;

		float fObjectDistance = 1.5f;


		public BrowserWindow (List<FolderObject> folderObjects_inp, List<FileObject> fileObjects_inp)
		{
			int iDirectoryObjectCount = folderObjects_inp.Count + fileObjects_inp.Count;
			iColoums = Mathf.CeilToInt(iDirectoryObjectCount/iRows);

			positions = CalculatePositions(iDirectoryObjectCount, iColoums, iRows);

			for (int i = 0; i < iDirectoryObjectCount; i++) 
			{
				if(i < folderObjects_inp.Count)
				{
					
				}
			}
		}

		private Vector3[] CalculatePositions(int iDirectoryObjectCount_inp, int iColoums_inp, int iRows_inp)
		{
			Vector3[] positionsRet = new Vector3[iDirectoryObjectCount_inp];

			int iRowCounter = 1;
			int iColoumCounter = 0;

			positionsRet[0] = startPosition;
			for (int i = 1; i < iDirectoryObjectCount_inp; i++)
			{
				if(iRowCounter%iRows_inp == 0)
				{
					iRowCounter = 0;
					iColoumCounter++;
				}

				positionsRet[i] = startPosition + new Vector3(iColoumCounter*fObjectDistance, -iRowCounter*fObjectDistance, 5);

				iRowCounter++;
			}
		}
	}

	private class FolderObject
	{
		public GameObject folderGameObject;
		public DirectoryInfo directoryInfo;

		public FolderObject (DirectoryInfo directoryInfo_inp)
		{
			directoryInfo = directoryInfo_inp;
			folderGameObject = GameObject.CreatePrimitive (PrimitiveType.Cube);
		}
	}

	private class FileObject
	{
		public GameObject fileGameObject;
		public FileInfo fileInfo;

		public FileObject (FileInfo fileInfo_inp)
		{
			fileInfo = fileInfo_inp;
			fileGameObject = GameObject.CreatePrimitive (PrimitiveType.Cube);
		}
	}

}
