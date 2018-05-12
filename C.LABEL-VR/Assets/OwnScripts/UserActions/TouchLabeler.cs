using UnityEngine;

/// <summary>
/// This script has to be attached to the Toucher Prefab which has to be attached to a hand of the avatar
/// </summary>
public class TouchLabeler : MonoBehaviour
{

    public static TouchLabeler Instance { get; private set; }

    public bool TouchLabelingEnabled { get; set; }

    private OVRHapticsClip _vibration;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    private void Start()
    {
        TouchLabelingEnabled = true;
        InitVibrationClip();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CustomAttributes>() && TouchLabelingEnabled)
        {
            CustomAttributes colidedObjectAttributes = other.gameObject.GetComponent<CustomAttributes>();
            if (colidedObjectAttributes.Label != Labeling.currentLabelClassID)
            {
                if (gameObject.transform.parent.name.Contains("Left"))
                {
                    OVRHaptics.LeftChannel.Queue(_vibration);
                }
                else
                {
                    OVRHaptics.RightChannel.Queue(_vibration);
                }

                colidedObjectAttributes.Label = Labeling.currentLabelClassID;
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
