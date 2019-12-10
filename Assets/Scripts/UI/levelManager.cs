using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class levelManager : MonoBehaviour
{
    public void loadALevel(int levelNum) 
    {
        SceneManager.LoadScene(levelNum);
    }
}
