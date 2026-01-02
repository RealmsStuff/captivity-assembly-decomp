using System.Collections;
using UnityEngine;

public class SkeletonActor : Skeleton
{
	private Coroutine m_coroutineFlashingLeapAttack;

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void HandleEnableRagdoll()
	{
		foreach (Bone bone in m_bones)
		{
			if ((bool)bone.GetRigidbody2D())
			{
				bone.GetRigidbody2D().isKinematic = false;
				bone.GetRigidbody2D().velocity = GetOwner().GetVelocity();
			}
		}
	}

	protected override void CenterSkeleton()
	{
		Vector3 position = GetBoneHips().transform.position;
		GetOwner().SetPos(position);
		GetBoneHips().transform.localPosition = Vector3.zero;
	}

	public void StartFlashingLeapAttack()
	{
		if (m_coroutineFlashingLeapAttack != null)
		{
			StopCoroutine(m_coroutineFlashingLeapAttack);
		}
		m_coroutineFlashingLeapAttack = StartCoroutine(CoroutineFlashRed());
	}

	public void StopFlashingLeapAttack()
	{
		if (m_coroutineFlashingLeapAttack != null)
		{
			StopCoroutine(m_coroutineFlashingLeapAttack);
		}
		SetColorOnAllBodyParts(Color.white);
	}

	private IEnumerator CoroutineFlashRed()
	{
		while (true)
		{
			SetColorOnAllBodyParts(Color.red);
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			SetColorOnAllBodyParts(Color.white);
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
		}
	}

	public void FadeBodyIn()
	{
		StartCoroutine(CoroutineFadeBodyIn());
	}

	private IEnumerator CoroutineFadeBodyIn()
	{
		float l_weightFrom = 0f;
		float l_weightTo = 1f;
		float l_timeToMove = 0.5f;
		float l_timeCurrent = 0f;
		while (l_timeCurrent < l_timeToMove)
		{
			l_timeCurrent += Time.fixedDeltaTime;
			float i_time = l_timeCurrent / l_timeToMove;
			float a = AnimationTools.CalculateOverTime(AnimationTools.Transition.Steep, AnimationTools.Transition.Smooth, l_weightFrom, l_weightTo, i_time);
			SetColorOnAllBodyParts(new Color(1f, 1f, 1f, a));
			yield return new WaitForFixedUpdate();
		}
	}

	private void SetColorOnAllBodyParts(Color i_color)
	{
		foreach (Bone bone in m_bones)
		{
			if ((bool)bone.GetBodyPart())
			{
				bone.GetBodyPart().GetComponent<SpriteRenderer>().color = i_color;
			}
		}
	}

	public Bone GetBoneHips()
	{
		return GetBone("hips");
	}

	public Bone GetBoneHead()
	{
		Bone bone = GetBone("head");
		if (bone == null)
		{
			return GetBoneHips();
		}
		return bone;
	}

	public void SetToRape()
	{
		SpriteRenderer[] componentsInChildren = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].sortingLayerName = "Player";
		}
	}

	public void SetToDefault()
	{
		SpriteRenderer[] componentsInChildren = GetComponentsInChildren<SpriteRenderer>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].sortingLayerName = "Actor";
		}
	}

	private void AnimEventPerformAttack()
	{
		GetComponentInParent<NPC>().PerformAttack(CommonReferences.Instance.GetPlayer());
	}

	private void AnimEventPlayAudioUnique(int i_numAudioUnique)
	{
		GetComponentInParent<Actor>().PlayAudioUnique(i_numAudioUnique - 1);
	}

	private void AnimEventLeap()
	{
		((AttackLeap)GetComponentInParent<NPC>().GetAttackCurrent()).Leap();
	}
}
