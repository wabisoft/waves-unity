using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using DentedPixel;
using UnityEngine;

public class hoverAnimation : MonoBehaviour
{
    public LTRect buttonRect4;
    void OnGUI()
    {
        if (GUI.Button(buttonRect4.rect, "RestartButton"))
        {
            Debug.Log("blah blah");
            LeanTween.rotate(buttonRect4, 150.0f, 1.0f).setEase(LeanTweenType.easeOutElastic);
            LeanTween.rotate(buttonRect4, 0.0f, 1.0f).setDelay(1.0f).setEase(LeanTweenType.easeOutElastic);
        }
    }
}
