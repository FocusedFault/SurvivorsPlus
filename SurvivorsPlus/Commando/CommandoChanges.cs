using RoR2;
using RoR2.Skills;
using RoR2.Projectile;
using EntityStates.Commando.CommandoWeapon;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SurvivorsPlus.Commando
{
    public class CommandoChanges
    {
        private GameObject commando = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoBody.prefab").WaitForCompletion();
        private GameObject stickyGrenade = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Commando/CommandoStickyGrenadeProjectile.prefab").WaitForCompletion();
        private GameObject grenadeVFX = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/OmniExplosionVFXCommandoGrenade.prefab").WaitForCompletion();

        public CommandoChanges()
        {

            ProjectileImpactExplosion explosion = stickyGrenade.GetComponent<ProjectileImpactExplosion>();
            explosion.lifetimeAfterImpact = 1f;
            explosion.falloffModel = BlastAttack.FalloffModel.None;

            SkillLocator skillLocator = commando.GetComponent<SkillLocator>();

            skillLocator.secondary.skillFamily.variants[1].skillDef.skillDescriptionToken = "Fire two close-range blasts that deal <style=cIsDamage>12x200% damage</style> total.";

            SkillDef phaseRound = skillLocator.secondary.skillFamily.variants[0].skillDef;
            phaseRound.skillDescriptionToken = "Fire a <style=cIsUtility>piercing</style> bullet for <style=cIsDamage>400% damage</style>. Deals <style=cIsDamage>40%</style> more damage every time it passes through an enemy.";

            SkillDef roll = skillLocator.utility.skillFamily.variants[0].skillDef;
            roll.cancelSprintingOnActivation = false;
            roll.skillDescriptionToken = "<style=cIsUtility>Roll</style> forward a short distance. You are <style=cIsDamage>invincible</style> during the roll.";

            SkillDef grenade = skillLocator.special.skillFamily.variants[1].skillDef;
            grenade.skillNameToken = "Sticky Grenade";
            grenade.skillDescriptionToken = "Throw a grenade that <style=cIsUtility>sticks to enemies</style> and explodes for <style=cIsDamage>600% damage</style>. Can hold up to 2.";
            grenade.activationState = new EntityStates.SerializableEntityStateType(typeof(ThrowStickyGrenade));
            stickyGrenade.GetComponent<ProjectileImpactExplosion>().impactEffect = grenadeVFX;

            SkillDef suppressiveFire = skillLocator.special.skillFamily.variants[0].skillDef;
            suppressiveFire.skillDescriptionToken = "Fire a barrage of high impact rounds that deal <style=cIsDamage>200% damage</style> with <style=cIsUtility>double the proc coefficient</style>.";
            suppressiveFire.activationState = new EntityStates.SerializableEntityStateType(typeof(BetterSuppressiveFire));
            suppressiveFire.baseRechargeInterval = 6f;

            On.EntityStates.GenericProjectileBaseState.OnEnter += AlterFMJ;
            On.EntityStates.Commando.DodgeState.OnEnter += AddInvincibility;
            On.EntityStates.Commando.DodgeState.OnExit += RemoveInvincibility;
            On.EntityStates.GenericProjectileBaseState.OnEnter += AlterSticky;
        }

        private void AlterFMJ(On.EntityStates.GenericProjectileBaseState.orig_OnEnter orig, EntityStates.GenericProjectileBaseState self)
        {
            if (self is FireFMJ)
                self.damageCoefficient = 4f;
            orig(self);
        }

        private void AlterSticky(On.EntityStates.GenericProjectileBaseState.orig_OnEnter orig, EntityStates.GenericProjectileBaseState self)
        {
            if (self is ThrowStickyGrenade)
                self.damageCoefficient = 6f;
            orig(self);
        }

        private void AddInvincibility(On.EntityStates.Commando.DodgeState.orig_OnEnter orig, EntityStates.Commando.DodgeState self)
        {
            self.initialSpeedCoefficient = 4f;
            self.finalSpeedCoefficient = 2f;
            self.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            orig(self);
        }

        private void RemoveInvincibility(On.EntityStates.Commando.DodgeState.orig_OnExit orig, EntityStates.Commando.DodgeState self)
        {
            orig(self);
            self.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
        }
    }
}