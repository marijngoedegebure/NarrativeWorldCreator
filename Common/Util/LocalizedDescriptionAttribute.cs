using System;
using System.ComponentModel;

namespace Common.Util
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        protected System.Resources.ResourceManager resourceManager = null;
        private bool localized;

        public LocalizedDescriptionAttribute(string value) : base(value)
        {
            localized = false;
        }

        public override string Description
        {
            get
            {
                if (!localized)
                {
                    string value = this.DescriptionValue;
                    try
                    {
                        string localizedName = resourceManager.GetString(value);
                        int dummy = localizedName.Length; // to provoke an exception, if localizedName is null
                        this.DescriptionValue = localizedName;
                    }
                    catch (Exception)
                    {
                        throw new Exception("Resource " + value + " not found in LocalizedDescriptionAttribute");
                    }
                }
                return base.Description;
            }
        }
    }
}
