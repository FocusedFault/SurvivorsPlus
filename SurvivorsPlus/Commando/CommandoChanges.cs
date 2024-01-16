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
        private GameObject barrageTracer = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/TracerCommandoBoost.prefab").WaitForCompletion();
        private GameObject stickyGrenade = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Commando/CommandoStickyGrenadeProjectile.prefab").WaitForCompletion();
        private GameObject grenadeVFX = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/OmniExplosionVFXCommandoGrenade.prefab").WaitForCompletion();

        public CommandoChanges()
        {
            stickyGrenade.GetComponent<ProjectileImpactExplosion>().lifetimeAfterImpact = 1f;

            SkillLocator skillLocator = commando.GetComponent<SkillLocator>();

            SkillDef roll = skillLocator.utility.skillFamily.variants[0].skillDef;
            roll.cancelSprintingOnActivation = false;
            roll.skillDescriptionToken = "<style=cIsUtility>Roll</style> forward a short distance. You are <style=cIsDamage>invincible</style> during the roll.";

            SkillDef grenade = skillLocator.special.skillFamily.variants[1].skillDef;
            grenade.skillNameToken = "Sticky Grenade";
            grenade.activationState = new EntityStates.SerializableEntityStateType(typeof(ThrowStickyGrenade));
            stickyGrenade.GetComponent<ProjectileImpactExplosion>().impactEffect = grenadeVFX;

            barrageTracer.GetComponent<LineRenderer>().startColor = Color.red;
            barrageTracer.GetComponent<LineRenderer>().endColor = Color.red;

            SkillDef suppressiveFire = skillLocator.special.skillFamily.variants[0].skillDef;
            suppressiveFire.skillNameToken = "Vortex Rounds";
            suppressiveFire.skillDescriptionToken = "Fire a barrage of high impact rounds that deal <style=cIsDamage>200% damage</style> each with a proc coefficient of <style=cIsUtility>1.5</style>.";
            suppressiveFire.activationState = new EntityStates.SerializableEntityStateType(typeof(BetterSuppressiveFire));
            suppressiveFire.baseRechargeInterval = 6f;

            On.EntityStates.Commando.DodgeState.OnEnter += AddInvincibility;
            On.EntityStates.Commando.DodgeState.OnExit += RemoveInvincibility;
        }

        private void AddInvincibility(On.EntityStates.Commando.DodgeState.orig_OnEnter orig, EntityStates.Commando.DodgeState self)
        {
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