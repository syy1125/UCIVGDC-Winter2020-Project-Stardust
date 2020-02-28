using System;
using UnityEngine;

[Serializable]
public struct ShipCost
{
	public Resource Resource;
	public int Amount;
}

[CreateAssetMenu(fileName = "Spaceship", menuName = "Scriptable Objects/Spaceship")]
public class SpaceshipTemplate : ScriptableObject
{
	public ShipCost[] Cost;
}