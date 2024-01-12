using RoR2;
using RoR2.Skills;
using UnityEngine;
using System;
using UnityEngine.AddressableAssets;

namespace SurvivorsPlus.Acrid
{
    public class AcridChanges
    {

        private SkillDef blight = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Croco/CrocoPassiveBlight.asset").WaitForCompletion();

        public AcridChanges()
        {
            blight.skillDescriptionToken = "Attacks that apply <style=cIsHealing>Poison</style> apply stacking <style=cIsDamage>Blight<style> instead, dealing <style=cIsDamage>30% damage per second</style> and <style=cIsUtility>stacks exponentially</style>.";
            On.RoR2.DotController.AddDot += ChangeBlight;
            On.RoR2.DotController.InitDotCatalog += ChangeBlightTicks;
        }

        private void ChangeBlightTicks(On.RoR2.DotController.orig_InitDotCatalog orig)
        {
            orig();
            DotController.dotDefs[5].damageCoefficient = 0.1f;
        }

        private void ChangeBlight(
            On.RoR2.DotController.orig_AddDot orig,
            DotController self,
            GameObject attackerObject,
            float duration,
            DotController.DotIndex dotIndex,
            float damageMultiplier,
            uint? maxStacksFromAttacker,
            float? totalDamage,
            DotController.DotIndex? preUpgradeDotIndex)
        {
            if (dotIndex == DotController.DotIndex.Blight)
            {
                duration = 10f;
                if (self.victimBody)
                {
                    int buffCount = self.victimBody.GetBuffCount(RoR2Content.Buffs.Blight);
                    float newDamage = Mathf.Pow(1.5f, buffCount);
                    damageMultiplier = newDamage;
                }
            }
            orig(self, attackerObject, duration, dotIndex, damageMultiplier, maxStacksFromAttacker, totalDamage, preUpgradeDotIndex);
        }
    }
}