using System.Collections.Generic;
using Tridion.ContentManager;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.ContentManagement;

namespace TemplatingExtensions.ExtensionMethods.Tridion
{
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Returns the root structure group for the specified publication
        /// </summary>		
        /// <returns>The Root Structure Group in the publication</returns>
        /// <remarks>copied and modified code from Repository.RootFolder :)</remarks>
        public static StructureGroup GetRootSG(this Repository publication)
        {
            RepositoryItemsFilter filter = new RepositoryItemsFilter(publication.Session)
            {
                ItemTypes = new[] { ItemType.StructureGroup }
            };

            List<RepositoryLocalObject> items = (List<RepositoryLocalObject>)publication.GetItems(filter);

            if (items.Count == 0)
                return null;

            return (StructureGroup)items[0];
        }

    }
}
