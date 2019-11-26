using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaPrefab : MonoBehaviour
{
	public float extents = 10;
	public GameObject segmentPrefab;
	public List<GameObject> segments;

	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
		var dummySegment = GameObject.Instantiate(segmentPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		var segmentExtents = dummySegment.GetComponent<BoxCollider2D>().bounds.extents.x;
		var segmentSize = segmentExtents;
		if (segmentSize == 0)
		{
			Debug.Log("Sea Segment Size is zero. I can't do anything with that!");
			return;
		}
		DestroyImmediate(dummySegment);
		var numSegments = extents / segmentSize;
		Debug.Log($"Segment Size: {segmentSize}");
		Debug.Log($"Number of Segments: {numSegments}");
		var halfNumSegments = (int)numSegments / 2;
		Debug.Log($"half number of Segments: {halfNumSegments}");
		for (int i = -halfNumSegments; i < halfNumSegments; ++i)
		{
			var newSegment = GameObject.Instantiate(segmentPrefab, transform.position, Quaternion.identity) as GameObject;
			float newSegmentPosX = ((float)i * segmentSize) - (segmentSize / 2.0f);
			newSegment.transform.parent = this.transform;
			newSegment.transform.localPosition = Vector3.zero;
			newSegment.transform.Translate(new Vector3(newSegmentPosX, 0, 0));
			segments.Add(newSegment);
		}
	}
}