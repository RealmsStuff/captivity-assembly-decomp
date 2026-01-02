using UnityEngine;

public class BodyPartActor : BodyPart
{
	[SerializeField]
	private DamageMultplier m_damageMultiplierBodyPart;

	[SerializeField]
	private bool m_isBodyPartDestroyedOnDeath;

	private float m_health;

	private bool m_isBodyPartDestroyed;

	private void Start()
	{
		m_health = 15f;
		m_isBodyPartDestroyedOnDeath = true;
		m_isBodyPartDestroyed = false;
	}

	public void TakeHit(float i_damage)
	{
		if (!m_isBodyPartDestroyed)
		{
			m_health -= i_damage;
			if (m_health < 0f)
			{
				m_health = 0f;
			}
			if (m_health == 0f && GetOwner().IsDead())
			{
				TryDestroyBodyPart();
			}
		}
	}

	public bool TakeHitProjectile(Bullet i_projectile)
	{
		if (!m_owner)
		{
			return false;
		}
		bool flag = ((NPC)GetOwner()).TakeHitBullet(i_projectile.GetOwner(), i_projectile, this);
		if (flag)
		{
			TakeHit(i_projectile.GetDamage() * GetDamageMultiplierAmountBodyPart());
		}
		return flag;
	}

	public float GetDamageMultiplierAmountBodyPart()
	{
		DamageMultplier damageMultiplierBodyPart = m_damageMultiplierBodyPart;
		if (1 == 0)
		{
		}
		float result = damageMultiplierBodyPart switch
		{
			DamageMultplier.Low => 0.75f, 
			DamageMultplier.Normal => 1f, 
			DamageMultplier.Crit => 2f, 
			DamageMultplier.Block => 0f, 
			_ => 1f, 
		};
		if (1 == 0)
		{
		}
		return result;
	}

	public void TryDestroyBodyPart()
	{
		if (Random.Range(0, 101) > 60)
		{
			Explode();
			m_isBodyPartDestroyed = true;
		}
	}

	public bool GetIsBodyPartDestroyedOnHitDeath()
	{
		return m_isBodyPartDestroyedOnDeath;
	}

	public bool GetIsBodyPartDestroyed()
	{
		return m_isBodyPartDestroyed;
	}
}
