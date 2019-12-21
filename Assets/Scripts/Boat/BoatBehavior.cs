using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoatBehavior : StatefulBehavior<BoatStateEnum, BoatStateScriptableObject>
{
    public new BoxCollider2D collider;
    public new Rigidbody2D rigidbody;
    public GameObject winPlatform;
    public Vector2 winThreshold;

    [HideInInspector]
    public Vector2 size;
    [HideInInspector]
    public Vector2 halfSize;
    private bool winFlag;

    public bool Won { get { return winFlag; } }

    // Start is called before the first frame Update
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
        EnterState(new DefaultBoatState(this));
        size = collider.size * transform.localScale;
        halfSize = size / 2;
    }

    void FixedUpdate()
    {
        CurrentState.FixedUpdate();
    }

    public void CollideSea(SeaBehavior sb)
    {
        if (sb == null) { return; }
        ((BoatState)CurrentState).CollideSea(sb);
    }

    public void CollidePlatform(PlatformBehavior pb) {
        var relpos = pb.transform.position - transform.position;
        if(Mathf.Abs(relpos.x) <= winThreshold.x && Mathf.Abs(relpos.y) <= winThreshold.y && pb.gameObject.GetInstanceID() == winPlatform.gameObject.GetInstanceID())
        {
            winFlag = true;
        }
        ((BoatState)CurrentState).CollidePlatform(pb);
    }

    public void UnCollidePlatform(PlatformBehavior pb) { }
}
