using RoR2;
using EntityStates;
using EntityStates.Merc;
using UnityEngine;
using UnityEngine.Networking;

namespace SurvivorsPlus.Mercenary
{
    public class EvisDashNux : BaseState
    {
        private Transform modelTransform;
        public static GameObject blinkPrefab;
        private float stopwatch;
        private Vector3 dashVector = Vector3.zero;
        public static float smallHopVelocity;
        public static float dashPrepDuration;
        public static float dashDuration = 0.3f;
        public static float speedCoefficient = 25f;
        public static string beginSoundString;
        public static string endSoundString;
        public static float overlapSphereRadius;
        public static float lollypopFactor;
        private Animator animator;
        private CharacterModel characterModel;
        private HurtBoxGroup hurtboxGroup;
        private bool isDashing;
        private CameraTargetParams.AimRequest aimRequest;

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound(EvisDash.beginSoundString, this.gameObject);
            this.modelTransform = this.GetModelTransform();
            if ((bool)this.cameraTargetParams)
                this.aimRequest = this.cameraTargetParams.RequestAimType(CameraTargetParams.AimType.Aura);
            if ((bool)this.modelTransform)
            {
                this.animator = this.modelTransform.GetComponent<Animator>();
                this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
            }
            if (this.isAuthority)
                this.SmallHop(this.characterMotor, EvisDash.smallHopVelocity);
            if (NetworkServer.active)
                this.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            this.PlayAnimation("FullBody, Override", "EvisPrep", "EvisPrep.playbackRate", EvisDash.dashPrepDuration);
            this.dashVector = this.inputBank.aimDirection;
            this.characterDirection.forward = this.dashVector;
        }

        private void CreateBlinkEffect(Vector3 origin)
        {
            EffectManager.SpawnEffect(EvisDash.blinkPrefab, new EffectData()
            {
                rotation = Util.QuaternionSafeLookRotation(this.dashVector),
                origin = origin
            }, false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.stopwatch += Time.fixedDeltaTime;
            if ((double)this.stopwatch > (double)EvisDash.dashPrepDuration && !this.isDashing)
            {
                this.isDashing = true;
                this.dashVector = this.inputBank.aimDirection;
                this.CreateBlinkEffect(Util.GetCorePosition(this.gameObject));
                this.PlayCrossfade("FullBody, Override", "EvisLoop", 0.1f);
                if ((bool)this.modelTransform)
                {
                    TemporaryOverlay temporaryOverlay1 = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay1.duration = 0.6f;
                    temporaryOverlay1.animateShaderAlpha = true;
                    temporaryOverlay1.alphaCurve = AnimationCurve.EaseInOut(0.0f, 1f, 1f, 0.0f);
                    temporaryOverlay1.destroyComponentOnEnd = true;
                    temporaryOverlay1.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashBright");
                    temporaryOverlay1.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
                    TemporaryOverlay temporaryOverlay2 = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay2.duration = 0.7f;
                    temporaryOverlay2.animateShaderAlpha = true;
                    temporaryOverlay2.alphaCurve = AnimationCurve.EaseInOut(0.0f, 1f, 1f, 0.0f);
                    temporaryOverlay2.destroyComponentOnEnd = true;
                    temporaryOverlay2.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashExpanded");
                    temporaryOverlay2.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
                }
            }
            bool flag = (double)this.stopwatch >= (double)EvisDash.dashDuration + (double)EvisDash.dashPrepDuration;
            if (this.isDashing)
            {
                if ((bool)this.characterMotor && (bool)this.characterDirection)
                    this.characterMotor.rootMotion += this.dashVector * (this.moveSpeedStat * EvisDash.speedCoefficient * Time.fixedDeltaTime);
                if (this.isAuthority)
                {
                    foreach (Component component1 in Physics.OverlapSphere(this.transform.position, this.characterBody.radius + EvisDash.overlapSphereRadius * (flag ? EvisDash.lollypopFactor : 1f), (int)LayerIndex.entityPrecise.mask))
                    {
                        HurtBox component2 = component1.GetComponent<HurtBox>();
                        if ((bool)component2 && component2.healthComponent != this.healthComponent)
                        {
                            this.outer.SetNextState(new EvisNux());
                            return;
                        }
                    }
                }
            }
            if (!flag || !this.isAuthority)
                return;
            this.outer.SetNextStateToMain();
        }

        public override void OnExit()
        {
            int num = (int)Util.PlaySound(EvisDash.endSoundString, this.gameObject);
            this.characterMotor.velocity *= 0.1f;
            this.SmallHop(this.characterMotor, EvisDash.smallHopVelocity);
            this.aimRequest?.Dispose();
            this.PlayAnimation("FullBody, Override", "EvisLoopExit");
            if (NetworkServer.active)
                this.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
            base.OnExit();
        }
    }
}
