using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Markdig.Parsers;
using Microsoft.DotNet.Try.Markdown;
using Wyam.Common.Execution;
using Wyam.Core.Modules.IO;
using Wyam.Core.Modules.Metadata;
using Xunit;

namespace MLS.Agent.Tests.Markdown
{
    public class PublishTests
    {
        [Fact]
        public async Task It_publishes_markdown()
        {
//            var engine = new Wyam.Core.Execution.Engine();

//            var p = new Pipeline();
//            engine.Pipelines.Add("Markdown",
//                new ReadApplicationInput(),
//                new Meta("WritePath", "Output.html"),
//                new Wyam.Markdown.Markdown().UseExtensions().UseExtension(new CodeBlockAnnotationExtension()),
//                new WriteFiles());

//            string ex = @"
//## heading
//### heading 2

//```csharp --region foo
//Here's some text
//```";
//            engine.ApplicationInput = ex;
//            engine.Execute();
//            foreach (var document in engine.Documents.FromPipeline("Markdown"))
//            {
//                var content = document.Content;
//            }
        }
    }
}
