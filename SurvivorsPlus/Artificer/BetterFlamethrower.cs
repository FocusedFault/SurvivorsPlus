using RoR2;
using UnityEngine;
using EntityStates;
using EntityStates.Mage.Weapon;

namespace SurvivorsPlus.Artificer
{
    public class BetterFlamethrower : BaseSkillState
    {
        public GameObject flamethrowerEffectPrefab = ArtificerChanges.flamethrowerEffect;
        public static GameObject impactEffectPrefab;
        public static GameObject tracerEffectPrefab = Flamethrower.tracerEffectPrefab;
        public float maxDistance = 32f;
        public static float radius;
        public static float baseEntryDuration = 1f;
        public static float baseFlamethrowerDuration = 2f;
        public static float totalDamageCoefficient = 1.2f;
        public static float procCoefficientPerTick;
        public static float tickFrequency;
        public static float force = 20f;
        public static string startAttackSoundString;
        public static string endAttackSoundString;
        public static float ignitePercentChance;
        public static float recoilForce;
        private float tickDamageCoefficient;
        private float flamethrowerStopwatch;
        private float stopwatch;
        private float entryDuration;
        private float flamethrowerDuration;
        private bool hasBegunFlamethrower;
        private ChildLocator childLocator;
        private Transform leftFlamethrowerTransform;
        private Transform rightFlamethrowerTransform;
        private Transform leftMuzzleTransform;
        private Transform rightMuzzleTransform;
        private bool isCrit;
        private const float flamethrowerEffectBaseDistance = 16f;

        public override void OnEnter()
        {
            base.OnEnter();
            this.stopwatch = 0.0f;
            this.entryDuration = Flamethrower.baseEntryDuration / this.attackSpeedStat;
            this.flamethrowerDuration = Flamethrower.baseFlamethrowerDuration;
            Transform modelTransform = this.GetModelTransform();
            if ((bool)(Object)this.characterBody)
                this.characterBody.SetAimTimer((float)((double)this.entryDuration + (double)this.flamethrowerDuration + 1.0));
            if ((bool)(Object)modelTransform)
            {
                this.childLocator = modelTransform.GetComponent<ChildLocator>();
                this.leftMuzzleTransform = this.childLocator.FindChild("MuzzleLeft");
                this.rightMuzzleTransform = this.childLocator.FindChild("MuzzleRight");
            }
            int num = Mathf.CeilToInt(this.flamethrowerDuration * Flamethrower.tickFrequency);
            this.tickDamageCoefficient = Flamethrower.totalDamageCoefficient / (float)num;
            if (this.isAuthority && (bool)(Object)this.characterBody)
                this.isCrit = Util.CheckRoll(this.critStat, this.characterBody.master);
            this.PlayAnimation("Gesture, Additive", "PrepFlamethrower", "Flamethrower.playbackRate", this.entryDuration);
        }

        public override void OnExit()
        {
            int num = (int)Util.PlaySound(Flamethrower.endAttackSoundString, this.gameObject);
            this.PlayCrossfade("Gesture, Additive", "ExitFlamethrower", 0.1f);
            if ((bool)(Object)this.leftFlamethrowerTransform)
                EntityState.Destroy((Object)this.leftFlamethrowerTransform.gameObject);
            if ((bool)(Object)this.rightFlamethrowerTransform)
                EntityState.Destroy((Object)this.rightFlamethrowerTransform.gameObject);
            base.OnExit();
        }

        private void FireGauntlet(string muzzleString)
        {
            Ray aimRay = this.GetAimRay();
            if (!this.isAuthority)
                return;
            new BulletAttack()
            {
                owner = this.gameObject,
                weapon = this.gameObject,
                origin = aimRay.origin,
                aimVector = aimRay.direction,
                minSpread = 0.0f,
                damage = (this.tickDamageCoefficient * this.damageStat),
                force = Flamethrower.force,
                muzzleName = muzzleString,
                hitEffectPrefab = Flamethrower.impactEffectPrefab,
                isCrit = this.isCrit,
                radius = Flamethrower.radius,
                falloffModel = BulletAttack.FalloffModel.None,
                stopperMask = LayerIndex.world.mask,
                procCoefficient = Flamethrower.procCoefficientPerTick,
                maxDistance = this.maxDistance,
                smartCollision = true,
                damageType = DamageType.IgniteOnHit
            }.Fire();
            if (!(bool)(Object)this.characterMotor)
                return;
            this.characterMotor.ApplyForce(aimRay.direction * -Flamethrower.recoilForce);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.stopwatch += Time.fixedDeltaTime;
            if ((double)this.stopwatch >= (double)this.entryDuration && !this.hasBegunFlamethrower)
            {
                this.hasBegunFlamethrower = true;
                int num = (int)Util.PlaySound(Flamethrower.startAttackSoundString, this.gameObject);
                this.PlayAnimation("Gesture, Additive", nameof(Flamethrower), "Flamethrower.playbackRate", this.flamethrowerDuration);
                if ((bool)(Object)this.childLocator)
                {
                    Transform child1 = this.childLocator.FindChild("MuzzleLeft");
                    Transform child2 = this.childLocator.FindChild("MuzzleRight");
                    if ((bool)(Object)child1)
                        this.leftFlamethrowerTransform = Object.Instantiate<GameObject>(this.flamethrowerEffectPrefab, child1).transform;
                    if ((bool)(Object)child2)
                        this.rightFlamethrowerTransform = Object.Instantiate<GameObject>(this.flamethrowerEffectPrefab, child2).transform;
                    if ((bool)(Object)this.leftFlamethrowerTransform)
                        this.leftFlamethrowerTransform.GetComponent<ScaleParticleSystemDuration>().newDuration = 99f;
                    if ((bool)(Object)this.rightFlamethrowerTransform)
                        this.rightFlamethrowerTransform.GetComponent<ScaleParticleSystemDuration>().newDuration = 99f;
                }
                this.FireGauntlet("MuzzleCenter");
            }
            if (this.hasBegunFlamethrower)
            {
                this.flamethrowerStopwatch += Time.deltaTime;
                float num = 1f / Flamethrower.tickFrequency / this.attackSpeedStat;
                if ((double)this.flamethrowerStopwatch > (double)num)
                {
                    this.flamethrowerStopwatch -= num;
                    this.FireGauntlet("MuzzleCenter");
                }
                this.UpdateFlamethrowerEffect();
            }
            if (((double)this.fixedAge < (double)this.entryDuration || this.IsKeyDownAuthority()) && !this.characterBody.isSprinting || !this.isAuthority)
                return;
            this.outer.SetNextStateToMain();
        }

        private void UpdateFlamethrowerEffect()
        {
            Ray aimRay = this.GetAimRay();
            Vector3 direction1 = aimRay.direction;
            Vector3 direction2 = aimRay.direction;
            if ((bool)(Object)this.leftFlamethrowerTransform)
                this.leftFlamethrowerTransform.forward = direction1;
            if (!(bool)(Object)this.rightFlamethrowerTransform)
                return;
            this.rightFlamethrowerTransform.forward = direction2;
        }

        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.Skill;
    }
}
