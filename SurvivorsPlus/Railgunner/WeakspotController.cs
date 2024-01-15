using RoR2;
using RoR2.HudOverlay;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SurvivorsPlus.Railgunner
{
    public class WeakspotController : MonoBehaviour
    {
        public GameObject scopeOverlayPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerScopeHeavyOverlay.prefab").WaitForCompletion().transform.GetChild(0).gameObject;
        private OverlayController overlayController;

        public void Start()
        {
            this.overlayController = HudOverlayManager.AddOverlay(this.gameObject, new OverlayCreationParams()
            {
                prefab = this.scopeOverlayPrefab,
                childLocatorEntry = "ScopeContainer"
            });
        }

        public void OnDisable()
        {
            this.RemoveOverlay(0.0f);
        }


        protected void RemoveOverlay(float transitionDuration)
        {
            if (this.overlayController == null)
                return;
            HudOverlayManager.RemoveOverlay(this.overlayController);
            this.overlayController = (OverlayController)null;
        }
    }
}