using RoR2;
using EntityStates;
using EntityStates.Mage.Weapon;
using UnityEngine;

namespace SurvivorsPlus.Artificer
{
  public class ChargeFireBomb : BaseChargeBombState
  {
    public new GameObject chargeEffectPrefab = ArtificerChanges.fireBombChargeEffect;
    public new string chargeSoundString = Flamethrower.startAttackSoundString;
    public new float baseDuration = 1.5f;
    public new float minBloomRadius = new ChargeNovabomb().minBloomRadius;
    public new float maxBloomRadius = new ChargeNovabomb().maxBloomRadius;
    public new GameObject crosshairOverridePrefab = new ChargeNovabomb().crosshairOverridePrefab;
    public override BaseThrowBombState GetNextState() => (BaseThrowBombState)new ThrowFireBomb();
  }
}