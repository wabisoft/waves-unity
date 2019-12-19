using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RockStateEnum
{
    Default,
    Sinking,
}

public abstract class RockState
{
    public RockState(RockBehavior rb) {
        rockBehavior = rb;
        values = rb.GetStateValues(Id);
    }

    public abstract RockStateEnum Id { get; }

    protected RockBehavior rockBehavior;
    protected RockStateScriptableObject values;
    protected float throwStartTime;
    protected float throwEndTime;
    protected Vector2 throwStartPosition;
    protected Vector2 throwEndPosition;


    public virtual void Enter() {
        rockBehavior.rigidbody.drag = values.linearDrag;
        rockBehavior.rigidbody.angularDrag = values.angularDrag;
        rockBehavior.rigidbody.gravityScale = values.gravityScale;
    }
	public virtual void Exit() { }
    public virtual void FixedUpdate() { }
    public virtual void OnMouseDown() {
        throwStartTime = Time.time;
        throwStartPosition = Input.mousePosition;
    }

    public virtual void OnMouseUp() {
        throwEndTime = Time.time;
		var throwTimeInterval = throwEndTime - throwStartTime;
		throwEndPosition = Input.mousePosition;
		var direction = throwEndPosition - throwStartPosition;
		rockBehavior.rigidbody.AddForce((direction / throwTimeInterval) * values.throwForce);
    }


    public virtual void CollideSea(SeaBehavior sb) { }
    public virtual void UnCollideSea(SeaBehavior sb) { }
}

public class DefaultRockState: RockState
{
    public override RockStateEnum Id { get { return RockStateEnum.Default; } }

    public DefaultRockState(RockBehavior rb) : base(rb) { }

    public override void CollideSea(SeaBehavior seaBehavior)
    {
        seaBehavior.CreateWave(seaBehavior.transform.InverseTransformPoint(rockBehavior.transform.position),
            rockBehavior.rigidbody.velocity.magnitude, (int)Mathf.Sign(rockBehavior.rigidbody.velocity.x), (rockBehavior.isNegative)? -1 : 1);
        rockBehavior.EnterState(new SinkingRockState(rockBehavior, seaBehavior));
    }
}

public class SinkingRockState : RockState
{
    public override RockStateEnum Id { get { return RockStateEnum.Sinking; } }

    public SeaBehavior seaBehavior;

    public SinkingRockState(RockBehavior rb, SeaBehavior sb) : base(rb) {
        seaBehavior = sb;
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
    private Dictionary<RockStateEnum, RockStateScriptableObject> stateValuesDictionary = new Dictionary<RockStateEnum, RockStateScriptableObject>();
    public List<RockStateScriptableObject> StateValues;

    public bool isNegative = false;
    public new Rigidbody2D rigidbody;
    public new CircleCollider2D collider;
    private Stack<RockState> states_ = new Stack<RockState>();
    public RockState CurrentState { get { return states_.Peek(); } }
    public string CurrentStateName { get { return CurrentState.Id.ToString(); }}

	public void EnterState(RockState state)
	{
        if (states_.Count > 0)
        {
            CurrentState.Exit();
        }
		state.Enter();
		states_.Push(state);
	}

	public void ExitState()
	{
		states_.Pop().Exit();
        states_.Peek().Enter();
	}

    public RockStateScriptableObject GetStateValues(RockStateEnum id)
    {
        return stateValuesDictionary.TryGetValue(id, out RockStateScriptableObject value) ? value : null;
    }

    void Awake()
    {
        foreach(var stateValue in StateValues)
        {
            stateValuesDictionary[stateValue.id] = stateValue;
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0029:Use coalesce expression", Justification = "<Pending>")]
    void Start()
	{
        rigidbody = rigidbody == null ? GetComponent<Rigidbody2D>() : rigidbody;
        collider = collider == null ? GetComponent<CircleCollider2D>() : collider;
        EnterState(new DefaultRockState(this));
	}

	void FixedUpdate()
	{
        CurrentState.FixedUpdate();
    }

	void OnMouseDown()
	{
        CurrentState.OnMouseDown();
	}

	void OnMouseUp()
	{
        CurrentState.OnMouseUp();	
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
        CurrentState.CollideSea(seaBehavior);
    }

    public void UnCollideSea(SeaBehavior seaBehavior)
    {
        var waterLevel = seaBehavior.WorldHeightAtWorldPosition(transform.position);
        if(Lower().y < waterLevel) { return; }
        CurrentState.UnCollideSea(seaBehavior);
    }

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Sea")
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
		if (other.gameObject.tag == "Sea")
		{
            CollideSea(other.gameObject.GetComponent<SeaBehavior>());
		}
	}


}
