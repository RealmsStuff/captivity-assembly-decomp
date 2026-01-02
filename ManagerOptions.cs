using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerOptions : MonoBehaviour
{
	[Header("---Input")]
	[SerializeField]
	private GameObject m_parentOptions;

	[SerializeField]
	private GameObject m_innerOptions;

	[SerializeField]
	private GameObject m_innerResetWindow;

	[SerializeField]
	private InputBox m_inputBoxDefault;

	[SerializeField]
	private Image m_imgDisableInput;

	[SerializeField]
	private List<KeyCode> m_keyCodesIllegalToSet = new List<KeyCode>();

	private List<InputButtonXGame> m_buttonsOriginal;

	private List<InputBox> m_inputBoxes = new List<InputBox>();

	private InputBox m_inputBoxSelected;

	[Header("---Volume")]
	[SerializeField]
	private Slider m_sliderMaster;

	[SerializeField]
	private Text m_txtVolumeMaster;

	[SerializeField]
	private Slider m_sliderMusic;

	[SerializeField]
	private Text m_txtVolumeMusic;

	[SerializeField]
	private Slider m_sliderAmbience;

	[SerializeField]
	private Text m_txtVolumeAmbience;

	[SerializeField]
	private Slider m_sliderVoice;

	[SerializeField]
	private Text m_txtVolumeVoice;

	[SerializeField]
	private Slider m_sliderSFX;

	[SerializeField]
	private Text m_txtVolumeSFX;

	[SerializeField]
	private Slider m_sliderHitsound;

	[SerializeField]
	private Text m_txtVolumeHitsound;

	[SerializeField]
	private Dropdown m_dropDownDifficulty;

	[SerializeField]
	private Toggle m_toggleGunFlash;

	private bool m_isOpen;

	private void Update()
	{
		if (!m_isOpen)
		{
			return;
		}
		m_txtVolumeMaster.text = m_sliderMaster.value.ToString();
		m_txtVolumeMusic.text = m_sliderMusic.value.ToString();
		m_txtVolumeAmbience.text = m_sliderAmbience.value.ToString();
		m_txtVolumeVoice.text = m_sliderVoice.value.ToString();
		m_txtVolumeSFX.text = m_sliderSFX.value.ToString();
		m_txtVolumeHitsound.text = m_sliderHitsound.value.ToString();
		if (m_inputBoxSelected != null)
		{
			List<KeyCode> anyKey = CommonReferences.Instance.GetManagerInput().GetAnyKey();
			if (anyKey.Count > 0 && !m_keyCodesIllegalToSet.Contains(anyKey[0]))
			{
				SetInputBoxKey(m_inputBoxSelected, anyKey[0]);
			}
		}
		CommonReferences.Instance.GetManagerAudio().SetVolume("Master", (int)m_sliderMaster.value);
		CommonReferences.Instance.GetManagerAudio().SetVolume("Music", (int)m_sliderMusic.value);
		CommonReferences.Instance.GetManagerAudio().SetVolume("Ambience", (int)m_sliderAmbience.value);
		CommonReferences.Instance.GetManagerAudio().SetVolume("Voice", (int)m_sliderVoice.value);
		CommonReferences.Instance.GetManagerAudio().SetVolume("SFX", (int)m_sliderSFX.value);
		CommonReferences.Instance.GetManagerAudio().SetVolume("Hitsound", (int)m_sliderHitsound.value);
	}

	private void BuildInputBoxes()
	{
		ClearInputBoxes();
		List<InputButtonXGame> inputButtons = ManagerDB.GetInputButtons();
		for (int i = 0; i < inputButtons.Count; i++)
		{
			InputBox inputBox = UnityEngine.Object.Instantiate(m_inputBoxDefault, m_inputBoxDefault.transform.parent);
			inputBox.Initialize(inputButtons[i]);
			inputBox.gameObject.SetActive(value: true);
			m_inputBoxes.Add(inputBox);
		}
	}

	private void ClearInputBoxes()
	{
		for (int i = 0; i < m_inputBoxes.Count; i++)
		{
			UnityEngine.Object.Destroy(m_inputBoxes[i].gameObject);
		}
		m_inputBoxes.Clear();
		m_inputBoxDefault.gameObject.SetActive(value: false);
	}

	public void SelectInputBox(InputBox i_inputBox)
	{
		m_inputBoxSelected = i_inputBox;
		m_inputBoxSelected.SetToListenInput();
		m_imgDisableInput.raycastTarget = true;
	}

	private void SetInputBoxKey(InputBox i_inputBox, KeyCode i_keyCode)
	{
		ManagerDB.SetInputButton(m_inputBoxSelected.GetNameButton(), i_keyCode);
		BuildInputBoxes();
		m_imgDisableInput.raycastTarget = false;
	}

	public void ResetInput()
	{
		ManagerDB.ResetInput();
		CommonReferences.Instance.GetManagerInput().SetButtonsToDefault();
		BuildInputBoxes();
	}

	private void BuildDropDownDifficulty()
	{
		m_dropDownDifficulty.options.Clear();
		List<string> list = new List<string>();
		int value = 0;
		for (int i = 0; i < Enum.GetValues(typeof(Difficulty)).Length; i++)
		{
			string text = Enum.GetNames(typeof(Difficulty))[i];
			list.Add(text);
			if (text == PlayerPrefs.GetString("Difficulty"))
			{
				value = i;
			}
		}
		m_dropDownDifficulty.AddOptions(list);
		m_dropDownDifficulty.value = value;
	}

	private void BuildToggleReduceGunFlash()
	{
		if (PlayerPrefs.GetInt("IsReduceGunFlash") == 1)
		{
			m_toggleGunFlash.isOn = true;
		}
		else
		{
			m_toggleGunFlash.isOn = false;
		}
	}

	public void Cancel()
	{
		for (int i = 0; i < m_buttonsOriginal.Count; i++)
		{
			ManagerDB.SetInputButton(m_buttonsOriginal[i].GetName(), m_buttonsOriginal[i].GetKeyCode());
		}
		CommonReferences.Instance.GetManagerInput().SetButtonsToSavedButtons();
		GetComponentInParent<ScreenTitle>().CloseOptions();
		CommonReferences.Instance.GetManagerAudio().SetVolumesToSaved();
	}

	public void Ok()
	{
		PlayerPrefs.SetInt("VolumeMaster", (int)m_sliderMaster.value);
		PlayerPrefs.SetInt("VolumeMusic", (int)m_sliderMusic.value);
		PlayerPrefs.SetInt("VolumeAmbience", (int)m_sliderAmbience.value);
		PlayerPrefs.SetInt("VolumeVoice", (int)m_sliderVoice.value);
		PlayerPrefs.SetInt("VolumeSFX", (int)m_sliderSFX.value);
		PlayerPrefs.SetInt("VolumeHitsound", (int)m_sliderHitsound.value);
		PlayerPrefs.SetString("Difficulty", m_dropDownDifficulty.options[m_dropDownDifficulty.value].text);
		if (m_toggleGunFlash.isOn)
		{
			PlayerPrefs.SetInt("IsReduceGunFlash", 1);
		}
		else
		{
			PlayerPrefs.SetInt("IsReduceGunFlash", 0);
		}
		CommonReferences.Instance.GetManagerAudio().SetVolumesToSaved();
		GetComponentInParent<ScreenTitle>().CloseOptions();
	}

	public void Open()
	{
		if (!ManagerDB.IsInputFilled())
		{
			ManagerDB.ResetInput();
			CommonReferences.Instance.GetManagerInput().SetButtonsToDefault();
		}
		m_buttonsOriginal = ManagerDB.GetInputButtons();
		m_sliderMaster.value = PlayerPrefs.GetInt("VolumeMaster");
		m_sliderMusic.value = PlayerPrefs.GetInt("VolumeMusic");
		m_sliderAmbience.value = PlayerPrefs.GetInt("VolumeAmbience");
		m_sliderVoice.value = PlayerPrefs.GetInt("VolumeVoice");
		m_sliderSFX.value = PlayerPrefs.GetInt("VolumeSFX");
		m_sliderHitsound.value = PlayerPrefs.GetInt("VolumeHitsound");
		BuildInputBoxes();
		BuildDropDownDifficulty();
		BuildToggleReduceGunFlash();
		m_parentOptions.SetActive(value: true);
		m_isOpen = true;
	}

	public void OpenResetSaveWindow()
	{
		m_innerOptions.SetActive(value: false);
		m_innerResetWindow.SetActive(value: true);
	}

	public void CloseResetSaveWindow()
	{
		m_innerOptions.SetActive(value: true);
		m_innerResetWindow.SetActive(value: false);
	}

	public void ResetSave()
	{
		ManagerDB.ResetSave();
		CloseResetSaveWindow();
		Cancel();
	}

	public void Close()
	{
		m_isOpen = false;
		m_parentOptions.SetActive(value: false);
	}
}
