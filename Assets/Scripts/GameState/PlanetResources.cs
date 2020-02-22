using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

public class PlanetResources : ISaveLoad<PlanetResources.Serialized>
{
	[Serializable]
	public struct Serialized
	{
		public string[] ResourcePaths;
		public int[] Amount;
	}

	private readonly PlanetBuildings _buildingController;

	private readonly Dictionary<Resource, int> _storage = new Dictionary<Resource, int>();

	public PlanetResources(PlanetBuildings buildingController)
	{
		_buildingController = buildingController;
	}

	public int this[Resource resource]
	{
		get => _storage.TryGetValue(resource, out int amount) ? amount : 0;
		set => _storage[resource] = value;
	}

	public int GetRawProduction(Resource resource)
	{
		int production = 0;
		foreach (BuildingInstance building in _buildingController.GetBuildings())
		{
			foreach (EffectGroupInstance group in building.EffectGroups)
			{
				production += group.Effects
					.Where(effect => effect.Type == BuildingEffect.EffectType.Produce && effect.Resource == resource)
					.Sum(effect => effect.Amount);
			}
		}

		return production;
	}

	public int GetRawConsumption(Resource resource)
	{
		int consumption = 0;

		foreach (BuildingInstance building in _buildingController.GetBuildings())
		{
			foreach (EffectGroupInstance group in building.EffectGroups)
			{
				consumption += group.Effects
					.Where(effect => effect.Type == BuildingEffect.EffectType.Consume && effect.Resource == resource)
					.Sum(effect => effect.Amount);
			}
		}

		return consumption;
	}

	public Dictionary<Resource, int> GetIdealResourceDelta()
	{
		var resourceDelta = new Dictionary<Resource, int>();
		var resourceConsumers = new Dictionary<Resource, List<Tuple<BuildingInstance, EffectGroupInstance>>>();
		foreach (BuildingInstance building in _buildingController.GetBuildings())
		{
			ComposeProductionEffect(resourceDelta, resourceConsumers, building);
		}

		return resourceDelta;
	}

	public Dictionary<Resource, int> GetIdealResourceCapacity()
	{
		var disabledGroups = new HashSet<EffectGroupInstance>();
		return ComputeResourceCapacity(_buildingController.GetBuildings(), disabledGroups);
	}

	public void DoTurnLogic()
	{
		var resourceDelta = new Dictionary<Resource, int>();
		var resourceConsumers = new Dictionary<Resource, List<Tuple<BuildingInstance, EffectGroupInstance>>>();
		var disabledGroups = new HashSet<EffectGroupInstance>();

		BuildingInstance[] buildings = _buildingController.GetBuildings();

		if (buildings.Length > 0)
		{
			foreach (BuildingInstance building in buildings)
			{
				ComposeProductionEffect(resourceDelta, resourceConsumers, building);
			}

			while (FindDeficit(resourceDelta, out Resource resource))
			{
				(BuildingInstance building, EffectGroupInstance effectGroup) =
					ChooseRandomConsumer(resource, resourceConsumers);

				disabledGroups.Add(effectGroup);

				RemoveEffectGroup(resourceDelta, effectGroup);

				Debug.Log($"Disabled an effect group of {building.Template.DisplayName} because of resource deficit");
				// TODO maybe send message warning about resource deficit
			}

			ApplyResourceDelta(resourceDelta);
		}

		Dictionary<Resource, int> capacity = ComputeResourceCapacity(buildings, disabledGroups);
		EnforceCapacity(capacity);
	}

	private static void ComposeProductionEffect(
		IDictionary<Resource, int> resourceDelta,
		IDictionary<Resource, List<Tuple<BuildingInstance, EffectGroupInstance>>> resourceConsumers,
		BuildingInstance building
	)
	{
		foreach (EffectGroupInstance group in building.EffectGroups)
		{
			foreach (BuildingEffect effect in group.Effects)
			{
				switch (effect.Type)
				{
					case BuildingEffect.EffectType.Produce:
						AddValue(resourceDelta, effect.Resource, effect.Amount);
						break;
					case BuildingEffect.EffectType.Consume:
						AddValue(resourceDelta, effect.Resource, -effect.Amount);
						AppendList(
							resourceConsumers,
							effect.Resource,
							new Tuple<BuildingInstance, EffectGroupInstance>(building, group)
						);
						break;
				}
			}
		}
	}

	private static void AddValue<T>(IDictionary<T, int> dict, T key, int delta)
	{
		if (dict.TryGetValue(key, out int value))
		{
			dict[key] += delta;
		}
		else
		{
			dict[key] = delta;
		}
	}

	private static void AppendList<TKey, TEntry, TList>(IDictionary<TKey, TList> dict, TKey key, TEntry entry)
		where TList : IList<TEntry>, new()
	{
		if (!dict.TryGetValue(key, out TList list))
		{
			list = dict[key] = new TList();
		}

		list.Add(entry);
	}

	private bool FindDeficit(IDictionary<Resource, int> resourceDelta, out Resource deficit)
	{
		deficit = null;

		foreach (KeyValuePair<Resource, int> entry in resourceDelta)
		{
			int currentStorage = _storage.TryGetValue(entry.Key, out int value) ? value : 0;
			if (currentStorage + entry.Value < 0)
			{
				deficit = entry.Key;
				return true;
			}
		}

		return false;
	}

	private static Tuple<BuildingInstance, EffectGroupInstance> ChooseRandomConsumer(
		Resource resource,
		IReadOnlyDictionary<Resource, List<Tuple<BuildingInstance, EffectGroupInstance>>> resourceConsumers
	)
	{
		List<Tuple<BuildingInstance, EffectGroupInstance>> consumers = resourceConsumers[resource];
		Debug.Assert(consumers.Count > 0, $"Resource {resource.DisplayName} is in deficit but no consumers found");
		return consumers[new Random().Next(consumers.Count)];
	}

	private static void RemoveEffectGroup(IDictionary<Resource, int> resourceDelta, EffectGroupInstance group)
	{
		foreach (BuildingEffect effect in group.Effects)
		{
			switch (effect.Type)
			{
				case BuildingEffect.EffectType.Produce:
					resourceDelta[effect.Resource] -= effect.Amount;
					break;
				case BuildingEffect.EffectType.Consume:
					resourceDelta[effect.Resource] += effect.Amount;
					break;
			}
		}
	}

	private void ApplyResourceDelta(IDictionary<Resource, int> resourceDelta)
	{
		foreach (KeyValuePair<Resource, int> entry in resourceDelta)
		{
			AddValue(_storage, entry.Key, entry.Value);
		}
	}

	private static Dictionary<Resource, int> ComputeResourceCapacity(
		IEnumerable<BuildingInstance> buildings,
		ICollection<EffectGroupInstance> disabledGroups
	)
	{
		var capacity = new Dictionary<Resource, int>();

		foreach (BuildingInstance building in buildings)
		{
			foreach (EffectGroupInstance group in building.EffectGroups)
			{
				if (disabledGroups.Contains(group)) continue;

				foreach (BuildingEffect effect in group.Effects)
				{
					if (effect.Type == BuildingEffect.EffectType.CapacityAdd)
					{
						AddValue(capacity, effect.Resource, effect.Amount);
					}
				}
			}
		}

		return capacity;
	}

	private void EnforceCapacity(IReadOnlyDictionary<Resource, int> capacity)
	{
		var planetResources = new List<Resource>(_storage.Keys);
		foreach (Resource resource in planetResources)
		{
			if (!capacity.TryGetValue(resource, out int max)) max = 0;
			_storage[resource] = Mathf.Min(_storage[resource], max);
		}
	}

	public Serialized Save()
	{
		var resourcePaths = new List<string>();
		var amount = new List<int>();

		foreach (KeyValuePair<Resource, int> entry in _storage)
		{
			resourcePaths.Add(AssetDatabase.GetAssetPath(entry.Key));
			amount.Add(entry.Value);
		}

		return new Serialized
		{
			ResourcePaths = resourcePaths.ToArray(),
			Amount = amount.ToArray()
		};
	}

	public void Load(Serialized serialized)
	{
		_storage.Clear();

		for (int i = 0; i < serialized.ResourcePaths.Length; i++)
		{
			_storage[AssetDatabase.LoadAssetAtPath<Resource>(serialized.ResourcePaths[i])] = serialized.Amount[i];
		}
	}
}