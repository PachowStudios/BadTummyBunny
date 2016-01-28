using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public abstract class FacadeView<TModel> : BaseView<TModel>
    where TModel : class, IFacade
  {
    protected virtual void Start()
      => Model.Initialize();

    protected virtual void Update()
      => Model.Tick();

    protected virtual void LateUpdate()
      => Model.LateTick();

    protected virtual void FixedUpdate()
      => Model.FixedTick();

    protected virtual void OnDestroy()
      => Model.Dispose();
  }
}