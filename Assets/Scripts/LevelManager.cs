using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static bool gameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject failMenu;
    public GameObject nextLevelMenu;
    public RockManager rockManager;
    public BoatBehavior boat;
    public int framesToWin = 10;
    public int winCounter = 0;

    void Start()
    {
        pauseMenuUI.SetActive(false);
        failMenu.SetActive(false);
        nextLevelMenu.SetActive(false);
    }

    void Update()
    {

        if (boat.Won)
        {
            winCounter++;
            if (winCounter > framesToWin)
            {
                // TODO: Only call once
                StartCoroutine(showNextLevelMenu(nextLevelMenu.GetComponent<RectTransform>()));
            }
        } else
        {
            winCounter = 0;
        }

        if (rockManager.allSunk && boat.CurrentState.Id != BoatStateEnum.Surfing && winCounter == 0 && !boat.Won) {
            // TODO: Only call once
            StartCoroutine(showRetryMenu(failMenu.GetComponent<RectTransform>()));
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
        WaitForSeconds wait = new WaitForSeconds(0.02f);
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

        int numSteps = 30;
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


    public void LoadALevel(int levelNum)
    {
        SceneManager.LoadScene(levelNum);
    }

    public void pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0.0f;
        gameIsPaused = true;
    }

    public void resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
        gameIsPaused = false;
    }
}
