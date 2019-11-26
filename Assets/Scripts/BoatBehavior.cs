using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatBehavior : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		// Time.timeScale = 0.125f;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		var seas = GameObject.FindGameObjectsWithTag("Sea");
		foreach (var g in seas)
		{
			CheckCollideWithSea(g.GetComponent<SeaBehavior>());
		}
	}

	/// <summary>
	/// Sent when an incoming collider makes contact with this object's
	/// collider (2D physics only).
	/// </summary>
	/// <param name="other">The Collision2D data associated with this collision.</param>
	void CheckCollideWithSea(SeaBehavior sb)
	{
		var posRelativeToSea = sb.transform.InverseTransformPoint(transform.position);
		var waterLevel = sb.transform.position.y + sb.heightAtX(posRelativeToSea.x);
		var collider = GetComponent<BoxCollider2D>();
		var lower = transform.position - (Vector3)(collider.size / 2);
		if (lower.y > waterLevel) { return; }
		var rigidbody = GetComponent<Rigidbody2D>();
		if (Vector3.Dot(rigidbody.velocity, Vector3.up) != 0)
		{
			var h = Mathf.Min(waterLevel - lower.y, collider.size.y);
			var w = collider.size.x;
			var displacedWater = h * w;
			var waterFluidDensity = 50f;

			var bouyancy = displacedWater * waterFluidDensity * -Physics2D.gravity * Time.deltaTime;
			// var drag = dragForce(waterFluidDensity * 2) * Time.deltaTime;
			// bouyancy = bouyancy * (1 - Time.deltaTime * drag.magnitude);
			rigidbody.AddForce(bouyancy, ForceMode2D.Impulse);
			rigidbody.velocity *= 0.75f;

		}
		var seaVel = sb.velocityAtX(posRelativeToSea.x);
		rigidbody.velocity += seaVel * 10;
	}

	public float sign(float t)
	{
		return t / Mathf.Abs(t);
	}

	public Vector2 dragForce(float fluidDensity)
	{
		var rigidbody = GetComponent<Rigidbody2D>();
		float dragCoef = 0.5f * fluidDensity * rigidbody.mass;
		var vsqrd = new Vector2(-sign(rigidbody.velocity.x) * rigidbody.velocity.x * rigidbody.velocity.x, -sign(rigidbody.velocity.y) * rigidbody.velocity.y * rigidbody.velocity.y);
		var drag = dragCoef * vsqrd;
		return drag;
	}

}
