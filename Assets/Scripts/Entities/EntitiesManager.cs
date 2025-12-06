using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EntitiesManager
{
    public static List<Entity> entities;

	public static void ClearEntities()
	{
		if (entities == null)
			entities = new List<Entity>();
		else 
			entities.Clear();
	}

    public static void RegisterEntity(Entity entity)
	{
		if (entities == null)
			entities = new List<Entity>();
		
		entities.Add(entity);
	}

	public static void DeregisterEntity(Entity entity)
	{
		if (entities.Contains(entity))
		{
			entities.Remove(entity);
		}
	}
}
