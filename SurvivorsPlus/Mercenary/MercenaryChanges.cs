using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SurvivorsPlus.Mercenary
{
    public class MercenaryChanges
    {
        private GameObject merc = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercBody.prefab").WaitForCompletion();

        public MercenaryChanges()
        {
            SurvivorsPlus.ChangeEntityStateValue("RoR2/Base/Merc/EntityStates.Merc.Evis.asset", "damageCoefficient", "3.3");
            SurvivorsPlus.ChangeEntityStateValue("RoR2/Base/Merc/EntityStates.Merc.Assaulter2.asset", "damageCoefficient", "4");
            SurvivorsPlus.ChangeEntityStateValue("RoR2/Base/Merc/EntityStates.Merc.FocusedAssaultDash.asset", "delayedDamageCoefficient", "8");

            CharacterBody mercBody = merc.GetComponent<CharacterBody>();
            mercBody.baseRegen = 2.5f;
            mercBody.levelRegen = 0.5f;

            SkillLocator skillLocator = merc.GetComponent<SkillLocator>();
            skillLocator.utility.skillFamily.variants[0].skillDef.skillDescriptionToken = "<style=cIsDamage>Stunning</style>. Dash forward, dealing <style=cIsDamage>400% damage</style>. If you hit an enemy, <style=cIsDamage>you can dash again</style>, up to <style=cIsDamage>3</style> total.";
            skillLocator.utility.skillFamily.variants[1].skillDef.skillDescriptionToken = "<style=cIsDamage>Stunning</style>. Dash forward, dealing <style=cIsDamage>800% damage</style> and <style=cIsUtility>Exposing</style> enemies after<style=cIsUtility> 1 second</style>.";
            skillLocator.special.skillFamily.variants[0].skillDef.skillDescriptionToken = "Target the nearest enemy, attacking them for <style=cIsDamage>330% damage</style> repeatedly. <style=cIsUtility>You cannot be hit for the duration</style>.";
        }
    }
}