﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlanetBuildingController : MonoBehaviour
{
	public CelestialBody Body;

	// Represents an instance of a constructed building.
	public class BuildingInstance
	{
		public readonly BuildingTemplate BuildingTemplate;
		public readonly int Rotation; // Number of times rotated counter-clockwise

		public BuildingInstance(BuildingTemplate buildingTemplate, int rotation)
		{
			BuildingTemplate = buildingTemplate;
			Rotation = rotation;
		}
	}

	private Dictionary<BuildingInstance, Vector2Int> _buildingOrigin;
	private Dictionary<Vector2Int, BuildingInstance> _slotToBuilding;

	private void Awake()
	{
		_buildingOrigin = new Dictionary<BuildingInstance, Vector2Int>();
		_slotToBuilding = new Dictionary<Vector2Int, BuildingInstance>();
	}

	public void ConstructBuilding(BuildingTemplate template, Vector2Int origin, int rotation)
	{
		if (!IsValidBuilding(template, origin, rotation))
		{
			Debug.LogError(
				$"Attempting to build invalid configuration.\n"
				+ $"Building{template.DisplayName} origin {origin} rotation {rotation}"
			);
			return;
		}

		var building = new BuildingInstance(template, rotation);
		_buildingOrigin[building] = origin;

		foreach (BuildingTile tile in template.Tiles)
		{
			Vector2Int offset = Rotate(tile.Offset, rotation);
			_slotToBuilding[origin + offset] = building;
		}
	}

	public bool IsValidBuilding(BuildingTemplate template, Vector2Int origin, int rotation)
	{
		if (template == null) return false;

		foreach (BuildingTile tile in template.Tiles)
		{
			Vector2Int position = origin + Rotate(tile.Offset, rotation);
			if (IsOccupied(position)) return false;
			if (!InBounds(position)) return false;
		}

		return true;
	}

	public bool IsOccupied(Vector2Int position)
	{
		return _slotToBuilding.ContainsKey(position);
	}

	public bool InBounds(Vector2Int position)
	{
		return position.x >= 0
		       && position.x < Body.BuildingGridWidth
		       && position.y >= 0
		       && position.y < Body.BuildingGridHeight;
	}

	public Tuple<Sprite, int> GetSpriteAndRotationAt(Vector2Int position)
	{
		if (!IsOccupied(position)) return null;

		BuildingInstance building = _slotToBuilding[position];
		Vector2Int origin = _buildingOrigin[building];
		Vector2Int offset = InverseRotate(position - origin, building.Rotation);

		return new Tuple<Sprite, int>(
			building.BuildingTemplate.Tiles.First(tile => tile.Offset == offset).Sprite,
			building.Rotation
		);
	}

	public static Vector2Int Rotate(Vector2Int vector, int rotation)
	{
		switch (rotation % 4)
		{
			case 0:
				return vector;
			case 1:
				return new Vector2Int(-vector.y, vector.x);
			case 2:
				return vector * -1;
			case 3:
				return new Vector2Int(vector.y, -vector.x);
			default:
				throw new Exception($"Unexpected branch reached in Rotate. Vector={vector} Rotations={rotation}");
		}
	}

	public static Vector2Int InverseRotate(Vector2Int vector, int rotation)
	{
		return Rotate(vector, 4 - rotation);
	}
}