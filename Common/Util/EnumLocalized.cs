using System;

namespace Common.Util
{
    public class EnumLocalized<T>
    {
        protected static System.Resources.ResourceManager resourceManager = null;
        protected static string resourcePrefix = "dummy_";

        protected static string _EnumToLocalizedName(T x)
        {
            string resourceName = resourcePrefix + Enum.GetName(typeof(T), x);
            try
            {
                string localizedName = resourceManager.GetString(resourceName);
                int dummy = localizedName.Length; // to provoke an exception, if localizedName is null
                return localizedName;
            }
            catch (Exception)
            {
                throw new Exception("Resource " + resourceName + " not found in EnumLocalized.EnumToLocalizedName");
            }
        }

        protected static T _LocalizedNameToEnum(string x)
        {
            for (int i = 0; i < Enum.GetValues(typeof(T)).Length; ++i)
            {
                string localizedName = _EnumToLocalizedName((T)(object)i);
                if (x.CompareTo(localizedName) == 0)
                    return (T)(object)i;
            }
            throw new Exception("Localized name " + x + " not found in EnumLocalized.localizedNameToEnum");
        }
    }
}