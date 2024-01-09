using R2API;
using RoR2;
using RoR2.Skills;
using EntityStates.Bandit2.Weapon;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SurvivorsPlus.Bandit
{
    public class BanditChanges
    {
        public DamageAPI.ModdedDamageType banditOpenWound;
        private GameObject lightsOutEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2ResetEffect.prefab").WaitForCompletion();
        private GameObject bandit = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2Body.prefab").WaitForCompletion();
        private Material bloodMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matBloodHumanLarge.mat").WaitForCompletion();

        public BanditChanges()
        {
            lightsOutEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().sharedMaterial = bloodMat;
            lightsOutEffect.transform.GetChild(1).GetComponent<ParticleSystemRenderer>().sharedMaterial = bloodMat;
            lightsOutEffect.transform.GetChild(2).GetComponent<ParticleSystemRenderer>().sharedMaterial = bloodMat;

            SkillLocator skillLocator = bandit.GetComponent<SkillLocator>();
            SkillDef lightsOut = skillLocator.special.skillFamily.variants[0].skillDef;
            lightsOut.skillNameToken = "Open Wound";
            lightsOut.skillDescriptionToken = "<style=cIsDamage>Slayer</style>. Fire a Hemogore Round for <style=cIsDamage>600% damage</style>. Critical hits <style=cIsUtility>double hemorrhage stacks</style>.";
            lightsOut.baseRechargeInterval = 6f;


            banditOpenWound = DamageAPI.ReserveDamageType();
            IL.RoR2.GlobalEventManager.OnHitEnemy += ReduceHemorrhageDuration;
            On.RoR2.HealthComponent.SendDamageDealt += ApplyOpenWound;
            On.RoR2.DotController.InitDotCatalog += ChangeHemorrhageTicks;
            On.EntityStates.Bandit2.Weapon.Bandit2FirePrimaryBase.OnEnter += ReduceRecoil;
            On.EntityStates.Bandit2.Weapon.FireSidearmResetRevolver.ModifyBullet += AddOpenWound;
        }

        private void ChangeHemorrhageTicks(On.RoR2.DotController.orig_InitDotCatalog orig)
        {
            orig();
            DotController.dotDefs[6].interval = 0.25f;
            DotController.dotDefs[6].damageCoefficient = 0.66f;
        }

        private void ReduceHemorrhageDuration(ILContext il)
        {
            ILCursor ilCursor = new ILCursor(il);
            if (ilCursor.TryGotoNext(MoveType.Before,
                x => x.MatchLdcI4(6),
                x => x.MatchLdcR4(15f)
            ))
            {
                ++ilCursor.Index;
                ilCursor.Next.Operand = 7.5f;
            }
            else
                Debug.LogError("SurvivorPlus: Failed to apply Hemorrhage Duration hook");
        }

        private void ApplyOpenWound(On.RoR2.HealthComponent.orig_SendDamageDealt orig, DamageReport damageReport)
        {
            if (DamageAPI.HasModdedDamageType(damageReport.damageInfo, banditOpenWound) && damageReport.damageInfo.crit)
            {
                if (damageReport.victimBody)
                {
                    EffectManager.SimpleImpactEffect(lightsOutEffect, damageReport.damageInfo.position, -damageReport.damageInfo.force, true);
                    if (damageReport.victimBody.HasBuff(RoR2Content.Buffs.SuperBleed))
                    {
                        int buffCount = damageReport.victimBody.GetBuffCount(RoR2Content.Buffs.SuperBleed);
                        for (int i = 0; i < buffCount; i++)
                            damageReport.victimBody.AddBuff(RoR2Content.Buffs.SuperBleed);
                    }
                }
            }
            orig(damageReport);
        }

        private void AddOpenWound(On.EntityStates.Bandit2.Weapon.FireSidearmResetRevolver.orig_ModifyBullet orig, FireSidearmResetRevolver self, BulletAttack bulletAttack)
        {
            bulletAttack.AddModdedDamageType(banditOpenWound);
            bulletAttack.damageType |= DamageType.SuperBleedOnCrit;
            //orig(self, bulletAttack);
        }

        private void ReduceRecoil(On.EntityStates.Bandit2.Weapon.Bandit2FirePrimaryBase.orig_OnEnter orig, Bandit2FirePrimaryBase self)
        {
            if (self is Bandit2FireRifle)
            {
                self.spreadBloomValue = 0.65f;
                self.spreadYawScale = 0.25f;
                self.spreadPitchScale = 0.3f;
            }
            orig(self);
        }
    }
}