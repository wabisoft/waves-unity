using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Wave
{
	const float E = 2.71828182845904523536f; // exponential constant

	public Wave(SeaBehavior sb, Vector2 pos, float amp, int dir, int s)
	{
		seaBehavior = sb;
		position = pos;
		amplitude = amp;
		direction = dir;
		sign = s;
	}

	public SeaBehavior seaBehavior;
	public Vector2 position = new Vector2(0, 0);
	public Vector2 velocity = new Vector2(0, 0);
	public float amplitude = 0;
	public float decay = 0;
	public int sign = 1;
	int direction = 1;
	bool grow = true;
	public bool killme = false;

	public void Update(float deltaTime)
	{
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

	public float MinimumX()
	{
		return position.x - 2.3f * (1 / seaBehavior.WaveWidth); // solve for HeightAtX == 0
	}

	public float MaximumX()
	{
		return position.x + 2.3f * (1 / seaBehavior.WaveWidth);
	}

	public float HeightAtX(float x)
	{
		return sign * amplitude * decay * Mathf.Pow(E, -Mathf.Pow(seaBehavior.WaveWidth * (x - position.x), 2));
	}

	public float SlopeAtX(float x)
	{
		// derivative of height
		return HeightAtX(x) * (-2 * seaBehavior.WaveWidth * (x - position.x) * seaBehavior.WaveWidth);
	}

	public Vector2 VelocityAtX(float x)
	{
		float distFromMid = Mathf.Abs(x - position.x);
		float max = MaximumX();
		if (max < x) { return Vector2.zero; }
		float min = MinimumX();
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

	void Start()
	{
	}

	void Update()
	{
		// if (Input.GetMouseButtonDown(0))
		// {
		// 	var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		// 	var localCoordinates = transform.InverseTransformPoint(mousePos);
		// 	CreateWave(localCoordinates);
		// }
		maxHeight = 0f;
		posOfMaxHeight = Vector2.zero;
		for (int i = 0; i < waves.Count; ++i)
		{
			waves[i].Update(Time.deltaTime);
			if (waves[i].killme)
			{
				waves.RemoveAt(i);
				continue;
			}
			var heightAtWavePos = HeightAtX(waves[i].position.x);
			if (maxHeight < heightAtWavePos)
			{
				maxHeight = heightAtWavePos;
				posOfMaxHeight = waves[i].position;
				maxHeightWave = waves[i];
			}
		}
        if (maxHeight == 0f)
        {
            maxHeight = 0.1f;
        }
		var size = collider.size;
		var halfSize = size / 2.0f;
		size.y = maxHeight;
		collider.size = size;
        var yOffset = maxHeight / 2;
        if(yOffset < 0.1f) { yOffset = 0f; }
        collider.offset = new Vector2(collider.offset.x, yOffset);
	}

	public float HeightAtX(float x)
	{
		float height = 0f;
		foreach (var wave in waves)
		{
			height += wave.HeightAtX(x);
		}
		return height;
	}

	public float SlopeAtX(float x)
	{
		float slope = 0;
		foreach (var wave in waves)
		{
			slope += wave.SlopeAtX(x);
		}
		return slope;
	}

	public Vector2 VelocityAtX(float x)
	{
		Vector2 velocity = Vector2.zero;
		foreach (Wave wave in waves)
		{
			velocity += wave.VelocityAtX(x);
		}
		return velocity;
	}

	public void CreateWave(Vector2 position, float amplitude, int direction, int sign)
	{
		waves.Add(new Wave(this, position, amplitude, direction, sign));
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        var boat = collider.gameObject.GetComponent<BoatBehavior>();
        if(boat != null) { boat.CollideSea(this); }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        var boat = collider.gameObject.GetComponent<BoatBehavior>();
        if(boat != null) { boat.CollideSea(this); }
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
			var p = transform.TransformPoint(new Vector3(i, HeightAtX(i), 0));
			var q = transform.TransformPoint(new Vector3(i + step, HeightAtX(i + step)));
			Gizmos.DrawLine(p, q);
		}
	}
}
