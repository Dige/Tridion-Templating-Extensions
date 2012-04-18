using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.ContentManagement.Fields;

namespace TemplatingExtensions.ExtensionMethods.Tridion
{
	public static class ComponentExtensions
	{
        public static ItemFields GetItemFields(this Component component)
        {
            return new ItemFields(component.Content, component.Schema);
        }

        public static ItemFields GetMetadataItemFields(this RepositoryLocalObject component)
        {
            return
                null != component.Metadata
                    ? new ItemFields(component.Metadata, component.MetadataSchema)
                    : null != component.MetadataSchema
                        ? new ItemFields(component.MetadataSchema)
                        : default(ItemFields);
        }
	}
}