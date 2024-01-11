// Decompiled with JetBrains decompiler
// Type: EntityStates.EngiTurret.EngiTurretWeapon.FireBeam
// Assembly: RoR2, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D532BE5-43BE-416B-9DB3-594E4B62447D
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Risk of Rain 2\Risk of Rain 2_Data\Managed\RoR2.dll

using RoR2;
using UnityEngine;
using EntityStates;
using EntityStates.EngiTurret.EngiTurretWeapon;

namespace SurvivorsPlus.Engineer
{
    public class BetterFireBeam : BaseState
    {
        public GameObject effectPrefab = null;
        public GameObject hitEffectPrefab = EngineerChanges.hitEffect;
        public GameObject laserPrefab = EngineerChanges.laser;
        public string muzzleString = "Muzzle";
        public string attackSoundString = "";
        public float damageCoefficient = 2f;
        public float procCoefficient = 3f;
        public float force = 0f;
        public float minSpread = 0f;
        public float maxSpread = 0f;
        public int bulletCount = 1;
        public float fireFrequency = 5;
        public float maxDistance = 50f;
        private float fireTimer;
        private Ray laserRay;
        private Transform modelTransform;
        private GameObject laserEffectInstance;
        private Transform laserEffectInstanceEndTransform;
        public int bulletCountCurrent = 1;

        public override void OnEnter()
        {
            base.OnEnter();
            // Util.PlaySound(this.attackSoundString, this.gameObject);
            this.fireTimer = 0.0f;
            this.modelTransform = this.GetModelTransform();
            if (!(bool)(Object)this.modelTransform)
                return;
            ChildLocator component = this.modelTransform.GetComponent<ChildLocator>();
            if (!(bool)(Object)component)
                return;
            Transform child = component.FindChild(this.muzzleString);
            if (!(bool)(Object)child || !(bool)(Object)this.laserPrefab)
                return;
            if (child.transform.childCount > 0)
            {
                this.laserEffectInstance = child.transform.GetChild(0).gameObject;
                this.laserEffectInstanceEndTransform = this.laserEffectInstance.GetComponent<ChildLocator>().FindChild("LaserEnd");
            }
            if (!(bool)(Object)this.laserEffectInstance)
            {
                this.laserEffectInstance = Object.Instantiate<GameObject>(this.laserPrefab, child.position, child.rotation);
                this.laserEffectInstance.transform.parent = child;
                this.laserEffectInstanceEndTransform = this.laserEffectInstance.GetComponent<ChildLocator>().FindChild("LaserEnd");
            }
        }
        /*
                public override void OnExit()
                {

                    if ((bool)(Object)this.laserEffectInstance)
                        EntityState.Destroy((Object)this.laserEffectInstance);

                    base.OnExit();
                }
        */
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.laserRay = this.GetLaserRay();
            this.fireTimer += Time.fixedDeltaTime;
            if ((double)this.fireTimer > (double)(1f / (this.fireFrequency * this.characterBody.attackSpeed)))
            {
                this.FireBullet(this.modelTransform, this.laserRay, this.muzzleString);
                this.fireTimer = 0.0f;
            }
            if ((bool)(Object)this.laserEffectInstance && (bool)(Object)this.laserEffectInstanceEndTransform)
                this.laserEffectInstanceEndTransform.position = this.GetBeamEndPoint();
            if (!this.isAuthority)
                return;
            if (this.fireTimer == 0.0f)
                this.outer.SetNextState(this.GetNextState());
        }

        protected Vector3 GetBeamEndPoint()
        {
            Vector3 point = this.laserRay.GetPoint(this.maxDistance);
            GameObject gameObject = this.gameObject;
            Ray laserRay = this.laserRay;
            RaycastHit raycastHit;
            double maxDistance = (double)this.maxDistance;
            if (Util.CharacterRaycast(gameObject, laserRay, out raycastHit, (float)maxDistance, LayerIndex.CommonMasks.bullet, QueryTriggerInteraction.UseGlobal))
                point = raycastHit.point;
            return point;
        }

        protected virtual EntityState GetNextState() => EntityStateCatalog.InstantiateState(this.outer.mainStateType);

        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.Skill;

        public virtual void ModifyBullet(BulletAttack bulletAttack) => bulletAttack.damageType |= DamageType.SlowOnHit;

        public virtual bool ShouldFireLaser() => (bool)(Object)this.inputBank && this.inputBank.skill1.down;

        public virtual Ray GetLaserRay() => this.GetAimRay();

        private void FireBullet(Transform modelTransform, Ray laserRay, string targetMuzzle)
        {
            if ((bool)(Object)this.effectPrefab)
                EffectManager.SimpleMuzzleFlash(this.effectPrefab, this.gameObject, targetMuzzle, false);
            if (!this.isAuthority)
                return;
            BulletAttack bulletAttack = new BulletAttack();
            bulletAttack.owner = this.gameObject;
            bulletAttack.weapon = this.gameObject;
            bulletAttack.origin = laserRay.origin;
            bulletAttack.aimVector = laserRay.direction;
            bulletAttack.minSpread = this.minSpread;
            bulletAttack.maxSpread = this.maxSpread;
            bulletAttack.bulletCount = 1U;
            bulletAttack.damage = this.damageCoefficient * this.damageStat / this.fireFrequency;
            bulletAttack.procCoefficient = this.procCoefficient / this.fireFrequency;
            bulletAttack.force = this.force;
            bulletAttack.muzzleName = targetMuzzle;
            bulletAttack.hitEffectPrefab = this.hitEffectPrefab;
            bulletAttack.isCrit = Util.CheckRoll(this.critStat, this.characterBody.master);
            bulletAttack.HitEffectNormal = false;
            bulletAttack.radius = 0.0f;
            bulletAttack.maxDistance = this.maxDistance;
            this.ModifyBullet(bulletAttack);
            bulletAttack.Fire();
        }
    }
}
