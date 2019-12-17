using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boatWinCondition : MonoBehaviour
{
    public bool winFlag;

    private Rigidbody2D rb;

    public GameObject winPlatform;
    public Vector2 winPos;
    private Vector2 boat2WinPlatVector;

    public float xThreshold;
    public float yThreshold;

    void Start()
    {
        //
        xThreshold = 1f;
        yThreshold = 1.2f * winPlatform.transform.localScale.y;
        //
        winFlag = false;
        rb = gameObject.GetComponent<Rigidbody2D>();
        winPos = new Vector2(winPlatform.transform.position.x, winPlatform.transform.position.y);
        boat2WinPlatVector = new Vector2(winPos.x - gameObject.transform.position.x, winPos.y - gameObject.transform.position.y);

        //Debug.Log("Scale in Y: " + " " + winPlatform.transform.localScale.y);
    }

    private void Update()
    {
        //
        boat2WinPlatVector.x = winPos.x - gameObject.transform.position.x;
        boat2WinPlatVector.y = winPos.y - gameObject.transform.position.y;

        //
        if (Mathf.Abs(boat2WinPlatVector.x) < xThreshold && Mathf.Abs(boat2WinPlatVector.y) < yThreshold && rb.IsSleeping())
        {
            winFlag = true;
            //Debug.Log("You win dawg");
        }
    }
}
