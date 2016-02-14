namespace PachowStudios.BadTummyBunny
{
  public class StarCompletedMessage : IMessage
  {
    public IStar Star { get; }

    public StarCompletedMessage(IStar star)
    {
      Star = star;
    }
  }

  public class StarFailedMessage : IMessage
  {
    public IStar Star { get; }

    public StarFailedMessage(IStar star)
    {
      Star = star;
    }
  }
}