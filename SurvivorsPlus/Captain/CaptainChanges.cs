using R2API;
using RoR2;
using RoR2.Skills;
using RoR2.Orbs;
using RoR2.Projectile;
using EntityStates;
using EntityStates.CaptainDefenseMatrixItem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace SurvivorsPlus.Captain
{
    public class CaptainChanges
    {
        private GameObject captain = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/CaptainBody.prefab").WaitForCompletion();

        public CaptainChanges()
        {
            /*
            foreach (GenericSkill gs in captain.GetComponents<GenericSkill>())
            {
                // Debug.LogWarning(gs.skillName);
                if (gs.skillName.Contains("Utility") || gs.skillName.Contains("Special") || gs.skillName.Contains("SupplyDrop1") || gs.skillName.Contains("SupplyDrop2"))
                    GameObject.Destroy(gs);
            }
            */

            SkillLocator skillLocator = captain.GetComponent<SkillLocator>();

            skillLocator.primary.skillFamily.variants[0].skillDef.activationState = new SerializableEntityStateType(typeof(EntityStates.Captain.Weapon.FireCaptainShotgun));

            On.EntityStates.CaptainDefenseMatrixItem.DefenseMatrixOn.DeleteNearbyProjectile += MicrobotEdit;
        }

        private void FamilyChanges()

        {
            SkillLocator skillLocator = captain.GetComponent<SkillLocator>();

            GenericSkill primarySkill = captain.AddComponent<GenericSkill>();
            primarySkill.skillName = "captainPrimary";

            SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            (newFamily as ScriptableObject).name = "captainPrimaryFamily";
            //newFamily.variants = new SkillFamily.Variant[] { new SkillFamily.Variant() { skillDef = snipeLight }, new SkillFamily.Variant() { skillDef = badPrimary } };

            primarySkill._skillFamily = newFamily;
            ContentAddition.AddSkillFamily(newFamily);
            skillLocator.primary = primarySkill;

            GenericSkill secondarySkill = captain.AddComponent<GenericSkill>();
            primarySkill.skillName = "captainSecondary";

            SkillFamily newFamily2 = ScriptableObject.CreateInstance<SkillFamily>();
            (newFamily2 as ScriptableObject).name = "captainSecondaryFamily";
            //  newFamily2.variants = new SkillFamily.Variant[] { new SkillFamily.Variant() { skillDef = snipeHeavy } };

            secondarySkill._skillFamily = newFamily2;
            ContentAddition.AddSkillFamily(newFamily2);
            skillLocator.secondary = secondarySkill;

            GenericSkill utilitySkill = captain.AddComponent<GenericSkill>();
            utilitySkill.skillName = "captainUtility";
            //utilitySkill._skillFamily = utilityFamily;
            skillLocator.utility = utilitySkill;

            GenericSkill specialSkill = captain.AddComponent<GenericSkill>();
            specialSkill.skillName = "captainSpecial";
            //specialSkill._skillFamily = specialFamily;
            skillLocator.special = specialSkill;
        }

        private bool MicrobotEdit(On.EntityStates.CaptainDefenseMatrixItem.DefenseMatrixOn.orig_DeleteNearbyProjectile orig, DefenseMatrixOn self)
        {
            Vector3 vector = self.attachedBody ? self.attachedBody.corePosition : Vector3.zero;
            TeamIndex teamIndex = self.attachedBody ? self.attachedBody.teamComponent.teamIndex : TeamIndex.None;
            float num = DefenseMatrixOn.projectileEraserRadius * DefenseMatrixOn.projectileEraserRadius;
            int num2 = 0;
            int itemStack = self.GetItemStack();
            bool result = false;
            List<ProjectileController> instancesList = InstanceTracker.GetInstancesList<ProjectileController>();
            List<ProjectileController> list = new List<ProjectileController>();
            int num3 = 0;
            int count = instancesList.Count;
            while (num3 < count && num2 < itemStack)
            {
                ProjectileController projectileController = instancesList[num3];
                if (!projectileController.cannotBeDeleted && projectileController.teamFilter.teamIndex != teamIndex && (projectileController.transform.position - vector).sqrMagnitude < num)
                {
                    bool canDelete = true;

                    ProjectileSimple ps = projectileController.gameObject.GetComponent<ProjectileSimple>();
                    ProjectileCharacterController pcc = projectileController.gameObject.GetComponent<ProjectileCharacterController>();

                    if ((!ps || (ps && ps.desiredForwardSpeed == 0f)) && !pcc)
                        canDelete = false;

                    if (canDelete)
                    {
                        list.Add(projectileController);
                        num2++;
                    }
                }
                num3++;
            }
            int i = 0;
            int count2 = list.Count;
            while (i < count2)
            {
                ProjectileController projectileController2 = list[i];
                if (projectileController2)
                {
                    result = true;
                    Vector3 position = projectileController2.transform.position;
                    Vector3 start = vector;
                    if (DefenseMatrixOn.tracerEffectPrefab)
                    {
                        EffectData effectData = new EffectData
                        {
                            origin = position,
                            start = start
                        };
                        EffectManager.SpawnEffect(DefenseMatrixOn.tracerEffectPrefab, effectData, true);
                    }
                    EntityState.Destroy(projectileController2.gameObject);
                }
                i++;
            }
            return result;
        }
    }
}