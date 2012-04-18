using Tridion.ContentManager.Templating;

namespace TemplatingExtensions.ExtensionMethods.Tridion
{
    public static class PackageExtensionMethods
    {
        public static Item PushStringItem(this Package package, string name, string value)
        {
            return package.PushStringItem(name, ContentType.Text, value);
        }

        public static Item PushStringItem(this Package package, string name, ContentType contentType, string value)
        {
            var item = package.CreateStringItem(contentType, value);
            package.PushItem(name, item);

            return item;
        }
        
        public static void RepushItem(this Package package, string name)
        {
            var item = package.GetByName(name);

            if (item != null)
            {
                package.Remove(item);
                package.PushItem(name, item);
            }
        }

        /// <summary>
        /// Returns the string value for an item in the package with the specified name
        /// </summary>
        /// <param name="package">TOM.Net Package</param>
        /// <param name="itemName">the name of the item to get the value from the package for</param>
        /// <returns>The string value of the item in the package or empty string if the item cannot be found</returns>
        /// <remarks>Same functionalty as Package.GetValue() but instead of null returns empty string if not found</remarks>
        public static string GetStringItemValue(this Package package, string itemName)
        {
            return package.GetStringItemValue(itemName, false);
        }

        /// <summary>
        /// Returns the string value for an item in the package with the specified name
        /// </summary>
        /// <param name="package">TOM.Net Package</param>
        /// <param name="itemName">the name of the item to get the value from the package for</param>
        /// <param name="dblCheck">whether to perfrom a second lookup using the value returned from the package</param>
        /// <returns>The string value of the item in the package or empty string if the item cannot be found</returns>
        /// <remarks>Same functionalty as Package.GetValue() but instead of null returns empty string if not found</remarks>
        public static string GetStringItemValue(this Package package, string itemName, bool dblCheck)
        {
            string res = package.GetValue(itemName);

            if (dblCheck && !string.IsNullOrEmpty(res))
            {
                res = package.GetValue(res);
            }

            return res ?? string.Empty;
        }
    }
}