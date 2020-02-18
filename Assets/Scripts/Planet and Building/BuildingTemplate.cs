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

[CreateAssetMenu(menuName = "Scriptable Objects/Building", fileName = "Building")]
public class BuildingTemplate : ScriptableObject
{
	public string DisplayName;
	public BuildingTile[] Tiles;

	public Vector2 GetCenterOfMass()
	{
		return Tiles.Aggregate(Vector2.zero, (current, tile) => current + tile.Offset) / Tiles.Length;
	}
}