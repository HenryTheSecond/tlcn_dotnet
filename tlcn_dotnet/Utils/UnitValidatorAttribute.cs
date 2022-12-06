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
                this.ErrorMessage = "Unit is invalid";
                return false;
            }
            if (unit == ProductUnit.WEIGHT)
                return true;
            var quantityProperty = type.GetProperty("Quantity");
            if (quantityProperty != null && (double)quantityProperty.GetValue(value) - Convert.ToInt32(quantityProperty.GetValue(value)) != 0)
            {
                this.ErrorMessage = "Quantity and Unit is not valid";
                return false;
            }
            var minPurchaseProperty = type.GetProperty("MinPurchase");
            if (minPurchaseProperty != null && (double)minPurchaseProperty.GetValue(value) - Convert.ToInt32(minPurchaseProperty.GetValue(value)) != 0)
            {
                this.ErrorMessage = "Min purchase and Unit is not valid";
                return false;
            }
            return true;
        }
    }
}
