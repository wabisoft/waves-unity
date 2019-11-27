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
	public abstract void CheckCollideWithSea(SeaBehavior sb);
}


public class DefaultBoatState : BoatState
{
	public DefaultBoatState(BoatBehavior bb) : base(bb) { }

	public override void FixedUpdate()
	{
		// Do nothing extra?
	}

	public override void CheckCollideWithSea(SeaBehavior sb)
	{
		var posRelativeToSea = sb.transform.InverseTransformPoint(boatBehavior.transform.position);
		var waterLevel = sb.transform.position.y + sb.heightAtX(posRelativeToSea.x);
		var lower = boatBehavior.transform.position - (Vector3)(boatBehavior.collider.size / 2);
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
		var seaVel = sb.velocityAtX(posRelativeToSea.x);
		rigidbody.velocity += seaVel;
		var distFromMax = Mathf.Abs(posRelativeToSea.x - sb.posOfMaxHeight.x);
		if (distFromMax <= 0.5f && sb.maxHeightWave != null)
		{
			boatBehavior.EnterState(new SurfinBoatState(boatBehavior, sb, sb.maxHeightWave));
		}
	}
}

public class SurfinBoatState : BoatState
{
	SeaBehavior seaBehavior;
	Wave wave;

	public SurfinBoatState(BoatBehavior bb, SeaBehavior sb, Wave w) : base(bb) { seaBehavior = sb; wave = w; }

	public override void Enter()
	{
		Debug.Log("Surfin Mon!");
	}

	public override void FixedUpdate()
	{
		if (wave.killme)
		{
			boatBehavior.ExitState();
			return;
		}
		var posRelativeToSea = seaBehavior.transform.InverseTransformPoint(boatBehavior.transform.position);
		if ((posRelativeToSea.x - wave.position.x) > 1)
		{
			boatBehavior.ExitState();
			return;
		}
		var heightOfWave = seaBehavior.heightAtX(wave.position.x);
		var newPos = new Vector3(wave.position.x, heightOfWave, 0);
		var relPos = newPos - posRelativeToSea;
		boatBehavior.transform.Translate(relPos * 0.5f);
	}

	public override void CheckCollideWithSea(SeaBehavior sb)
	{

		var posRelativeToSea = sb.transform.InverseTransformPoint(boatBehavior.transform.position);
		var waterLevel = sb.transform.position.y + sb.heightAtX(posRelativeToSea.x);
		var lower = boatBehavior.transform.position - (Vector3)(boatBehavior.collider.size / 2);
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
		var seaVel = sb.velocityAtX(posRelativeToSea.x);
		rigidbody.velocity += seaVel * 5;
	}
}

public class BoatBehavior : MonoBehaviour
{
	public BoxCollider2D collider;
	public Rigidbody2D rigidbody;

	public Stack<BoatState> states = new Stack<BoatState>();

	public void EnterState(BoatState boatState)
	{
		boatState.Enter();
		states.Push(boatState);
	}

	public void ExitState()
	{
		states.Pop().Exit();
	}

	// Start is called before the first frame update
	void Start()
	{
		collider = GetComponent<BoxCollider2D>();
		rigidbody = GetComponent<Rigidbody2D>();
		// Time.timeScale = 0.125f;
		EnterState(new DefaultBoatState(this));
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		var seas = GameObject.FindGameObjectsWithTag("Sea");
		foreach (var g in seas)
		{
			states.Peek().CheckCollideWithSea(g.GetComponent<SeaBehavior>());
		}
		states.Peek().FixedUpdate();
	}

	/// <summary>
	/// Sent when an incoming collider makes contact with this object's
	/// collider (2D physics only).
	/// </summary>
	/// <param name="other">The Collision2D data associated with this collision.</param>
	void CheckCollideWithSea(SeaBehavior sb)
	{

	}

	public float sign(float t)
	{
		return t / Mathf.Abs(t);
	}

}
