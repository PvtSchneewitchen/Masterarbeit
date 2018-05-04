using UnityEngine;

public abstract class TutorialStep<T> : TutorialMenu where T : TutorialStep<T>
{ 
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        Instance = (T)this;
    }

    protected virtual void OnDestroy()
    {
        Instance = null;
    }

    protected static void Open()
    {
        if (Instance == null)
            TutorialMenuManager.Instance.CreateInstance<T>();

        TutorialMenuManager.Instance.OpenMenu(Instance);
    }

    protected static void Close()
    {
        if (Instance == null)
        {
            Debug.LogErrorFormat("Trying to close menu {0} but Instance is null", typeof(T));
            return;
        }

        TutorialMenuManager.Instance.CloseMenu(Instance);
    }

    public abstract void OnClickNext();
}

public abstract class TutorialMenu : MonoBehaviour
{
    //[Tooltip("Destroy the Game Object when menu is closed (reduces memory usage)")]
    //public bool DestroyWhenClosed = false;

    //[Tooltip("Disable menus that are under this one in the stack")]
    //public bool DisableMenusUnderneath = true;
}

public abstract class SimpleTutorialMenu<T> : Menu<T> where T : SimpleMenu<T>
{
    public static void Show()
    {
        Open();
    }

    public static void Hide()
    {
        Close();
    }
}
