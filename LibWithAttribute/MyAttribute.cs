using System;

namespace LibWithAttribute
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class MyAttribute : Attribute { }
}
