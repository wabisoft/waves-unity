using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RockState
{
    public virtual string Tag { get { return "base";  } }

    public RockState(RockBehavior rb) {
        rockBehavior = rb;
    }

    protected RockBehavior rockBehavior;
    public virtual void Enter() { }
	public virtual void Exit() { }
    public virtual void FixedUpdate() { }
    public virtual void CollideSea(SeaBehavior sb) { }
    public virtual void UnCollideSea(SeaBehavior sb) { }
}

public class DefaultRockState: RockState
{
    public override string Tag { get { return "default";  } }

    public float linearDrag = 0;
    public float angularDrag = 0.54f;
    public float gravityScale = 0.54f;

    public DefaultRockState(RockBehavior rb) : base(rb) { }
    public override void Enter() {
        rockBehavior.rigidbody.drag = linearDrag;
        rockBehavior.rigidbody.angularDrag = angularDrag;
        rockBehavior.rigidbody.gravityScale = gravityScale;
    }

    public override void CollideSea(SeaBehavior seaBehavior)
    {
        seaBehavior.CreateWave(seaBehavior.transform.InverseTransformPoint(rockBehavior.transform.position),
            rockBehavior.rigidbody.velocity.magnitude, (int)Mathf.Sign(rockBehavior.rigidbody.velocity.x), (rockBehavior.isNegative)? -1 : 1);
        rockBehavior.EnterState(new SinkingRockState(rockBehavior, seaBehavior));
    }
}

public class SinkingRockState : RockState
{
    public override string Tag { get { return "sinking";  } }

    public float linearDrag = 3f;
    public float angularDrag = 3.54f;
    public float gravityScale = 0.54f;
    public SeaBehavior seaBehavior;

    public SinkingRockState(RockBehavior rb, SeaBehavior sb) : base(rb) {
        seaBehavior = sb;
    }

    public override void Enter()
    {
        rockBehavior.rigidbody.drag = linearDrag;
        rockBehavior.rigidbody.angularDrag = angularDrag;
        rockBehavior.rigidbody.gravityScale = gravityScale;
    }

    public override void FixedUpdate()
    {
        var waterLevel = seaBehavior.WorldHeightAtWorldPosition(rockBehavior.transform.position);
        if(rockBehavior.Lower().y < waterLevel) { return; }
        UnCollideSea(seaBehavior);
    }

    public override void UnCollideSea(SeaBehavior sb)
    {
        rockBehavior.ExitState();
    }
}

public class RockBehavior : MonoBehaviour
{
    public bool isNegative = false;
	public new Rigidbody2D rigidbody;
    public new CircleCollider2D collider;
	public bool hitWater = false;
	private Vector2 throwStartPos, throwEndPos;
	private float throwStartTime, throwEndTime, throwTimeInterval;
	[Range(0.05f, 1f)]
	public float throwForce = 0.05f;
    public Stack<RockState> states = new Stack<RockState>();
    public string stateTag = "";

	public void EnterState(RockState state)
	{
        if (states.Count > 0)
        {
            states.Peek().Exit();
        }
		state.Enter();
		states.Push(state);
        stateTag = state.Tag;
	}

	public void ExitState()
	{
		states.Pop().Exit();
        states.Peek().Enter();
        stateTag = states.Peek().Tag;
	}

	void Start()
	{
		if (rigidbody == null)
		{
			rigidbody = GetComponent<Rigidbody2D>();
		}
        if(collider == null)
        {
            collider = GetComponent<CircleCollider2D>();
        }
        EnterState(new DefaultRockState(this));
	}

	void FixedUpdate()
	{
        states.Peek().FixedUpdate();
    }

	void OnMouseDown()
	{
		throwStartTime = Time.time;
		throwStartPos = Input.mousePosition;
	}

	void OnMouseUp()
	{
		throwEndTime = Time.time;
		throwTimeInterval = throwEndTime - throwStartTime;
		throwEndPos = Input.mousePosition;
		var direction = throwEndPos - throwStartPos;
		rigidbody.AddForce((direction / throwTimeInterval) * throwForce);

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
        states.Peek().CollideSea(seaBehavior);
    }

    public void UnCollideSea(SeaBehavior seaBehavior)
    {
        var waterLevel = seaBehavior.WorldHeightAtWorldPosition(transform.position);
        if(Lower().y < waterLevel) { return; }
        states.Peek().UnCollideSea(seaBehavior);
    }

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Sea" && !hitWater)
		{
            CollideSea(other.gameObject.GetComponent<SeaBehavior>());
		}
		if (other.gameObject.tag == "Boat")
		{
			Destroy(other.gameObject);
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.tag == "Sea" && !hitWater)
		{
            CollideSea(other.gameObject.GetComponent<SeaBehavior>());
		}
	}


}
