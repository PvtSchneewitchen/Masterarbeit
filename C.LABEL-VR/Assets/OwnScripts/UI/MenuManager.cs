using System.Linq;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    [Header("Main Menu Specific")]
    public MainMenu_Main mainMenuMainPrefab;
    public MainMenu_CreateSession mainMenuCreateSessionPrefab;
    public MainMenu_Demo mainMenuDemoPrefab;

    [Header("Application Menu Specific")]
    public AppMenu_Movement appMenuMovement;
    public AppMenu_Labeling appMenuLabeling;
    public NumPad numpadPrefab;
    public LabelClassEditor labelClassEditorPrefab;

    [Header("General")]
    public Transform vrCamera;
    public KeyboardManager keyBoardManagerPrefab;
    public FileBrowserScript fileBrowserPrefab;
    public LoadingScreen loadingScreen;

    public bool OptionModeActive;

    public static MenuManager Instance { get; set; }

    public CustomStack<Menu> MenuStack = new CustomStack<Menu>();

    private string SceneName;

    private float DistanceToCamera = 5;

    private void Awake()
    {
        Instance = this;
        OptionModeActive = false;
        SceneName = SceneManager.GetActiveScene().name;

        if(SceneName.Contains("MainMenu"))
        {
            MainMenu_Main.Show();
            //MainMenu_Demo.Show();
        }
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void CreateInstance<T>() where T : Menu
    {
        var prefab = GetPrefab<T>();

        Instantiate(prefab, transform);
    }

 //   public GameObject CreateInstance<T>(bool withReturnValue = true) where T : Menu
 //   {
 //       var prefab = GetPrefab<T>();
 //
 //       var instantiatedObject = Instantiate(prefab, transform);
 //
 //       return instantiatedObject as GameObject;
 //   }

    public void OpenMenu(Menu instance)
    {
        if (MenuStack.Contains(instance))
        {
            MenuStack.Remove(instance);
        }

        // De-activate top menu
        if (MenuStack.Count > 0)
        {
            if (instance.DisableMenusUnderneath)
            {
                //foreach (var menu in menuStack)
                //{
                //    menu.gameObject.SetActive(false);

                //    if (menu.DisableMenusUnderneath)
                //        break;
                //}

                for (int i = MenuStack.Count-1; i >= 0 ; i--)
                {
                    MenuStack.ElementAt(i).gameObject.SetActive(false);

                    if (MenuStack.ElementAt(i).DisableMenusUnderneath)
                        break;
                }
            }

            var topCanvas = instance.GetComponent<Canvas>();
            var previousCanvas = MenuStack.Peek().GetComponent<Canvas>();
            topCanvas.sortingOrder = previousCanvas.sortingOrder + 1;
        }
        
        MenuStack.Push(instance);

        instance.transform.position = vrCamera.position + vrCamera.forward * DistanceToCamera;
        instance.transform.rotation = vrCamera.rotation;
    }

    public void CloseAll()
    {
        for (int i = 0; i < MenuStack.Count; i++)
        {
            var instance = MenuStack.Pop();

            instance.gameObject.SetActive(false);
        }
    }

    private T GetPrefab<T>() where T : Menu
    {
        // Get prefab dynamically, based on public fields set from Unity
        // You can use private fields with SerializeField attribute too
        var fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        foreach (var field in fields)
        {
            var prefab = field.GetValue(this) as T;
            if (prefab != null)
            {
                return prefab;
            }
        }

        throw new MissingReferenceException("Prefab not found for type " + typeof(T));
    }

    public void CloseMenu(Menu menu)
    {
        if (MenuStack.Count == 0)
        {
            Debug.LogErrorFormat(menu, "{0} cannot be closed because menu stack is empty", menu.GetType());
            return;
        }

        if (MenuStack.Peek() != menu)
        {
            Debug.LogErrorFormat(menu, "{0} cannot be closed because it is not on top of stack", menu.GetType());
            return;
        }

        CloseTopMenu();
    }

    public void CloseTopMenu()
    {
        var instance = MenuStack.Pop();

        if (instance.DestroyWhenClosed)
            Destroy(instance.gameObject);
        else
            instance.gameObject.SetActive(false);

        // Re-activate top menu
        // If a re-activated menu is an overlay we need to activate the menu under it
        //foreach (var menu in menuStack)
        //{
        //    menu.gameObject.SetActive(true);

        //    if (menu.DisableMenusUnderneath)
        //        break;
        //}

        for (int i = MenuStack.Count - 1; i >= 0; i--)
        {
            MenuStack.ElementAt(i).gameObject.SetActive(true);

            if (MenuStack.ElementAt(i).DisableMenusUnderneath)
                break;
        }
    }

    public void OnMenuOpenRoutine()
    {
        OptionModeActive = true;
        Movement.Instance.enabled = false;
        PointerLabeler.Instance.ClusterLabelingEnabled = false;
        PointerLabeler.Instance.LabelingEnabled = false;
        PointerTeleport.Instance.PointerTeleportEnabled = false;
        TouchLabeler.Instance.TouchLabelingEnabled = false;
        LabelClassDisplayUpdate.Instance.DisplayEnabled = false;
        LabelClassPipette.Instance.enabled = false;
    }

    public void OnMenuCloseRoutine()
    {
        OptionModeActive = false;
        Movement.Instance.enabled = true;
        PointerLabeler.Instance.ClusterLabelingEnabled = true;
        PointerLabeler.Instance.LabelingEnabled = true;
        PointerTeleport.Instance.PointerTeleportEnabled = true;
        TouchLabeler.Instance.TouchLabelingEnabled = true;
        LabelClassDisplayUpdate.Instance.DisplayEnabled = true;
        LabelClassPipette.Instance.enabled = true;

        MovementOptions.SaveOptions();
    }

    private void Update()
    {
        if (SceneName.Contains("Application"))
        {
            if (OVRInput.GetDown(OVRInput.Button.Start))
            {
                if(MenuStack.Count==0)
                {
                    AppMenu_Movement.Show();
                    OnMenuOpenRoutine();
                }
            }

            //if (!optionModeActive)
            //{
            //    if (OVRInput.GetDown(OVRInput.Button.Start))
            //    {
            //        AppMenu_Movement.Show();
            //        OnMenuOpenRoutine();
            //    }
            //}
            //else
            //{
            //    if (OVRInput.GetDown(OVRInput.Button.Start))
            //    {
            //        for (int i = 0; i < menuStack.Count; i++)
            //        {
            //            CloseTopMenu();
            //        }

            //        OnMenuCloseRoutine();
            //    }
            //}
        }
    }
}
