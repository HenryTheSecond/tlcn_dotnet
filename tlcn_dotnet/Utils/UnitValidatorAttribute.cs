using System.ComponentModel.DataAnnotations;
using tlcn_dotnet.Constant;

namespace tlcn_dotnet.Utils
{
    [AttributeUsage(AttributeTargets.All)]
    public class UnitValidatorAttribute: ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            Type type = value.GetType();
            var unitProperty = type.GetProperty("Unit");
            if (unitProperty == null)
                return true;
            ProductUnit? unit = null;
            if (unitProperty.PropertyType == typeof(ProductUnit))
                unit = (ProductUnit)unitProperty.GetValue(value);
            else if (unitProperty.PropertyType == typeof(string))
                unit = Enum.Parse<ProductUnit>((string)unitProperty.GetValue(value));
            if (unit == null)
            {
                this.ErrorMessage = "UNIT IS INVALID";
                return false;
            }
            var quantityProperty = type.GetProperty("Quantity");
            if (unit == ProductUnit.UNIT 
                && quantityProperty != null 
                && (double)quantityProperty.GetValue(value) - Convert.ToInt32(quantityProperty.GetValue(value)) != 0)
            {
                this.ErrorMessage = "QUANTITY AND UNIT IS NOT VALID";
                return false;
            }
            var minPurchaseProperty = type.GetProperty("MinPurchase");
            if (unit == ProductUnit.UNIT 
                && minPurchaseProperty != null 
                && (double)minPurchaseProperty.GetValue(value) - Convert.ToInt32(minPurchaseProperty.GetValue(value)) != 0)
            {
                this.ErrorMessage = "MIN PURCHASE AND UNIT IS NOT VALID";
                return false;
            }
            return true;
        }
    }
}
