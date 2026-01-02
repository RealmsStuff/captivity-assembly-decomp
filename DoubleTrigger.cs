public class DoubleTrigger : Usable
{
	protected override bool HandleUse(bool i_isAltFire)
	{
		SEDoubleTrigger sEDoubleTrigger = new SEDoubleTrigger("Double Trigger", "DoubleTrigger", TypeStatusEffect.Positive, 99999f, i_isStackable: false);
		sEDoubleTrigger.AddPlayerStatusHudItem("Double Trigger", "+100% Firerate", StatusPlayerHudItemColor.Buff);
		bool flag = CommonReferences.Instance.GetPlayer().ApplyStatusEffect(sEDoubleTrigger);
		if (flag)
		{
			CommonReferences.Instance.GetManagerPostProcessing().PlayEffectDoubleTrigger();
		}
		return flag;
	}
}
