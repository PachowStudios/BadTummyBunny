using System;

namespace PachowStudios.BadTummyBunny
{
  [Flags]
  public enum CameraAxis
  {
    Horizontal = 1 << 0,
    Vertical   = 1 << 1
  }
}
