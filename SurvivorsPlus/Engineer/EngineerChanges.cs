using RoR2;
using RoR2.Skills;
using EntityStates;
using EntityStates.EngiTurret.EngiTurretWeapon;
using EntityStates.Engi.Mine;
using EntityStates.Engi.EngiWeapon;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SurvivorsPlus.Engineer
{
    public class EngineerChanges
    {
        private GameObject bubbleShieldProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiBubbleShield.prefab").WaitForCompletion();
        private GameObject engi = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiBody.prefab").WaitForCompletion();

        public EngineerChanges()
        {
            SurvivorsPlus.ChangeEntityStateValue("RoR2/Base/Engi/EntityStates.EngiTurret.EngiTurretWeapon.FireBeam.asset", "maxDistance", "50");
            SurvivorsPlus.ChangeEntityStateValue("RoR2/Base/Engi/EntityStates.Engi.EngiBubbleShield.Deployed.asset", "lifetime", "10");

            SkillLocator skillLocator = engi.GetComponent<SkillLocator>();

            SkillDef grenades = skillLocator.primary.skillFamily.variants[0].skillDef;
            grenades.skillDescriptionToken = "Fire <style=cIsDamage>3</style> grenades that deal <style=cIsDamage>100% damage</style> each.";
            grenades.activationState = new SerializableEntityStateType(typeof(FireGrenades));

            SkillDef mine1 = skillLocator.secondary.skillFamily.variants[0].skillDef;
            mine1.skillDescriptionToken = "Place a large blast radius mine that deals <style=cIsDamage>deal 300% damage</style> each. Can place up to 4.";

            bubbleShieldProjectile.GetComponent<BeginRapidlyActivatingAndDeactivating>().delayBeforeBeginningBlinking = 9.5f;
            foreach (Transform child in bubbleShieldProjectile.transform.GetChild(0))
            {
                if (child.name != "ActiveVisual")
                    GameObject.Destroy(child.gameObject);
                else
                    child.gameObject.AddComponent<MeshCollider>();
            }
            On.EntityStates.Engi.Mine.BaseMineArmingState.OnEnter += BaseMineArmingState_OnEnter;
            On.EntityStates.EngiTurret.EngiTurretWeapon.FireBeam.GetBeamEndPoint += FireBeam_GetBeamEndPoint;
            On.EntityStates.Engi.EngiWeapon.FireGrenades.OnEnter += FireGrenades_OnEnter;
        }

        private Vector3 FireBeam_GetBeamEndPoint(On.EntityStates.EngiTurret.EngiTurretWeapon.FireBeam.orig_GetBeamEndPoint orig, FireBeam self)
        {
            Vector3 point = self.laserRay.GetPoint(self.maxDistance);
            GameObject gameObject = self.gameObject;
            Ray laserRay = self.laserRay;
            RaycastHit raycastHit;
            double maxDistance = (double)self.maxDistance;
            if (Util.CharacterRaycast(gameObject, laserRay, out raycastHit, (float)maxDistance, LayerIndex.CommonMasks.bullet, QueryTriggerInteraction.UseGlobal))
                point = raycastHit.point;
            return point;
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