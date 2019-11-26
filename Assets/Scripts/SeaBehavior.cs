using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Wave
{
	const float E = 2.71828182845904523536f; // exponential constant
	const float WAVE_WIDTH_MULTIPLIER = 0.55f;
	const float WAVE_POSITIVE_DECAY_MULTIPLIER = 1;
	const float WAVE_NEGATIVE_DECAY_MULTIPLIER = -0.25f;

	public Wave(Vector2 pos, float amp, int dir, int a_sign)
	{
		position = pos;
		amplitude = amp;
		direction = dir;
		sign = a_sign;
	}
	public Vector2 position = new Vector2(0, 0);
	public Vector2 velocity = new Vector2(0, 0);
	float amplitude = 0;
	float decay = 0;
	float time = 0;
	int direction = 1;
	int sign = 1;
	bool grow = true;
	public bool killme = false;
	public void update(float deltaTime)
	{
		time += deltaTime * 0.25f;
		if (decay >= 1)
		{
			grow = false;
		}

		else if ((decay <= 0 && !grow))
		{
			// delete this wave
			killme = true;
		}
		if (decay <= 1 && grow)
		{
			decay += deltaTime * WAVE_POSITIVE_DECAY_MULTIPLIER;
		}
		else
		{
			decay += deltaTime * WAVE_NEGATIVE_DECAY_MULTIPLIER;
		}
		// Update velocity with timestep
		// velocity.x += deltaTime * 0.05f * amplitude;
		velocity.x += deltaTime * 0.05f * amplitude;
		// velocity += dragForce(velocity, 1.225f, .4f);
		// velocity.x *= 0.95f;
		// Update position with velocity;
		position += (float)direction * velocity;
	}

	public float minimumX()
	{
		return position.x - 2.3f * (1 / WAVE_WIDTH_MULTIPLIER); // solve for heightAtX == 0
	}

	public float maximumX()
	{
		return position.x + 2.3f * (1 / WAVE_WIDTH_MULTIPLIER);
	}

	public float heightAtX(float x)
	{
		return sign * amplitude * decay * Mathf.Pow(E, -Mathf.Pow(WAVE_WIDTH_MULTIPLIER * (x - position.x), 2));
	}
	public float slopeAtX(float x)
	{
		// derivative of height
		return heightAtX(x) * (-2 * WAVE_WIDTH_MULTIPLIER * (x - position.x) * WAVE_WIDTH_MULTIPLIER);
	}

	public Vector2 velocityAtX(float x)
	{
		float distFromMid = Mathf.Abs(x - position.x);
		float max = maximumX();
		if (max < x) { return Vector2.zero; }
		float min = minimumX();
		if (min > x) { return Vector2.zero; }
		float halfLength = (max - min) / 2.0f;
		return ((halfLength - distFromMid) / halfLength) * direction * velocity;
	}
}

public class SeaBehavior : MonoBehaviour
{
	public List<Wave> waves = new List<Wave>();
	public BoxCollider2D collider;
	private float maxHeight;
	// public SeaPrefab prefab; // prefab is not a great name, TODO: change to SeaState or SeaData or something like that

	/// <summary>
	/// Start is called on the frame when a script is enabled just before
	/// any of the Update methods is called the first time.
	/// </summary>
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			var localCoordinates = transform.InverseTransformPoint(mousePos);
			createWave(localCoordinates);
		}
		for (int i = 0; i < waves.Count; ++i)
		{
			waves[i].update(Time.deltaTime);
			if (waves[i].killme)
			{
				waves.RemoveAt(i);
			}
		}
		var size = collider.size;
		var halfSize = size / 2.0f;
		var maxHeight = 0.1f;
		for (int i = 0; i < waves.Count; ++i)
		{
			maxHeight = Mathf.Max(maxHeight, heightAtX(waves[i].position.x));
		}
		size.y = maxHeight;
		collider.size = size;
		collider.offset = new Vector2(collider.offset.x, maxHeight / 2);
		for (float i = -halfSize.x; i < halfSize.x - 0.5f; i += 0.5f)
		{
			var p = transform.TransformPoint(new Vector3(i, heightAtX(i), 0));
			var q = transform.TransformPoint(new Vector3(i + 0.5f, heightAtX(i + 0.5f)));
			Debug.DrawLine(p, q);
		}
		// var segments = prefab.segments;
		// for (int i = 0; i < segments.Count; ++i)
		// {
		// 	// col.points[i].y = heightAtX(col.points[i].x);

		// 	var pos = segments[i].transform.localPosition;
		// 	var m = slopeAtX(pos.x);
		// 	var rotationRadians = Mathf.Atan(m);
		// 	Quaternion q = new Quaternion();
		// 	q.eulerAngles = new Vector3(0, 0, rotationRadians * Mathf.Rad2Deg);
		// 	segments[i].transform.localRotation = q;
		// 	var scale = transform.localScale;
		// 	scale.x += m;
		// 	transform.localScale = scale;
		// 	segments[i].transform.localPosition = new Vector3(pos.x, heightAtX(pos.x), 0);
		// }
	}
	public float heightAtX(float x)
	{
		float height = 0f;
		foreach (var wave in waves)
		{
			height += wave.heightAtX(x);
		}
		return height;
	}

	public float slopeAtX(float x)
	{
		float slope = 0;
		foreach (var wave in waves)
		{
			slope += wave.slopeAtX(x);
		}
		return slope;
	}

	public Vector2 velocityAtX(float x)
	{
		Vector2 velocity = Vector2.zero;
		foreach (Wave wave in waves)
		{
			velocity += wave.velocityAtX(x);
		}
		return velocity;
	}


	public void createWave(Vector2 position)
	{
		Debug.Log(position);
		waves.Add(new Wave(position, 4, 1, 1));
	}
	/// <summary>
	/// Callback to draw gizmos that are pickable and always drawn.
	/// </summary>
	void OnDrawGizmos()
	{
		var halfSize = collider.size / 2;
		for (float i = -halfSize.x; i < halfSize.x - 0.1f; i += 0.1f)
		{
			Gizmos.color = Color.blue;
			var p = transform.TransformPoint(new Vector3(i, heightAtX(i), 0));
			var q = transform.TransformPoint(new Vector3(i + 0.1f, heightAtX(i + 0.1f)));
			Gizmos.DrawLine(p, q);
		}
	}
}
