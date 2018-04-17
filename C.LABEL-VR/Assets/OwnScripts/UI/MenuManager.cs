using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Main Menu Specific")]
    public MainMenu_Main mainMenuMainPrefab;
    public MainMenu_CreateSession mainMenuCreateSessionPrefab;

    [Header("Application Menu Specific")]
    public AppMenu_Movement appMenuMovement;
    public AppMenu_Labeling appMenuLabeling;

    [Header("General")]
    public Transform vrCamera;
    public NumPad numpadPrefab;
    public LabelClassEditor labelClassEditorPrefab;
    public KeyboardManager keyBoardManagerPrefab;
    public FileBrowserScript fileBrowserPrefab;
    public LoadingScreen loadingScreen;

    public bool optionModeActive;

    public static MenuManager Instance { get; set; }

    public CustomStack<Menu> menuStack = new CustomStack<Menu>();

    private string sceneName;

    private float distanceToCamera = 5;

    private void Awake()
    {
        Instance = this;
        optionModeActive = false;
        sceneName = SceneManager.GetActiveScene().name;

        if(sceneName.Contains("Menu"))
        {
            MainMenu_Main.Show();
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

    public GameObject CreateInstance<T>(bool withReturnValue = true) where T : Menu
    {
        var prefab = GetPrefab<T>();

        var instantiatedObject = Instantiate(prefab, transform);

        return instantiatedObject as GameObject;
    }

    public void OpenMenu(Menu instance)
    {
        if (menuStack.Contains(instance))
        {
            menuStack.Remove(instance);
        }

        // De-activate top menu
        if (menuStack.Count > 0)
        {
            if (instance.DisableMenusUnderneath)
            {
                //foreach (var menu in menuStack)
                //{
                //    menu.gameObject.SetActive(false);

                //    if (menu.DisableMenusUnderneath)
                //        break;
                //}

                for (int i = menuStack.Count-1; i >= 0 ; i--)
                {
                    menuStack.ElementAt(i).gameObject.SetActive(false);

                    if (menuStack.ElementAt(i).DisableMenusUnderneath)
                        break;
                }
            }

            var topCanvas = instance.GetComponent<Canvas>();
            var previousCanvas = menuStack.Peek().GetComponent<Canvas>();
            topCanvas.sortingOrder = previousCanvas.sortingOrder + 1;
        }
        
        menuStack.Push(instance);

        instance.transform.position = vrCamera.position + vrCamera.forward * distanceToCamera;
        instance.transform.rotation = vrCamera.rotation;
    }

    public void CloseAll()
    {
        for (int i = 0; i < menuStack.Count; i++)
        {
            var instance = menuStack.Pop();

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
        if (menuStack.Count == 0)
        {
            Debug.LogErrorFormat(menu, "{0} cannot be closed because menu stack is empty", menu.GetType());
            return;
        }

        if (menuStack.Peek() != menu)
        {
            Debug.LogErrorFormat(menu, "{0} cannot be closed because it is not on top of stack", menu.GetType());
            return;
        }

        CloseTopMenu();
    }

    public void CloseTopMenu()
    {
        var instance = menuStack.Pop();

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

        for (int i = menuStack.Count - 1; i >= 0; i--)
        {
            menuStack.ElementAt(i).gameObject.SetActive(true);

            if (menuStack.ElementAt(i).DisableMenusUnderneath)
                break;
        }
    }

    public void OnMenuOpenRoutine()
    {
        optionModeActive = true;
        Movement.Instance.MovementEnabled = false;
        PointerLabeler.Instance.ClusterLabelingEnabled = false;
        PointerLabeler.Instance.PointerLabelingEnabled = false;
        PointerTeleport.Instance.PointerTeleportEnabled = false;
        TouchLabeler.Instance.TouchLabelingEnabled = false;
    }

    public void OnMenuCloseRoutine()
    {
        optionModeActive = false;
        Movement.Instance.MovementEnabled = true;
        PointerLabeler.Instance.ClusterLabelingEnabled = true;
        PointerLabeler.Instance.PointerLabelingEnabled = true;
        PointerTeleport.Instance.PointerTeleportEnabled = true;
        TouchLabeler.Instance.TouchLabelingEnabled = true;

        MovementOptions.SaveOptions();
    }

    private void Update()
    {
        if (sceneName.Contains("Application"))
        {
            if (!optionModeActive)
            {
                if (OVRInput.GetDown(OVRInput.Button.Start))
                {
                    AppMenu_Movement.Show();
                    OnMenuOpenRoutine();
                }
            }
            else
            {
                if (OVRInput.GetDown(OVRInput.Button.Start))
                {
                    for (int i = 0; i < menuStack.Count; i++)
                    {
                        CloseTopMenu();
                    }

                    OnMenuCloseRoutine();
                }
            }
        }
    }
}
