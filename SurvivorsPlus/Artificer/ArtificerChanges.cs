using R2API;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using EntityStates;
using EntityStates.CaptainDefenseMatrixItem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SurvivorsPlus.Artificer
{
    public class ArtificerChanges
    {
        private GameObject arti = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageBody.prefab").WaitForCompletion();

        public ArtificerChanges()
        {
            arti.AddComponent<ArtificerController>();
        }
    }
}