using R2API;
using RoR2;
using RoR2.Projectile;
using EntityStates;
using EntityStates.Mage;
using EntityStates.Mage.Weapon;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.Skills;

namespace SurvivorsPlus.Artificer
{
    public class ArtificerChanges
    {
        // 
        public static GameObject uiOverlay = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorCorruptionUI.prefab").WaitForCompletion(), "MageAttunementUI", false);
        public static BuffDef ionBuff;
        public static BuffDef iceBuff;
        public static BuffDef fireBuff;

        public static GameObject flamethrowerEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageFlamethrowerEffect.prefab").WaitForCompletion();

        public static GameObject fireBombChargeEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Mage/ChargeMageFireBomb.prefab").WaitForCompletion();
        public static GameObject fireBombExplosionEffect = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BFG/BeamSphereExplosion.prefab").WaitForCompletion(), "FireBombExplosion", false);
        public static GameObject fireBombGhost = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Mage/ChargeMageFireBomb.prefab").WaitForCompletion(), "FireBombCore", false);
        public static GameObject fireBombGhost2 = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/PaladinRocketGhost.prefab").WaitForCompletion(), "FireBombGhost2", false);
        public static GameObject iceBoltGhost = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Mage/MageIceBoltGhost.prefab").WaitForCompletion();
        public static GameObject iceBolt = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Mage/MageIceBolt.prefab").WaitForCompletion();
        public static GameObject iceBoltExplosion = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MuzzleflashMageIceLarge.prefab").WaitForCompletion();
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
        public static SkillDef fireUtilitySkillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Mage/MageBodyFlamethrower.asset").WaitForCompletion();

        private SkillDef attunementSkill = ScriptableObject.CreateInstance<SkillDef>();
        private SkillDef multSwapSkill = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Toolbot/ToolbotBodySwap.asset").WaitForCompletion();
        private Sprite buffIcon = Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/GameModes/InfiniteTowerRun/InfiniteTowerAssets/texITWaveLunarIcon.png").WaitForCompletion();
        private Material ionMat = new Material(Addressables.LoadAssetAsync<Material>("RoR2/Base/Mage/matMageLightningCore.mat").WaitForCompletion());
        private Material iceMat = new Material(Addressables.LoadAssetAsync<Material>("RoR2/Base/Mage/matMageIceCore.mat").WaitForCompletion());
        private Material fireMat = new Material(Addressables.LoadAssetAsync<Material>("RoR2/Base/Mage/matMageFireCore.mat").WaitForCompletion());

        private Material fireBombMat = new Material(Addressables.LoadAssetAsync<Material>("RoR2/Base/Mage/matMageFirebolt.mat").WaitForCompletion());
        private Material fireBombMat2 = new Material(Addressables.LoadAssetAsync<Material>("RoR2/Base/Mage/matMageMatrixFire.mat").WaitForCompletion());
        private Material fireBombMat3 = new Material(Addressables.LoadAssetAsync<Material>("RoR2/Base/Mage/matMageMatrixTriFire.mat").WaitForCompletion());
        private Material iceBoltMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/Mage/matMageMatrixTriIce.mat").WaitForCompletion();
        private Material iceBoltTrailMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/Mage/matMageMatrixDirectionalIce.mat").WaitForCompletion();

        private GameObject arti = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageBody.prefab").WaitForCompletion();

        public ArtificerChanges()
        {
            ContentAddition.AddEntityState<ChargeFireBomb>(out _);
            ContentAddition.AddEntityState<ThrowFireBomb>(out _);

            // GameObject.Destroy();
            fireBombExplosionEffect.GetComponent<ShakeEmitter>().enabled = false;
            fireBombExplosionEffect.transform.GetChild(0).GetChild(0).GetComponent<ParticleSystemRenderer>().sharedMaterial = fireBombMat;
            fireBombExplosionEffect.transform.GetChild(0).GetChild(2).GetComponent<ParticleSystemRenderer>().sharedMaterial = fireBombMat2;
            fireBombExplosionEffect.transform.GetChild(0).GetChild(4).GetComponent<Light>().color = new Color(1f, 0.4924f, 0.3309f, 1f);
            fireBombExplosionEffect.transform.GetChild(0).GetChild(5).GetComponent<ParticleSystemRenderer>().sharedMaterial = fireBombMat3;

            ContentAddition.AddEffect(fireBombExplosionEffect);

            GameObject.Destroy(fireBombGhost.GetComponent<ObjectScaleCurve>());
            fireBombGhost.transform.localScale = new Vector3(2f, 2f, 2f);
            fireBombGhost2.transform.localScale = new Vector3(2f, 2f, 2f);
            fireBombGhost.transform.parent = fireBombGhost2.transform;
            fireBombGhost.transform.localPosition = new Vector3(0f, 0f, 0.5f);

            fireBombProjectile.GetComponent<ProjectileController>().ghostPrefab = fireBombGhost2;
            fireBombProjectile.GetComponent<ProjectileImpactExplosion>().impactEffect = fireBombExplosionEffect;

            fireUtilitySkillDef.baseRechargeInterval = 16f;

            iceBoltGhost.transform.GetChild(4).GetChild(0).GetComponent<TrailRenderer>().sharedMaterial = iceBoltTrailMat;
            iceBoltGhost.transform.GetChild(5).GetComponent<MeshRenderer>().sharedMaterial = iceBoltMat;
            iceBolt.GetComponent<ProjectileImpactExplosion>().impactEffect = iceBoltExplosion;
            icePrimarySkillDef.skillNameToken = "Ice Bolt";
            icePrimarySkillDef.skillDescriptionToken = "<style=cIsUtility>Freezing</style>. Fire a slow, short-range bolt for <style=cIsDamage>300% damage</style>. Hold up to 4.";
            iceSecondarySkillDef.baseRechargeInterval = 8f;
            iceUtilitySkillDef.baseRechargeInterval = 16f;

            ionSecondarySkillDef.baseRechargeInterval = 8f;
            ionUtilitySkillDef.baseRechargeInterval = 16f;


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
            On.EntityStates.Mage.FlyUpState.OnEnter += TweakIonSurge;
            On.EntityStates.Mage.Weapon.Flamethrower.OnEnter += TweakFlamethrower;
            On.EntityStates.Mage.Weapon.FireFireBolt.OnEnter += FixIceBolt;
            On.EntityStates.Mage.Weapon.BaseChargeBombState.OnEnter += CheckValues1;
            On.EntityStates.Mage.Weapon.BaseThrowBombState.OnEnter += CheckValues2;
        }

        private void TweakIonSurge(On.EntityStates.Mage.FlyUpState.orig_OnEnter orig, FlyUpState self)
        {
            orig(self);
            AnimationCurve curve = AnimationCurve.Linear(0f, 5f, 1f, 0f);
            curve.AddKey(5f, 0.1f);
            curve.AddKey(5f, 0.3125126f);
            curve.AddKey(0f, 1f);
            FlyUpState.speedCoefficientCurve = curve;
            //  self.flyVector = self.inputBank.aimDirection;
        }

        private void TweakFlamethrower(On.EntityStates.Mage.Weapon.Flamethrower.orig_OnEnter orig, Flamethrower self)
        {
            Flamethrower.ignitePercentChance = 100f;
            self.maxDistance = 32f;
            Flamethrower.tickFrequency = 5f;
            orig(self);
        }

        private void FixIceBolt(On.EntityStates.Mage.Weapon.FireFireBolt.orig_OnEnter orig, FireFireBolt self)
        {
            if (self is FireIceBolt)
                self.baseDuration = 0.25f;
            orig(self);
        }

        private void CheckValues1(On.EntityStates.Mage.Weapon.BaseChargeBombState.orig_OnEnter orig, BaseChargeBombState self)
        {
            if (self is ChargeFireBomb)
            {
                self.chargeEffectPrefab = fireBombChargeEffect;
                //  Flamethrower.startAttackSoundString
                self.chargeSoundString = Flamethrower.endAttackSoundString;
                self.baseDuration = 1.5f;
                self.minBloomRadius = 0.1f;
                self.maxBloomRadius = 0.5f;
                self.crosshairOverridePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageCrosshair.prefab").WaitForCompletion();
                self.minChargeDuration = 0.5f;
            }
            orig(self);
        }

        private void CheckValues2(On.EntityStates.Mage.Weapon.BaseThrowBombState.orig_OnEnter orig, BaseThrowBombState self)
        {
            if (self is ThrowFireBomb)
            {
                self.projectilePrefab = fireBombProjectile;
                self.muzzleflashEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MuzzleflashMageFire.prefab").WaitForCompletion();
                self.baseDuration = 0.4f;
                self.minDamageCoefficient = 4;
                self.maxDamageCoefficient = 16;
                self.force = 1500f;
                self.selfForce = 500;
            }
            orig(self);
        }

        private void AddIonBuff(On.RoR2.CharacterMaster.orig_OnBodyStart orig, CharacterMaster self, CharacterBody body)
        {
            orig(self, body);
            if (body && body.name == "MageBody(Clone)")
            {
                body.AddBuff(ionBuff);
                body.skillLocator.primary.SetSkillOverride((object)this, ArtificerChanges.ionPrimarySkillDef, GenericSkill.SkillOverridePriority.Default);
                body.skillLocator.secondary.SetSkillOverride((object)this, ArtificerChanges.ionSecondarySkillDef, GenericSkill.SkillOverridePriority.Default);
                body.skillLocator.utility.SetSkillOverride((object)this, ArtificerChanges.ionUtilitySkillDef, GenericSkill.SkillOverridePriority.Default);
            }
        }

        private void CreateFireSecondary()
        {
            fireSecondarySkillDef.skillName = "FireBomb";
            (fireSecondarySkillDef as ScriptableObject).name = "FireBomb";
            fireSecondarySkillDef.skillNameToken = "Flame Bomb";
            fireSecondarySkillDef.skillDescriptionToken = "<style=cIsDamage>Ignite</style>. Charge up an <style=cIsDamage>exploding</style> fire-bomb that deals <style=cIsDamage>400%-1600%</style> damage.";
            fireSecondarySkillDef.icon = ionSecondarySkillDef.icon;

            fireSecondarySkillDef.activationState = new SerializableEntityStateType(typeof(ChargeFireBomb));
            fireSecondarySkillDef.activationStateMachineName = "Weapon";
            fireSecondarySkillDef.interruptPriority = ionSecondarySkillDef.interruptPriority;

            fireSecondarySkillDef.baseMaxStock = 1;
            fireSecondarySkillDef.baseRechargeInterval = 8f;

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

            attunementSkill.baseMaxStock = 3;
            attunementSkill.baseRechargeInterval = 30f;

            attunementSkill.rechargeStock = 1;
            attunementSkill.requiredStock = 1;
            attunementSkill.stockToConsume = 1;

            attunementSkill.dontAllowPastMaxStocks = true;
            attunementSkill.beginSkillCooldownOnSkillEnd = true;
            attunementSkill.canceledFromSprinting = false;
            attunementSkill.forceSprintDuringState = false;
            attunementSkill.fullRestockOnAssign = false;
            attunementSkill.resetCooldownTimerOnUse = false;
            attunementSkill.isCombatSkill = true;
            attunementSkill.mustKeyPress = true;
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
            newFamily.variants = new SkillFamily.Variant[] { new SkillFamily.Variant() { skillDef = ionPrimarySkillDef }, new SkillFamily.Variant() { skillDef = firePrimarySkillDef }, new SkillFamily.Variant() { skillDef = icePrimarySkillDef } };
            primarySkill._skillFamily = newFamily;
            ContentAddition.AddSkillFamily(newFamily);
            skillLocator.primary = primarySkill;

            GenericSkill secondarySkill = arti.AddComponent<GenericSkill>();
            primarySkill.skillName = "ArtiSecondary";
            SkillFamily newFamily2 = ScriptableObject.CreateInstance<SkillFamily>();
            (newFamily2 as ScriptableObject).name = "ArtiSecondaryFamily";
            newFamily2.variants = new SkillFamily.Variant[] { new SkillFamily.Variant() { skillDef = ionSecondarySkillDef }, new SkillFamily.Variant() { skillDef = fireSecondarySkillDef }, new SkillFamily.Variant() { skillDef = iceSecondarySkillDef } };
            primarySkill._skillFamily = newFamily;
            secondarySkill._skillFamily = newFamily2;
            ContentAddition.AddSkillFamily(newFamily2);
            skillLocator.secondary = secondarySkill;

            GenericSkill utilitySkill = arti.AddComponent<GenericSkill>();
            utilitySkill.skillName = "ArtiUtility";
            SkillFamily newFamily3 = ScriptableObject.CreateInstance<SkillFamily>();
            (newFamily3 as ScriptableObject).name = "ArtiUtilityFamily";
            newFamily3.variants = new SkillFamily.Variant[] { new SkillFamily.Variant() { skillDef = ionUtilitySkillDef }, new SkillFamily.Variant() { skillDef = fireUtilitySkillDef }, new SkillFamily.Variant() { skillDef = iceUtilitySkillDef } };
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