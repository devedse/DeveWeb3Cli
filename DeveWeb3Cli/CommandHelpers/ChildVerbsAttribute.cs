namespace DeveWeb3Cli.CommandHelpers
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ChildVerbsAttribute : Attribute
    {
        public Type[] Types { get; }

        public ChildVerbsAttribute(params Type[] types)
        {
            Types = types;
        }
    }
}
