namespace PachowStudios.BadTummyBunny
{
  public interface IAttachable<in T>
  {
    void Attach(T attachedObject);
    void Detach();
  }
}