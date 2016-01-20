using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public class BasicFart : BaseFart<FartSettings>
  {
    [Inject] protected override FartSettings Config { get; set; }
  }
}