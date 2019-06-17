// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.CommandLine;
using System.Threading.Tasks;
using Microsoft.DotNet.Try.Markdown;
using Octokit;
using Wyam.Common.Configuration;
using Wyam.Common.Execution;
using Wyam.Core.Modules.IO;

namespace MLS.Agent.CommandLine
{
    internal class PublishCommand
    {
        internal static async Task Do(VerifyOptions options, IConsole console, StartupOptions startupOptions)
        {
            var engine = new Wyam.Core.Execution.Engine();


            engine.Pipelines.Add("Markdown",
                new ReadFiles(options.Dir.FullName + "*.md"),
                new Wyam.Markdown.Markdown().UseExtensions().UseExtension(new CodeBlockAnnotationExtension()),
                new WriteFiles());

            engine.Execute();
        }
    }
}