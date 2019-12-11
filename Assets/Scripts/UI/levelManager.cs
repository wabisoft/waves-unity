using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class levelManager : MonoBehaviour
{
    public static bool gameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject failMenu;
    public Rigidbody2D leBoat;
    public Transform leRock;


    void Start()
    {
        pauseMenuUI.SetActive(false);
        failMenu.SetActive(false);
    }

    void Update()
    {
        // blah
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

    public void loadALevel(int levelNum)
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
