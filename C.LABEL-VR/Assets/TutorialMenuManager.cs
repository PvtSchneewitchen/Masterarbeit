using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialMenuManager : MonoBehaviour
{
    public Transform vrCamera;
    public Step1_UiInteraction step1;
    public Step2_Labeling_LabelClass step2;
    public Step3_Labeling_PointerLabeler step3;
    public Step4_Labeling_ClusterLabeling step4;
    public Step5_Labeling_TouchLabeling step5;
    public Step6_InGameMenu step6;
    public Step7_MovementFreeFly step7;
    public Step8_MovementTeleport step8;


    public static TutorialMenuManager Instance { get; set; }

    public CustomStack<TutorialMenu> menuStack = new CustomStack<TutorialMenu>();

    private float distanceToCamera = 5;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void CreateInstance<T>() where T : TutorialMenu
    {
        var prefab = GetPrefab<T>();

        Instantiate(prefab, transform);
    }

    public GameObject CreateInstance<T>(bool withReturnValue = true) where T : TutorialMenu
    {
        var prefab = GetPrefab<T>();

        var instantiatedObject = Instantiate(prefab, transform);

        return instantiatedObject as GameObject;
    }

    public void OpenMenu(TutorialMenu instance)
    {
        if (menuStack.Contains(instance))
        {
            menuStack.Remove(instance);
        }

        // De-activate top menu
        if (menuStack.Count > 0)
        {
            CloseMenu(menuStack.ElementAt(0));
        }

        menuStack.Push(instance);

        instance.transform.position = vrCamera.position + vrCamera.forward * distanceToCamera;
        instance.transform.rotation = vrCamera.rotation;
    }

    public void AlignMenu(TutorialMenu instance)
    {
        instance.transform.position = vrCamera.position + vrCamera.forward * distanceToCamera;
        instance.transform.rotation = vrCamera.rotation;
    }

    private T GetPrefab<T>() where T : TutorialMenu
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

    public void CloseMenu(TutorialMenu menu)
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
        Destroy(instance.gameObject);
    }

}
