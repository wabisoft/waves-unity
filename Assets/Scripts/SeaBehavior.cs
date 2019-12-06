using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Wave
{
	const float E = 2.71828182845904523536f; // exponential constant

	public Wave(SeaBehavior sb, Vector2 pos, float amp, int dir, int a_sign)
	{
		seaBehavior = sb;
		position = pos;
		amplitude = amp;
		direction = dir;
		sign = a_sign;
	}

	public SeaBehavior seaBehavior;
	public Vector2 position = new Vector2(0, 0);
	public Vector2 velocity = new Vector2(0, 0);
	public float amplitude = 0;
	public float decay = 0;
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
			decay += deltaTime * seaBehavior.growFactor;
		}
		else
		{
			decay += deltaTime * seaBehavior.decayFactor;
		}
		// Update velocity with timestep
		// velocity.x += deltaTime * 0.05f * amplitude;
		velocity.x += deltaTime * 0.05f * amplitude;
		// velocity += dragForce(velocity, 1.225f, .4f);
		velocity.x *= 0.95f;
		// Update position with velocity;
		position += (float)direction * velocity;
	}

	public float minimumX()
	{
		return position.x - 2.3f * (1 / seaBehavior.WaveWidth); // solve for heightAtX == 0
	}

	public float maximumX()
	{
		return position.x + 2.3f * (1 / seaBehavior.WaveWidth);
	}

	public float heightAtX(float x)
	{
		return sign * amplitude * decay * Mathf.Pow(E, -Mathf.Pow(seaBehavior.WaveWidth * (x - position.x), 2));
	}

	public float slopeAtX(float x)
	{
		// derivative of height
		return heightAtX(x) * (-2 * seaBehavior.WaveWidth * (x - position.x) * seaBehavior.WaveWidth);
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
	public new BoxCollider2D collider;
	private float maxHeight;
	public Vector2 posOfMaxHeight = Vector2.zero;
	public Wave maxHeightWave;
	public float WaveWidth = 0.55f;
	public float growFactor = 1;
	public float decayFactor = -0.25f;


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
		// if (Input.GetMouseButtonDown(0))
		// {
		// 	var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		// 	var localCoordinates = transform.InverseTransformPoint(mousePos);
		// 	createWave(localCoordinates);
		// }
		maxHeight = 0.1f;
		posOfMaxHeight = Vector2.zero;
		for (int i = 0; i < waves.Count; ++i)
		{
			waves[i].update(Time.deltaTime);
			if (waves[i].killme)
			{
				waves.RemoveAt(i);
				continue;
			}
			var heightAtWavePos = heightAtX(waves[i].position.x);
			if (maxHeight < heightAtWavePos)
			{
				maxHeight = heightAtWavePos;
				posOfMaxHeight = waves[i].position;
				maxHeightWave = waves[i];
			}
		}
		var size = collider.size;
		var halfSize = size / 2.0f;
		size.y = maxHeight;
		collider.size = size;
		collider.offset = new Vector2(collider.offset.x, maxHeight / 2);
	}

	/// <summary>
	/// LateUpdate is called every frame, if the Behaviour is enabled.
	/// It is called after all Update functions have been called.
	/// </summary>
	void LateUpdate()
	{
		var size = collider.size;
		var halfSize = size / 2.0f;
		size.y = maxHeight;
		collider.size = size;
		collider.offset = new Vector2(collider.offset.x, maxHeight / 2);
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
		waves.Add(new Wave(this, position, 4, 1, 1));
	}

	/// <summary>
	/// Callback to draw gizmos that are pickable and always drawn.
	/// </summary>
	void OnDrawGizmos()
	{
		var halfSize = collider.size / 2;
		var step = 0.5f;
		for (float i = -halfSize.x; i < halfSize.x - step; i += step)
		{
			Gizmos.color = Color.blue;
			var p = transform.TransformPoint(new Vector3(i, heightAtX(i), 0));
			var q = transform.TransformPoint(new Vector3(i + step, heightAtX(i + step)));
			Gizmos.DrawLine(p, q);
		}
	}
}
