using JetBrains.Annotations;

namespace MarkLight
{
  /// <summary>
  /// Generic base class for dependency view fields.
  /// </summary>
  public class ViewField<T> : ViewFieldBase
  {
    public T _value;

    /// <summary>
    /// Gets or sets view field notifying observers if the value has changed.
    /// </summary>
    public T Value
    {
      get
      {
        if (ParentView != null)
        {
          return (T)ParentView.GetValue(ViewFieldPath);
        }

        return _value;
      }
      set
      {
        if (ParentView != null)
        {
          ParentView.SetValue(ViewFieldPath, value);
        }
        else
        {
          _value = value;
          _isSet = true;
        }
      }
    }

    /// <summary>
    /// Sets view field directly without notifying observers that the value has changed.
    /// </summary>
    public T DirectValue
    {
      set
      {
        if (ParentView != null)
        {
          ParentView.SetValue(ViewFieldPath, value, true, null, null, false);
        }
        else
        {
          _value = value;
          _isSet = true;
        }
      }
    }

    /// <summary>
    /// Gets boolean indicating if the value has been set. 
    /// </summary>
    public bool IsSet
    {
      get
      {
        if (ParentView != null)
        {
          return ParentView.IsSet(ViewFieldPath);
        }
        else
        {
          return _isSet;
        }
      }
    }

    public ViewField()
      : this(default(T)) { }

    public ViewField(T value)
    {
      this._value = value;
    }

    public static implicit operator T([NotNull] ViewField<T> @this)
      => @this.Value;
  }
}
