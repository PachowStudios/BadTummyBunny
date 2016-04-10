using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public abstract class FacadeView<TModel> : BaseView<TModel>
    where TModel : class, IFacade
  {
    public bool IsDisposed { get; private set; }

    public override void Dispose()
    {
      if (IsDisposed)
        return;

      IsDisposed = true;
      Model.Dispose();
      base.Dispose();
    }

    protected virtual void Start()
      => Model.Initialize();

    protected virtual void Update()
      => Model.Tick();

    protected virtual void LateUpdate()
      => Model.LateTick();

    protected virtual void FixedUpdate()
      => Model.FixedTick();

    protected virtual void OnDestroy()
      => Dispose();
  }
}