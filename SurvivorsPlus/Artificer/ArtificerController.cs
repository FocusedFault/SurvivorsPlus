using RoR2;
using RoR2.HudOverlay;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

namespace SurvivorsPlus.Artificer
{
    [RequireComponent(typeof(InputBankTest))]
    [RequireComponent(typeof(TeamComponent))]
    [RequireComponent(typeof(CharacterBody))]
    public class ArtificerController : NetworkBehaviour
    {
        [Header("Cached Components")]
        public CharacterBody characterBody;
        public EntityStateMachine corruptionModeStateMachine;
        public EntityStateMachine bodyStateMachine;
        public EntityStateMachine weaponStateMachine;
        [Header("Corruption Values")]
        public float maxCorruption = 100;
        public float minimumCorruption = 0;
        public float corruptionPerSecondInCombat = 3f;
        public float corruptionPerSecondOutOfCombat = 1.5f;
        public BuffDef ionBuffDef;
        public BuffDef iceBuffDef;
        public BuffDef fireBuffDef;
        [Header("UI")]
        public GameObject overlayPrefab = ArtificerChanges.uiOverlay;
        public string overlayChildLocatorEntry = "CrosshairExtras";
        private ChildLocator overlayInstanceChildLocator;
        private OverlayController overlayController;
        private List<ImageFillController> fillUiList = new List<ImageFillController>();
        private TextMeshProUGUI uiCorruptionText;
        [SyncVar(hook = "OnCorruptionModified")]
        private float _corruption;

        public float corruption => this._corruption;

        public float corruptionFraction => this.corruption / this.maxCorruption;

        public float corruptionPercentage => this.corruptionFraction * 100f;

        public bool isFullCorruption => (double)this.corruption >= (double)this.maxCorruption;
        public string currentElement = "Ion";

        private void OnEnable()
        {
            this.characterBody = this.GetComponent<CharacterBody>();
            EntityStateMachine[] components = this.GetComponents<EntityStateMachine>();
            for (int index = 0; index < components.Length; ++index)
            {
                EntityStateMachine entityStateMachine = components[index];
                if (entityStateMachine.customName == "Weapon")
                    this.weaponStateMachine = entityStateMachine;
                else if (entityStateMachine.customName == "Body")
                    this.bodyStateMachine = entityStateMachine;
                else
                    this.corruptionModeStateMachine = entityStateMachine;
            }

            this.overlayController = HudOverlayManager.AddOverlay(this.gameObject, new OverlayCreationParams()
            {
                prefab = this.overlayPrefab,
                childLocatorEntry = this.overlayChildLocatorEntry
            });
            this.overlayController.onInstanceAdded += new Action<OverlayController, GameObject>(this.OnOverlayInstanceAdded);
            this.overlayController.onInstanceRemove += new Action<OverlayController, GameObject>(this.OnOverlayInstanceRemoved);
        }

        private void OnDisable()
        {
            if (this.overlayController != null)
            {
                this.overlayController.onInstanceAdded -= new Action<OverlayController, GameObject>(this.OnOverlayInstanceAdded);
                this.overlayController.onInstanceRemove -= new Action<OverlayController, GameObject>(this.OnOverlayInstanceRemoved);
                this.fillUiList.Clear();
                HudOverlayManager.RemoveOverlay(this.overlayController);
            }
        }

        private void FixedUpdate()
        {
            float num2 = this.characterBody.outOfCombat ? this.corruptionPerSecondOutOfCombat : this.corruptionPerSecondInCombat;
            if (NetworkServer.active && !this.characterBody.HasBuff(RoR2Content.Buffs.HiddenInvincibility))
                this.AddCorruption(num2 * Time.fixedDeltaTime);
            this.UpdateUI();
        }

        private void UpdateUI()
        {
            foreach (ImageFillController fillUi in this.fillUiList)
                fillUi.SetTValue(this.corruption / this.maxCorruption);
            if ((bool)(UnityEngine.Object)this.overlayInstanceChildLocator)
            {
                this.overlayInstanceChildLocator.FindChild("CorruptionThreshold").rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.InverseLerp(0.0f, this.maxCorruption, this.corruption) * -360f);
                this.overlayInstanceChildLocator.FindChild("MinCorruptionThreshold").rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.InverseLerp(0.0f, this.maxCorruption, this.minimumCorruption) * -360f);
            }
            if (!(bool)(UnityEngine.Object)this.uiCorruptionText)
                return;
            StringBuilder stringBuilder = HG.StringBuilderPool.RentStringBuilder();
            stringBuilder.AppendInt(Mathf.FloorToInt(this.corruption), maxDigits: 3U).Append("%");
            this.uiCorruptionText.SetText(stringBuilder);
            HG.StringBuilderPool.ReturnStringBuilder(stringBuilder);
        }

        private void OnOverlayInstanceAdded(OverlayController controller, GameObject instance)
        {
            this.fillUiList.Add(instance.GetComponent<ImageFillController>());
            this.uiCorruptionText = instance.GetComponentInChildren<TextMeshProUGUI>();
            this.overlayInstanceChildLocator = instance.GetComponent<ChildLocator>();
        }

        private void OnOverlayInstanceRemoved(OverlayController controller, GameObject instance) => this.fillUiList.Remove(instance.GetComponent<ImageFillController>());

        [Server]
        public void AddCorruption(float amount)
        {
            if (!NetworkServer.active)
                Debug.LogWarning((object)"[Server] function 'System.Void RoR2.VoidSurvivorController::AddCorruption(System.Single)' called on client");
            else
                this.Network_corruption = Mathf.Clamp(this.corruption + amount, this.minimumCorruption, this.maxCorruption);
        }

        private void OnCorruptionModified(float newCorruption)
        {
            this.Network_corruption = newCorruption;
        }

        private void UNetVersion()
        {
        }

        public float Network_corruption
        {
            get => this._corruption;
            [param: In]
            set
            {
                if (NetworkServer.localClientActive && !this.syncVarHookGuard)
                {
                    this.syncVarHookGuard = true;
                    this.OnCorruptionModified(value);
                    this.syncVarHookGuard = false;
                }
                this.SetSyncVar<float>(value, ref this._corruption, 1U);
            }
        }

        public override bool OnSerialize(NetworkWriter writer, bool forceAll)
        {
            if (forceAll)
            {
                writer.Write(this._corruption);
                return true;
            }
            bool flag = false;
            if (((int)this.syncVarDirtyBits & 1) != 0)
            {
                if (!flag)
                {
                    writer.WritePackedUInt32(this.syncVarDirtyBits);
                    flag = true;
                }
                writer.Write(this._corruption);
            }
            if (!flag)
                writer.WritePackedUInt32(this.syncVarDirtyBits);
            return flag;
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            if (initialState)
            {
                this._corruption = reader.ReadSingle();
            }
            else
            {
                if (((int)reader.ReadPackedUInt32() & 1) == 0)
                    return;
                this.OnCorruptionModified(reader.ReadSingle());
            }
        }

        public override void PreStartClient()
        {
        }

    }
}