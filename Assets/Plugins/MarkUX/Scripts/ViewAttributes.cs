using System;
using JetBrains.Annotations;

namespace MarkUX
{
  /// <summary>
  /// Tells the view processor that a view field shouldn't be set from xml.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
  public class NotSetFromXml : Attribute { }

  /// <summary>
  /// Tells the view processor to add a script component when the view object is created.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public class AddComponent : Attribute
  {
    private Type _componentType;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    public AddComponent(Type componentType)
    {
      this._componentType = componentType;
    }

    /// <summary>
    /// Gets or sets component type.
    /// </summary>
    public Type ComponentType
    {
      get { return this._componentType; }
      set { this._componentType = value; }
    }
  }

  /// <summary>
  /// Tells the view processor to remove a script component when the view object is created.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
  public class RemoveComponent : Attribute
  {
    private Type _componentType;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    public RemoveComponent(Type componentType)
    {
      this._componentType = componentType;
    }

    /// <summary>
    /// Gets or sets component type.
    /// </summary>
    public Type ComponentType
    {
      get { return this._componentType; }
      set { this._componentType = value; }
    }
  }

  /// <summary>
  /// Sets a view field change handler.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
  [MeansImplicitUse]
  public class ChangeHandler : Attribute
  {
    private string _changeHandlerName;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    public ChangeHandler(string changeHandlerName)
    {
      this._changeHandlerName = changeHandlerName;
    }

    /// <summary>
    /// Gets or sets component type.
    /// </summary>
    public string ChangeHandlerName
    {
      get { return this._changeHandlerName; }
      set { this._changeHandlerName = value; }
    }
  }
}
