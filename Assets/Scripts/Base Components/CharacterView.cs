using UnityEngine;
using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public abstract class CharacterView<TModel> : FacadeView<TModel>
    where TModel : class, IFacade
  {
    public virtual Vector2 FacingDirection => new Vector2(Transform.localScale.x, 0f);
    public virtual bool IsFacingRight => FacingDirection.x > 0f;
  }
}