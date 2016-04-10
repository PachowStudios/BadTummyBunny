namespace PachowStudios.BadTummyBunny
{
  public class CharacterTookDamageMessage : IMessage
  {
    public int DamageTaken { get; }

    public CharacterTookDamageMessage(int damageTaken)
    {
      DamageTaken = damageTaken;
    }
  }

  public class CharacterKillzoneTriggeredMessage : IMessage { }
}