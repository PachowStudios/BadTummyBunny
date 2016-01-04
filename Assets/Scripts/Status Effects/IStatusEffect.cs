using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public interface IStatusEffect : IAttachable<IStatusEffectable>, ITickable
  {
    string Name { get; }
    StatusEffectType Type { get; }
    IStatusEffectable AffectedCharacter { get; }
  }
}
