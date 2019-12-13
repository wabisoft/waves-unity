using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBehavior : MonoBehaviour
{
    public bool isNegative = false;
	public new Rigidbody2D rigidbody;
	public bool hitWater = false;
	private Vector2 throwStartPos, throwEndPos;
	private float throwStartTime, throwEndTime, throwTimeInterval;
	// Start is called before the first frame Update
	[Range(0.05f, 1f)]
	public float throwForce = 0.05f;
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

	void OnMouseDown()
	{
		rigidbody.isKinematic = true;
		throwStartTime = Time.time;
		throwStartPos = Input.mousePosition;
	}

	void OnMouseUp()
	{
		rigidbody.isKinematic = false;
		throwEndTime = Time.time;
		throwTimeInterval = throwEndTime - throwStartTime;
		throwEndPos = Input.mousePosition;
		var direction = throwEndPos - throwStartPos;
		rigidbody.AddForce((direction / throwTimeInterval) * throwForce);

	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "Sea" && !hitWater)
		{
			other.GetComponent<SeaBehavior>().CreateWave(other.gameObject.transform.InverseTransformPoint(transform.position), rigidbody.velocity.magnitude, (int)Mathf.Sign(rigidbody.velocity.x), (isNegative)? -1 : 1);
			hitWater = true;
            rigidbody.drag += 3;
            rigidbody.angularDrag += 3;
			// Destroy(this, 3);
		}
		if (other.gameObject.tag == "Boat")
		{
			Debug.Log("ded boat");
			Destroy(other.gameObject);
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (other.gameObject.tag == "Sea")
		{
		}
	}

}
