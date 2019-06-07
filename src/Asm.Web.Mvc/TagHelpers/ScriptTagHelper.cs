using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Asm.AmCom.Web.TagHelpers
{
    public class ScriptTagHelper : IntegrityTagHelper
    {
        private string _urlAttributeName = "src";
        private bool _emitMinifiedUrls = false;

        public ScriptTagHelper(IActionContextAccessor actionContextAccessor, IUrlHelperFactory urlHelperFactory, IHostingEnvironment hostingEnvironment, IMemoryCache memoryCache, IConfiguration configuration) : base(actionContextAccessor, urlHelperFactory, hostingEnvironment, memoryCache)
        {
             Boolean.TryParse(configuration["EmitMinifiedUrls"] ?? "false", out _emitMinifiedUrls);
        }

        protected override string UrlSourceAttributeName => _urlAttributeName;

        protected override string UrlOutputAttributeName => "src";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context.AllAttributes["integrity"] != null) return;

            _urlAttributeName = _emitMinifiedUrls && context.AllAttributes["src-min"] != null ? "src-min" : "src";

            base.Process(context, output);

            output.Attributes.RemoveAll("src-min");
        }
    }
}
