using RoR2;
using EntityStates;
using UnityEngine;
using RoR2.Skills;

namespace SurvivorsPlus.Artificer
{
  class Attunement : BaseState
  {
    private float stopwatch;
    private float swapDuration = 0.5f;
    private float blastAttackDamageCoefficient = 3f;
    private float blastAttackForce = 500f;
    private ArtificerController attunementController;

    private GameObject ionEffect = ArtificerChanges.ionEffect;
    private GameObject iceEffect = ArtificerChanges.iceEffect;
    private GameObject fireEffect = ArtificerChanges.fireEffect;

    private SkillDef ionPrimarySkillDef = ArtificerChanges.ionPrimarySkillDef;
    private SkillDef ionSecondarySkillDef = ArtificerChanges.ionSecondarySkillDef;
    private SkillDef ionUtilitySkillDef = ArtificerChanges.ionUtilitySkillDef;

    private SkillDef icePrimarySkillDef = ArtificerChanges.icePrimarySkillDef;
    private SkillDef iceSecondarySkillDef = ArtificerChanges.iceSecondarySkillDef;
    private SkillDef iceUtilitySkillDef = ArtificerChanges.iceUtilitySkillDef;

    private SkillDef firePrimarySkillDef = ArtificerChanges.firePrimarySkillDef;
    private SkillDef fireSecondarySkillDef = ArtificerChanges.fireSecondarySkillDef;
    private SkillDef fireUtilitySkillDef = ArtificerChanges.fireUtilitySkillDef;


    public override void OnEnter()
    {
      base.OnEnter();
      this.attunementController = this.GetComponent<ArtificerController>();
      this.PlayAnimation("Gesture, Additive", "PrepWall", "PrepWall.playbackRate", this.swapDuration);
    }

    public override void FixedUpdate()
    {
      base.FixedUpdate();
      this.stopwatch += Time.fixedDeltaTime;
      if ((double)this.stopwatch < (double)this.swapDuration && !this.isAuthority)
        return;
      this.PlayAnimation("Gesture, Additive", "FireWall");
      SwapElement();
      FireBlast();
      this.outer.SetNextStateToMain();
    }

    private void SwapElement()
    {
      string currentElement = this.attunementController.currentElement;
      switch (currentElement)
      {
        case "Ion":
          this.GetComponent<ArtificerController>().currentElement = "Ice";
          if (this.characterBody.HasBuff(ArtificerChanges.ionBuff))
            this.characterBody.RemoveBuff(ArtificerChanges.ionBuff);
          this.characterBody.AddBuff(ArtificerChanges.iceBuff);
          this.skillLocator.primary.SetSkillOverride((object)this, this.icePrimarySkillDef, GenericSkill.SkillOverridePriority.Replacement);
          this.skillLocator.secondary.SetSkillOverride((object)this, this.iceSecondarySkillDef, GenericSkill.SkillOverridePriority.Replacement);
          this.skillLocator.utility.SetSkillOverride((object)this, this.iceUtilitySkillDef, GenericSkill.SkillOverridePriority.Replacement);
          EffectManager.SpawnEffect(this.iceEffect, new EffectData
          {
            origin = this.characterBody.corePosition,
            scale = 1f
          }, true);
          break;
        case "Ice":
          this.GetComponent<ArtificerController>().currentElement = "Fire";
          this.characterBody.RemoveBuff(ArtificerChanges.iceBuff);
          this.characterBody.AddBuff(ArtificerChanges.fireBuff);
          this.skillLocator.primary.SetSkillOverride((object)this, this.firePrimarySkillDef, GenericSkill.SkillOverridePriority.Replacement);
          this.skillLocator.secondary.SetSkillOverride((object)this, this.fireSecondarySkillDef, GenericSkill.SkillOverridePriority.Replacement);
          this.skillLocator.utility.SetSkillOverride((object)this, this.fireUtilitySkillDef, GenericSkill.SkillOverridePriority.Replacement);
          EffectManager.SpawnEffect(this.fireEffect, new EffectData
          {
            origin = this.characterBody.corePosition,
            scale = 1f
          }, true);
          break;
        case "Fire":
          this.GetComponent<ArtificerController>().currentElement = "Ion";
          this.characterBody.RemoveBuff(ArtificerChanges.fireBuff);
          this.characterBody.AddBuff(ArtificerChanges.ionBuff);
          this.skillLocator.primary.SetSkillOverride((object)this, this.ionPrimarySkillDef, GenericSkill.SkillOverridePriority.Replacement);
          this.skillLocator.secondary.SetSkillOverride((object)this, this.ionSecondarySkillDef, GenericSkill.SkillOverridePriority.Replacement);
          this.skillLocator.utility.SetSkillOverride((object)this, this.ionUtilitySkillDef, GenericSkill.SkillOverridePriority.Replacement);
          EffectManager.SpawnEffect(this.ionEffect, new EffectData
          {
            origin = this.characterBody.corePosition,
            scale = 0.25f
          }, true);
          break;
      }
    }

    private void FireBlast()
    {
      new BlastAttack()
      {
        attacker = this.gameObject,
        inflictor = this.gameObject,
        teamIndex = TeamComponent.GetObjectTeam(this.gameObject),
        baseDamage = this.damageStat * this.blastAttackDamageCoefficient,
        baseForce = this.blastAttackForce,
        position = this.characterBody.corePosition,
        radius = this.characterBody.radius + 10f,
        falloffModel = BlastAttack.FalloffModel.Linear,
        attackerFiltering = AttackerFiltering.NeverHitSelf
      }.Fire();
    }
  }
}
