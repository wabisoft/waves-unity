using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockManager : MonoBehaviour
{
    public List<GameObject> children = new List<GameObject>();
    public bool allHitWater = false;

    void Awake()
    {
        for (int i = 0; i < transform.childCount; ++i){
            children.Add(transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool allHitWater_ = true;
        foreach(var child in children)
        {
            var rock = child.GetComponent<RockBehavior>();
            Debug.Assert(rock != null);
            allHitWater_ &= rock.hitWater;
        }
        if(allHitWater_) { allHitWater = allHitWater_; }
    }
}
