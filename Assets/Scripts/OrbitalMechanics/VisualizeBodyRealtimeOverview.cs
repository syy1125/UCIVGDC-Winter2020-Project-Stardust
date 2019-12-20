using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class VisualizeBodyRealtimeOverview : MonoBehaviour
{
	public CelestialBody Body;
	
	private void Update()
	{
		if (Body == null) return;

		// transform.position = Body.GetGlobalPositionAt(Application.isPlaying ? Time.time : 0);
	}
}
