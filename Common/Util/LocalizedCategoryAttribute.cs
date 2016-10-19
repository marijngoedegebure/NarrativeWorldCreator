using System;
using System.ComponentModel;

namespace Common.Util
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class LocalizedCategoryAttribute : CategoryAttribute
    {
        protected System.Resources.ResourceManager resourceManager = null;

        public LocalizedCategoryAttribute(string value) : base(value) { }

        protected override string GetLocalizedString(string value)
        {
            try
            {
                string localizedName = resourceManager.GetString(value);
                int dummy = localizedName.Length; // to provoke an exception, if localizedName is null
                return localizedName;
            }
            catch (Exception)
            {
                throw new Exception("Resource " + value + " not found in LocalizedCategoryAttribute");
            }
        }
    }
}
