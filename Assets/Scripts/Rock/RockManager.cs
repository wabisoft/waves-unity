using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockManager : MonoBehaviour
{
    public RockBehavior[] rocks;
    public bool allSunk = false;

    void Awake()
    {
        rocks = GameObject.FindObjectsOfType<RockBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        allSunk = true;
        foreach(var rock in rocks)
        {
            Debug.Assert(rock != null);
            allSunk &= rock.CurrentState.Id == RockStateEnum.Sunk;
        }
    }
}
