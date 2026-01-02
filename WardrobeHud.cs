using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WardrobeHud : MonoBehaviour
{
	[SerializeField]
	private AudioClip m_audioOpen;

	[SerializeField]
	private AudioClip m_audioClose;

	[SerializeField]
	private AudioClip m_audioSelectClothing;

	[SerializeField]
	private AudioClip m_audioUnselectClothing;

	[SerializeField]
	private GameObject m_parent;

	[SerializeField]
	private GameObject m_tabDefault;

	[SerializeField]
	private GameObject m_tabMenuDefault;

	[SerializeField]
	private ClothingHudItem m_clothingItemDefault;

	private SkeletonPlayer m_skeletonShowcase;

	private List<Clothing> m_clothes = new List<Clothing>();

	private List<GameObject> m_tabs = new List<GameObject>();

	private List<GameObject> m_tabMenus = new List<GameObject>();

	private List<ClothingHudItem> m_clothingItems = new List<ClothingHudItem>();

	private List<ClothingHudItem> m_clothingItemsSelected = new List<ClothingHudItem>();

	private SkinColor m_skinColorSelected;

	private EyeColor m_eyeColorSelected;

	private void LateUpdate()
	{
		if (IsShowing() && Input.GetKeyDown(KeyCode.Escape))
		{
			Hide();
		}
	}

	private void BuildHudInterface()
	{
		m_tabDefault.SetActive(value: false);
		m_tabMenuDefault.SetActive(value: false);
		m_clothingItemDefault.gameObject.SetActive(value: false);
		string[] names = Enum.GetNames(typeof(ClothingCategory));
		string[] array = names;
		foreach (string text in array)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(m_tabDefault, m_tabDefault.transform.parent);
			gameObject.name = "tab" + text;
			gameObject.GetComponentInChildren<Text>().text = text;
			m_tabs.Add(gameObject);
			gameObject.SetActive(value: true);
			GameObject gameObject2 = UnityEngine.Object.Instantiate(m_tabMenuDefault, m_tabMenuDefault.transform.parent);
			gameObject2.name = "tabMenu" + text;
			m_tabMenus.Add(gameObject2);
			gameObject2.SetActive(value: false);
		}
		m_tabMenus[0].SetActive(value: true);
		RetrieveAllClothes();
		CreateClothingItems();
		CreatePlayerShowcase();
		AddAlreadyEquippedClothes();
	}

	private void RetrieveAllClothes()
	{
		foreach (int idsUnlockedClothe in ManagerDB.GetIdsUnlockedClothes())
		{
			m_clothes.Add(Library.Instance.Clothes.GetClothing(idsUnlockedClothe));
		}
	}

	private void CreateClothingItems()
	{
		foreach (Clothing clothe in m_clothes)
		{
			ClothingHudItem clothingHudItem = UnityEngine.Object.Instantiate(m_clothingItemDefault, m_clothingItemDefault.transform);
			clothingHudItem.SetClothing(clothe);
			string i_categoryName = clothe.GetCatergoryClothing().ToString();
			clothingHudItem.transform.SetParent(GetTabMenu(i_categoryName).transform);
			m_clothingItems.Add(clothingHudItem);
			clothingHudItem.gameObject.SetActive(value: true);
		}
	}

	private void CreatePlayerShowcase()
	{
		StageHub stageHub = (StageHub)CommonReferences.Instance.GetManagerStages().GetStageCurrent();
		m_skeletonShowcase = UnityEngine.Object.Instantiate(CommonReferences.Instance.GetPlayer().GetSkeletonPlayer(), stageHub.GetParentPlayerShowcaseWardrobe());
		m_skeletonShowcase.transform.localPosition = Vector3.zero;
		m_skeletonShowcase.UpdateClothesEquippedAlready();
		m_skeletonShowcase.RemoveAllClothing();
		m_skinColorSelected = CommonReferences.Instance.GetPlayer().GetSkeletonPlayer().GetSkinColor();
		m_skeletonShowcase.SetSkinColor(m_skinColorSelected);
		m_eyeColorSelected = CommonReferences.Instance.GetPlayer().GetSkeletonPlayer().GetEyeColor();
		m_skeletonShowcase.SetEyeColor(CommonReferences.Instance.GetPlayer().GetSkeletonPlayer().GetEyeColor());
		if (CommonReferences.Instance.GetPlayer().GetIsFacingLeft())
		{
			m_skeletonShowcase.transform.localScale = new Vector3(-1f, 1f, 1f);
		}
	}

	private void AddAlreadyEquippedClothes()
	{
		foreach (Clothing item in CommonReferences.Instance.GetPlayer().GetSkeletonPlayer().GetClothesEquipped())
		{
			SelectClothingItem(GetClothingItemAttachedToClothing(item));
		}
	}

	public void ClickClothingItem(ClothingHudItem i_clothingItemSelected)
	{
		if (m_clothingItemsSelected.Contains(i_clothingItemSelected))
		{
			UnSelectClothingItem(i_clothingItemSelected);
			CommonReferences.Instance.GetManagerAudio().PlayAudioSFX(m_audioUnselectClothing);
		}
		else
		{
			SelectClothingItem(i_clothingItemSelected);
			CommonReferences.Instance.GetManagerAudio().PlayAudioSFX(m_audioSelectClothing);
		}
	}

	private void SelectClothingItem(ClothingHudItem i_clothingItem)
	{
		m_clothingItemsSelected.Add(i_clothingItem);
		m_skeletonShowcase.EquipClothing(i_clothingItem.GetClothing());
		UnEquipIncompatibleClothes(i_clothingItem);
		i_clothingItem.SetIsSelected(i_isSelected: true);
	}

	private void UnSelectClothingItem(ClothingHudItem i_clothingItem)
	{
		m_skeletonShowcase.RemoveClothing(i_clothingItem.GetClothing());
		m_clothingItemsSelected.Remove(i_clothingItem);
		i_clothingItem.SetIsSelected(i_isSelected: false);
	}

	private void UnEquipIncompatibleClothes(ClothingHudItem i_clothingItemSelected)
	{
		List<ClothingHudItem> list = new List<ClothingHudItem>();
		foreach (ClothingHudItem item in m_clothingItemsSelected)
		{
			list.Add(item);
		}
		foreach (ClothingHudItem item2 in list)
		{
			if (item2.GetClothing().GetId() != i_clothingItemSelected.GetClothing().GetId() && !item2.GetClothing().IsCompatibleWithClothing(i_clothingItemSelected.GetClothing()))
			{
				UnSelectClothingItem(item2);
			}
		}
	}

	public void ClickTab(Text i_textButtonTabClicked)
	{
		foreach (GameObject tab in m_tabs)
		{
			if (tab.GetComponentInChildren<Text>().text == i_textButtonTabClicked.text)
			{
				HideAllTabMenus();
				OpenTabMenu(GetTabMenu(tab.GetComponentInChildren<Text>().text));
				break;
			}
		}
	}

	private void OpenTabMenu(GameObject i_tabMenu)
	{
		i_tabMenu.SetActive(value: true);
	}

	private GameObject GetTabMenu(string i_categoryName)
	{
		foreach (GameObject tabMenu in m_tabMenus)
		{
			if (tabMenu.name == "tabMenu" + i_categoryName)
			{
				return tabMenu;
			}
		}
		return null;
	}

	private void HideAllTabMenus()
	{
		foreach (GameObject tabMenu in m_tabMenus)
		{
			tabMenu.SetActive(value: false);
		}
	}

	private void ClearAllData()
	{
		m_clothes.Clear();
		foreach (GameObject tab in m_tabs)
		{
			UnityEngine.Object.Destroy(tab);
		}
		m_tabs.Clear();
		foreach (GameObject tabMenu in m_tabMenus)
		{
			UnityEngine.Object.Destroy(tabMenu);
		}
		m_tabMenus.Clear();
		foreach (ClothingHudItem clothingItem in m_clothingItems)
		{
			UnityEngine.Object.Destroy(clothingItem.gameObject);
		}
		m_clothingItems.Clear();
		if ((bool)m_skeletonShowcase)
		{
			UnityEngine.Object.Destroy(m_skeletonShowcase.gameObject);
		}
		m_clothingItemsSelected.Clear();
	}

	public void AcceptAndClose()
	{
		CommonReferences.Instance.GetPlayer().GetSkeletonPlayer().RemoveAllClothing();
		foreach (Clothing item in GetClothesEquippedShowcase())
		{
			CommonReferences.Instance.GetPlayer().GetSkeletonPlayer().EquipClothing(item);
		}
		ManagerDB.EquipClothes(GetClothesEquippedShowcase());
		CommonReferences.Instance.GetPlayer().GetSkeletonPlayer().SetSkinColor(m_skinColorSelected);
		ManagerDB.SetSkinColor(m_skinColorSelected);
		CommonReferences.Instance.GetPlayer().GetSkeletonPlayer().SetEyeColor(m_eyeColorSelected);
		ManagerDB.SetEyeColor(m_eyeColorSelected);
		Hide();
	}

	private ClothingHudItem GetClothingItemAttachedToClothing(Clothing i_clothing)
	{
		foreach (ClothingHudItem clothingItem in m_clothingItems)
		{
			if (clothingItem.GetClothing().GetId() == i_clothing.GetId())
			{
				return clothingItem;
			}
		}
		return null;
	}

	private List<Clothing> GetClothesEquippedShowcase()
	{
		List<Clothing> list = new List<Clothing>();
		foreach (ClothingHudItem item in m_clothingItemsSelected)
		{
			list.Add(item.GetClothing());
		}
		return list;
	}

	public void SetSkinColor(Text i_textBtn)
	{
		switch (i_textBtn.text)
		{
		case "Pale":
			m_skeletonShowcase.SetSkinColor(SkinColor.Pale);
			m_skinColorSelected = SkinColor.Pale;
			break;
		case "White":
			m_skeletonShowcase.SetSkinColor(SkinColor.White);
			m_skinColorSelected = SkinColor.White;
			break;
		case "Tan":
			m_skeletonShowcase.SetSkinColor(SkinColor.Tan);
			m_skinColorSelected = SkinColor.Tan;
			break;
		case "Black":
			m_skeletonShowcase.SetSkinColor(SkinColor.Black);
			m_skinColorSelected = SkinColor.Black;
			break;
		}
	}

	public void SetEyeColor(Text i_textBtn)
	{
		switch (i_textBtn.text)
		{
		case "Blue":
			m_skeletonShowcase.SetEyeColor(EyeColor.Blue);
			m_eyeColorSelected = EyeColor.Blue;
			break;
		case "Brown":
			m_skeletonShowcase.SetEyeColor(EyeColor.Brown);
			m_eyeColorSelected = EyeColor.Brown;
			break;
		case "Green":
			m_skeletonShowcase.SetEyeColor(EyeColor.Green);
			m_eyeColorSelected = EyeColor.Green;
			break;
		case "Yellow":
			m_skeletonShowcase.SetEyeColor(EyeColor.Yellow);
			m_eyeColorSelected = EyeColor.Yellow;
			break;
		}
	}

	public void Show()
	{
		CommonReferences.Instance.GetPlayerController().SetIsForceIgnoreInput(i_isForceIgnoreInput: true);
		m_parent.SetActive(value: true);
		ClearAllData();
		BuildHudInterface();
		CommonReferences.Instance.GetManagerAudio().PlayAudioSFX(m_audioOpen);
	}

	public void Hide()
	{
		CommonReferences.Instance.GetPlayerController().SetIsForceIgnoreInput(i_isForceIgnoreInput: false);
		ClearAllData();
		m_parent.SetActive(value: false);
		CommonReferences.Instance.GetManagerAudio().PlayAudioSFX(m_audioClose);
	}

	public bool IsShowing()
	{
		return m_parent.activeSelf;
	}
}
