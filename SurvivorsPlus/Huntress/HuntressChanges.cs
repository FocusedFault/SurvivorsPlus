using RoR2;
using RoR2.Skills;
using RoR2.Projectile;
using EntityStates.Huntress;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SurvivorsPlus.Huntress
{
    public class HuntressChanges
    {

        GameObject arrowRainProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressArrowRain.prefab").WaitForCompletion();
        GameObject huntress = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressBody.prefab").WaitForCompletion();

        public HuntressChanges()
        {
            arrowRainProjectile.GetComponent<ProjectileDotZone>().overlapProcCoefficient = 0.6f;
            arrowRainProjectile.GetComponent<ProjectileDamage>().damageType = DamageType.Generic | DamageType.Shock5s;

            SkillLocator skillLocator = huntress.GetComponent<SkillLocator>();

            skillLocator.utility.skillFamily.variants[0].skillDef.baseRechargeInterval = 6f;
            SkillDef phaseBlink = skillLocator.utility.skillFamily.variants[1].skillDef;
            phaseBlink.baseMaxStock = 1;
            phaseBlink.baseRechargeInterval = 3f;
            phaseBlink.skillDescriptionToken = "<style=cIsUtility>Agile</style>. <style=cIsUtility>Disappear</style> and <style=cIsUtility>teleport</style> a short distance.";
            SkillDef arrowRain = skillLocator.special.skillFamily.variants[0].skillDef;
            arrowRain.skillNameToken = "Electric Volley";
            arrowRain.skillDescriptionToken = "<style=cIsUtility>Teleport</style> into the sky. Target an area to rain arrows, <style=cIsUtility>shocking</style> enemies for <style=cIsDamage>350% damage per second</style>.";


            On.EntityStates.Huntress.ArrowRain.OnEnter += ChangeDamageCoeff;
            On.EntityStates.Huntress.ArrowRain.UpdateAreaIndicator += ChangeAimLayer;
            On.EntityStates.Huntress.HuntressWeapon.FireFlurrySeekingArrow.OnEnter += IncreaseProcCoeff;
        }

        private void ChangeDamageCoeff(On.EntityStates.Huntress.ArrowRain.orig_OnEnter orig, EntityStates.Huntress.ArrowRain self)
        {
            ArrowRain.damageCoefficient = 3.5f;
            orig(self);
        }

        private void ChangeAimLayer(On.EntityStates.Huntress.ArrowRain.orig_UpdateAreaIndicator orig, EntityStates.Huntress.ArrowRain self)
        {
            if (!(bool)self.areaIndicatorInstance)
                return;
            float maxDistance = 1000f;
            RaycastHit hitInfo;
            if (!Physics.Raycast(self.GetAimRay(), out hitInfo, maxDistance, (int)LayerIndex.CommonMasks.bullet))
                return;
            self.areaIndicatorInstance.transform.position = hitInfo.point;
            self.areaIndicatorInstance.transform.up = hitInfo.normal;
        }

        private void IncreaseProcCoeff(On.EntityStates.Huntress.HuntressWeapon.FireFlurrySeekingArrow.orig_OnEnter orig, EntityStates.Huntress.HuntressWeapon.FireFlurrySeekingArrow self)
        {
            self.orbProcCoefficient = 0.8f;
            orig(self);
        }
    }
}