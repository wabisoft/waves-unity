using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBehavior : MonoBehaviour
{
	public Rigidbody2D rigidbody;
	private bool hitWater = false;
	private bool mouseDown = false;
	private Vector3 mouseStartPos = Vector2.zero;
	// Start is called before the first frame update
	void Start()
	{
		if (rigidbody == null)
		{
			rigidbody = GetComponent<Rigidbody2D>();
		}

	}

	// Update is called once per frame
	void FixedUpdate()
	{
		// if (hitWater)
		// {
		// 	var displacedWater = Mathf.PI * Mathf.Pow(GetComponent<CircleCollider2D>().radius, 2);
		// 	var buoyancy = displacedWater * 25.0f * -Physics2D.gravity * Time.deltaTime;
		//  rigidbody.AddForce(buoyancy, ForceMode2D.Impulse);
		// }
	}

	/// <summary>
	/// OnMouseDown is called when the user has pressed the mouse button while
	/// over the GUIElement or Collider.
	/// </summary>
	void OnMouseDown()
	{
		mouseDown = true;
		mouseStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		rigidbody.isKinematic = true;
	}

	/// <summary>
	/// OnMouseDrag is called when the user has clicked on a GUIElement or Collider
	/// and is still holding down the mouse.
	/// </summary>
	void OnMouseDrag()
	{
		var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		rigidbody.MovePosition(mousePos);
	}


	/// <summary>
	/// OnMouseUp is called when the user has released the mouse button.
	/// </summary>
	void OnMouseUp()
	{
		rigidbody.isKinematic = false;
		mouseDown = false;
		var mouseEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		var relpos = mouseEndPos - mouseStartPos;
		rigidbody.AddForce(relpos, ForceMode2D.Impulse);
	}

	/// <summary>
	/// Sent when another object enters a trigger collider attached to this
	/// object (2D physics only).
	/// </summary>
	/// <param name="other">The other Collider2D involved in this collision.</param>
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Sea" && !hitWater)
		{
			other.GetComponent<SeaBehavior>().createWave(other.gameObject.transform.InverseTransformPoint(transform.position));
			// Destroy(this);
			hitWater = true;
			// Destroy(this, 3);
		}
		if (other.gameObject.tag == "Boat")
		{
			Debug.Log("ded boat");
			Destroy(other.gameObject);
		}
	}
	/// <summary>
	/// Sent each frame where another object is within a trigger collider
	/// attached to this object (2D physics only).
	/// </summary>
	/// <param name="other">The other Collider2D involved in this collision.</param>
	void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.tag == "Sea")
		{
		}
	}

}
