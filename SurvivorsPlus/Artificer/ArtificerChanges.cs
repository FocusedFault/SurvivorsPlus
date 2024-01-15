using R2API;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using EntityStates;
using EntityStates.CaptainDefenseMatrixItem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.UI;
using UnityEngine.UI;
using RoR2.Skills;
using System.Linq;

namespace SurvivorsPlus.Artificer
{
    public class ArtificerChanges
    {
        // 
        public static GameObject uiOverlay = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorCorruptionUI.prefab").WaitForCompletion(), "MageAttunementUI");
        public static BuffDef ionBuff;
        public static BuffDef iceBuff;
        public static BuffDef fireBuff;

        public static GameObject fireBombChargeEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Mage/ChargeMageFireBomb.prefab").WaitForCompletion();
        public static GameObject fireBombProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Mage/MageFireBombProjectile.prefab").WaitForCompletion();
        public static GameObject ionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/OmniImpactVFXLightningMage.prefab").WaitForCompletion();
        public static GameObject iceEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteIce/AffixWhiteExplosion.prefab").WaitForCompletion();
        public static GameObject fireEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/OmniExplosionVFXCommandoGrenade.prefab").WaitForCompletion();

        public static SkillDef ionPrimarySkillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Mage/MageBodyFireLightningBolt.asset").WaitForCompletion();
        public static SkillDef ionSecondarySkillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Mage/MageBodyNovaBomb.asset").WaitForCompletion();
        public static SkillDef ionUtilitySkillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Mage/MageBodyFlyUp.asset").WaitForCompletion();

        public static SkillDef icePrimarySkillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Junk/Mage/MageBodyFireIceBolt.asset").WaitForCompletion();
        public static SkillDef iceSecondarySkillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Mage/MageBodyIceBomb.asset").WaitForCompletion();
        public static SkillDef iceUtilitySkillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Mage/MageBodyWall.asset").WaitForCompletion();

        public static SkillDef firePrimarySkillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Mage/MageBodyFireFirebolt.asset").WaitForCompletion();
        public static SkillDef fireSecondarySkillDef = ScriptableObject.CreateInstance<SkillDef>();
        public static SkillDef fireUtilitySkillDef = ScriptableObject.CreateInstance<SkillDef>();

        private SkillDef attunementSkill = ScriptableObject.CreateInstance<SkillDef>();
        private SkillDef multSwapSkill = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Toolbot/ToolbotBodySwap.asset").WaitForCompletion();
        private Sprite buffIcon = Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerAssets/texITWaveLunarIcon.png").WaitForCompletion();
        private Material ionMat = new Material(Addressables.LoadAssetAsync<Material>("RoR2/Base/Mage/matMageLightningCore.mat").WaitForCompletion());
        private Material iceMat = new Material(Addressables.LoadAssetAsync<Material>("RoR2/Base/Mage/matMageIceCore.mat").WaitForCompletion());
        private Material fireMat = new Material(Addressables.LoadAssetAsync<Material>("RoR2/Base/Mage/matMageFireCore.mat").WaitForCompletion());
        private GameObject arti = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageBody.prefab").WaitForCompletion();

        public ArtificerChanges()
        {
            string[] keywords = new string[5] { ionMat.shaderKeywords[0], ionMat.shaderKeywords[1], "FRESNEL", "USE_CLOUDS", "_EMISSION" };
            ionMat.shaderKeywords = keywords;
            iceMat.shaderKeywords = keywords;
            fireMat.shaderKeywords = keywords;

            arti.AddComponent<ArtificerController>();

            SkillLocator skillLocator = arti.GetComponent<SkillLocator>();

            foreach (GenericSkill gs in arti.GetComponents<GenericSkill>())
            {
                if (!gs.skillName.Contains("Passive"))
                    GameObject.Destroy(gs);
            }

            CreateBuffs();
            CreateSpecial();
            CreateFireSecondary();
            FamilyChanges();
            CreateFireSecondary();
            icePrimarySkillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Mage.Weapon.FireIceBolt));

            // create fire utility
            // edit ion surge to be more of a dash

            ContentAddition.AddBuffDef(ionBuff);
            ContentAddition.AddBuffDef(iceBuff);
            ContentAddition.AddBuffDef(fireBuff);
            ContentAddition.AddEntityState<Attunement>(out _);

            On.RoR2.CharacterModel.UpdateOverlays += AddOverlay;
            On.RoR2.CharacterMaster.OnBodyStart += AddIonBuff;
        }

        private void AddIonBuff(On.RoR2.CharacterMaster.orig_OnBodyStart orig, CharacterMaster self, CharacterBody body)
        {
            orig(self, body);
            if (body && body.name == "MageBody(Clone)")
                body.AddBuff(ionBuff);
        }

        private void CreateFireSecondary()
        {
            fireSecondarySkillDef.skillName = "Fire Bomb";
            (fireSecondarySkillDef as ScriptableObject).name = "Fire Bomb";
            fireSecondarySkillDef.skillNameToken = "Fire Bomb";
            fireSecondarySkillDef.skillDescriptionToken = "something";
            fireSecondarySkillDef.icon = ionSecondarySkillDef.icon;

            fireSecondarySkillDef.activationState = new SerializableEntityStateType(typeof(ChargeFireBomb));
            fireSecondarySkillDef.activationStateMachineName = "Weapon";
            fireSecondarySkillDef.interruptPriority = ionSecondarySkillDef.interruptPriority;

            fireSecondarySkillDef.baseMaxStock = 1;
            fireSecondarySkillDef.baseRechargeInterval = ionSecondarySkillDef.baseRechargeInterval;

            fireSecondarySkillDef.rechargeStock = 1;
            fireSecondarySkillDef.requiredStock = 1;
            fireSecondarySkillDef.stockToConsume = 1;

            fireSecondarySkillDef.dontAllowPastMaxStocks = true;
            fireSecondarySkillDef.beginSkillCooldownOnSkillEnd = ionSecondarySkillDef.beginSkillCooldownOnSkillEnd;
            fireSecondarySkillDef.canceledFromSprinting = ionSecondarySkillDef.canceledFromSprinting;
            fireSecondarySkillDef.forceSprintDuringState = false;
            fireSecondarySkillDef.fullRestockOnAssign = true;
            fireSecondarySkillDef.resetCooldownTimerOnUse = false;
            fireSecondarySkillDef.isCombatSkill = true;
            fireSecondarySkillDef.mustKeyPress = false;
            fireSecondarySkillDef.cancelSprintingOnActivation = ionSecondarySkillDef.cancelSprintingOnActivation;

            ContentAddition.AddSkillDef(fireSecondarySkillDef);
        }

        private void CreateSpecial()
        {
            attunementSkill.skillName = "Attunement";
            (attunementSkill as ScriptableObject).name = "Attunement";
            attunementSkill.skillNameToken = "Attunement";
            attunementSkill.skillDescriptionToken = "Use <style=cIsUtility>25% Mana</style> to change your element that creates a <style=cIsDamage>300% damage</style> blast around you.";
            attunementSkill.icon = multSwapSkill.icon;

            attunementSkill.activationState = new SerializableEntityStateType(typeof(Attunement));
            attunementSkill.activationStateMachineName = "Weapon";
            attunementSkill.interruptPriority = InterruptPriority.Death;

            attunementSkill.baseMaxStock = 1;
            attunementSkill.baseRechargeInterval = 0.5f;

            attunementSkill.rechargeStock = 1;
            attunementSkill.requiredStock = 1;
            attunementSkill.stockToConsume = 1;

            attunementSkill.dontAllowPastMaxStocks = true;
            attunementSkill.beginSkillCooldownOnSkillEnd = true;
            attunementSkill.canceledFromSprinting = false;
            attunementSkill.forceSprintDuringState = false;
            attunementSkill.fullRestockOnAssign = true;
            attunementSkill.resetCooldownTimerOnUse = false;
            attunementSkill.isCombatSkill = true;
            attunementSkill.mustKeyPress = false;
            attunementSkill.cancelSprintingOnActivation = false;

            ContentAddition.AddSkillDef(attunementSkill);
        }

        private void FamilyChanges()

        {
            SkillLocator skillLocator = arti.GetComponent<SkillLocator>();

            GenericSkill primarySkill = arti.AddComponent<GenericSkill>();
            primarySkill.skillName = "ArtiPrimary";
            SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            (newFamily as ScriptableObject).name = "ArtiPrimaryFamily";
            newFamily.variants = new SkillFamily.Variant[] { new SkillFamily.Variant() { skillDef = ionPrimarySkillDef } };
            primarySkill._skillFamily = newFamily;
            ContentAddition.AddSkillFamily(newFamily);
            skillLocator.primary = primarySkill;

            GenericSkill secondarySkill = arti.AddComponent<GenericSkill>();
            primarySkill.skillName = "ArtiSecondary";
            SkillFamily newFamily2 = ScriptableObject.CreateInstance<SkillFamily>();
            (newFamily2 as ScriptableObject).name = "ArtiSecondaryFamily";
            newFamily2.variants = new SkillFamily.Variant[] { new SkillFamily.Variant() { skillDef = ionSecondarySkillDef } };
            secondarySkill._skillFamily = newFamily2;
            ContentAddition.AddSkillFamily(newFamily2);
            skillLocator.secondary = secondarySkill;

            GenericSkill utilitySkill = arti.AddComponent<GenericSkill>();
            utilitySkill.skillName = "ArtiUtility";
            SkillFamily newFamily3 = ScriptableObject.CreateInstance<SkillFamily>();
            (newFamily3 as ScriptableObject).name = "ArtiUtilityFamily";
            newFamily3.variants = new SkillFamily.Variant[] { new SkillFamily.Variant() { skillDef = ionUtilitySkillDef } };
            utilitySkill._skillFamily = newFamily3;
            ContentAddition.AddSkillFamily(newFamily3);
            skillLocator.utility = utilitySkill;

            GenericSkill specialSkill = arti.AddComponent<GenericSkill>();
            specialSkill.skillName = "ArtiSpecial";
            SkillFamily newFamily4 = ScriptableObject.CreateInstance<SkillFamily>();
            (newFamily4 as ScriptableObject).name = "ArtiSpecialFamily";
            newFamily4.variants = new SkillFamily.Variant[] { new SkillFamily.Variant() { skillDef = attunementSkill } };
            specialSkill._skillFamily = newFamily4;
            ContentAddition.AddSkillFamily(newFamily4);
            skillLocator.special = specialSkill;
        }

        private void AddOverlay(On.RoR2.CharacterModel.orig_UpdateOverlays orig, CharacterModel self)
        {
            orig(self);
            if (self.body)
            {
                if (self.activeOverlayCount >= CharacterModel.maxOverlays) return;
                if (self.body.HasBuff(ionBuff))
                {
                    Material[] array = self.currentOverlays;
                    int num = self.activeOverlayCount;
                    self.activeOverlayCount = num + 1;
                    array[num] = ionMat;
                }
                if (self.body.HasBuff(iceBuff))
                {
                    Material[] array = self.currentOverlays;
                    int num = self.activeOverlayCount;
                    self.activeOverlayCount = num + 1;
                    array[num] = iceMat;
                }
                if (self.body.HasBuff(fireBuff))
                {
                    Material[] array = self.currentOverlays;
                    int num = self.activeOverlayCount;
                    self.activeOverlayCount = num + 1;
                    array[num] = fireMat;
                }
            }
        }

        private void CreateBuffs()
        {
            ionBuff = ScriptableObject.CreateInstance<BuffDef>();
            ionBuff.name = "bdIonMage";
            ionBuff.canStack = false;
            ionBuff.isCooldown = false;
            ionBuff.isDebuff = false;
            ionBuff.buffColor = new Color(0f, 0.5f, 1f);
            ionBuff.iconSprite = buffIcon;
            (ionBuff as UnityEngine.Object).name = ionBuff.name;

            iceBuff = ScriptableObject.CreateInstance<BuffDef>();
            iceBuff.name = "bdIceMage";
            iceBuff.canStack = false;
            iceBuff.isCooldown = false;
            iceBuff.isDebuff = false;
            iceBuff.buffColor = Color.cyan;
            iceBuff.iconSprite = buffIcon;
            (iceBuff as UnityEngine.Object).name = iceBuff.name;

            fireBuff = ScriptableObject.CreateInstance<BuffDef>();
            fireBuff.name = "bdFireMage";
            fireBuff.canStack = false;
            fireBuff.isCooldown = false;
            fireBuff.isDebuff = false;
            fireBuff.buffColor = new Color(1f, 0.25f, 0.25f);
            fireBuff.iconSprite = buffIcon;
            (fireBuff as UnityEngine.Object).name = fireBuff.name;
        }
    }
}