using R2API;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using EntityStates;
using EntityStates.CaptainDefenseMatrixItem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace SurvivorsPlus.Captain
{
    public class CaptainChanges
    {
        private GameObject tazerProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/CaptainTazer.prefab").WaitForCompletion();
        private GameObject tazerImpact = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/CaptainTazerNova.prefab").WaitForCompletion();
        private GameObject captain = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/CaptainBody.prefab").WaitForCompletion();
        public static DamageAPI.ModdedDamageType captainTaserSource;

        public CaptainChanges()
        {
            captainTaserSource = DamageAPI.ReserveDamageType();

            DamageAPI.ModdedDamageTypeHolderComponent mdc = tazerProjectile.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(captainTaserSource);

            ProjectileDamage pd = tazerProjectile.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.Shock5s;

            UnityEngine.Object.Destroy(tazerProjectile.GetComponent<ProjectileImpactExplosion>());
            ProjectileSingleTargetImpact psi = tazerProjectile.AddComponent<ProjectileSingleTargetImpact>();
            psi.destroyOnWorld = false;
            psi.destroyWhenNotAlive = true;
            psi.impactEffect = tazerImpact;

            captain.GetComponent<SkillLocator>().secondary.skillFamily.variants[0].skillDef.skillDescriptionToken = "<style=cIsDamage>Shocking</style>. Fire a fast tazer that deals <style=cIsDamage>100% damage</style>. <style=cIsUtility>Bounces to nearby enemies</style>.";

            On.RoR2.GlobalEventManager.OnHitEnemy += AddTaserBounce;
            On.EntityStates.CaptainDefenseMatrixItem.DefenseMatrixOn.DeleteNearbyProjectile += MicrobotEdit;
        }

        private void AddTaserBounce(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);
            if (damageInfo.HasModdedDamageType(captainTaserSource))
            {
                CharacterBody victimBody = victim.GetComponent<CharacterBody>();
                CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                if (victimBody && attackerBody)
                {
                    List<HealthComponent> bouncedObjects = new List<HealthComponent>();
                    bouncedObjects.Add(victimBody.healthComponent);

                    int initialTargets = 2; //Since the range is smaller than Epidemic, add more initial targets.
                    float range = 15f;

                    //Need to individually find all targets for the first bounce.
                    for (int i = 0; i < initialTargets; i++)
                    {
                        LightningOrb taserLightning = new LightningOrb
                        {
                            bouncedObjects = bouncedObjects,
                            attacker = damageInfo.attacker,
                            inflictor = damageInfo.attacker,
                            damageValue = damageInfo.damage,
                            procCoefficient = 1f,
                            teamIndex = attackerBody.teamComponent.teamIndex,
                            isCrit = damageInfo.crit,
                            procChainMask = damageInfo.procChainMask,
                            lightningType = LightningOrb.LightningType.Ukulele,
                            damageColorIndex = DamageColorIndex.Default,
                            bouncesRemaining = 2,
                            targetsToFindPerBounce = 1,
                            range = range,
                            origin = damageInfo.position,
                            damageType = DamageType.Shock5s,
                            speed = 120f
                        };
                        //2 initial lightnings
                        //Each lightning will hit, then bounce 2 extra times

                        HurtBox hurtBox = taserLightning.PickNextTarget(victimBody.corePosition);

                        //Fire orb if HurtBox is found.
                        if (hurtBox)
                        {
                            taserLightning.target = hurtBox;
                            OrbManager.instance.AddOrb(taserLightning);
                            taserLightning.bouncedObjects.Add(hurtBox.healthComponent);
                        }
                    }
                }
            }
        }

        private bool MicrobotEdit(On.EntityStates.CaptainDefenseMatrixItem.DefenseMatrixOn.orig_DeleteNearbyProjectile orig, DefenseMatrixOn self)
        {
            Vector3 vector = self.attachedBody ? self.attachedBody.corePosition : Vector3.zero;
            TeamIndex teamIndex = self.attachedBody ? self.attachedBody.teamComponent.teamIndex : TeamIndex.None;
            float num = DefenseMatrixOn.projectileEraserRadius * DefenseMatrixOn.projectileEraserRadius;
            int num2 = 0;
            int itemStack = self.GetItemStack();
            bool result = false;
            List<ProjectileController> instancesList = InstanceTracker.GetInstancesList<ProjectileController>();
            List<ProjectileController> list = new List<ProjectileController>();
            int num3 = 0;
            int count = instancesList.Count;
            while (num3 < count && num2 < itemStack)
            {
                ProjectileController projectileController = instancesList[num3];
                if (!projectileController.cannotBeDeleted && projectileController.teamFilter.teamIndex != teamIndex && (projectileController.transform.position - vector).sqrMagnitude < num)
                {
                    bool canDelete = true;

                    ProjectileSimple ps = projectileController.gameObject.GetComponent<ProjectileSimple>();
                    ProjectileCharacterController pcc = projectileController.gameObject.GetComponent<ProjectileCharacterController>();

                    if ((!ps || (ps && ps.desiredForwardSpeed == 0f)) && !pcc)
                        canDelete = false;

                    if (canDelete)
                    {
                        list.Add(projectileController);
                        num2++;
                    }
                }
                num3++;
            }
            int i = 0;
            int count2 = list.Count;
            while (i < count2)
            {
                ProjectileController projectileController2 = list[i];
                if (projectileController2)
                {
                    result = true;
                    Vector3 position = projectileController2.transform.position;
                    Vector3 start = vector;
                    if (DefenseMatrixOn.tracerEffectPrefab)
                    {
                        EffectData effectData = new EffectData
                        {
                            origin = position,
                            start = start
                        };
                        EffectManager.SpawnEffect(DefenseMatrixOn.tracerEffectPrefab, effectData, true);
                    }
                    EntityState.Destroy(projectileController2.gameObject);
                }
                i++;
            }
            return result;
        }
    }
}