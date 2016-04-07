using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Extensions;
using System.Xml.Serialization;
using JetBrains.Annotations;
using PachowStudios.Assertions;

namespace PachowStudios
{
  [Serializable]
  [TypeConverter(typeof(VersionCodeConverter))]
  public class VersionCode : ICloneable, IComparable<VersionCode>, IEquatable<VersionCode>
  {
    [XmlIgnore]
    public int Major { get; private set; }
    [XmlIgnore]
    public int Minor { get; private set; }
    [XmlIgnore]
    public int Build { get; private set; } = -1;
    [XmlIgnore]
    public int Revision { get; private set; } = -1;

    [XmlAttribute]
    public string Value
    {
      get { return ToString(); }
      private set { Parse(value); }
    }

    public VersionCode() { }

    public VersionCode(int major, int minor, int build = 0, int revision = 0)
    {
      major.Should().BeAtLeast(0);
      minor.Should().BeAtLeast(0);
      build.Should().BeAtLeast(0);
      revision.Should().BeAtLeast(0);

      Major = major;
      Minor = minor;
      Build = build;
      Revision = revision;
    }

    public VersionCode([NotNull] string version)
    {
      Parse(version);
    }

    public object Clone()
      => new VersionCode(Major, Minor, Build, Revision);

    public int CompareTo(VersionCode version) 
      => Major != version.Major ? Major.CompareTo(version.Major)
           : Minor != version.Minor ? Minor.CompareTo(version.Minor)
               : Build != version.Build ? Build.CompareTo(version.Build)
                   : Revision.CompareTo(version.Revision);

    public override bool Equals(object obj)
      => obj is VersionCode && Equals((VersionCode)obj);

    public bool Equals(VersionCode version)
      => Major == version.Major
         && Minor == version.Minor
         && Build == version.Build
         && Revision == version.Revision;

    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
    public override int GetHashCode()
    {
      var hash = 0;

      hash |= (Major & 15) << 0x1c;
      hash |= (Minor & 0xff) << 20;
      hash |= (Build & 0xff) << 12;
      hash |= (Revision & 0xfff);

      return hash;
    }

    public static bool operator ==(VersionCode v1, VersionCode v2)
      => ReferenceEquals(v1, null) ? ReferenceEquals(v2, null) : v1.Equals(v2);

    public static bool operator !=(VersionCode v1, VersionCode v2)
      => !(v1 == v2);

    public static bool operator >(VersionCode v1, VersionCode v2)
      => v2 < v1;

    public static bool operator >=(VersionCode v1, VersionCode v2)
      => v2 <= v1;

    public static bool operator <(VersionCode v1, VersionCode v2)
      => !ReferenceEquals(v1, null) && v1.CompareTo(v2) < 0;

    public static bool operator <=(VersionCode v1, VersionCode v2)
      => !ReferenceEquals(v1, null) && v1.CompareTo(v2) <= 0;

    public override string ToString()
      => Build < 0 ? $"{Major}.{Minor}"
        : Revision < 0 ? $"{Major}.{Minor}.{Build}"
          : $"{Major}.{Minor}.{Build}.{Revision}";

    private void Parse(string version)
    {
      if (version == null)
        throw new ArgumentNullException(nameof(version));

      var components = new Stack<string>(version.Split('.'));

      components.Should()
        .HaveAtLeast(2, "because a major and minor version is required")
        .And.HaveAtMost(4, "because there are only 4 possible version components");

      Major = int.Parse(components.Pop(), CultureInfo.InvariantCulture);
      Major.Should().BeAtLeast(0);

      Minor = int.Parse(components.Pop(), CultureInfo.InvariantCulture);
      Minor.Should().BeAtLeast(0);

      if (components.IsEmpty())
        return;

      Build = int.Parse(components.Pop(), CultureInfo.InvariantCulture);
      Build.Should().BeAtLeast(0);

      if (components.IsEmpty())
        return;

      Revision = int.Parse(components.Pop(), CultureInfo.InvariantCulture);
      Revision.Should().BeAtLeast(0);
    }
  }

  public class VersionCodeConverter : TypeConverter
  {
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
      => sourceType == typeof(string)
      || base.CanConvertFrom(context, sourceType);

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      var version = value as string;

      return version != null
        ? new VersionCode(version)
        : base.ConvertFrom(context, culture, value);
    }
  }
}