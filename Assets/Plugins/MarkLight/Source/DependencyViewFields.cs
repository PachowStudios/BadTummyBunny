using System;
using UnityEngine;

namespace MarkLight
{
  [Serializable]
  public class _float : ViewField<float>
  {
    public _float() { }

    public _float(float value)
      : base(value) { }
  }

  [Serializable]
  public class _string : ViewField<string>
  {
    public _string() { }

    public _string(string value)
      : base(value) { }
  }

  [Serializable]
  public class _int : ViewField<int>
  {
    public _int() { }

    public _int(int value)
      : base(value) { }
  }

  [Serializable]
  public class _bool : ViewField<bool>
  {
    public _bool() { }

    public _bool(bool value)
      : base(value) { }
  }

  [Serializable]
  public class _char : ViewField<char>
  {
    public _char() { }

    public _char(char value)
      : base(value) { }
  }

  [Serializable]
  public class _Color : ViewField<Color>
  {
    public _Color() { }

    public _Color(Color value)
      : base(value) { }
  }

  [Serializable]
  public class _ElementSize : ViewField<ElementSize> { }

  [Serializable]
  public class _Font : ViewField<Font> { }

  [Serializable]
  public class _ElementMargin : ViewField<ElementMargin> { }

  [Serializable]
  public class _Material : ViewField<Material> { }

  [Serializable]
  public class _Quaternion : ViewField<Quaternion> { }

  [Serializable]
  public class _Sprite : ViewField<Sprite> { }

  [Serializable]
  public class _Vector2 : ViewField<Vector2> { }

  [Serializable]
  public class _Vector3 : ViewField<Vector3> { }

  [Serializable]
  public class _Vector4 : ViewField<Vector4> { }

  [Serializable]
  public class _ElementAlignment : ViewField<ElementAlignment> { }

  [Serializable]
  public class _ElementOrientation : ViewField<ElementOrientation> { }

  [Serializable]
  public class _AdjustToText : ViewField<AdjustToText> { }

  [Serializable]
  public class _FontStyle : ViewField<FontStyle> { }

  [Serializable]
  public class _HorizontalWrapMode : ViewField<HorizontalWrapMode> { }

  [Serializable]
  public class _VerticalWrapMode : ViewField<VerticalWrapMode> { }

  [Serializable]
  public class _FillMethod : ViewField<UnityEngine.UI.Image.FillMethod> { }

  [Serializable]
  public class _ImageType : ViewField<UnityEngine.UI.Image.Type> { }

  [Serializable]
  public class _ElementSortDirection : ViewField<ElementSortDirection> { }

  [Serializable]
  public class _ImageFillMethod : ViewField<UnityEngine.UI.Image.FillMethod> { }

  [Serializable]
  public class _InputFieldCharacterValidation : ViewField<UnityEngine.UI.InputField.CharacterValidation> { }

  [Serializable]
  public class _InputFieldContentType : ViewField<UnityEngine.UI.InputField.ContentType> { }

  [Serializable]
  public class _InputFieldInputType : ViewField<UnityEngine.UI.InputField.InputType> { }

  [Serializable]
  public class _TouchScreenKeyboardType : ViewField<UnityEngine.TouchScreenKeyboardType> { }

  [Serializable]
  public class _InputFieldLineType : ViewField<UnityEngine.UI.InputField.LineType> { }

  [Serializable]
  public class _ScrollbarDirection : ViewField<UnityEngine.UI.Scrollbar.Direction> { }

#if !UNITY_4_6 && !UNITY_5_0 && !UNITY_5_1
  [Serializable]
  public class _ScrollbarVisibility : ViewField<UnityEngine.UI.ScrollRect.ScrollbarVisibility> { }
#endif

  [Serializable]
  public class _ScrollRectMovementType : ViewField<UnityEngine.UI.ScrollRect.MovementType> { }

  [Serializable]
  public class _RectTransformComponent : ViewField<RectTransform> { }

  [Serializable]
  public class _ScrollbarComponent : ViewField<UnityEngine.UI.Scrollbar> { }

  [Serializable]
  public class _GraphicRenderMode : ViewField<UnityEngine.RenderMode> { }

  [Serializable]
  public class _CameraComponent : ViewField<UnityEngine.Camera> { }

  [Serializable]
  public class _CanvasScaleMode : ViewField<UnityEngine.UI.CanvasScaler.ScaleMode> { }

  [Serializable]
  public class _BlockingObjects : ViewField<UnityEngine.UI.GraphicRaycaster.BlockingObjects> { }

  [Serializable]
  public class _GameObject : ViewField<UnityEngine.GameObject> { }

  [Serializable]
  public class _HideFlags : ViewField<UnityEngine.HideFlags> { }

  [Serializable]
  public class _OverflowMode : ViewField<OverflowMode> { }

  [Serializable]
  public class _RaycastBlockMode : ViewField<RaycastBlockMode> { }

  [Serializable]
  public class _Mesh : ViewField<Mesh> { }

  [Serializable]
  public class _object : ViewField<object> { }

  [Serializable]
  public class _IObservableList : ViewField<IObservableList> { }

  [Serializable]
  public class _GenericObservableList : ViewField<GenericObservableList> { }
}
