using Zenject;

namespace PachowStudios.BadTummyBunny
{
  public abstract class FacadeView<TModel> : BaseView<TModel>
    where TModel : class, IFacade
  {
    private void Start()
      => Model.Initialize();

    private void Update()
      => Model.Tick();

    private void LateUpdate()
      => Model.LateTick();

    private void FixedUpdate()
      => Model.FixedTick();

    private void OnDestroy()
      => Model.Dispose();
  }
}