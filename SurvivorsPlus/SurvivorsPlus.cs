using BepInEx;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SurvivorsPlus
{
  [BepInPlugin("com.Nuxlar.SurvivorsPlus", "SurvivorsPlus", "0.0.3")]

  public class SurvivorsPlus : BaseUnityPlugin
  {


    public void Awake()
    {
      new Commando.CommandoChanges();
      new Huntress.HuntressChanges();
      new Bandit.BanditChanges();
      new Engineer.EngineerChanges();
    }

  }
}