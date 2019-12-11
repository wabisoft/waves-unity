using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class levelManager : MonoBehaviour
{
    public static bool gameIsPaused = false;
    //
    public GameObject pauseMenu;
    public GameObject failMenu;
    public GameObject nextLevelMenu;
    public Rigidbody2D leBoat;
    public Transform leRock;
    // 
    public GameObject theBoat;
    public bool winFlag;

    void Start()
    {
        pauseMenu.SetActive(false);
        failMenu.SetActive(false);
        nextLevelMenu.SetActive(false);
        winFlag = theBoat.GetComponent<boatBlah>().winFlag;
    }

    void Update()
    {
        winFlag = theBoat.GetComponent<boatBlah>().winFlag;

        if (winFlag == true) 
        {
            StartCoroutine(showNextLevelMenu(nextLevelMenu.GetComponent<RectTransform>())); ;
        }

        // CHANGE ME
        if (leRock.position.y < -25 * Camera.main.orthographicSize)
        {
            StartCoroutine(showRetryMenu(failMenu.GetComponent<RectTransform>()));
            //Time.timeScale = 0.0f;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (gameIsPaused)
            {
                resume();
            }
            else
            {
                pause();
            }
        }
    }

    private IEnumerator showRetryMenu(RectTransform rt)
    {
        WaitForSeconds wait = new WaitForSeconds(0.01f);
        failMenu.SetActive(true);

        int numSteps = 20;
        float stepSize = 1f / (float)numSteps;
        float tt = 0;
        float easingFunction;

        for (int i = 0; i < numSteps; i++)
        {
            easingFunction = Mathf.Pow(tt, 3f);
            rt.localScale = new Vector3(easingFunction, easingFunction);
            tt += stepSize;
            yield return wait;
        }
        rt.localScale = new Vector3(1f, 1f);
    }

    private IEnumerator showNextLevelMenu(RectTransform rt)
    {
        WaitForSeconds wait = new WaitForSeconds(0.01f);

        
        nextLevelMenu.SetActive(true);

        int numSteps = 20;
        float stepSize = 1f / (float)numSteps;
        float tt = 0;
        float easingFunction;

        for (int i = 0; i < numSteps; i++)
        {
            easingFunction = Mathf.Pow(tt, 3f);
            rt.localScale = new Vector3(easingFunction, easingFunction);
            tt += stepSize;
            yield return wait;
        }
        rt.localScale = new Vector3(1f, 1f);
    }

    public void loadALevel(int levelNum)
    {
        SceneManager.LoadScene(levelNum);
    }

    public void pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0.0f;
        gameIsPaused = true;
    }

    public void resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        gameIsPaused = false;
    }
}
