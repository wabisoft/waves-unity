using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pauseMenu : MonoBehaviour
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
        // quick and dirty check
        if (leRock.position.y < -10f * Camera.main.orthographicSize && Mathf.Abs(leBoat.velocity.x) < 0.01)
        {
            failMenu.SetActive(true);
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
