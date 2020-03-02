using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct ShipCost
{
	public Resource Resource;
	public int Amount;
}

[CreateAssetMenu(fileName = "Spaceship", menuName = "Scriptable Objects/Spaceship")]
public class SpaceshipTemplate : ScriptableObject
{
	public string DisplayName;
	[FormerlySerializedAs("Mass")]
	public int DryMass;
	[FormerlySerializedAs("Cost")]
	public ShipCost[] Costs;
}