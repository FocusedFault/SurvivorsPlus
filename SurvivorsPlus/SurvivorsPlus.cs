using BepInEx;
using BepInEx.Configuration;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SurvivorsPlus
{
  [BepInPlugin("com.Nuxlar.SurvivorsPlus", "SurvivorsPlus", "1.1.1")]

  public class SurvivorsPlus : BaseUnityPlugin
  {
    private GameObject arti = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageBody.prefab").WaitForCompletion();
    private GameObject mult = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ToolbotBody.prefab").WaitForCompletion();

    public static ConfigEntry<bool> enableAccelerationChanges;
    public static ConfigEntry<bool> enableCommandoChanges;
    public static ConfigEntry<bool> enableHuntressChanges;
    public static ConfigEntry<bool> enableBanditChanges;
    public static ConfigEntry<bool> enableEngineerChanges;
    public static ConfigEntry<bool> enableMercChanges;
    public static ConfigEntry<bool> enableREXChanges;
    public static ConfigEntry<bool> enableRunnerChanges;
    public static ConfigEntry<bool> enableAcridChanges;
    public static ConfigEntry<bool> enableCaptainChanges;
    public static ConfigEntry<bool> enableVoidFiendChanges;
    public static ConfigEntry<bool> enableArtificerChanges;
    public static ConfigEntry<bool> enableLoaderChanges;

    private static ConfigFile SPConfig { get; set; }

    public void Awake()
    {

      SPConfig = new ConfigFile(Paths.ConfigPath + "\\com.Nuxlar.SurvivorsPlus.cfg", true);
      enableAccelerationChanges = SPConfig.Bind<bool>("General", "Enable Acceleration Changes", true, "Toggle Artificer and MUL-T acceleration changes on/off");
      enableCommandoChanges = SPConfig.Bind<bool>("General", "Enable Commando Changes", true, "Toggle commando changes on/off");
      enableHuntressChanges = SPConfig.Bind<bool>("General", "Enable Huntress Changes", true, "Toggle huntress changes on/off");
      enableBanditChanges = SPConfig.Bind<bool>("General", "Enable Bandit Changes", true, "Toggle bandit changes on/off");
      enableEngineerChanges = SPConfig.Bind<bool>("General", "Enable Engineer Changes", true, "Toggle engineer changes on/off");
      enableMercChanges = SPConfig.Bind<bool>("General", "Enable Mercenary Changes", true, "Toggle mercenary changes on/off");
      enableREXChanges = SPConfig.Bind<bool>("General", "Enable REX Changes", true, "Toggle REX changes on/off");
      enableRunnerChanges = SPConfig.Bind<bool>("General", "Enable Railgunner Changes", true, "Toggle Railgunner changes on/off");
      enableAcridChanges = SPConfig.Bind<bool>("General", "Enable Acrid Changes", true, "Toggle Acrid changes on/off");
      enableCaptainChanges = SPConfig.Bind<bool>("General", "Enable Captain Changes", true, "Toggle Captain changes on/off");
      enableVoidFiendChanges = SPConfig.Bind<bool>("General", "Enable Void Fiend Changes", true, "Toggle Void Fiend changes on/off");
      enableArtificerChanges = SPConfig.Bind<bool>("General", "Enable Artificer Changes", true, "Toggle Artificer changes on/off");
      enableLoaderChanges = SPConfig.Bind<bool>("General", "Enable Loader Changes", true, "Toggle Loader changes on/off");

      if (enableAccelerationChanges.Value)
      {
        arti.GetComponent<CharacterBody>().baseAcceleration = 80f;
        mult.GetComponent<CharacterBody>().baseAcceleration = 80f;
      }

      if (enableCommandoChanges.Value)
        new Commando.CommandoChanges();
      if (enableHuntressChanges.Value)
        new Huntress.HuntressChanges();
      if (enableBanditChanges.Value)
        new Bandit.BanditChanges();
      if (enableEngineerChanges.Value)
        new Engineer.EngineerChanges();
      if (enableMercChanges.Value)
        new Mercenary.MercenaryChanges();
      if (enableREXChanges.Value)
        new REX.REXChanges();
      if (enableRunnerChanges.Value)
        new Railgunner.RailgunnerChanges();
      if (enableCaptainChanges.Value)
        new Captain.CaptainChanges();
      if (enableVoidFiendChanges.Value)
        new VoidFiend.VoidFiendChanges();
      if (enableLoaderChanges.Value)
        new Loader.LoaderChanges();
    }

    public static void ChangeEntityStateValue(string entityStateConfiguration, string fieldName, string newValue)
    {
      EntityStateConfiguration entityState = Addressables.LoadAssetAsync<EntityStateConfiguration>(entityStateConfiguration).WaitForCompletion();
      for (int i = 0; i < entityState.serializedFieldsCollection.serializedFields.Length; i++)
      {
        if (entityState.serializedFieldsCollection.serializedFields[i].fieldName == fieldName)
          entityState.serializedFieldsCollection.serializedFields[i].fieldValue.stringValue = newValue;
      }
    }
    public static void ChangeEntityStateValueO(string entityStateConfiguration, string fieldName, Object newValue)
    {
      EntityStateConfiguration entityState = Addressables.LoadAssetAsync<EntityStateConfiguration>(entityStateConfiguration).WaitForCompletion();
      for (int i = 0; i < entityState.serializedFieldsCollection.serializedFields.Length; i++)
      {
        if (entityState.serializedFieldsCollection.serializedFields[i].fieldName == fieldName)
          entityState.serializedFieldsCollection.serializedFields[i].fieldValue.objectValue = newValue;
      }
    }
  }
}