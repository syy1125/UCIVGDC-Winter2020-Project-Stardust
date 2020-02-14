using System;
using UnityEngine;

[Serializable]
public struct BuildingTile
{
	public Sprite Sprite;
	public Vector3 Offset;
}

[CreateAssetMenu(menuName = "Scriptable Objects/Building", fileName = "Building")]
public class Building : ScriptableObject
{
	public BuildingTile[] Tiles;
}