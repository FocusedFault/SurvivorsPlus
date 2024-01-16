using RoR2;
using RoR2.Skills;
using EntityStates;
using EntityStates.EngiTurret.EngiTurretWeapon;
using EntityStates.Engi.Mine;
using EntityStates.Engi.EngiWeapon;
using UnityEngine;
using UnityEngine.AddressableAssets;
using R2API;

namespace SurvivorsPlus.Engineer
{
    public class EngineerChanges
    {
        public static GameObject hitEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/Hitspark1.prefab").WaitForCompletion();
        public static GameObject laser = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/LaserEngiTurret.prefab").WaitForCompletion();

        private GameObject bubbleShieldProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiBubbleShield.prefab").WaitForCompletion();
        private GameObject engi = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiBody.prefab").WaitForCompletion();
        private GameObject engiWalkerTurret = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiWalkerTurretBody.prefab").WaitForCompletion();

        public EngineerChanges()
        {
            ContentAddition.AddEntityState<BetterFireBeam>(out _);
            SurvivorsPlus.ChangeEntityStateValue("RoR2/Base/Engi/EntityStates.EngiTurret.EngiTurretWeapon.FireBeam.asset", "maxDistance", "50");
            SurvivorsPlus.ChangeEntityStateValue("RoR2/Base/Engi/EntityStates.Engi.EngiBubbleShield.Deployed.asset", "lifetime", "10.5");

            SkillLocator skillLocator = engi.GetComponent<SkillLocator>();

            SkillDef grenades = skillLocator.primary.skillFamily.variants[0].skillDef;
            grenades.skillDescriptionToken = "Fire <style=cIsDamage>3</style> grenades that deal <style=cIsDamage>100% damage</style> each.";
            grenades.activationState = new SerializableEntityStateType(typeof(FireGrenades));

            SkillDef mine1 = skillLocator.secondary.skillFamily.variants[0].skillDef;
            mine1.skillDescriptionToken = "Place a large blast radius mine that deals <style=cIsDamage>deal 300% damage</style> each. Can place up to 4.";
            mine1.baseRechargeInterval = 5f;

            engiWalkerTurret.AddComponent<TurretBeamRemover>();
            engiWalkerTurret.GetComponent<SkillLocator>().primary.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(BetterFireBeam));

            bubbleShieldProjectile.GetComponent<BeginRapidlyActivatingAndDeactivating>().delayBeforeBeginningBlinking = 10f;
            foreach (Transform child in bubbleShieldProjectile.transform.GetChild(0))
            {
                if (child.name != "ActiveVisual")
                    GameObject.Destroy(child.gameObject);
                else
                    child.gameObject.AddComponent<MeshCollider>();
            }
            On.EntityStates.Engi.Mine.BaseMineArmingState.OnEnter += BaseMineArmingState_OnEnter;
            On.EntityStates.Engi.EngiWeapon.FireGrenades.OnEnter += FireGrenades_OnEnter;
        }

        private void BaseMineArmingState_OnEnter(On.EntityStates.Engi.Mine.BaseMineArmingState.orig_OnEnter orig, BaseMineArmingState self)
        {
            orig(self);
            if (self is MineArmingFull)
                self.damageScale = 1f;
            if (!(self is MineArmingWeak))
                return;
            self.outer.SetState(new MineArmingFull());
        }

        private void FireGrenades_OnEnter(On.EntityStates.Engi.EngiWeapon.FireGrenades.orig_OnEnter orig, FireGrenades self)
        {
            self.grenadeCountMax = 3;
            orig(self);
        }
    }
}