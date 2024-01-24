using R2API;
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
        public static GameObject newTracer = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Commando/TracerBarrage.prefab").WaitForCompletion();
        public static GameObject newHitEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/HitsparkCommandoShotgun.prefab").WaitForCompletion();
        public static GameObject FMJMuzzleEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/MuzzleflashFMJ.prefab").WaitForCompletion();
        public static GameObject FMJTracer = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/TracerRailgunSuper.prefab").WaitForCompletion(), "NuxFMJTracer", false);
        public static GameObject FMJHitEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/OmniExplosionVFXFMJ.prefab").WaitForCompletion();
        private Material ringMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/Commando/matCommandoFMJRing.mat").WaitForCompletion();
        private Material lineMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/Commando/matCommandoShotgunTracerCore.mat").WaitForCompletion();
        private Material glowMat = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Railgunner/matRailgunSoftGlow.mat").WaitForCompletion();
        private Material headMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/Commando/matCommandoFmjSweetSpotBurst.mat").WaitForCompletion();

        public CommandoChanges()
        {
            FMJTracer.transform.GetChild(4).GetChild(2).GetChild(0).gameObject.SetActive(false);
            FMJTracer.transform.GetChild(4).GetChild(3).GetChild(0).gameObject.SetActive(false);
            FMJTracer.transform.GetChild(4).GetChild(2).GetComponent<ParticleSystemRenderer>().sharedMaterial = ringMat;
            FMJTracer.transform.GetChild(4).GetChild(1).GetChild(0).GetComponent<LineRenderer>().sharedMaterial = lineMat;
            FMJTracer.transform.GetChild(4).GetChild(1).GetChild(2).GetComponent<LineRenderer>().sharedMaterial = glowMat;
            FMJTracer.transform.GetChild(2).GetChild(0).GetComponent<ParticleSystemRenderer>().sharedMaterial = headMat;
            FMJTracer.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            ContentAddition.AddEffect(FMJTracer);

            ProjectileImpactExplosion explosion = stickyGrenade.GetComponent<ProjectileImpactExplosion>();
            explosion.lifetimeAfterImpact = 1f;
            explosion.falloffModel = BlastAttack.FalloffModel.None;

            SkillLocator skillLocator = commando.GetComponent<SkillLocator>();

            skillLocator.primary.skillFamily.variants[0].skillDef.activationState = new EntityStates.SerializableEntityStateType(typeof(BetterFirePistols));

            skillLocator.secondary.skillFamily.variants[1].skillDef.skillDescriptionToken = "Fire two close-range blasts that deal <style=cIsDamage>12x200% damage</style> total.";

            SkillDef phaseRound = skillLocator.secondary.skillFamily.variants[0].skillDef;
            phaseRound.activationState = new EntityStates.SerializableEntityStateType(typeof(BetterPhaseRound));
            phaseRound.skillNameToken = "Full Metal Jacket";
            phaseRound.skillDescriptionToken = "Fire a <style=cIsUtility>piercing</style> bullet for <style=cIsDamage>400% damage</style>. Deals <style=cIsDamage>40%</style> more damage every time it passes through an enemy.";

            SkillDef roll = skillLocator.utility.skillFamily.variants[0].skillDef;
            roll.cancelSprintingOnActivation = false;
            roll.skillDescriptionToken = "<style=cIsUtility>Roll</style> forward a short distance. You are <style=cIsDamage>invincible</style> during the roll.";

            SkillDef grenade = skillLocator.special.skillFamily.variants[1].skillDef;
            grenade.skillNameToken = "Sticky Grenade";
            grenade.activationState = new EntityStates.SerializableEntityStateType(typeof(ThrowStickyGrenade));
            stickyGrenade.GetComponent<ProjectileImpactExplosion>().impactEffect = grenadeVFX;

            newTracer.GetComponent<LineRenderer>().startWidth = 0.3f;
            newTracer.GetComponent<LineRenderer>().endWidth = 0.3f;

            SkillDef suppressiveFire = skillLocator.special.skillFamily.variants[0].skillDef;
            suppressiveFire.skillNameToken = "Vortex Rounds";
            suppressiveFire.skillDescriptionToken = "Fire a barrage of high impact rounds that deal <style=cIsDamage>300% damage</style> with <style=cIsUtility>double the proc coefficient</style>.";
            suppressiveFire.activationState = new EntityStates.SerializableEntityStateType(typeof(BetterSuppressiveFire));
            suppressiveFire.baseRechargeInterval = 6f;

            On.EntityStates.Commando.DodgeState.OnEnter += AddInvincibility;
            On.EntityStates.Commando.DodgeState.OnExit += RemoveInvincibility;
            On.EntityStates.Commando.CommandoWeapon.FireShotgunBlast.OnEnter += AlterShotgun;
            On.EntityStates.GenericBulletBaseState.OnEnter += WhyDoIHaveToDoThis;
        }

        private void AlterShotgun(On.EntityStates.Commando.CommandoWeapon.FireShotgunBlast.orig_OnEnter orig, FireShotgunBlast self)
        {
            self.bulletCount = 6;
            orig(self);
        }

        private void WhyDoIHaveToDoThis(On.EntityStates.GenericBulletBaseState.orig_OnEnter orig, EntityStates.GenericBulletBaseState self)
        {
            if (self is BetterPhaseRound)
            {
                self.maxDistance = 1000f;
                self.tracerEffectPrefab = FMJTracer;
                self.hitEffectPrefab = FMJHitEffect;
                self.muzzleFlashPrefab = FMJMuzzleEffect;
                self.damageCoefficient = 4f;
                self.bulletRadius = 0.5f;
                self.bulletCount = 1;
                self.muzzleName = "MuzzleCenter";
            }
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