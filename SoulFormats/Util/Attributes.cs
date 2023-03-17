using System;

namespace SoulsFormats
{
    [System.AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class HideProperty : Attribute
    {
    }

    [System.AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RotationRadians : Attribute
    {
    }

    [System.AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RotationXZY : Attribute
    {
    }
}
