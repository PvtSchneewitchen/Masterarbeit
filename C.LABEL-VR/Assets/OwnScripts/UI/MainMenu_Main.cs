using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_Main : SimpleMenu<MainMenu_Main>
{
    public void OnCreateClick()
    {
        MainMenu_CreateSession.Open();
    }

    public void OnLoadClick()
    {
        string userInfo = "Choose the Save File you want to load!";
        FileBrowserScript.Show(Application.persistentDataPath, ".dat", LoadSessionFromPath, userInfo);
    }

    public void OnQuitClick()
    {
        Application.Quit();
    }

    private void LoadSessionFromPath(string path_inp)
    {
        if (path_inp.Contains(".dat"))
        {
            if (!path_inp.Contains("SaveFile"))
            {
                return;
            }

            Util.DataLoadInfo._sourceDataPath = path_inp;
            Util.DataLoadInfo._accessMode = Util.AccesMode.Load;

            LoadingScreen.Show();
            SceneManager.LoadScene("ApplicationScene");
        }
    }
}
