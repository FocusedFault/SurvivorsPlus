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

            CharacterBody mercBody = merc.GetComponent<CharacterBody>();
            mercBody.baseRegen = 2.5f;
            mercBody.levelRegen = 0.5f;

            merc.GetComponent<SkillLocator>().special.skillFamily.variants[0].skillDef.skillDescriptionToken = "Target the nearest enemy, attacking them for <style=cIsDamage>330% damage</style> repeatedly. <style=cIsUtility>You cannot be hit for the duration</style>.";
        }
    }
}