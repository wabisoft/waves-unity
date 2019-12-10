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
        WaitForSeconds wait = new WaitForSeconds(0.05f);

        for (int i = 0; i < 4; i++)
        {
            rt.localRotation = Quaternion.Euler(rt.localRotation.x, rt.localRotation.y, rt.localRotation.z + i * 20.0f);
            yield return wait;
        }
    }
}
