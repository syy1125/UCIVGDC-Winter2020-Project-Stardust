using UnityEngine;

public class Transfer
{
	// Game and animation data
	public float LaunchTime;
	public CelestialBody LaunchBody;
	public float ArrivalTime;
	public CelestialBody TargetBody;
	
	// delta-v data
	public float LaunchCostGravity;
	public float LaunchCostOrbitSpeed;
	public float LaunchCostDrag;

	public float LaunchCost =>
		Mathf.Sqrt(LaunchCostGravity * LaunchCostGravity + LaunchCostOrbitSpeed * LaunchCostOrbitSpeed)
		+ LaunchCostDrag;

	public float TransferCostEscape;
	public float TransferCostArrival;

	public float TransferCost => TransferCostEscape + TransferCostArrival;

	public float LandingCostGravity;
	public float LandingCostOrbitSpeed;
	public float LandingCostDrag;

	public float LandingCost =>
		Mathf.Sqrt(LandingCostGravity * LandingCostGravity + LandingCostOrbitSpeed * LandingCostOrbitSpeed)
		+ LandingCostDrag;
}