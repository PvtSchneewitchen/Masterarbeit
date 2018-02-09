using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GracesGames.SimpleFileBrowser.Scripts;

public class TutirialAccess : MonoBehaviour
{
    public MainMenuController _mainMenu;

    private float _time;
    private Slider _slider;

    void Start()
    {
        _time = 0;
        _slider = GetComponent<Slider>();
    }

    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.One))
        {
            _time += Time.deltaTime;
        }
        else
        {
            _time = 0;
        }

        _slider.value = _time;

        if(_slider.value >= 1)
        {
            _mainMenu.OnButtonCLick_Tutorial();
        }
    }
}
