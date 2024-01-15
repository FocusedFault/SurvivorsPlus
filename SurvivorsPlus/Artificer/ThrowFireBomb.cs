using RoR2;
using EntityStates;
using EntityStates.Mage.Weapon;
using UnityEngine;

namespace SurvivorsPlus.Artificer
{
  public class ThrowFireBomb : BaseThrowBombState
  {
    public new GameObject projectilePrefab = ArtificerChanges.fireBombProjectile;
    public new GameObject muzzleflashEffectPrefab = new ThrowNovabomb().muzzleflashEffectPrefab;
    public new float baseDuration = new ThrowNovabomb().baseDuration;
    public new float minDamageCoefficient = new ThrowNovabomb().minDamageCoefficient;
    public new float maxDamageCoefficient = new ThrowNovabomb().maxDamageCoefficient;
    public new float force = new ThrowNovabomb().force;
    public new float selfForce = new ThrowNovabomb().selfForce;
  }
}