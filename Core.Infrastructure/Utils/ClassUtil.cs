using System;
namespace Core.Infrastructure.Utils
{
    public static class ClassUtil
    {
        //public static bool HasProperty(this object obj, string propertyName)
        //{
        //	return obj.GetType().GetProperty(propertyName) != null;
        //}

        public static bool HasProperty(this object obj, string propertyName)
        {
            if (!string.IsNullOrEmpty(propertyName))
            {
                string[] arr = propertyName.Split('.');
                return obj.GetType().GetProperty(arr[0]) != null;
            }

            return obj.GetType().GetProperty(propertyName) != null;
        }
    }
}
