using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HubMainMenu : MonoBehaviour
{
	[SerializeField]
	private AudioClip m_audioStart;

	[SerializeField]
	private AudioClip m_audioClose;

	[SerializeField]
	private GameObject m_parent;

	[SerializeField]
	private Image m_overlay;

	[SerializeField]
	private UnityEngine.UI.Button m_btnLocations;

	[SerializeField]
	private UnityEngine.UI.Button m_btnShop;

	[SerializeField]
	private UnityEngine.UI.Button m_btnDiary;

	[SerializeField]
	private Sprite m_sprBtnClosed;

	[SerializeField]
	private Sprite m_sprBtnOpen;

	private void Awake()
	{
		m_parent.SetActive(value: false);
	}

	private void Update()
	{
		if (IsOpen() && Input.GetKeyDown(KeyCode.Escape))
		{
			BtnClose();
		}
	}

	public bool IsOpen()
	{
		return m_parent.activeSelf;
	}

	public void Open()
	{
		m_parent.SetActive(value: true);
		StartCoroutine(CoroutineAnimateOverlayFadeOut());
		CommonReferences.Instance.GetManagerAudio().PlayAudioSFX(m_audioStart);
	}

	public void Close()
	{
		StopAllCoroutines();
		m_overlay.gameObject.SetActive(value: false);
		m_parent.SetActive(value: false);
		CommonReferences.Instance.GetManagerAudio().PlayAudioSFX(m_audioClose);
	}

	public void BtnClose()
	{
		((StageHub)CommonReferences.Instance.GetManagerStages().GetStageCurrent()).GetComponentInChildren<ChairHubTerminal>().CloseHubMainMenu();
	}

	public void OpenMenu(Menu i_menuToOpen)
	{
		GetComponentInChildren<ManagerMenus>().OpenMenu(i_menuToOpen);
	}

	public void HandleButtonsTopBar(UnityEngine.UI.Button i_btnPressed)
	{
		m_btnLocations.GetComponent<Image>().sprite = m_sprBtnClosed;
		m_btnShop.GetComponent<Image>().sprite = m_sprBtnClosed;
		m_btnDiary.GetComponent<Image>().sprite = m_sprBtnClosed;
		i_btnPressed.GetComponent<Image>().sprite = m_sprBtnOpen;
	}

	private IEnumerator CoroutineAnimateOverlayFadeOut()
	{
		m_overlay.gameObject.SetActive(value: true);
		float l_tranparencyFrom = 1f;
		float l_tranparencyTo = 0f;
		float l_timeToMove = 1f;
		float l_timeCurrent = 0f;
		while (l_timeCurrent < l_timeToMove)
		{
			l_timeCurrent += Time.fixedDeltaTime;
			float i_time = l_timeCurrent / l_timeToMove;
			float a = AnimationTools.CalculateOverTime(AnimationTools.Transition.Steep, AnimationTools.Transition.Steep, l_tranparencyFrom, l_tranparencyTo, i_time);
			m_overlay.color = new Color(m_overlay.color.r, m_overlay.color.g, m_overlay.color.b, a);
			yield return new WaitForFixedUpdate();
		}
		m_overlay.gameObject.SetActive(value: false);
	}
}
