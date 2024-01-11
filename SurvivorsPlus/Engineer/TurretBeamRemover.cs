using RoR2;
using UnityEngine;

namespace SurvivorsPlus.Engineer
{
    public class TurretBeamRemover : MonoBehaviour
    {
        private GameObject muzzle;
        private InputBankTest inputBank;

        private void Start()
        {
            muzzle = this.GetComponent<CharacterBody>().modelLocator.modelTransform.GetComponent<ChildLocator>().FindChild("Muzzle").gameObject;
            inputBank = this.GetComponent<CharacterBody>().inputBank;
        }

        private void FixedUpdate()
        {
            if (muzzle.transform.childCount > 0 && !inputBank.skill1.down)
                GameObject.Destroy(muzzle.transform.GetChild(0).gameObject);
        }
    }
}