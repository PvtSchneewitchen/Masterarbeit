//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Events;

//public class ContinueEvent : MonoBehaviour
//{

//    public OVRInput.Button continueEvent;
//    public TutorialManager _tutorialManager;
//    [Tooltip("Seconds how long the Continue Event must be activated to continue")]
//    public float _feventDuration;
//    public UnityEvent EnableEvent;
//    public UnityEvent UpdateEvent;

//    [HideInInspector]
//    public float _fTimeSum;

//    private bool _pressedTooEarly = false;

//    private void OnEnable()
//    {
//        if (EnableEvent != null)
//            EnableEvent.Invoke();

//        if (OVRInput.Get(continueEvent))
//        {
//            _pressedTooEarly = true;
//        }
//    }

//    private void Update()
//    {
//        if (UpdateEvent != null)
//            UpdateEvent.Invoke();

//        if (_pressedTooEarly)
//        {
//            if (!OVRInput.Get(continueEvent))
//            {
//                _pressedTooEarly = false;
//            }
//        }
//        else
//        {
//            if (OVRInput.Get(continueEvent))
//            {
//                _fTimeSum += Time.deltaTime;
//                if (_fTimeSum >= _feventDuration)
//                {
//                    _tutorialManager.NextTutorialStep();
//                }
//            }
//        }
//    }
//}
