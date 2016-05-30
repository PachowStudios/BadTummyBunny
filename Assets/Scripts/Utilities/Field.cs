using JetBrains.Annotations;

namespace PachowStudios
{
  public class Field<T>
    where T : struct
  {
    public T Value { get; set; }

    public Field()
      : this(default(T)) { }

    public Field(T value)
    {
      Value = value;
    }

    public static implicit operator T([NotNull] Field<T> @this)
      => @this.Value;

    public static implicit operator Field<T>(T value)
      => new Field<T>(value);
  }
}