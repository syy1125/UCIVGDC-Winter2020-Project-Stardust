using System;
using System.Linq;
using UnityEngine;
using UnityEngine.WSA;

[Serializable]
public struct BuildingTile
{
	public Sprite Sprite;
	public Vector2Int Offset;
}

[Serializable]
public struct BuildingEffect
{
	public enum EffectType
	{
		Produce,
		Consume,
		CapacityAdd
	}

	public EffectType Type;
	public Resource Resource;
	public int Amount;
}

// Represents a group of effects that must activate/deactivate at the same time.
// Useful for when buildings do multiple things, some dependent on a certain input while others don't.
// In that case, some effects can be disabled without completely disabling the building.
[Serializable]
public struct EffectGroup
{
	public BuildingEffect[] Effects;
}

[CreateAssetMenu(menuName = "Scriptable Objects/Building", fileName = "Building")]
public class BuildingTemplate : ScriptableObject
{
	public string DisplayName;
	public BuildingTile[] Tiles;
	public EffectGroup[] EffectGroups;

	public Vector2 GetCenterOfMass()
	{
		return Tiles.Aggregate(Vector2.zero, (current, tile) => current + tile.Offset) / Tiles.Length;
	}
}