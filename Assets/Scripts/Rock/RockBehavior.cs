using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RockState
{
    public RockState(RockBehavior rb) {
        rockBehavior = rb;
        values = rb.GetStateValues(Tag);
    }

    public virtual string Tag { get { return "base";  } }

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
    public override string Tag { get { return "default";  } }

    // public float linearDrag = 0;
    // public float angularDrag = 0.54f;
    // public float gravityScale = 0.54f;

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
    public override string Tag { get { return "sinking";  } }

    // public float linearDrag = 3f;
    // public float angularDrag = 3.54f;
    // public float gravityScale = 0.54f;
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
    private Dictionary<string, RockStateScriptableObject> stateValuesDictionary = new Dictionary<string, RockStateScriptableObject>();
    public List<RockStateScriptableObject> StateValues;

    public bool isNegative = false;
	public new Rigidbody2D rigidbody;
    public new CircleCollider2D collider;
	public bool hitWater = false;
	private Vector2 throwStartPos, throwEndPos;
	private float throwStartTime, throwEndTime, throwTimeInterval;
	[Range(0.05f, 1f)]
	public float throwForce = 0.05f;
    public string stateTag = "";

    private Stack<RockState> states_ = new Stack<RockState>();

	public void EnterState(RockState state)
	{
        if (states_.Count > 0)
        {
            states_.Peek().Exit();
        }
		state.Enter();
		states_.Push(state);
        stateTag = state.Tag;
	}

	public void ExitState()
	{
		states_.Pop().Exit();
        states_.Peek().Enter();
        stateTag = states_.Peek().Tag;
	}
    public RockStateScriptableObject GetStateValues(string name)
    {
        return stateValuesDictionary.TryGetValue(name, out RockStateScriptableObject value) ? value : null;
    }

    void Awake()
    {
        foreach(var stateValue in StateValues)
        {
            stateValuesDictionary[stateValue.stateName] = stateValue;
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
        states_.Peek().FixedUpdate();
    }

	void OnMouseDown()
	{
        states_.Peek().OnMouseDown();
	}

	void OnMouseUp()
	{
        states_.Peek().OnMouseUp();	
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
        states_.Peek().CollideSea(seaBehavior);
    }

    public void UnCollideSea(SeaBehavior seaBehavior)
    {
        var waterLevel = seaBehavior.WorldHeightAtWorldPosition(transform.position);
        if(Lower().y < waterLevel) { return; }
        states_.Peek().UnCollideSea(seaBehavior);
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
