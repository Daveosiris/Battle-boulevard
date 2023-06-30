using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
	private CharacterControl _characterControl;

	private void Awake()
	{
		_characterControl = base.transform.parent.GetComponent<CharacterControl>();
	}

	public void PlaySFX(string name)
	{
		_characterControl._gamePlayManager._makeNoise.PlaySFX(name);
	}

	public void AddForce(float value)
	{
	}

	public void AttackLock()
	{
		_characterControl.AttackLock();
	}

	public void ToIdle()
	{
		_characterControl.ToIDLE();
	}

	public void Check4Hit()
	{
		_characterControl.Check4Hit();
	}

	public void PickWeapon()
	{
		_characterControl.WeaponPicked();
	}

	public void ThrowWeapon()
	{
		_characterControl.WeaponThrowed();
	}

	public void GrabHit()
	{
		_characterControl.GrabHit();
	}

	public void GrabReaction()
	{
		_characterControl.GrabReaction();
	}

	public void Lifting()
	{
		_characterControl.Lifting();
	}

	public void Lifted()
	{
		_characterControl.Lifted();
	}

	public void LiftThrowed()
	{
		_characterControl.LiftThrowed();
	}

	public void FireGun()
	{
		PlaySFX("GunShot");
		_characterControl._gamePlayManager._camSlowMotionDelay.StartSlowMotionDelay(0.2f);
	}

	public void Drop()
	{
		PlaySFX(_characterControl._femalePrefix + "Land");
		PlaySFX("Drop");
		ShowDustEffectLand();
	}

	public void ShowHitEffect(float hitPosition)
	{
		_characterControl.HitEffect.transform.position = base.transform.position + Vector3.up * hitPosition;
		_characterControl.HitEffect.SetActive(value: false);
		_characterControl.HitEffect.SetActive(value: true);
	}

	public void ShowDefendEffect(float hitPosition)
	{
		_characterControl.DefendEffect.transform.position = base.transform.position + Vector3.up * hitPosition;
		_characterControl.DefendEffect.SetActive(value: false);
		_characterControl.DefendEffect.SetActive(value: true);
	}

	public void ShowDustEffectLand()
	{
		_characterControl.DustEffectLand.transform.position = base.transform.position;
		_characterControl.DustEffectLand.SetActive(value: false);
		_characterControl.DustEffectLand.SetActive(value: true);
	}

	public void ShowDustEffectJump()
	{
		_characterControl.DustEffectJump.transform.position = base.transform.position;
		_characterControl.DustEffectJump.SetActive(value: false);
		_characterControl.DustEffectJump.SetActive(value: true);
	}
}
