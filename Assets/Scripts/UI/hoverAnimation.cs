using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class hoverAnimation : MonoBehaviour
{
    public RectTransform rt;
    public void restartAnimation()
    {
        if (rt != null)
        {
            //rt.localScale = new Vector3(2.0f, 2.0f);  why do I have to use heap allocation for this?
            //rt.localRotation = Quaternion.Euler(rt.localRotation.x, rt.localRotation.y, rt.localRotation.z + 180.0f);
            //rt.pivot = new Vector2(1.0f, 0.5f);
            StartCoroutine(restartAnimationCoroutine());
        }
    }
    private IEnumerator restartAnimationCoroutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.03f);

        int numSteps = 20;
        float stepSize = 1f / (float)numSteps;
        float tt = 0;
        float easingFunction, rotationVal;

        for (int i = 0; i < numSteps; i++)
        {
            easingFunction = -5f * Mathf.Sin(Mathf.PI * (tt - 1f));
            rotationVal = easingFunction * 5f; // base
            rt.localScale = new Vector3(1f + 0.3f * easingFunction, 1f + 0.3f * easingFunction);
            rt.localRotation = Quaternion.Euler(rt.localRotation.x, rt.localRotation.y, rt.localRotation.z + rotationVal);
            tt += stepSize;
            yield return wait;
        }
        rt.localScale = new Vector3(1f, 1f);
        rt.localRotation = Quaternion.Euler(rt.localRotation.x, rt.localRotation.y, 0f);
    }
}
