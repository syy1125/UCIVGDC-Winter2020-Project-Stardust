﻿using System;
using UnityEngine;

[Serializable]
public struct BuildingTile
{
	public Sprite Sprite;
	public Vector2Int Offset;
}

[CreateAssetMenu(menuName = "Scriptable Objects/Building", fileName = "Building")]
public class Building : ScriptableObject
{
	public BuildingTile[] Tiles;
}