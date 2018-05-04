using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_CreateSession : SimpleMenu<MainMenu_CreateSession>
{
    private Util.Datatype datatype;
    private string sessionName;

	public void OnPCDClick()
    {
        string userInfo = "Choose the name of your PCD-Session!";
        KeyboardManager.Show(Receive_PCD_SessionName, userInfo);
    }

    public void OnHDF5_Daimler_CLick()
    {
        string userInfo = "Choose the name of your HDF5-Session!";
        KeyboardManager.Show(Receive_HDF5Damiler_SessionName, userInfo);
    }

    public void OnBackClick()
    {
        MenuManager.Instance.CloseTopMenu();
    }

    private void Receive_PCD_SessionName(string sessionName_inp)
    {
        datatype = Util.Datatype.pcd;
        sessionName = sessionName_inp;
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        string userInfo = "Choose a directory or a file with PCD-Data";
        FileBrowserScript.Show(desktopPath, ".pcd", LoadFromPath, userInfo);
    }

    private void Receive_HDF5Damiler_SessionName(string sessionName_inp)
    {
        datatype = Util.Datatype.hdf5_DaimlerLidar;
        sessionName = sessionName_inp;
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        string userInfo = "Choose a directory or a file with HDF5-Data";
        FileBrowserScript.Show(desktopPath, ".hdf5", LoadFromPath, userInfo);
    }

    private void LoadFromPath(string loadPath)
    {
        Util.DataLoadInfo._accessMode = Util.AccesMode.Create;
        Util.DataLoadInfo._dataType = datatype;
        Util.DataLoadInfo._sessionName = sessionName;
        Util.DataLoadInfo._sourceDataPath = loadPath;
        //Util.DataLoadInfo._sessionFolderPath = Application.persistentDataPath + "/" + Util.DataLoadInfo._sessionName;

        ReferenceHandler.Instance.GetRightPointerRenderer().enabled = false;
        LoadingScreen.Show();
        SceneManager.LoadScene("ApplicationScene");
    }
}
