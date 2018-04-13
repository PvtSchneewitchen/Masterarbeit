using UnityEngine;
using UnityEngine.UI;

public class ColorPickerValueChange : MonoBehaviour {

    public Image ClassColorImage;

    void OnColorChange(HSBColor hsbColor_inp)
    {
        Color color = hsbColor_inp.ToColor();
        color.a = 255;
        ClassColorImage.color = color;
    }
}
