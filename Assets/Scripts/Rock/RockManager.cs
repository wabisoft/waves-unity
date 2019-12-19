using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockManager : MonoBehaviour
{
    public RockBehavior[] rocks;
    public bool allHitWater = false;

    void Awake()
    {
        rocks = GameObject.FindObjectsOfType<RockBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        bool allHitWater_ = true;
        foreach(var rock in rocks)
        {
            Debug.Assert(rock != null);
            allHitWater_ &= rock.CurrentState.Id == RockStateEnum.Sinking;
        }
        if(allHitWater_) { allHitWater = allHitWater_; }
    }
}
