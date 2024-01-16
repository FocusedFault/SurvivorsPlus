using EntityStates.VoidSurvivor;
using R2API;
using RoR2;

namespace SurvivorsPlus.VoidFiend
{
    public class VoidFiendChanges
    {
        public VoidFiendChanges()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.VoidSurvivorController.OnEnable += ChangeValues;
            On.RoR2.GlobalEventManager.OnCharacterDeath += ExtraCorruptionOnKill;
            On.EntityStates.VoidSurvivor.CorruptionTransitionBase.OnEnter += ReduceCorruptionAnim;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.HasBuff(DLC1Content.Buffs.VoidSurvivorCorruptMode))
                args.armorAdd -= 100;
        }

        private void ReduceCorruptionAnim(On.EntityStates.VoidSurvivor.CorruptionTransitionBase.orig_OnEnter orig, EntityStates.VoidSurvivor.CorruptionTransitionBase self)
        {
            if (self is EnterCorruptionTransition)
                self.duration = 0.75f;
            orig(self);
        }

        private void ChangeValues(On.RoR2.VoidSurvivorController.orig_OnEnable orig, VoidSurvivorController self)
        {
            self.corruptionPerCrit = 0;
            self.corruptionForFullHeal = 0;
            self.corruptionFractionPerSecondWhileCorrupted = -0.033333335f;
            self.corruptionPerSecondInCombat = 1f;
            self.corruptionPerSecondOutOfCombat = 0f;
            self.corruptionForFullDamage = 0f;
            orig(self);
        }

        private void ExtraCorruptionOnKill(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            orig(self, damageReport);
            if (damageReport.attackerBody && damageReport.attackerBody.GetComponent<VoidSurvivorController>() && !damageReport.attackerBody.HasBuff(DLC1Content.Buffs.VoidSurvivorCorruptMode))
            {
                VoidSurvivorController voidSurvivorController = damageReport.attackerBody.GetComponent<VoidSurvivorController>();
                voidSurvivorController.AddCorruption(2f);
            }
        }
    }
}