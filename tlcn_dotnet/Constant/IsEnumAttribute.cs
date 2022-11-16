using System.ComponentModel.DataAnnotations;

namespace tlcn_dotnet.Constant
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class IsEnumAttribute: ValidationAttribute
    {
        public Type EnumType { get; set; }

        public override bool IsValid(object? value)
        {
            try
            {
                Enum.Parse(EnumType, value.ToString());
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
    }
}
