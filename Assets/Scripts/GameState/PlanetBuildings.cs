using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.XR;

public struct EffectGroupInstance
{
	private readonly EffectGroup _effectGroup;
	public BuildingEffect[] Effects => _effectGroup.Effects;

	public EffectGroupInstance(EffectGroup effectGroup)
	{
		_effectGroup = effectGroup;
	}
}

// Represents an instance of a constructed building.
public class BuildingInstance : ISaveLoad<BuildingInstance.Serialized>
{
	[Serializable]
	public struct Serialized
	{
		public string TemplatePath;
		public int Rotation;
	}

	public BuildingTemplate Template;
	public EffectGroupInstance[] EffectGroups;
	public int Rotation; // Number of times rotated counter-clockwise

	public BuildingInstance(BuildingTemplate template, int rotation)
	{
		Template = template;
		EffectGroups = template.EffectGroups.Select(group => new EffectGroupInstance(group)).ToArray();
		Rotation = rotation;
	}

	// FOR SAVE/LOAD USE ONLY
	public BuildingInstance()
	{}

	public Serialized Save()
	{
		return new Serialized
		{
			TemplatePath = AssetDatabase.GetAssetPath(Template),
			Rotation = Rotation
		};
	}

	public void Load(Serialized serialized)
	{
		Template = AssetDatabase.LoadAssetAtPath<BuildingTemplate>(serialized.TemplatePath);
		EffectGroups = Template.EffectGroups.Select(group => new EffectGroupInstance(group)).ToArray();
		Rotation = serialized.Rotation;
	}
}

public class PlanetBuildings : ISaveLoad<PlanetBuildings.Serialized>
{
	[Serializable]
	public struct Serialized
	{
		public BuildingInstance.Serialized[] Buildings;
		public Vector2Int[] BuildingOrigins;
	}

	private readonly Planet _planet;
	private CelestialBody Body => _planet.Body;

	private readonly Dictionary<BuildingInstance, Vector2Int> _buildingOrigin =
		new Dictionary<BuildingInstance, Vector2Int>();
	private readonly Dictionary<Vector2Int, BuildingInstance> _slotToBuilding =
		new Dictionary<Vector2Int, BuildingInstance>();

	public PlanetBuildings(Planet planet)
	{
		_planet = planet;
	}

	public int GridWidth => Body.BuildingGridWidth;
	public int GridHeight => Body.BuildingGridHeight;

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

	public BuildingInstance[] GetBuildings()
	{
		return _buildingOrigin.Keys.ToArray();
	}

	public Tuple<Sprite, int> GetSpriteAndRotationAt(Vector2Int position)
	{
		if (!IsOccupied(position)) return null;

		BuildingInstance building = _slotToBuilding[position];
		Vector2Int origin = _buildingOrigin[building];
		Vector2Int offset = InverseRotate(position - origin, building.Rotation);

		return new Tuple<Sprite, int>(
			building.Template.Tiles.First(tile => tile.Offset == offset).Sprite,
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

	public Serialized Save()
	{
		var buildings = new List<BuildingInstance.Serialized>(_buildingOrigin.Count);
		var buildingOrigins = new List<Vector2Int>(_buildingOrigin.Count);

		foreach (KeyValuePair<BuildingInstance, Vector2Int> entry in _buildingOrigin)
		{
			buildings.Add(entry.Key.Save());
			buildingOrigins.Add(entry.Value);
		}

		return new Serialized {Buildings = buildings.ToArray(), BuildingOrigins = buildingOrigins.ToArray()};
	}

	public void Load(Serialized serialized)
	{
		_buildingOrigin.Clear();
		_slotToBuilding.Clear();

		for (int i = 0; i < serialized.Buildings.Length; i++)
		{
			var building = new BuildingInstance();
			building.Load(serialized.Buildings[i]);

			_buildingOrigin.Add(building, serialized.BuildingOrigins[i]);
		}
	}
}