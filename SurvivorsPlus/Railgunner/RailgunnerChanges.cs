using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.Projectile;

namespace SurvivorsPlus.Railgunner
{
    public class RailgunnerChanges
    {
        private GameObject railgunner = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerBody.prefab").WaitForCompletion();
        private GameObject polarMine = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerMineAltDetonated.prefab").WaitForCompletion();

        public RailgunnerChanges()
        {
            SurvivorsPlus.ChangeEntityStateValue("RoR2/DLC1/Railgunner/EntityStates.Railgunner.Weapon.FireSnipeHeavy.asset", "damageCoefficient", "8");
            SurvivorsPlus.ChangeEntityStateValue("RoR2/DLC1/Railgunner/EntityStates.Railgunner.Weapon.FireSnipeSuper.asset", "damageCoefficient", "26");
            SurvivorsPlus.ChangeEntityStateValue("RoR2/DLC1/Railgunner/EntityStates.Railgunner.Weapon.FireSnipeCryo.asset", "damageCoefficient", "12");

            polarMine.AddComponent<PolarSlow>();
            GameObject.Destroy(polarMine.GetComponent<SlowDownProjectiles>());

            SkillLocator skillLocator = railgunner.GetComponent<SkillLocator>();

            skillLocator.secondary.skillFamily.variants[0].skillDef.skillDescriptionToken = "Activate your <style=cIsUtility>long-range scope</style>, highlighting <style=cIsDamage>Weak Points</style> and transforming your weapon into a piercing <style=cIsDamage>800% damage</style> railgun.";
            skillLocator.special.skillFamily.variants[0].skillDef.skillDescriptionToken = "Fire a <style=cIsDamage>piercing</style> round for <style=cIsDamage>2600% damage</style> and <style=cIsDamage>150% Weak Point damage</style>. Afterwards, <style=cIsHealth>all your weapons are disabled</style> for <style=cIsHealth>5</style> seconds.";
            skillLocator.special.skillFamily.variants[1].skillDef.skillDescriptionToken = "<style=cIsUtility>Freezing</style>. Fire <style=cIsDamage>piercing</style> round for <style=cIsDamage>1200% damage</style>.";
            skillLocator.utility.skillFamily.variants[1].skillDef.skillDescriptionToken = "Throw out a device that <style=cIsUtility>slows down</style> all nearby enemies and <style=cIsUtility>reflects projectiles</style>.";
        }
    }
}