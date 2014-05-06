﻿using System.Collections.Specialized;
using Sitecore.Data.Items;
using Sitecore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Mapper.Sc.Web.Mvc
{
    public class GlassController : SitecoreController
    {

        public ISitecoreContext SitecoreContext { get; set; }
        public IGlassHtml GlassHtml { get; set; }
         
        public GlassController()
        {
            try
            {
                SitecoreContext = new SitecoreContext(Sitecore.Mvc.Presentation.RenderingContext.Current.ContextItem.Database);
                GlassHtml = new GlassHtml(SitecoreContext);
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("Failed to create SitecoreContext", ex, this);
            }
        }

        protected GlassController(ISitecoreContext sitecoreContext, IGlassHtml glassHtml)
        {
            SitecoreContext = sitecoreContext;
            GlassHtml = glassHtml;
        }

        public virtual T GetRenderingParameters<T>() where T:class
        {
            var parameters = new NameValueCollection();
            foreach (var pair in Sitecore.Mvc.Presentation.RenderingContext.CurrentOrNull.Rendering.Parameters)
            {
                parameters[pair.Key] = pair.Value;
            }
            return
                GlassHtml.GetRenderingParameters<T>(parameters);
        }

        public virtual T GetControllerItem<T>(bool isLazy = false, bool inferType = false) where T : class
        {
            try
            {
                //The call to Sitecore.Mvc.Presentation.RenderingContext.Current cause the exception
                //      attempt to retrieve context object of type 'Sitecore.Mvc.Presentation.RenderingContext' from empty stack.
                //when used in a controller that is targeted by a route and not a rendering.

                if (Sitecore.Mvc.Presentation.RenderingContext.Current == null ||
                    Sitecore.Mvc.Presentation.RenderingContext.Current.Rendering == null ||
                    Sitecore.Mvc.Presentation.RenderingContext.Current.Rendering.DataSource.IsNullOrEmpty())
                    return SitecoreContext.GetCurrentItem<T>();



                return SitecoreContext.GetItem<T>(
                    Sitecore.Mvc.Presentation.RenderingContext.Current.Rendering.DataSource, isLazy, inferType
                    );
            }
            catch (InvalidOperationException ex)
            {
                return SitecoreContext.GetCurrentItem<T>();

            }

        }
    }
}
