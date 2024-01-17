using EntityStates.Mage.Weapon;

namespace SurvivorsPlus.Artificer
{
  public class ChargeFireBomb : BaseChargeBombState
  {
    public override BaseThrowBombState GetNextState() => (BaseThrowBombState)new ThrowFireBomb();
  }
}