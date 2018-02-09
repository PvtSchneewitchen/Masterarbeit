using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script has to be attached to the Toucher Prefab which has to be attached to a hand of the avatar
/// </summary>
public class TouchLabeler : MonoBehaviour {

    public GameObject _labelPointPrefab;

    private PointAttributes _collidedObjectAttributes;
    private OVRHapticsClip _vibration;

    private void Start()
    {
        InitVibrationClip();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == _labelPointPrefab.name)
        {
            _collidedObjectAttributes = other.gameObject.GetComponent<PointAttributes>();
            if (_collidedObjectAttributes._group != Util.Labeling._currentGoup)
            {
                if(gameObject.transform.parent.name.Contains("left"))
                {
                    OVRHaptics.LeftChannel.Queue(_vibration);
                }
                else
                {
                    OVRHaptics.RightChannel.Queue(_vibration);
                }
                
                _collidedObjectAttributes.ChangeGroup(Util.Labeling._currentGoup);
            }
        }
    }

    private void InitVibrationClip()
    {
        byte[] b = new byte[10];

        for (int i = 0; i < b.Length; i++)
        {
            b[i] = 100;
        }
        _vibration = new OVRHapticsClip(b, b.Length);
    }
}
