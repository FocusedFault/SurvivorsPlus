using R2API;
using RoR2;
using RoR2.EntityLogic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SurvivorsPlus.Mercenary
{
    public class MercenaryChanges
    {
        private GameObject merc = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercBody.prefab").WaitForCompletion();
        private GameObject evisOverlapProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/EvisOverlapProjectile.prefab").WaitForCompletion();
        public MercenaryChanges()
        {
            GameObject.Destroy(evisOverlapProjectile.GetComponent<DelayedEvent>());

            ContentAddition.AddEntityState<EvisDashNux>(out _);
            ContentAddition.AddEntityState<EvisNux>(out _);

            SurvivorsPlus.ChangeEntityStateValue("RoR2/Base/Merc/EntityStates.Merc.Assaulter2.asset", "damageCoefficient", "4");
            SurvivorsPlus.ChangeEntityStateValue("RoR2/Base/Merc/EntityStates.Merc.FocusedAssaultDash.asset", "delayedDamageCoefficient", "8");

            CharacterBody mercBody = merc.GetComponent<CharacterBody>();
            mercBody.baseRegen = 2.5f;
            mercBody.levelRegen = 0.5f;

            SkillLocator skillLocator2 = merc.GetComponent<SkillLocator>();
            skillLocator2.utility.skillFamily.variants[0].skillDef.skillDescriptionToken = "<style=cIsDamage>Stunning</style>. Dash forward, dealing <style=cIsDamage>400% damage</style>. If you hit an enemy, <style=cIsDamage>you can dash again</style>, up to <style=cIsDamage>3</style> total.";
            skillLocator2.utility.skillFamily.variants[1].skillDef.skillDescriptionToken = "<style=cIsDamage>Stunning</style>. Dash forward, dealing <style=cIsDamage>800% damage</style> and <style=cIsUtility>Exposing</style> enemies after<style=cIsUtility> 1 second</style>.";
            skillLocator2.special.skillFamily.variants[0].skillDef.skillDescriptionToken = "Target a nearby enemy, attacking them for <style=cIsDamage>150% damage</style> repeatedly. <style=cIsUtility>Exposing</style> on the final hit. <style=cIsUtility>You cannot be hit for the duration</style>.";
            skillLocator2.special.skillFamily.variants[0].skillDef.activationState = new EntityStates.SerializableEntityStateType(typeof(EvisDashNux));
        }
    }
}