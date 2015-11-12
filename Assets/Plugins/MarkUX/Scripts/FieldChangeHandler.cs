using System;
using System.Reflection;
using UnityEngine;

namespace MarkUX
{
  /// <summary>
  /// Contains information about a field change handler.
  /// </summary>
  [Serializable]
  public class FieldChangeHandler
  {
    public string FieldName;
    public string ChangeHandlerName;
    public GameObject Source;

    private View _sourceView;
    private MethodInfo _changeHandler;
    private FieldInfo _fieldInfo;
    private bool _initialized;

    /// <summary>
    /// Initalizes a new instance of the class.
    /// </summary>
    public FieldChangeHandler(string fieldName, string changeHandlerName)
    {
      FieldName = fieldName;
      ChangeHandlerName = changeHandlerName;
      _initialized = false;
    }

    /// <summary>
    /// Initalizes a new instance of the class.
    /// </summary>
    public FieldChangeHandler(string fieldName, string changeHandlerName, GameObject source)
    {
      FieldName = fieldName;
      ChangeHandlerName = changeHandlerName;
      Source = source;
      _initialized = false;
    }

    /// <summary>
    /// Initializes field change handler information.
    /// </summary>
    private void InitializeFieldChangeHandler()
    {
      _sourceView = Source.GetComponent<View>();
      _changeHandler = _sourceView.GetType().GetMethod(ChangeHandlerName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      _fieldInfo = _sourceView.GetType().GetField(FieldName);
      _initialized = true;
    }

    /// <summary>
    /// Gets or sets field info.
    /// </summary>
    public FieldInfo FieldInfo
    {
      get
      {
        if (!_initialized)
        {
          InitializeFieldChangeHandler();
        }
        return _fieldInfo;
      }
      set { _fieldInfo = value; }
    }

    /// <summary>
    /// Gets or sets change handler info.
    /// </summary>
    public MethodInfo ChangeHandler
    {
      get
      {
        if (!_initialized)
        {
          InitializeFieldChangeHandler();
        }
        return _changeHandler;
      }
      set { _changeHandler = value; }
    }
  }
}
