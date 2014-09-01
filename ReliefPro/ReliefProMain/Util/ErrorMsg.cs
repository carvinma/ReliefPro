using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace ReliefProMain.Util
{
    public class RequiredAttribute : System.ComponentModel.DataAnnotations.RequiredAttribute
    {
        private string displayName;

        public RequiredAttribute()
        {
        }

        protected override ValidationResult IsValid
        (object value, ValidationContext validationContext)
        {
            displayName = validationContext.DisplayName;
            return base.IsValid(value, validationContext);
        }

        public override string FormatErrorMessage(string name)
        {
            return name + "" + LanguageHelper.GetValueByKey(this.ErrorMessage);
        }
    }
    public class RangeAttribute : System.ComponentModel.DataAnnotations.RangeAttribute
    {
        public RangeAttribute(int minimum, int maximum)
            : base(minimum, maximum)
        {
        }

        public RangeAttribute(double minimum, double maximum)
            : base(minimum, maximum)
        {
        }

        public RangeAttribute(Type type, string minimum, string maximum)
            : base(type, minimum, maximum)
        {
        }

        public override string FormatErrorMessage(string name)
        {
            return LanguageHelper.GetValueByKey(this.ErrorMessage);
        }
    }

    public class RegularExpression : System.ComponentModel.DataAnnotations.RegularExpressionAttribute
    {
        public RegularExpression(string pattern)
            : base(pattern)
        { }
        public override string FormatErrorMessage(string name)
        {
            return LanguageHelper.GetValueByKey(this.ErrorMessage);
        }
    }
}
