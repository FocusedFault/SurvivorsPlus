using RoR2;
using RoR2.Skills;
using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using UnityEngine;

namespace SurvivorsPlus.Commando
{
    public class BetterFirePistols : BaseSkillState, SteppedSkillDef.IStepSetter
    {
        private int pistol;
        private Ray aimRay;
        private float duration;

        void SteppedSkillDef.IStepSetter.SetStep(int i) => this.pistol = i;

        private void FireBullet(string targetMuzzle)
        {
            int num = (int)Util.PlaySound(FirePistol2.firePistolSoundString, this.gameObject);
            if ((bool)(Object)FirePistol2.muzzleEffectPrefab)
                EffectManager.SimpleMuzzleFlash(FirePistol2.muzzleEffectPrefab, this.gameObject, targetMuzzle, false);
            this.AddRecoil(-0.4f * FirePistol2.recoilAmplitude, -0.8f * FirePistol2.recoilAmplitude, -0.3f * FirePistol2.recoilAmplitude, 0.3f * FirePistol2.recoilAmplitude);
            if (this.isAuthority)
                new BulletAttack()
                {
                    owner = this.gameObject,
                    weapon = this.gameObject,
                    origin = this.aimRay.origin,
                    aimVector = this.aimRay.direction,
                    maxDistance = 100f,
                    falloffModel = BulletAttack.FalloffModel.None,
                    minSpread = 0.0f,
                    maxSpread = this.characterBody.spreadBloomAngle,
                    damage = (FirePistol2.damageCoefficient * this.damageStat),
                    force = FirePistol2.force,
                    tracerEffectPrefab = FirePistol2.tracerEffectPrefab,
                    muzzleName = targetMuzzle,
                    hitEffectPrefab = FirePistol2.hitEffectPrefab,
                    isCrit = Util.CheckRoll(this.critStat, this.characterBody.master),
                    radius = 0.1f,
                    smartCollision = true
                }.Fire();
            this.characterBody.AddSpreadBloom(FirePistol2.spreadBloomValue);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = FirePistol2.baseDuration / this.attackSpeedStat;
            this.aimRay = this.GetAimRay();
            this.StartAimMode(this.aimRay, 3f);
            if (this.pistol % 2 == 0)
            {
                this.PlayAnimation("Gesture Additive, Left", "FirePistol, Left");
                this.FireBullet("MuzzleLeft");
            }
            else
            {
                this.PlayAnimation("Gesture Additive, Right", "FirePistol, Right");
                this.FireBullet("MuzzleRight");
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if ((double)this.fixedAge < (double)this.duration || !this.isAuthority)
                return;
            if (this.activatorSkillSlot.stock <= 0)
                this.outer.SetNextState((EntityState)new ReloadPistols());
            else
                this.outer.SetNextStateToMain();
        }

        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.Skill;
    }
}
