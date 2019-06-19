// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.DotNet.Try.Markdown;
using Microsoft.EntityFrameworkCore.Metadata;
using Wyam.Common.Configuration;
using Wyam.Common.Documents;
using Wyam.Common.Execution;
using Wyam.Common.Modules;
using Wyam.Core.Modules.Control;
using Wyam.Core.Modules.Extensibility;
using Wyam.Core.Modules.IO;
using Wyam.Core.Modules.Metadata;
using Wyam.Yaml;

namespace MLS.Agent.CommandLine
{
    internal class PublishCommand
    {
        internal static async Task Do(VerifyOptions options, IConsole console, StartupOptions startupOptions)
        {
            var engine = new Wyam.Core.Execution.Engine();
            engine.FileSystem.InputPaths.Add(options.Dir.FullName);
            engine.FileSystem.RootPath = options.Dir.FullName;
            Console.WriteLine("stuff");

            engine.Pipelines.Add("CopyFiles",
                  new ReadFiles("**/*")
                    .Where(x => x.Path.Extension != ".md"),
                  new WriteFiles()
                );

            string commandlineMetadata = null;
            engine.Pipelines.Add("Markdown",
                new ReadFiles("**/*.md"),
                new FrontMatter('-', new Yaml()),
                new Trace((doc, ctx) => {
                    Console.WriteLine(doc.Values);
                    //var commandlineMetadata2 = doc.GetMetadata("commandline").Values.FirstOrDefault()?.ToString();
                    //var extension = new CodeBlockAnnotationExtension(new CodeFenceAnnotationsParser(StartupOptions.FromCommandLine(commandlineMetadata ?? "")));
                    //var markdown = new Wyam.Markdown.Markdown().UseExtensions().UseExtension(extension);
                    //var it = markdown.Execute(new List<IDocument>() { doc }, ctx);
                    //return it;
                    return doc;
                }
                ),
                new MLS.Agent.Wyam.Markdown.Markdown(),
                new Meta("WriteExtension", (doc) => ".html"),
                new WriteFiles());

            engine.Execute();
        }


    }
}