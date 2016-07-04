//-----------------------------------------------------------------------
// <copyright file="ViewEngineDoodlRenderer.cs" company="EmptyAudio">
//     Copyright EmptyAudio (c) 2016
// </copyright>
//-----------------------------------------------------------------------

namespace Doodl.Service.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewEngines;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.AspNetCore.Routing;
    using ViewModel;

    /// <summary>
    /// Renders doodl web pages using the current MVC view engine.
    /// </summary>
    public class ViewEngineDoodlRenderer : IDoodlRenderer
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ICompositeViewEngine viewEngine;
        private readonly ITempDataProvider tempDataProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewEngineDoodlRenderer"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider to use.</param>
        /// <param name="viewEngine">The view engine to use.</param>
        /// <param name="tempDataProvider">The temp data provider to use.</param>
        public ViewEngineDoodlRenderer(
            IServiceProvider serviceProvider,
            ICompositeViewEngine viewEngine,
            ITempDataProvider tempDataProvider)
        {
            this.serviceProvider = serviceProvider;
            this.viewEngine = viewEngine;
            this.tempDataProvider = tempDataProvider;
        }

        /// <summary>
        /// Renders a doodl's display page.
        /// </summary>
        /// <param name="doodl">The doodl to render.</param>
        /// <param name="original">The doodl this was based on.</param>
        /// <param name="edits">The edits based on this doodl.</param>
        /// <param name="cancellationToken">The cancellation token to use.</param>
        /// <returns>The rendered page.</returns>
        public async Task<string> RenderDoodlDisplayAsync(Doodl doodl, Doodl original, IEnumerable<Doodl> edits, CancellationToken cancellationToken)
        {
            var model = new DoodlDisplayModel()
            {
                Doodl = doodl,
                Original = original,
                Edits = edits.ToList(),
            };

            return await this.RenderView("DoodlDisplay", model);
        }

        private async Task<string> RenderView<TModel>(string viewName, TModel model)
        {
            var httpContext = new DefaultHttpContext()
            {
                RequestServices = this.serviceProvider
            };

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var viewResult = this.viewEngine.FindView(actionContext, viewName, true);

            if (viewResult.Success)
            {
                using (var writer = new StringWriter())
                {
                    var viewDataDictionary = new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                    {
                        Model = model
                    };

                    var viewContext = new ViewContext(
                        actionContext,
                        viewResult.View,
                        viewDataDictionary,
                        new TempDataDictionary(actionContext.HttpContext, this.tempDataProvider),
                        writer,
                        new HtmlHelperOptions());

                    await viewResult.View.RenderAsync(viewContext);

                    return writer.ToString();
                }
            }
            else
            {
                throw new Exception(string.Format("View {0} not found!", viewName));
            }
        }
    }
}
