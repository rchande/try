using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Markdig;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Syntax;
using Microsoft.DotNet.Try.Markdown;
using MLS.Agent.CommandLine;
using Wyam.Common.Caching;
using Wyam.Common.Documents;
using Wyam.Common.Execution;
using Wyam.Common.Meta;
using Wyam.Common.Modules;
using Wyam.Common.Tracing;

namespace MLS.Agent.Wyam.Markdown
{
    /// <summary>
    /// Parses markdown content and renders it to HTML.
    /// </summary>
    /// <remarks>
    /// Parses markdown content in each input document and outputs documents with rendered HTML content.
    /// Note that <c>@</c> (at) symbols will be automatically HTML escaped for better compatibility with downstream
    /// Razor modules. If you want to include a raw <c>@</c> symbol when <c>EscapeAt()</c> is <c>true</c>, use
    /// <c>\@</c>. Use the <c>EscapeAt()</c> fluent method to modify this behavior.
    /// </remarks>
    /// <category>Templates</category>
    public class Markdown : IModule
    {
        /// <summary>
        /// The default Markdown configuration.
        /// </summary>
        public const string DefaultConfiguration = "common";

        private static readonly Regex EscapeAtRegex = new Regex("(?<!\\\\)@");

        private readonly string _sourceKey;
        private readonly string _destinationKey;
        private readonly OrderedList<IMarkdownExtension> _extensions = new OrderedList<IMarkdownExtension>();
        private string _configuration = DefaultConfiguration;
        private bool _escapeAt = true;
        private bool _prependLinkRoot = false;

        /// <summary>
        /// Processes Markdown in the content of the document.
        /// </summary>
        public Markdown()
        {
        }


        /// <summary>
        /// Includes a set of useful advanced extensions, e.g., citations, footers, footnotes, math,
        /// grid-tables, pipe-tables, and tasks, in the Markdown processing pipeline.
        /// </summary>
        /// <returns>The current module instance.</returns>
        public Markdown UseExtensions()
        {
            _configuration = "advanced";
            return this;
        }

        /// <summary>
        /// Includes a set of extensions defined as a string, e.g., "pipetables", "citations",
        /// "mathematics", or "abbreviations". Separate different extensions with a '+'.
        /// </summary>
        /// <param name="extensions">The extensions string.</param>
        /// <returns>The current module instance.</returns>
        public Markdown UseConfiguration(string extensions)
        {
            _configuration = extensions;
            return this;
        }


        /// <summary>
        /// Specifies if the <see cref="Keys.LinkRoot"/> setting must be used to rewrite root-relative links when rendering markdown.
        /// By default, root-relative links, which are links starting with a '/' are left untouched.
        /// When setting this value to <c>true</c>, the <see cref="Keys.LinkRoot"/> setting value is added before the link.
        /// </summary>
        /// <param name="prependLinkRoot">If set to <c>true</c>, the <see cref="Keys.LinkRoot"/> setting value is added before any root-relative link (eg. stating with a '/').</param>
        /// <returns>The current module instance.</returns>
        public Markdown PrependLinkRoot(bool prependLinkRoot = false)
        {
            _prependLinkRoot = prependLinkRoot;
            return this;
        }

        /// <inheritdoc />
        public IEnumerable<IDocument> Execute(IReadOnlyList<IDocument> inputs, IExecutionContext context)
        {
            return inputs.AsParallel().Select(context, input =>
            {
                Trace.Verbose(
                    "Processing Markdown {0} for {1}",
                    string.IsNullOrEmpty(_sourceKey) ? string.Empty : ("in" + _sourceKey),
                    input.SourceString());

                string result;

                IExecutionCache executionCache = context.ExecutionCache;

                if (!executionCache.TryGetValue<string>(input, _sourceKey, out result))
                {
                    string content;
                    if (string.IsNullOrEmpty(_sourceKey))
                    {
                        content = input.Content;
                    }
                    else if (input.ContainsKey(_sourceKey))
                    {
                        content = input.String(_sourceKey) ?? string.Empty;
                    }
                    else
                    {
                        // Don't do anything if the key doesn't exist
                        return input;
                    }

                    MarkdownPipeline pipeline = CreatePipeline(input);

                    using (StringWriter writer = new StringWriter())
                    {
                        HtmlRenderer htmlRenderer = new HtmlRenderer(writer);
                        pipeline.Setup(htmlRenderer);

                        if (_prependLinkRoot && context.Settings.ContainsKey(Keys.LinkRoot))
                        {
                            htmlRenderer.LinkRewriter = (link) =>
                            {
                                if (link == null || link.Length == 0)
                                {
                                    return link;
                                }

                                if (link[0] == '/')
                                {
                                    // root-based url, must be rewritten by prepending the LinkRoot setting value
                                    // ex: '/virtual/directory' + '/relative/abs/link.html' => '/virtual/directory/relative/abs/link.html'
                                    link = context.Settings[Keys.LinkRoot] + link;
                                }

                                return link;
                            };
                        }

                        MarkdownDocument document = MarkdownParser.Parse(content, pipeline);
                        htmlRenderer.Render(document);
                        writer.Flush();
                        result = writer.ToString();
                    }

                    if (_escapeAt)
                    {
                        result = EscapeAtRegex.Replace(result, "&#64;");
                        result = result.Replace("\\@", "@");
                    }

                    executionCache.Set(input, _sourceKey, result);
                }

                return string.IsNullOrEmpty(_sourceKey)
                    ? context.GetDocument(input, context.GetContentStream(result))
                    : context.GetDocument(input, new MetadataItems
                    {
                        { string.IsNullOrEmpty(_destinationKey) ? _sourceKey : _destinationKey, result }
                    });
            });
        }

        private MarkdownPipeline CreatePipeline(IDocument document)
        {
            var commandlineMetadata2 = document.GetMetadata("commandline").Values.FirstOrDefault()?.ToString();
            var extension = new CodeBlockAnnotationExtension(new CodeFenceAnnotationsParser(StartupOptions.FromCommandLine(commandlineMetadata2 ?? "")));
            MarkdownPipelineBuilder pipelineBuilder = new MarkdownPipelineBuilder();
            pipelineBuilder.Configure(_configuration);
            pipelineBuilder.Extensions.Add(extension);
            return pipelineBuilder.Build();
        }
    }
}