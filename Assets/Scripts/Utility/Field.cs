public class Field<T>
  where T : struct
{
  public T Value;

  public Field()
    : this(default(T)) { }

  public Field(T value)
  {
    this.Value = value;
  } 
}