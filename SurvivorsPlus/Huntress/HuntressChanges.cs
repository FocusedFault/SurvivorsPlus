using RoR2;
using RoR2.Skills;
using RoR2.Projectile;
using EntityStates.Huntress;
using EntityStates.Huntress.HuntressWeapon;
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
            SurvivorsPlus.ChangeEntityStateValue("RoR2/Base/Huntress/EntityStates.Huntress.HuntressWeapon.FireFlurrySeekingArrow.asset", "orbProcCoefficient", "0.8");
            SurvivorsPlus.ChangeEntityStateValue("RoR2/Base/Huntress/EntityStates.Huntress.HuntressWeapon.FireFlurrySeekingArrow.asset", "baseDuration", "1");
            SurvivorsPlus.ChangeEntityStateValue("RoR2/Base/Huntress/EntityStates.Huntress.ArrowRain.asset", "damageCoefficient", "4.5");

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
            arrowRain.skillDescriptionToken = "<style=cIsUtility>Teleport</style> into the sky. Target an area to rain arrows, <style=cIsUtility>shocking</style> enemies for <style=cIsDamage>450% damage per second</style>.";

            On.EntityStates.Huntress.ArrowRain.UpdateAreaIndicator += ChangeAimLayer;
        }

        private void ChangeAimLayer(On.EntityStates.Huntress.ArrowRain.orig_UpdateAreaIndicator orig, ArrowRain self)
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

    }
}