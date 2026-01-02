using UnityEngine.Experimental.Rendering.Universal;

public class FlashLight : Gun
{
	protected override bool HandleUse(bool i_isAltFire)
	{
		base.HandleUse(i_isAltFire);
		GetComponentInChildren<Light2D>().enabled = !GetComponentInChildren<Light2D>().enabled;
		return true;
	}
}
