namespace LogRedactorSharp.Core.Attributes;

[AttributeUsage(AttributeTargets.Enum | 
    AttributeTargets.Property | 
    AttributeTargets.Field, 
    AllowMultiple = false)]
public class RedactAttribute : Attribute
{
}
