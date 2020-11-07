using System;

namespace ModuleAPI
{
    /// <summary>
    /// This signals Ash that this method is included in the Module.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class IncludedInModule : Attribute
    {
        public IncludedInModule() {}
    }
}
