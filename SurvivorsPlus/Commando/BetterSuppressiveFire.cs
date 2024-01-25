using RoR2;
using EntityStates;
using EntityStates.Commando.CommandoWeapon;
using UnityEngine;

namespace SurvivorsPlus.Commando
{
    public class BetterSuppressiveFire : BaseState
    {
        public static GameObject effectPrefab = FireBarrage.effectPrefab;
        public static GameObject hitEffectPrefab = CommandoChanges.newHitEffect;
        public static GameObject tracerEffectPrefab = CommandoChanges.newTracer;
        public static float damageCoefficient = 2f;
        public static float force = FireSweepBarrage.force;
        public static float minSpread = FireBarrage.minSpread;
        public static float maxSpread = FireBarrage.maxSpread;
        public static float baseDurationBetweenShots = 1f;
        public static float totalDuration = 2f;
        public static float bulletRadius = 1.5f;
        public static int baseBulletCount = FireBarrage.baseBulletCount;
        public static string fireBarrageSoundString = FireBarrage.fireBarrageSoundString;
        public static float recoilAmplitude = FireBarrage.recoilAmplitude;
        public static float spreadBloomValue = FireBarrage.spreadBloomValue;
        private int totalBulletsFired;
        private int bulletCount;
        public float stopwatchBetweenShots;
        private Animator modelAnimator;
        private Transform modelTransform;
        private float duration;
        private float durationBetweenShots;

        public override void OnEnter()
        {
            base.OnEnter();
            this.characterBody.SetSpreadBloom(0.2f, false);
            this.durationBetweenShots = FireBarrage.baseDurationBetweenShots / this.attackSpeedStat;
            this.bulletCount = (int)((double)FireBarrage.baseBulletCount * (double)this.attackSpeedStat);
            this.duration = this.durationBetweenShots * this.bulletCount;
            this.modelAnimator = this.GetModelAnimator();
            this.modelTransform = this.GetModelTransform();
            this.PlayCrossfade("Gesture, Additive", nameof(FireBarrage), "FireBarrage.playbackRate", this.duration, 0.2f);
            this.PlayCrossfade("Gesture, Override", nameof(FireBarrage), "FireBarrage.playbackRate", this.duration, 0.2f);
            if ((bool)(Object)this.characterBody)
                this.characterBody.SetAimTimer(2f);
            this.FireBullet();
        }

        private void FireBullet()
        {
            Ray aimRay = this.GetAimRay();
            string muzzleName = "MuzzleRight";
            if ((bool)(Object)this.modelAnimator)
            {
                if ((bool)(Object)BetterSuppressiveFire.effectPrefab)
                    EffectManager.SimpleMuzzleFlash(BetterSuppressiveFire.effectPrefab, this.gameObject, muzzleName, false);
                this.PlayAnimation("Gesture Additive, Right", "FirePistol, Right");
            }
            this.AddRecoil(-0.8f * BetterSuppressiveFire.recoilAmplitude, -1f * BetterSuppressiveFire.recoilAmplitude, -0.1f * BetterSuppressiveFire.recoilAmplitude, 0.15f * BetterSuppressiveFire.recoilAmplitude);
            if (this.isAuthority)
                new BulletAttack()
                {
                    owner = this.gameObject,
                    weapon = this.gameObject,
                    origin = aimRay.origin,
                    aimVector = aimRay.direction,
                    minSpread = BetterSuppressiveFire.minSpread,
                    maxSpread = BetterSuppressiveFire.maxSpread,
                    bulletCount = 1U,
                    falloffModel = BulletAttack.FalloffModel.None,
                    damage = (BetterSuppressiveFire.damageCoefficient * this.damageStat),
                    force = BetterSuppressiveFire.force,
                    tracerEffectPrefab = BetterSuppressiveFire.tracerEffectPrefab,
                    muzzleName = muzzleName,
                    hitEffectPrefab = BetterSuppressiveFire.hitEffectPrefab,
                    isCrit = Util.CheckRoll(this.critStat, this.characterBody.master),
                    radius = BetterSuppressiveFire.bulletRadius,
                    smartCollision = true,
                    damageType = DamageType.Generic,
                    procCoefficient = 2f
                }.Fire();
            this.characterBody.AddSpreadBloom(BetterSuppressiveFire.spreadBloomValue);
            ++this.totalBulletsFired;
            int num = (int)Util.PlaySound(BetterSuppressiveFire.fireBarrageSoundString, this.gameObject);
        }

        public override void OnExit() => base.OnExit();

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.stopwatchBetweenShots += Time.fixedDeltaTime;
            if ((double)this.stopwatchBetweenShots >= (double)this.durationBetweenShots && this.totalBulletsFired < this.bulletCount)
            {
                this.stopwatchBetweenShots -= this.durationBetweenShots;
                this.FireBullet();
            }
            if (this.totalBulletsFired != this.bulletCount || !this.isAuthority)
                return;
            this.outer.SetNextStateToMain();
        }

        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.Skill;
    }
}