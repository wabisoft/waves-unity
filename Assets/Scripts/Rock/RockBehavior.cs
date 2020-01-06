using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBehavior : StatefulBehavior<RockStateEnum, RockStateScriptableObject>
{

    public bool isNegative = false;
    public new Rigidbody2D rigidbody;
    public new CircleCollider2D collider;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0029:Use coalesce expression", Justification = "No supported? Or otherwise not behaving as expected")]
    void Start()
	{
        rigidbody = rigidbody == null ? GetComponent<Rigidbody2D>() : rigidbody;
        collider = collider == null ? GetComponent<CircleCollider2D>() : collider;
        EnterState(new DefaultRockState(this));
	}

    public Vector3 Lower()
    {
        return transform.position - (collider.radius * transform.localScale);
    }

    public void CollideSea(SeaBehavior seaBehavior)
    {
        var waterLevel = seaBehavior.WorldHeightAtWorldPosition(transform.position);
        if(Lower().y > waterLevel) { return; }
        // else if( rigidbody.velocity.magnitude <= 0.001) { return; } // not really moving
        ((RockState)CurrentState).CollideSea(seaBehavior);
    }

    public void UnCollideSea(SeaBehavior seaBehavior)
    {
        var waterLevel = seaBehavior.WorldHeightAtWorldPosition(transform.position);
        if(Lower().y < waterLevel) { return; }
        ((RockState)CurrentState).UnCollideSea(seaBehavior);
    }

    public void CollideFloor(GameObject floor)
    {
        ((RockState)CurrentState).CollideFloor(floor);
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Boat")
		{
			// Destroy(collision.gameObject);
		}
        if (collision.gameObject.tag == "Floor")
        {
            CollideFloor(collision.gameObject);
        }       
    }

    public override void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            CollideFloor(collision.gameObject);
        }
    }

    public override void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Sea")
		{
            CollideSea(other.gameObject.GetComponent<SeaBehavior>());
		}	
	}

	public override void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.tag == "Sea")
		{
            CollideSea(other.gameObject.GetComponent<SeaBehavior>());
		}
    }


}
