using R2API;
using RoR2;
using RoR2.Skills;
using RoR2.Projectile;
using EntityStates;
using EntityStates.Railgunner.Weapon;
using EntityStates.Railgunner.Reload;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace SurvivorsPlus.Railgunner
{
    public class RailgunnerChanges
    {
        private GameObject railgunner = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerBody.prefab").WaitForCompletion();
        private GameObject polarMine = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerMineAltDetonated.prefab").WaitForCompletion();
        private SkillDef snipeLight = Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC1/Railgunner/RailgunnerBodyFireSnipeLight.asset").WaitForCompletion();
        private SkillDef snipeHeavy = Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC1/Railgunner/RailgunnerBodyScopeHeavy.asset").WaitForCompletion();
        private SkillFamily utilityFamily = Addressables.LoadAssetAsync<SkillFamily>("RoR2/DLC1/Railgunner/RailgunnerBodyUtilityFamily.asset").WaitForCompletion();
        private SkillFamily specialFamily = Addressables.LoadAssetAsync<SkillFamily>("RoR2/DLC1/Railgunner/RailgunnerBodySpecialFamily.asset").WaitForCompletion();

        public RailgunnerChanges()
        {
            SurvivorsPlus.ChangeEntityStateValue("RoR2/DLC1/Railgunner/EntityStates.Railgunner.Backpack.Offline.asset", "baseDuration", "10");
            SurvivorsPlus.ChangeEntityStateValue("RoR2/DLC1/Railgunner/EntityStates.Railgunner.Weapon.FireSnipeLight.asset", "critDamageMultiplier", "0");
            SurvivorsPlus.ChangeEntityStateValue("RoR2/DLC1/Railgunner/EntityStates.Railgunner.Weapon.FireSnipeLight.asset", "damageCoefficient", "3");
            SurvivorsPlus.ChangeEntityStateValue("RoR2/DLC1/Railgunner/EntityStates.Railgunner.Weapon.FireSnipeHeavy.asset", "damageCoefficient", "6");
            SurvivorsPlus.ChangeEntityStateValue("RoR2/DLC1/Railgunner/EntityStates.Railgunner.Weapon.FireSnipeSuper.asset", "damageCoefficient", "24");
            SurvivorsPlus.ChangeEntityStateValue("RoR2/DLC1/Railgunner/EntityStates.Railgunner.Weapon.FireSnipeCryo.asset", "damageCoefficient", "12");

            polarMine.AddComponent<PolarSlow>();
            GameObject.Destroy(polarMine.GetComponent<SlowDownProjectiles>());

            SkillLocator skillLocator = railgunner.GetComponent<SkillLocator>();

            foreach (GenericSkill gs in railgunner.GetComponents<GenericSkill>())
            {
                if (!gs.skillName.Contains("Passive"))
                    GameObject.Destroy(gs);
            }

            FamilyChanges();

            skillLocator.primary.skillFamily.variants[0].skillDef.skillNameToken = "HH44 Rounds";
            skillLocator.primary.skillFamily.variants[0].skillDef.skillDescriptionToken = "Fire a light projectile for <style=cIsDamage>300% damage</style>.";
            skillLocator.secondary.skillFamily.variants[0].skillDef.skillDescriptionToken = "Activate your <style=cIsUtility>long-range scope</style>, highlighting <style=cIsDamage>Weak Points</style> and transforming your weapon into a piercing <style=cIsDamage>600% damage</style> railgun.";
            skillLocator.special.skillFamily.variants[0].skillDef.skillDescriptionToken = "Fire a <style=cIsDamage>piercing</style> round for <style=cIsDamage>2400% damage</style> and <style=cIsDamage>150% Weak Point damage</style>. Afterwards, <style=cIsHealth>all your weapons are disabled</style> for <style=cIsHealth>10</style> seconds.";
            skillLocator.special.skillFamily.variants[1].skillDef.skillDescriptionToken = "<style=cIsUtility>Freezing</style>. Fire <style=cIsDamage>piercing</style> round for <style=cIsDamage>1200% damage</style>.";
            skillLocator.utility.skillFamily.variants[1].skillDef.skillDescriptionToken = "Throw out a device that <style=cIsUtility>slows down</style> all nearby enemies and <style=cIsUtility>reflects projectiles</style>.";

            IL.RoR2.CharacterBody.RecalculateStats += DoubleLope;
            IL.RoR2.CharacterBody.RecalculateStats += DoubleCritMultiplier;
            On.EntityStates.Railgunner.Weapon.BaseFireSnipe.OnEnter += AddReload;
            On.EntityStates.Railgunner.Weapon.BaseFireSnipe.ModifyBullet += AlterBullet;
            On.RoR2.HurtBox.OnEnable += ReduceWeakpointSize;
        }

        private void ReduceWeakpointSize(On.RoR2.HurtBox.orig_OnEnable orig, HurtBox self)
        {
            orig(self);
            HurtBox.sniperTargetRadius = 0.75f;
        }

        private void DoubleLope(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.After,
                x => x.MatchLdcR4(2f),
                x => x.MatchLdcR4(1f)
                );
            c.Emit(OpCodes.Ldarg_0);
            c.EmitDelegate<Func<float, CharacterBody, float>>((num, body) =>
            {
                if (body.inventory && body.inventory.GetItemCount(DLC1Content.Items.ConvertCritChanceToCritDamage) > 0)
                    return 2f;
                else
                    return num;
            });
        }

        private void DoubleCritMultiplier(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            c.GotoNext(MoveType.Before,
                x => x.MatchCall<CharacterBody>("get_critMultiplier"),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(0.01f)
                );
            c.Index += 2;
            c.Next.Operand = 0.02f;
        }

        private void AddReload(On.EntityStates.Railgunner.Weapon.BaseFireSnipe.orig_OnEnter orig, BaseFireSnipe self)
        {
            if (self is FireSnipeLight)
            {
                // self.critDamageMultiplier = 0.25f;
                self.useSecondaryStocks = true;
                self.queueReload = true;
            }
            orig(self);
        }

        private void AlterBullet(On.EntityStates.Railgunner.Weapon.BaseFireSnipe.orig_ModifyBullet orig, BaseFireSnipe self, BulletAttack bulletAttack)
        {
            if (self is FireSnipeLight)
            {
                bulletAttack.sniper = true;
                bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
                EntityStateMachine byCustomName1 = EntityStateMachine.FindByCustomName(self.gameObject, "Reload");
                if ((bool)byCustomName1)
                {
                    if (byCustomName1.state is Boosted state2)
                    {
                        bulletAttack.damage += state2.GetBonusDamage() / 4;
                        state2.ConsumeBoost(self.queueReload);
                    }
                    else if (self.queueReload && byCustomName1.state is Waiting state1)
                        state1.QueueReload();
                }
                EntityStateMachine byCustomName2 = EntityStateMachine.FindByCustomName(self.gameObject, "Backpack");
                EntityState newNextState = self.InstantiateBackpackState();
                if (!(bool)byCustomName2 || newNextState == null)
                    return;
                byCustomName2.SetNextState(newNextState);
            }
            else
                orig(self, bulletAttack);
        }

        private void FamilyChanges()

        {
            SkillLocator skillLocator = railgunner.GetComponent<SkillLocator>();

            GenericSkill primarySkill = railgunner.AddComponent<GenericSkill>();
            primarySkill.skillName = "RailgunnerPrimary";

            SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            (newFamily as ScriptableObject).name = "RailgunnerPrimaryFamily";
            newFamily.variants = new SkillFamily.Variant[] { new SkillFamily.Variant() { skillDef = snipeLight } };

            primarySkill._skillFamily = newFamily;
            ContentAddition.AddSkillFamily(newFamily);
            skillLocator.primary = primarySkill;

            GenericSkill secondarySkill = railgunner.AddComponent<GenericSkill>();
            primarySkill.skillName = "RailgunnerSecondary";

            SkillFamily newFamily2 = ScriptableObject.CreateInstance<SkillFamily>();
            (newFamily2 as ScriptableObject).name = "RailgunnerSecondaryFamily";
            newFamily2.variants = new SkillFamily.Variant[] { new SkillFamily.Variant() { skillDef = snipeHeavy } };

            secondarySkill._skillFamily = newFamily2;
            ContentAddition.AddSkillFamily(newFamily2);
            skillLocator.secondary = secondarySkill;

            GenericSkill utilitySkill = railgunner.AddComponent<GenericSkill>();
            utilitySkill.skillName = "RailgunnerUtility";
            utilitySkill._skillFamily = utilityFamily;
            skillLocator.utility = utilitySkill;

            GenericSkill specialSkill = railgunner.AddComponent<GenericSkill>();
            specialSkill.skillName = "RailgunnerSpecial";
            specialSkill._skillFamily = specialFamily;
            skillLocator.special = specialSkill;
        }
    }
}