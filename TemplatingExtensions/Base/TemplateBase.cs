using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Tridion.ContentManager;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.Publishing;
using Tridion.ContentManager.Publishing.Rendering;
using Tridion.ContentManager.Templating;
using Tridion.ContentManager.Templating.Assembly;

namespace TemplatingExtensions.Base
{
    /// <summary>
    /// This class contains useful helper functions for typical things you might want to do in a
    /// template. It can be used as the base class for your own Template Building Blocks.
    ///  Use this abstract class to derive your new Template from.
    /// </summary>
    /// <author>
    ///    SDL Tridion Professional Services
    /// </author>
    /// <date>
    ///     created 31-January-2008
    ///		updated: April-2012 
    /// </date>	
    public abstract class TemplateBase : ITemplate, IDisposable
    {
        #region Private Members

        protected Engine _engine;
        protected Package _package;
        protected int _renderContext = -1;

        private bool _disposed; //Indicates whether system resources used by this instance have been released
        private TemplatingLogger _logger;
        private XmlNamespaceManager _NSM;

        #endregion

        #region Properties

        /// <summary>
        /// An XmlNameSpaceManager already initialized with several XML namespaces such like: tcm, xlink and xhtml
        /// </summary>
        protected XmlNamespaceManager NSManager
        {
            get
            {
                if (_NSM == null)
                {
                    _NSM = new XmlNamespaceManager(new NameTable());

                    _NSM.AddNamespace("tcm", "http://www.tridion.com/ContentManager/5.0");
                    _NSM.AddNamespace("xlink", "http://www.w3.org/1999/xlink");
                    _NSM.AddNamespace("xhtml", "http://www.w3.org/1999/xhtml");
                }

                return _NSM;
            }
        }

        protected TemplatingLogger Logger
        {
            get { return _logger ?? (_logger = TemplatingLogger.GetLogger(GetType())); }
        }
        #endregion

        #region Internal Methods

        /// <summary>
        /// Initializes the engine and package to use in this TemplateBase object.
        /// </summary>
        /// <param name="engine">The engine to use in calls to the other methods of this TemplateBase object</param>
        /// <param name="package">The package to use in calls to the other methods of this TemplateBase object</param>
        protected void Initialize(Engine engine, Package package)
        {
            _engine = engine;
            _package = package;
        }

        /// <summary>
        /// Checks whether the TemplateBase object has been initialized correctly.
        /// This method should be called from any method that requires the <c>m_Engine</c>, 
        /// <c>m_Package</c> or <c>_log</c> member fields.
        /// </summary>
        protected void CheckInitialized()
        {
            if (_engine == null || _package == null)
            {
                throw new InvalidOperationException("This method can not be invoked, unless Initialize has been called");
            }
        }

        #endregion

        #region Base Functionality


        protected bool IsCurrentRenderMode(string renderMode)
        {
            RenderMode mode;
            return Enum.TryParse(renderMode.Trim(), true, out mode) && IsCurrentRenderMode(mode);
        }

        protected bool IsCurrentRenderMode(RenderMode renderMode)
        {
            return (_engine.RenderMode == renderMode) ? true : false;
        }

        /// <summary>
        /// Checks whether a Target Type URI is associated with the current publication target being published to
        /// </summary>
        protected bool IsTargetTypeInPublicationContext(string targetTypeUri)
        {
            CheckInitialized();
            return IsTargetTypeInPublicationContext(new[] { targetTypeUri });
        }

        /// <summary>
        /// Checks whether at least one of a list of Target Type URIs is associated with the current publication target being published to
        /// </summary>
        protected bool IsTargetTypeInPublicationContext(IEnumerable<string> targetTypeUris)
        {
            CheckInitialized();

            return _engine.PublishingContext.PublicationTarget != null 
                && targetTypeUris.Any(uri => _engine.PublishingContext.PublicationTarget.TargetTypes.Any(tt => tt.Id == uri));
        }

        /// <summary>
        /// Checks whether there is an item in the package of type tridion/page
        /// </summary>
        /// <returns>True if there is a page item in the package</returns>
        protected bool IsPage()
        {
            Item page = _package.GetByType(ContentType.Page);

            return (page != null);
        }

        /// <summary>
        /// Checks whether there is an item in the package of type tridion/component
        /// </summary>
        /// <returns>True if there is a component item in the package</returns>
        protected bool IsComponent()
        {
            Item component = _package.GetByType(ContentType.Component);

            return (component != null);
        }

        /// <summary>
        /// True if the rendering context is a page, rather than component
        /// </summary>
        protected bool IsPageTemplate
        {
            get
            {
                if (_renderContext == -1)
                {
                    _renderContext = _engine.PublishingContext.ResolvedItem.Item is Page ? 1 : 0;
                }
                return _renderContext == 1;
            }
        }

        /// <summary>
        /// Returns the component object that is defined in the package for this template.
        /// </summary>
        /// <remarks>
        /// This method should only be called when there is an actual Component item in the package. 
        /// It does not currently handle the situation where no such item is available.
        /// </remarks>
        /// <returns>the component object that is defined in the package for this template.</returns>
        protected Component GetComponent()
        {
            CheckInitialized();

            Item component = _package.GetByName("Component");

            if (component == null)
            {
                throw new Exception("There is no Component item in the Package.");
            }

            return (Component)_engine.GetObject(component.GetAsSource().GetValue("ID"));
        }

        /// <summary>
        /// Returns the Template from the resolved item if it's a Component Template
        /// </summary>
        /// <returns>A Component Template or null</returns>
        protected ComponentTemplate GetComponentTemplate()
        {
            CheckInitialized();

            var template = _engine.PublishingContext.ResolvedItem.Template;
            return (template is ComponentTemplate) ? template as ComponentTemplate : null;
        }

        /// <summary>
        /// Returns the Template from the resolved item if it's a Page Template
        /// </summary>
        /// <returns>A Page Template or null</returns>
        protected PageTemplate GetPageTemplate()
        {
            CheckInitialized();
            Template template = _engine.PublishingContext.ResolvedItem.Template;

            return (template is PageTemplate) ? template as PageTemplate : null;
        }

        /// <summary>
        /// Returns the page object that is defined in the package for this template.
        /// </summary>
        /// <remarks>
        /// This method should only be called when there is an actual Page item in the package. 
        /// It does not currently handle the situation where no such item is available.
        /// </remarks>
        /// <returns>the page object that is defined in the package for this template.</returns>
        protected Page GetPage()
        {
            CheckInitialized();
            //first try to get from the render context
            RenderContext renderContext = _engine.PublishingContext.RenderContext;
            if (renderContext != null)
            {
                Page contextPage = renderContext.ContextItem as Page;
                if (contextPage != null)
                    return contextPage;
            }
            Item pageItem = _package.GetByType(ContentType.Page);
            if (pageItem != null)
                return (Page)_engine.GetObject(pageItem.GetAsSource().GetValue("ID"));
           
            return null;
        }


        /// <summary>
        /// Returns the publication object that can be determined from the package for this template.
        /// </summary>
        /// <remarks>
        /// This method currently depends on a Page item being available in the package, meaning that
        /// it will only work when invoked from a Page Template.
        /// </remarks>
        /// <returns>the Publication object that can be determined from the package for this template.</returns>
        protected Publication GetPublication()
        {
            CheckInitialized();

            Repository repository = null;

            RepositoryLocalObject pubItem = (_package.GetByType(ContentType.Page) != null)
                ?  (RepositoryLocalObject) GetPage() : GetComponent();

            if (pubItem != null) repository = pubItem.ContextRepository;

            return repository as Publication;
        }

        /// <summary>
        /// Returns the component of the first embedded component presentation
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public Component GetFirstComponent(Page page)
        {
            Component component = null;
            if (page.ComponentPresentations.Count > 0)
            {
                component = page.ComponentPresentations[0].Component;
            }
            return component;
        }

        /// <summary>
        /// Add an Item object to the ContextVariables based on key (variableName)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="variableName"></param>
        protected void AddContextVariableItem(string variableName, Item item)
        {
            if (_engine.PublishingContext.RenderContext != null && !_engine.PublishingContext.RenderContext.ContextVariables.Contains(variableName))
            {                
                _engine.PublishingContext.RenderContext.ContextVariables.Add(variableName, item);                
            }
        }

        /// <summary>
        /// Push a context variable item to the package
        /// </summary>
        /// <param name="variableName"></param>
        protected void PushFromContextVariableItem(string variableName)
        {
            if (_engine.PublishingContext.RenderContext != null && _engine.PublishingContext.RenderContext.ContextVariables.Contains(variableName))
            {
                Item item = _engine.PublishingContext.RenderContext.ContextVariables[variableName] as Item;
                if (item != null)
                {
                    _package.PushItem(variableName, item);
                }
            }
        }

        /// <summary>
        /// Remove ContextVariable based on key (variableName)
        /// </summary>
        /// <param name="variableName"></param>
        protected void RemoveContextVariable(string variableName)
        {
            if (_engine.PublishingContext.RenderContext != null && !_engine.PublishingContext.RenderContext.ContextVariables.Contains(variableName))
            {
                _engine.PublishingContext.RenderContext.ContextVariables.Remove(variableName);                
            }
        }


        public bool IsContextVariableSet(string variableName)
        {
            return _engine.PublishingContext.RenderContext != null &&
                _engine.PublishingContext.RenderContext.ContextVariables.Contains(variableName);
        }

        /// <summary>
        /// Get ContextVariable based on key (variableName)
        /// </summary>
        /// <param name="variableName"></param>
        public object GetContextVariable(string variableName)
        {
            object value = null;

            if (IsContextVariableSet(variableName))
            {
                value = _engine.PublishingContext.RenderContext.ContextVariables[variableName];
            }

            return value;
        }

        /// <summary>
        /// Push a dictionary of package items into the package, and  store them in the 
        /// Publishing Context Variables, so they can be reused across multiple template
        /// executions in a publish transaction.
        /// This acts as a caching mechanism, so that global items like config components
        /// need only be loaded once within a publish transaction. Items can be retrieved 
        /// from the context variable and pushed into the package using the PushFromContextVariable
        /// method
        /// CAUTION: Only global package items should be pushed, not items that will differ
        /// from component presentation to component presentation, or page to page
        /// </summary>
        /// <param name="items">The Dictionary of package items to push (the key for each dictionary item will be used as the name of the item in the package)</param>
        /// <param name="variableName">The context variable name to store these items in</param>
        protected void PushAndAddToContextVariables(Dictionary<string, Item> items, string variableName)
        {
            if (items != null)
            {
                foreach (string key in items.Keys)
                {
                    _package.PushItem(key, items[key]);
                }
            }
            if (_engine.PublishingContext.RenderContext != null && !_engine.PublishingContext.RenderContext.ContextVariables.Contains(variableName))
            {
                _engine.PublishingContext.RenderContext.ContextVariables.Add(variableName, items);
            }
        }
        
        /// <summary>
        /// Load package items from the publishing context variables and push them into the package.
        /// The context variables are used as a cache, so they can be populated (using PushAndAddToContextVariables)
        /// in the first template execution in a publish transaction, and reused in all others
        /// </summary>
        /// <param name="variableName">The context variable name which contains the items to push</param>
        /// <returns>false if the context variable is empty, true otherwise</returns>
        protected bool PushFromContextVariable(string variableName)
        {
            if (_engine.PublishingContext.RenderContext != null && _engine.PublishingContext.RenderContext.ContextVariables.Contains(variableName))
            {
                Dictionary<string, Item> items = _engine.PublishingContext.RenderContext.ContextVariables[variableName] as Dictionary<string, Item>;
                if (items != null)
                {
                    foreach (string key in items.Keys)
                    {
                        _package.PushItem(key, items[key]);
                    }
                }
                return true;
            }
            return false;
        }

        public void AddBinariesFromFolder(string rootSGWebDavUrl, Folder folder, string path)
        {

        }

              /// <summary>
        /// Add a binary to the package and ensure it is published into the given structure group
        /// </summary>
        /// <param name="mmComp">The binary to add</param>
        protected Item AddBinary(Component mmComp)
        {
            return null;
        }

        /// <summary>
        /// Add a binary to the package and ensure it is published into the given structure group
        /// </summary>
        /// <param name="mmComp">The binary to add</param>
        /// <param name="structureGroupUri">The target SG Uri</param>
        protected Item AddBinary(Component mmComp, TcmUri structureGroupUri)
        {
            return null;
        }

        /// <summary>
        /// Add a binary to the package and ensure it is published into the given structure group
        /// </summary>
        /// <param name="mmComp">The binary to add</param>
        /// <param name="sg">The target SG</param>
        protected Item AddBinary(Component mmComp, StructureGroup sg)
        {
            return null;
        }

        public string AddBinaryWithUniqueFilename(Component comp)
        {
            return null;
        }


        #endregion


        #region ITemplate Members

        public virtual void Transform(Engine engine, Package package) { }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Indicates whether system resources used by this instance have been released
        /// </summary>
        protected bool Disposed
        {
            get
            {
                lock (this)
                {
                    return (_disposed);
                }
            }
        }

        /// <summary>
        /// Releases allocated resources
        /// </summary>
        void IDisposable.Dispose()
        {
            lock (this)
            {
                if (_disposed) return;
                _package = null;
                _engine = null;
                _logger = null;
                _NSM = null;

                _disposed = true;
                // take yourself off the finalization queue
                // to prevent finalization from executing a second time
                GC.SuppressFinalize(this);
            }
        }
        #endregion
    }
}
