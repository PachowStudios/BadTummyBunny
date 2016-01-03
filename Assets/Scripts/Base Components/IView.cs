using System;
using UnityEngine;

namespace PachowStudios
{
  public interface IView : IDisposable
  {
    Transform Transform { get; }
    Collider2D Collider { get; }
    SpriteRenderer SpriteRenderer { get; }
    Animator Animator { get; }
    CharacterController2D CharacterController { get; }
  }

  public interface IView<out TModel> : IView
  {
    TModel Model { get; }
  }
}