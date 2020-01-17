using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class TestScripts : MonoBehaviour
{
	public CelestialBody Body;
	public GameObject Earth;
	public GameObject Mars;

	public GameObject OrbitPrefab;
	public GameObject VesselPrefab;

	public void TestStateVectorToOrbit()
	{
		(Vector3 re, Vector3 ve) = Earth.GetComponent<VisualizeBodyRealtimeOverview>()
			.Body.GetGlobalPositionAndVelocityAt(Time.time);

		Orbit orbit = Orbit.FromPositionAndVelocity(Body, re, ve * 1.1f, Time.time);
		var body = ScriptableObject.CreateInstance<CelestialBody>();
		body.SetOrbit(orbit);

		GameObject orbitVisual = Instantiate(OrbitPrefab);
		orbitVisual.GetComponent<VisualizeOrbit>().Body = body;
		GameObject vesselVisual = Instantiate(VesselPrefab);
		vesselVisual.GetComponent<VisualizeBodyRealtimeOverview>().Body = body;
	}

	public void TestLambertSolver()
	{
		(Vector3 re, Vector3 ve) = Earth.GetComponent<VisualizeBodyRealtimeOverview>()
			.Body.GetGlobalPositionAndVelocityAt(Time.time);
		(Vector3 rm, Vector3 vm) = Mars.GetComponent<VisualizeBodyRealtimeOverview>()
			.Body.GetGlobalPositionAndVelocityAt(Time.time + 8);

		LambertSolver.Solve(
			Body.GravitationalParameter,
			re, ve,
			rm, vm,
			8,
			0,
			out Vector3 vi,
			out Vector3 vf
		);

		Orbit orbit = Orbit.FromPositionAndVelocity(Body, re, vi, Time.time);
		var body = ScriptableObject.CreateInstance<CelestialBody>();
		body.SetOrbit(orbit);

		GameObject orbitVisual = Instantiate(OrbitPrefab);
		orbitVisual.GetComponent<VisualizeOrbit>().Body = body;
		GameObject vesselVisual = Instantiate(VesselPrefab);
		vesselVisual.GetComponent<VisualizeBodyRealtimeOverview>().Body = body;
	}
}