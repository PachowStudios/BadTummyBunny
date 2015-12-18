using JetBrains.Annotations;

namespace PachowStudios
{
  public class Field<T>
    where T : struct
  {
    [UsedImplicitly] public T Value;

    public Field()
      : this(default(T)) { }

    public Field(T value)
    {
      this.Value = value;
    } 
  }
}