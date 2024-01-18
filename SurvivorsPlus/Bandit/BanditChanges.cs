using R2API;
using RoR2;
using RoR2.Skills;
using EntityStates.Bandit2.Weapon;
using MonoMod.Cil;
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

        public BanditChanges()
        {
            SurvivorsPlus.ChangeEntityStateValue("RoR2/Base/Bandit2/EntityStates.Bandit2.StealthMode.asset", "duration", "4");

            lightsOutEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().sharedMaterial = bloodMat;
            lightsOutEffect.transform.GetChild(1).GetComponent<ParticleSystemRenderer>().sharedMaterial = bloodMat;
            lightsOutEffect.transform.GetChild(2).GetComponent<ParticleSystemRenderer>().sharedMaterial = bloodMat;

            SkillLocator skillLocator = bandit.GetComponent<SkillLocator>();

            skillLocator.utility.skillFamily.variants[0].skillDef.baseRechargeInterval = 8f;

            SkillDef dagger = skillLocator.secondary.skillFamily.variants[0].skillDef;
            dagger.skillDescriptionToken = "Lunge and slash for <style=cIsDamage>360% damage</style>. Critical Strikes also cause <style=cIsHealth>2 Hemorrhage stacks</style>.";

            SkillDef lightsOut = skillLocator.special.skillFamily.variants[0].skillDef;
            lightsOut.skillNameToken = "Open Wound";
            lightsOut.skillDescriptionToken = "<style=cIsDamage>Slayer</style>. Fire a Hemogore round for <style=cIsDamage>600% damage</style>. Critical hits <style=cIsUtility>double Hemorrhage stacks</style>.";
            lightsOut.baseRechargeInterval = 6f;

            banditOpenWound = DamageAPI.ReserveDamageType();
            banditDoubleHemorrhage = DamageAPI.ReserveDamageType();
            IL.RoR2.GlobalEventManager.OnHitEnemy += ReduceHemorrhageDuration;
            On.RoR2.HealthComponent.SendDamageDealt += ApplyNewDamageTypes;
            //  On.RoR2.DotController.InitDotCatalog += ChangeHemorrhageTicks;
            On.EntityStates.Bandit2.Weapon.SlashBlade.AuthorityModifyOverlapAttack += IncreaseHemorrhageStacks;
            On.EntityStates.Bandit2.Weapon.Bandit2FirePrimaryBase.OnEnter += ReduceRecoil;
            On.EntityStates.Bandit2.Weapon.FireSidearmResetRevolver.ModifyBullet += AddOpenWound;
        }

        private void IncreaseHemorrhageStacks(On.EntityStates.Bandit2.Weapon.SlashBlade.orig_AuthorityModifyOverlapAttack orig, SlashBlade self, OverlapAttack overlapAttack)
        {
            DamageAPI.AddModdedDamageType(overlapAttack, banditDoubleHemorrhage);
            orig(self, overlapAttack);
        }
        /*
        private void ChangeHemorrhageTicks(On.RoR2.DotController.orig_InitDotCatalog orig)
        {
            orig();
            DotController.dotDefs[6].interval = 0.25f;
            DotController.dotDefs[6].damageCoefficient = 0.495f; 0.33 15 is 750%
        }
        */
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

        private void ApplyNewDamageTypes(On.RoR2.HealthComponent.orig_SendDamageDealt orig, DamageReport damageReport)
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
                            DotController.InflictDot(damageReport.victimBody.gameObject, damageReport.attacker, DotController.DotIndex.SuperBleed);
                    }
                }
            }
            if (DamageAPI.HasModdedDamageType(damageReport.damageInfo, banditDoubleHemorrhage) && damageReport.damageInfo.crit)
            {
                if (damageReport.victimBody)
                    DotController.InflictDot(damageReport.victimBody.gameObject, damageReport.attacker, DotController.DotIndex.SuperBleed);
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