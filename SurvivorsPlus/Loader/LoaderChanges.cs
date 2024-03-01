using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SurvivorsPlus.Loader
{
    public class LoaderChanges
    {
        GameObject loader = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Loader/LoaderBody.prefab").WaitForCompletion();

        public LoaderChanges()
        {
            CharacterBody body = loader.GetComponent<CharacterBody>();
            body.baseMaxHealth = 110f;
            body.levelMaxHealth = 33f;
            body.baseArmor = 10f;
        }
    }
}