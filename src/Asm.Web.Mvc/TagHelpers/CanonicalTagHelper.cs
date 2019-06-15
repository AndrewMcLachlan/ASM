using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Asm.Web.Mvc.TagHelpers
{
    [HtmlTargetElement("canonical", Attributes = "path")]
    public class CanonicalTagHelper : TagHelper
    {
        [HtmlAttributeName("path")]
        public string Path
        {
            get;
            set;
        }

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string path = context.AllAttributes["path"].Value as string;

            string href = $"{ViewContext.HttpContext.Request.Scheme}://{ViewContext.HttpContext.Request.Host}/{path}";

            output.TagName = "link";

            output.Attributes.Add("rel", "canonical");
            output.Attributes.Add("href", href);
        }
    }
}
