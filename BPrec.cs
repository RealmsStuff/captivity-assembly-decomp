public class BPrec : Usable
{
	protected override bool HandleUse(bool i_isAltFire)
	{
		base.HandleUse(i_isAltFire);
		StatusEffect statusEffect = new SEBPrec("BPrec", "BPrec", TypeStatusEffect.Positive, 99999f, i_isStackable: false);
		statusEffect.AddPlayerStatusHudItem("BPrec", "-50% recoil", StatusPlayerHudItemColor.Buff);
		bool flag = CommonReferences.Instance.GetPlayer().ApplyStatusEffect(statusEffect);
		if (flag)
		{
			CommonReferences.Instance.GetManagerPostProcessing().PlayEffectBPrec();
		}
		return flag;
	}
}
