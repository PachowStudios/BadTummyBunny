namespace PachowStudios
{
  public interface IFiniteState<T>
    where T : class
  {
    void Begin();
    void Reason();
    void Tick(float deltaTime);
    void End();
  }

}