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
    public class ArtificerController : NetworkBehaviour, IOnTakeDamageServerReceiver, IOnDamageDealtServerReceiver
    {
        [Header("Cached Components")]
        public CharacterBody characterBody;
        public Animator characterAnimator;
        public EntityStateMachine corruptionModeStateMachine;
        public EntityStateMachine bodyStateMachine;
        public EntityStateMachine weaponStateMachine;
        [Header("Corruption Values")]
        public float maxCorruption;
        public float minimumCorruptionPerVoidItem;
        public float corruptionPerSecondInCombat;
        public float corruptionPerSecondOutOfCombat;
        public float corruptionForFullDamage;
        public float corruptionForFullHeal;
        public float corruptionPerCrit;
        public float corruptionDeltaThresholdToAnimate;
        [Header("Corruption Mode")]
        public BuffDef corruptedBuffDef;
        public float corruptionFractionPerSecondWhileCorrupted;
        [Header("UI")]
        [SerializeField]
        public GameObject overlayPrefab;
        [SerializeField]
        public string overlayChildLocatorEntry;
        private ChildLocator overlayInstanceChildLocator;
        private Animator overlayInstanceAnimator;
        private OverlayController overlayController;
        private List<ImageFillController> fillUiList = new List<ImageFillController>();
        private TextMeshProUGUI uiCorruptionText;
        private int voidItemCount;
        [SyncVar(hook = "OnCorruptionModified")]
        private float _corruption;

        public float corruption => this._corruption;

        public float corruptionFraction => this.corruption / this.maxCorruption;

        public float corruptionPercentage => this.corruptionFraction * 100f;

        public float minimumCorruption => this.minimumCorruptionPerVoidItem * (float)this.voidItemCount;

        public bool isFullCorruption => (double)this.corruption >= (double)this.maxCorruption;

        public bool isCorrupted => (bool)(UnityEngine.Object)this.characterBody && this.characterBody.HasBuff(this.corruptedBuffDef);

        public bool isPermanentlyCorrupted => (double)this.minimumCorruption >= (double)this.maxCorruption;

        private HealthComponent bodyHealthComponent => this.characterBody.healthComponent;

        private void OnEnable()
        {
            this.overlayController = HudOverlayManager.AddOverlay(this.gameObject, new OverlayCreationParams()
            {
                prefab = this.overlayPrefab,
                childLocatorEntry = this.overlayChildLocatorEntry
            });
            this.overlayController.onInstanceAdded += new Action<OverlayController, GameObject>(this.OnOverlayInstanceAdded);
            this.overlayController.onInstanceRemove += new Action<OverlayController, GameObject>(this.OnOverlayInstanceRemoved);
            if (!(bool)(UnityEngine.Object)this.characterBody)
                return;
            this.characterBody.onInventoryChanged += new Action(this.OnInventoryChanged);
            if (!NetworkServer.active)
                return;
            HealthComponent.onCharacterHealServer += new Action<HealthComponent, float, ProcChainMask>(this.OnCharacterHealServer);
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
            if (!(bool)(UnityEngine.Object)this.characterBody)
                return;
            this.characterBody.onInventoryChanged -= new Action(this.OnInventoryChanged);
            if (!NetworkServer.active)
                return;
            HealthComponent.onCharacterHealServer -= new Action<HealthComponent, float, ProcChainMask>(this.OnCharacterHealServer);
        }

        private void FixedUpdate()
        {
            float num1 = 0.0f;
            float num2 = !this.characterBody.HasBuff(this.corruptedBuffDef) ? (this.characterBody.outOfCombat ? this.corruptionPerSecondOutOfCombat : this.corruptionPerSecondInCombat) : num1 + this.corruptionFractionPerSecondWhileCorrupted * (this.maxCorruption - this.minimumCorruption);
            if (NetworkServer.active && !this.characterBody.HasBuff(RoR2Content.Buffs.HiddenInvincibility))
                this.AddCorruption(num2 * Time.fixedDeltaTime);
            this.UpdateUI();
            if (!(bool)(UnityEngine.Object)this.characterAnimator)
                return;
            this.characterAnimator.SetFloat("corruptionFraction", this.corruptionFraction);
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
            if ((bool)(UnityEngine.Object)this.overlayInstanceAnimator)
            {
                this.overlayInstanceAnimator.SetFloat("corruption", this.corruption);
                this.overlayInstanceAnimator.SetBool("isCorrupted", this.isCorrupted);
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
            this.overlayInstanceAnimator = instance.GetComponent<Animator>();
        }

        private void OnOverlayInstanceRemoved(OverlayController controller, GameObject instance) => this.fillUiList.Remove(instance.GetComponent<ImageFillController>());

        private void OnCharacterHealServer(
          HealthComponent healthComponent,
          float amount,
          ProcChainMask procChainMask)
        {
            if (healthComponent != this.bodyHealthComponent || procChainMask.HasProc(ProcType.VoidSurvivorCrush))
                return;
            this.AddCorruption(amount / this.bodyHealthComponent.fullCombinedHealth * this.corruptionForFullHeal);
        }

        public void OnDamageDealtServer(DamageReport damageReport)
        {
            if (!damageReport.damageInfo.crit)
                return;
            this.AddCorruption(damageReport.damageInfo.procCoefficient * this.corruptionPerCrit);
        }

        public void OnTakeDamageServer(DamageReport damageReport)
        {
            float num = damageReport.damageDealt / this.bodyHealthComponent.fullCombinedHealth;
            if (damageReport.damageInfo.procChainMask.HasProc(ProcType.VoidSurvivorCrush))
                return;
            this.AddCorruption(num * this.corruptionForFullDamage);
        }

        private void OnInventoryChanged()
        {
            this.voidItemCount = 0;
            Inventory inventory = this.characterBody.inventory;
            if (!(bool)(UnityEngine.Object)inventory)
                return;
            this.voidItemCount = inventory.GetTotalItemCountOfTier(ItemTier.VoidTier1) + inventory.GetTotalItemCountOfTier(ItemTier.VoidTier2) + inventory.GetTotalItemCountOfTier(ItemTier.VoidTier3) + inventory.GetTotalItemCountOfTier(ItemTier.VoidBoss);
        }

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
            if ((bool)(UnityEngine.Object)this.overlayInstanceAnimator && (double)Mathf.Abs(newCorruption - this.corruption) > (double)this.corruptionDeltaThresholdToAnimate)
                this.overlayInstanceAnimator.SetTrigger("corruptionIncreased");
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