using RoR2;
using System.Linq;
using EntityStates;
using EntityStates.Merc;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

#nullable disable
namespace SurvivorsPlus.Mercenary
{
    public class EvisNux : BaseState
    {
        private Transform modelTransform;
        public static GameObject blinkPrefab;
        public static float duration = 2f;
        public static float damageCoefficient;
        public static float damageFrequency;
        public static float procCoefficient;
        public static string beginSoundString;
        public static string endSoundString;
        public static float maxRadius;
        public static GameObject hitEffectPrefab;
        public static string slashSoundString;
        public static string impactSoundString;
        public static string dashSoundString;
        public static float slashPitch;
        public static float smallHopVelocity;
        public static float lingeringInvincibilityDuration;
        private CharacterModel characterModel;
        private float stopwatch;
        private float attackStopwatch;
        private bool crit;
        private static float minimumDuration = 0.5f;
        private CameraTargetParams.AimRequest aimRequest;

        public override void OnEnter()
        {
            base.OnEnter();
            this.CreateBlinkEffect(Util.GetCorePosition(this.gameObject));
            Util.PlayAttackSpeedSound(Evis.beginSoundString, this.gameObject, 1.2f);
            this.crit = Util.CheckRoll(this.critStat, this.characterBody.master);
            this.modelTransform = this.GetModelTransform();
            if ((bool)this.modelTransform)
                this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
            if ((bool)this.characterModel)
                ++this.characterModel.invisibilityCount;
            if ((bool)this.cameraTargetParams)
                this.aimRequest = this.cameraTargetParams.RequestAimType(CameraTargetParams.AimType.Aura);
            if (!NetworkServer.active)
                return;
            this.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.stopwatch += Time.fixedDeltaTime;
            this.attackStopwatch += Time.fixedDeltaTime;
            float num1 = 1f / Evis.damageFrequency / this.attackSpeedStat;
            if (this.attackStopwatch >= (double)num1)
            {
                this.attackStopwatch -= num1;
                List<HurtBox> hurtboxList = this.SearchForTarget();
                if (hurtboxList.Count > 0)
                {
                    Util.PlayAttackSpeedSound(Evis.slashSoundString, this.gameObject, Evis.slashPitch);
                    Util.PlaySound(Evis.dashSoundString, this.gameObject);
                    Util.PlaySound(Evis.impactSoundString, this.gameObject);
                    foreach (HurtBox hurtBox in hurtboxList)
                    {
                        HurtBoxGroup hurtBoxGroup = hurtBox.hurtBoxGroup;
                        HurtBox hurtBox2 = hurtBoxGroup.hurtBoxes[Random.Range(0, hurtBoxGroup.hurtBoxes.Length - 1)];
                        if ((bool)hurtBox2)
                        {
                            Vector3 position = hurtBox2.transform.position;
                            Vector2 normalized = Random.insideUnitCircle.normalized;
                            Vector3 normal = new Vector3(normalized.x, 0.0f, normalized.y);
                            EffectManager.SimpleImpactEffect(Evis.hitEffectPrefab, position, normal, false);
                            Transform transform = hurtBox.hurtBoxGroup.transform;
                            TemporaryOverlay temporaryOverlay = transform.gameObject.AddComponent<TemporaryOverlay>();
                            temporaryOverlay.duration = num1;
                            temporaryOverlay.animateShaderAlpha = true;
                            temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0.0f, 1f, 1f, 0.0f);
                            temporaryOverlay.destroyComponentOnEnd = true;
                            temporaryOverlay.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matMercEvisTarget");
                            temporaryOverlay.AddToCharacerModel(transform.GetComponent<CharacterModel>());
                            if (NetworkServer.active)
                            {
                                DamageInfo damageInfo = new DamageInfo();
                                damageInfo.damage = Evis.damageCoefficient * this.damageStat;
                                damageInfo.attacker = this.gameObject;
                                damageInfo.procCoefficient = Evis.procCoefficient;
                                damageInfo.position = hurtBox2.transform.position;
                                damageInfo.crit = this.crit;
                                hurtBox2.healthComponent.TakeDamage(damageInfo);
                                GlobalEventManager.instance.OnHitEnemy(damageInfo, hurtBox2.healthComponent.gameObject);
                                GlobalEventManager.instance.OnHitAll(damageInfo, hurtBox2.healthComponent.gameObject);
                            }
                        }
                    }
                }
                else if (this.isAuthority && (double)this.stopwatch > Evis.minimumDuration)
                    this.outer.SetNextStateToMain();
            }
            if ((bool)this.characterMotor)
                this.characterMotor.velocity = Vector3.zero;
            if (this.stopwatch < (double)Evis.duration || !this.isAuthority)
                return;
            this.outer.SetNextStateToMain();
        }

        private List<HurtBox> SearchForTarget()
        {
            BullseyeSearch bullseyeSearch = new BullseyeSearch();
            bullseyeSearch.searchOrigin = this.transform.position;
            bullseyeSearch.searchDirection = Random.onUnitSphere;
            bullseyeSearch.maxDistanceFilter = 8f;
            bullseyeSearch.teamMaskFilter = TeamMask.GetUnprotectedTeams(this.GetTeam());
            bullseyeSearch.sortMode = BullseyeSearch.SortMode.Distance;
            bullseyeSearch.RefreshCandidates();
            bullseyeSearch.FilterOutGameObject(this.gameObject);
            return bullseyeSearch.GetResults().ToList();
        }

        private void CreateBlinkEffect(Vector3 origin)
        {
            EffectManager.SpawnEffect(Evis.blinkPrefab, new EffectData()
            {
                rotation = Util.QuaternionSafeLookRotation(Vector3.up),
                origin = origin
            }, false);
        }

        public override void OnExit()
        {
            Util.PlaySound(Evis.endSoundString, this.gameObject);
            this.CreateBlinkEffect(Util.GetCorePosition(this.gameObject));
            this.modelTransform = this.GetModelTransform();
            if ((bool)this.modelTransform)
            {
                TemporaryOverlay temporaryOverlay1 = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay1.duration = 0.6f;
                temporaryOverlay1.animateShaderAlpha = true;
                temporaryOverlay1.alphaCurve = AnimationCurve.EaseInOut(0.0f, 1f, 1f, 0.0f);
                temporaryOverlay1.destroyComponentOnEnd = true;
                temporaryOverlay1.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matMercEvisTarget");
                temporaryOverlay1.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
                TemporaryOverlay temporaryOverlay2 = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay2.duration = 0.7f;
                temporaryOverlay2.animateShaderAlpha = true;
                temporaryOverlay2.alphaCurve = AnimationCurve.EaseInOut(0.0f, 1f, 1f, 0.0f);
                temporaryOverlay2.destroyComponentOnEnd = true;
                temporaryOverlay2.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashExpanded");
                temporaryOverlay2.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
            }
            if ((bool)this.characterModel)
                --this.characterModel.invisibilityCount;
            this.aimRequest?.Dispose();
            if (NetworkServer.active)
            {
                this.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
                this.characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, Evis.lingeringInvincibilityDuration);
            }
            Util.PlaySound(Evis.endSoundString, this.gameObject);
            this.SmallHop(this.characterMotor, Evis.smallHopVelocity);
            base.OnExit();
        }
    }
}
