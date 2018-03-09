using VRTK;
using UnityEngine;
using UnityEngine.UI;

public class LabelClassEditor : MonoBehaviour
{
    public VRTK_UIPointer _rightUiPointer;
    public KeyboardController _numpad;
    public GameObject _keyboard;
    public GameObject _labelPanel;

    private InputField _className;
    private InputField _classID;
    private Image _classColorImage;
    //private ColorPicker _cPicker;

    void Start()
    {
        InputField[] inputFields = GetComponentsInChildren<InputField>();
        Image[] images = GetComponentsInChildren<Image>();
        //_cPicker = GetComponentInChildren<ColorPicker>();

        _className = inputFields[0];
        _classID = inputFields[1];
        _classColorImage = images[3];

        _rightUiPointer.UIPointerElementClick += OnInputFieldClick;
        gameObject.SetActive(false);
    }

    public void OnAddClick()
    {
        EnableLabelClassEditor();
    }

    private void OnInputFieldClick(object sender, UIPointerEventArgs args)
    {
        var clickedObj = args.currentTarget.GetComponent<InputField>();

        if (Util.IsGameobjectTypeOf<InputField>(args.currentTarget))
        {
            if (clickedObj.name == _className.name)
            {
                EnableKeyBoard();
            }
            else if (clickedObj.name == _classID.name)
            {
                _numpad.EnableNumpad(clickedObj, gameObject);
            }
        }
    }

    private void EnableKeyBoard()
    {
        _keyboard.transform.rotation = transform.rotation;
        _keyboard.transform.position = transform.position + transform.forward;
    }

    private void EnableLabelClassEditor()
    {
        print("EnableClassEditor");
        transform.rotation = _labelPanel.transform.rotation;
        transform.position = _labelPanel.transform.position - _labelPanel.transform.transform.forward - _labelPanel.transform.transform.right;
        gameObject.SetActive(true);
    }

}
