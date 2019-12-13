using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BoatState
{
	public BoatState(BoatBehavior bb)
	{
		boatBehavior = bb;
	}

	protected BoatBehavior boatBehavior;
	public virtual void Enter() { }
	public virtual void Exit() { }
	public abstract void FixedUpdate();
	public virtual void CollideSea(SeaBehavior sb)
    {
		var posRelativeToSea = sb.transform.InverseTransformPoint(boatBehavior.transform.position);
		var waterLevel = sb.transform.position.y + sb.HeightAtX(posRelativeToSea.x);
        var lower = boatBehavior.transform.position - (Vector3)boatBehavior.halfSize;
		if (lower.y > waterLevel) { return; }
		var h = Mathf.Min(waterLevel - lower.y, boatBehavior.collider.size.y);
		var w = boatBehavior.collider.size.x;
		var displacedWater = h * w;
		var rigidbody = boatBehavior.rigidbody;
		if (Vector3.Dot(rigidbody.velocity, Vector3.up) != 0)
		{
			var waterFluidDensity = 50f;
			var bouyancy = displacedWater * waterFluidDensity * -Physics2D.gravity * Time.deltaTime;
			rigidbody.AddForce(bouyancy, ForceMode2D.Impulse);
			rigidbody.velocity *= 0.75f;
		}
    }
}


public class DefaultBoatState : BoatState
{
	public DefaultBoatState(BoatBehavior bb) : base(bb) { }

    public override void Enter()
    {
        // Debug.Log("Humdrumb. Default...");
    }

	public override void FixedUpdate()
	{
		// Do nothing extra?
	}

    public override void CollideSea(SeaBehavior sb)
    {
        base.CollideSea(sb);
        boatBehavior.EnterState(new SailingBoatState(boatBehavior, sb));
    }
}

public class SailingBoatState : BoatState
{
    public SeaBehavior seaBehavior;

    public SailingBoatState(BoatBehavior bb, SeaBehavior sb): base(bb) {
        seaBehavior = sb;
    }

    public override void Enter()
    {
        // Debug.Log("Ahoy! Sailing, Matey!");
    }

    public override void CollideSea(SeaBehavior sb)
    {
        base.CollideSea(sb);
		var posRelativeToSea = sb.transform.InverseTransformPoint(boatBehavior.transform.position);
        var seaVel = sb.VelocityAtX(posRelativeToSea.x);
        boatBehavior.rigidbody.velocity += seaVel;
        var distFromMax = Mathf.Abs(posRelativeToSea.x - sb.posOfMaxHeight.x);
        if (distFromMax <= 0.5f && sb.maxHeightWave != null)
        {
            boatBehavior.ExitState();
            boatBehavior.EnterState(new SurfinBoatState(boatBehavior, sb, sb.maxHeightWave));
        }
    }

    public override void FixedUpdate()
    {
        CollideSea(seaBehavior);
    }
}

public class SurfinBoatState : BoatState
{
	SeaBehavior seaBehavior;
	Wave wave;

	public SurfinBoatState(BoatBehavior bb, SeaBehavior sb, Wave w) : base(bb) { seaBehavior = sb; wave = w; }

	public override void Enter()
	{
		// Debug.Log("Surfin Mon!");
	}

	public override void FixedUpdate()
	{
		if (wave.killme)
		{
			boatBehavior.ExitState();
			return;
		}
		var posRelativeToSea = seaBehavior.transform.InverseTransformPoint(boatBehavior.transform.position);
        var diff = posRelativeToSea - (Vector3)wave.position;
		if (Mathf.Abs(diff.x) > 0.5)
		{
			boatBehavior.ExitState();
			return;
		}
		var heightOfWave = seaBehavior.HeightAtX(wave.position.x);
		var newPos = new Vector3(wave.position.x, heightOfWave+boatBehavior.halfSize.y, 0);
		var relPos = newPos - posRelativeToSea;
        // boatBehavior.rigidbody.velocity = new Vector2(wave.velocity.x, boatBehavior.rigidbody.velocity.y);
        boatBehavior.rigidbody.MovePosition(boatBehavior.transform.position + relPos);
	}

	public override void CollideSea(SeaBehavior sb)
	{
        base.CollideSea(sb);
        var posRelativeToSea = sb.transform.InverseTransformPoint(boatBehavior.transform.position);
		var seaVel = sb.VelocityAtX(posRelativeToSea.x);
		boatBehavior.rigidbody.velocity += seaVel * 5;
	}
}

public class BoatBehavior : MonoBehaviour
{
	public new BoxCollider2D collider;
	public new Rigidbody2D rigidbody;
    public Vector2 size;
    public Vector2 halfSize;

	public Stack<BoatState> states = new Stack<BoatState>();

	public void EnterState(BoatState boatState)
	{
        if (states.Count > 0)
        {
            states.Peek().Exit();
        }
		boatState.Enter();
		states.Push(boatState);
	}

	public void ExitState()
	{
		states.Pop().Exit();
        states.Peek().Enter();
	}

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
		states.Peek().FixedUpdate();
	}

    public void CollideSea(SeaBehavior sb)
	{
        if(sb == null) { return; }
        states.Peek().CollideSea(sb);
	}
}
