using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace SurvivorsPlus.Artificer
{
    [RequireComponent(typeof(InputBankTest))]
    [RequireComponent(typeof(TeamComponent))]
    [RequireComponent(typeof(CharacterBody))]
    public class ArtificerController : NetworkBehaviour
    {
        public BuffDef ionBuffDef;
        public BuffDef iceBuffDef;
        public BuffDef fireBuffDef;
        public string currentElement = "Ion";

    }
}