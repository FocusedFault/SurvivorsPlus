using R2API;
using RoR2;
using RoR2.Skills;
using EntityStates.Bandit2.Weapon;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SurvivorsPlus.Bandit
{
    public class BanditChanges
    {
        public DamageAPI.ModdedDamageType banditOpenWound;
        public DamageAPI.ModdedDamageType banditDoubleHemorrhage;
        private GameObject lightsOutEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2ResetEffect.prefab").WaitForCompletion();
        private GameObject bandit = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2Body.prefab").WaitForCompletion();
        private Material bloodMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matBloodHumanLarge.mat").WaitForCompletion();
        private DotController.DotIndex emptyDotIdx;

        public BanditChanges()
        {
            lightsOutEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().sharedMaterial = bloodMat;
            lightsOutEffect.transform.GetChild(1).GetComponent<ParticleSystemRenderer>().sharedMaterial = bloodMat;
            lightsOutEffect.transform.GetChild(2).GetComponent<ParticleSystemRenderer>().sharedMaterial = bloodMat;

            SkillLocator skillLocator = bandit.GetComponent<SkillLocator>();

            skillLocator.utility.skillFamily.variants[0].skillDef.beginSkillCooldownOnSkillEnd = true;

            SkillDef dagger = skillLocator.secondary.skillFamily.variants[0].skillDef;
            dagger.skillDescriptionToken = "Lunge and slash for <style=cIsDamage>360% damage</style>. Critical Strikes also cause <style=cIsHealth>2 Hemorrhage stacks</style>.";

            SkillDef lightsOut = skillLocator.special.skillFamily.variants[0].skillDef;
            lightsOut.skillNameToken = "Open Wound";
            lightsOut.skillDescriptionToken = "<style=cIsDamage>Slayer</style>. Fire a revolver shot for <style=cIsDamage>600% damage</style>. Clears all stacks of <style=cIsHealth>Hemorrhage, dealing remaining damage immediately</style>.";
            lightsOut.baseRechargeInterval = 6f;

            banditDoubleHemorrhage = DamageAPI.ReserveDamageType();
            banditOpenWound = DamageAPI.ReserveDamageType();

            DotAPI.CustomDotBehaviour customOpenWoundBehavior = OpenWoundBehavior;
            emptyDotIdx = DotAPI.RegisterDotDef(0f, 0f, DamageColorIndex.Default, null, customOpenWoundBehavior);

            On.RoR2.HealthComponent.SendDamageDealt += ApplyNewDamageTypes;
            On.EntityStates.Bandit2.Weapon.FireShotgun2.FireBullet += ModifyFire;
            On.EntityStates.Bandit2.Weapon.SlashBlade.AuthorityModifyOverlapAttack += IncreaseHemorrhageStacks;
            On.EntityStates.Bandit2.Weapon.Bandit2FirePrimaryBase.OnEnter += ReduceRecoil;
            On.EntityStates.Bandit2.Weapon.FireSidearmResetRevolver.ModifyBullet += AddOpenWound;
        }
        private void AddOpenWound(On.EntityStates.Bandit2.Weapon.FireSidearmResetRevolver.orig_ModifyBullet orig, FireSidearmResetRevolver self, BulletAttack bulletAttack)
        {
            bulletAttack.AddModdedDamageType(banditOpenWound);
            orig(self, bulletAttack);
        }

        private void OpenWoundBehavior(DotController self, DotController.DotStack dotStack)
        {
            if (dotStack.dotIndex == emptyDotIdx)
            {
                if (self.victimBody && self.victimBody.healthComponent)
                {
                    int i = 0;
                    int count = self.dotStackList.Count;
                    float totalDamage = 0f;

                    while (i < count)
                    {
                        if (self.dotStackList[i].dotIndex == DotController.DotIndex.SuperBleed)
                        {
                            // 0.25f
                            float remainingTicks = self.dotStackList[i].timer / 0.25f;
                            float damage = self.dotStackList[i].damage;
                            totalDamage += damage * remainingTicks;
                            self.dotStackList[i].timer = 0f;
                        }
                        i++;
                    }

                    if (totalDamage > 0f)
                    {
                        DamageInfo damageInfo = new DamageInfo();
                        damageInfo.damage = totalDamage;
                        damageInfo.position = self.victimBody.corePosition;
                        damageInfo.force = Vector3.zero;
                        damageInfo.crit = false;
                        damageInfo.procChainMask = default;
                        damageInfo.procCoefficient = 1f;
                        damageInfo.damageColorIndex = DamageColorIndex.SuperBleed;
                        damageInfo.damageType = DamageType.Generic;
                        self.victimBody.healthComponent.TakeDamage(damageInfo);
                        // EffectManager.SimpleImpactEffect(lightsOutEffect, self.victimBody.corePosition, Vector3.zero, true);
                        EffectManager.SpawnEffect(lightsOutEffect, new EffectData
                        {
                            origin = self.victimBody.corePosition,
                            scale = self.victimBody.radius
                        }, true);
                    }
                }
            }
        }

        private void ModifyFire(On.EntityStates.Bandit2.Weapon.FireShotgun2.orig_FireBullet orig, FireShotgun2 self, Ray aimRay)
        {
            self.StartAimMode(aimRay, 3f);
            self.DoFireEffects();
            self.PlayFireAnimation();
            self.AddRecoil(-1f * self.recoilAmplitudeY, -1.5f * self.recoilAmplitudeY, -1f * self.recoilAmplitudeX, 1f * self.recoilAmplitudeX);
            if (!self.isAuthority)
                return;
            Vector3 axis = Vector3.Cross(aimRay.direction, Vector3.up);
            float spreadAngle = 5f; // This is the total spread angle.
            for (int i = 0; i < self.bulletCount; ++i)
            {
                // Randomize yaw and pitch within the spread angle
                float randomYaw = Random.Range(-spreadAngle / 2f, spreadAngle / 2f);
                float randomPitch = Random.Range(-spreadAngle / 2f, spreadAngle / 2f);

                // Create a rotation for yaw around the up axis and for pitch around the calculated axis
                Quaternion yawRotation = Quaternion.AngleAxis(randomYaw, Vector3.up);
                Quaternion pitchRotation = Quaternion.AngleAxis(randomPitch, axis);

                // Apply rotations to the aim direction
                Vector3 direction = yawRotation * pitchRotation * aimRay.direction;

                // Create a new Ray for the bullet
                Ray aimRay1 = new Ray(aimRay.origin, direction);

                // Generate and fire the bullet
                BulletAttack bulletAttack = self.GenerateBulletAttack(aimRay1);
                self.ModifyBullet(bulletAttack);
                bulletAttack.Fire();
            }
        }

        private void IncreaseHemorrhageStacks(On.EntityStates.Bandit2.Weapon.SlashBlade.orig_AuthorityModifyOverlapAttack orig, SlashBlade self, OverlapAttack overlapAttack)
        {
            DamageAPI.AddModdedDamageType(overlapAttack, banditDoubleHemorrhage);
            orig(self, overlapAttack);
        }

        private void ApplyNewDamageTypes(On.RoR2.HealthComponent.orig_SendDamageDealt orig, DamageReport damageReport)
        {
            if (DamageAPI.HasModdedDamageType(damageReport.damageInfo, banditOpenWound))
            {
                if (damageReport.victimBody)
                    DotController.InflictDot(damageReport.victimBody.gameObject, damageReport.attacker, emptyDotIdx, 0f);
            }
            if (DamageAPI.HasModdedDamageType(damageReport.damageInfo, banditDoubleHemorrhage) && damageReport.damageInfo.crit)
            {
                if (damageReport.victimBody)
                    DotController.InflictDot(damageReport.victimBody.gameObject, damageReport.attacker, DotController.DotIndex.SuperBleed);
            }
            orig(damageReport);
        }

        private void ReduceRecoil(On.EntityStates.Bandit2.Weapon.Bandit2FirePrimaryBase.orig_OnEnter orig, Bandit2FirePrimaryBase self)
        {
            if (self is Bandit2FireRifle)
            {
                // self.characterBody.SetSpreadBloom(0);
                self.spreadBloomValue = 0.65f;
                self.spreadYawScale = 0.25f;
                self.spreadPitchScale = 0.3f;
            }
            orig(self);
        }
    }
}