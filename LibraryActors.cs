using System.Collections.Generic;
using UnityEngine;

public class LibraryActors : MonoBehaviour
{
	private void Start()
	{
		foreach (Actor allActor in GetAllActors())
		{
			allActor.gameObject.SetActive(value: false);
		}
	}

	public GameObject GetRandomActor()
	{
		int index = Random.Range(0, GetAllActors().Count);
		return GetAllActors()[index].gameObject;
	}

	public List<Actor> GetAllActors()
	{
		List<Actor> list = new List<Actor>();
		Actor[] componentsInChildren = GetComponentsInChildren<Actor>(includeInactive: true);
		Actor[] array = componentsInChildren;
		foreach (Actor item in array)
		{
			list.Add(item);
		}
		return list;
	}

	public List<NPC> GetAllNpcs()
	{
		List<NPC> list = new List<NPC>();
		NPC[] componentsInChildren = GetComponentsInChildren<NPC>(includeInactive: true);
		NPC[] array = componentsInChildren;
		foreach (NPC item in array)
		{
			list.Add(item);
		}
		return list;
	}

	public List<Player> GetAllPlayers()
	{
		List<Player> list = new List<Player>();
		Player[] componentsInChildren = GetComponentsInChildren<Player>(includeInactive: true);
		Player[] array = componentsInChildren;
		foreach (Player item in array)
		{
			list.Add(item);
		}
		return list;
	}

	public Actor GetActor(string i_nameActor)
	{
		foreach (Actor allActor in GetAllActors())
		{
			if (allActor.GetName() == i_nameActor)
			{
				return allActor;
			}
		}
		return null;
	}

	public NPC GetNpc(string i_nameNpc)
	{
		foreach (NPC allNpc in GetAllNpcs())
		{
			if (allNpc.GetName() == i_nameNpc)
			{
				return allNpc;
			}
		}
		return null;
	}

	public NPC GetNpc(int i_idNpc)
	{
		foreach (NPC allNpc in GetAllNpcs())
		{
			if (allNpc.GetId() == i_idNpc)
			{
				return allNpc;
			}
		}
		return null;
	}

	public Actor GetActorDupe(string i_nameActor)
	{
		foreach (Actor allActor in GetAllActors())
		{
			if (allActor.GetName() == i_nameActor)
			{
				return Object.Instantiate(allActor);
			}
		}
		return null;
	}

	public void UpdateUnsetIds()
	{
		int num = 0;
		NPC[] componentsInChildren = GetComponentsInChildren<NPC>(includeInactive: true);
		NPC[] array = componentsInChildren;
		foreach (NPC nPC in array)
		{
			if (nPC.GetId() != 0 && nPC.GetId() != -1 && nPC.GetId() > num)
			{
				num = nPC.GetId();
			}
		}
		componentsInChildren = GetComponentsInChildren<NPC>(includeInactive: true);
		NPC[] array2 = componentsInChildren;
		foreach (NPC nPC2 in array2)
		{
			if (nPC2.GetId() == 0)
			{
				nPC2.SetId(num + 1);
				num++;
			}
		}
	}

	public List<Interaction> GetAllInteractions()
	{
		List<Interaction> list = new List<Interaction>();
		Interaction[] componentsInChildren = GetComponentsInChildren<Interaction>(includeInactive: true);
		Interaction[] array = componentsInChildren;
		foreach (Interaction item in array)
		{
			list.Add(item);
		}
		return list;
	}

	public void TriggerInteraction(string i_nameInteraction, NPC i_npc)
	{
		Interaction[] componentsInChildren = GetComponentsInChildren<Interaction>(includeInactive: true);
		Interaction[] array = componentsInChildren;
		foreach (Interaction interaction in array)
		{
			if (interaction.GetName() == i_nameInteraction)
			{
				interaction.Trigger(i_npc);
				break;
			}
		}
	}
}
