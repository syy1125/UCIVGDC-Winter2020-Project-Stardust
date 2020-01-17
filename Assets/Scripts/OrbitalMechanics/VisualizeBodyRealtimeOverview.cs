using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class VisualizeBodyRealtimeOverview : MonoBehaviour
{
	public CelestialBody Body;
	public float TimeMultiplier;
	
	private void Update()
	{
		if (Body == null) return;

		transform.position = Body.GetGlobalPositionAndVelocityAt(Application.isPlaying ? Time.time * TimeMultiplier : 0).Item1;
	}
}
