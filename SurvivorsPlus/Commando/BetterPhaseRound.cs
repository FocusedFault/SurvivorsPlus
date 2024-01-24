using RoR2;
using RoR2.Skills;
using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using EntityStates.Railgunner.Weapon;
using UnityEngine;

namespace SurvivorsPlus.Commando
{
    public class BetterPhaseRound : GenericBulletBaseState
    {
        public static string fireFMJSoundString = "Play_railgunner_m2_fire";
        public float piercingDamageCoefficientPerTarget = 1.4f;

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound(BetterPhaseRound.fireFMJSoundString, this.gameObject);
            if (!(bool)(Object)this.GetModelAnimator())
                return;
            this.PlayAnimation("Gesture, Additive", nameof(FireFMJ), "FireFMJ.playbackRate", 0.25f);
            this.PlayAnimation("Gesture, Override", nameof(FireFMJ), "FireFMJ.playbackRate", 0.25f);
        }

        public override void ModifyBullet(BulletAttack bulletAttack)
        {
            bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
            bulletAttack.stopperMask = LayerIndex.noCollision.mask;
            bulletAttack.modifyOutgoingDamageCallback = (BulletAttack _bulletAttack, ref BulletAttack.BulletHit hitInfo, DamageInfo damageInfo) =>
            {
                _bulletAttack.damage *= this.piercingDamageCoefficientPerTarget;
            };
        }
    }
}
