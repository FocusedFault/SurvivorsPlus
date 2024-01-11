using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SurvivorsPlus.REX
{
    public class REXChanges
    {
        public static GameObject syringe = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Treebot/SyringeProjectile.prefab").WaitForCompletion();

        public REXChanges()
        {
            syringe.GetComponent<ProjectileController>().procCoefficient = 0.8f;
        }
    }
}