using RoR2;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;

namespace SurvivorsPlus.Railgunner
{
    public class PolarSlow : MonoBehaviour
    {
        public TeamFilter teamFilter;
        public float slowDownCoefficient = 0.1f;
        private List<SlowDownProjectiles.SlowDownProjectileInfo> slowDownProjectileInfos;

        private void Start()
        {
            this.teamFilter = this.GetComponent<TeamFilter>();
            this.slowDownProjectileInfos = new List<SlowDownProjectiles.SlowDownProjectileInfo>();
        }

        private void OnTriggerEnter(Collider other)
        {
            TeamFilter teamFilter = other.GetComponent<TeamFilter>();
            Rigidbody rigidbody = other.GetComponent<Rigidbody>();
            if (!(bool)(Object)rigidbody || teamFilter.teamIndex == this.teamFilter.teamIndex)
                return;
            teamFilter.teamIndex = this.teamFilter.teamIndex;
            other.gameObject.AddComponent<PolarReversal>().initialVelocity = rigidbody.velocity;
            this.slowDownProjectileInfos.Add(new SlowDownProjectiles.SlowDownProjectileInfo()
            {
                rb = rigidbody,
                previousVelocity = rigidbody.velocity
            });
        }

        private void OnTriggerExit(Collider other)
        {
            TeamFilter component1 = other.GetComponent<TeamFilter>();
            Rigidbody component2 = other.GetComponent<Rigidbody>();
            if (!(bool)(Object)component2 || component1.teamIndex == this.teamFilter.teamIndex)
                return;
            this.RemoveFromSlowDownProjectileInfos(component2);
        }

        private void RemoveFromSlowDownProjectileInfos(Rigidbody rb)
        {
            for (int index = 0; index < this.slowDownProjectileInfos.Count; ++index)
            {
                if ((Object)this.slowDownProjectileInfos[index].rb == (Object)rb)
                {
                    this.slowDownProjectileInfos.RemoveAt(index);
                    break;
                }
            }
        }

        private void FixedUpdate()
        {

            for (int index = 0; index < this.slowDownProjectileInfos.Count; ++index)
            {

                SlowDownProjectiles.SlowDownProjectileInfo downProjectileInfo = this.slowDownProjectileInfos[index];
                Rigidbody rb = downProjectileInfo.rb;
                Vector3 previousVelocity = downProjectileInfo.previousVelocity;
                if ((bool)(Object)rb && rb.GetComponent<PolarReversal>() != null)
                {
                    rb.MovePosition(rb.position - Vector3.Lerp(previousVelocity, Vector3.zero, this.slowDownCoefficient) * Time.fixedDeltaTime);
                    downProjectileInfo.previousVelocity = rb.velocity;
                    this.slowDownProjectileInfos[index] = downProjectileInfo;
                }
                else
                    this.RemoveFromSlowDownProjectileInfos(rb);
            }
        }

        private struct SlowDownProjectileInfo
        {
            public Rigidbody rb;
            public Vector3 previousVelocity;
        }
    }
}