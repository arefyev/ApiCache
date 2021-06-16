using System;

namespace Sample8.Common
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EnumMetadata : Attribute
    {
        public EnumMetadata()
        {
            this.Value = "text/plain";
            this.IsText = true;
        }

        public string Value { get; set; }
        public bool IsText { get; set; }
        public bool IsBinary
        {
            get
            {
                return !this.IsText;
            }
            set
            {
                this.IsText = !value;
            }
        }
    }
}
