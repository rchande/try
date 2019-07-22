﻿// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.CommandLine;
using System.CommandLine.Invocation;
using Microsoft.DotNet.Interactive;
using Microsoft.DotNet.Interactive.Events;
using WorkspaceServer.PackageRestore;

namespace WorkspaceServer.Kernel
{
    public static class CSharpKernelExtensions
    {
        private static PackageRestoreContext _restoreContext = new PackageRestoreContext();

        public static CSharpKernel UseNugetDirective(this CSharpKernel kernel)
        {
            var packageRefArg = new Argument<NugetPackageReference>((SymbolResult result, out NugetPackageReference reference) =>
                                                                        NugetPackageReference.TryParse(result.Token.Value, out reference))
            {
                Name = "package"
            };

            var r = new Command("#r")
            {
                packageRefArg
            };

            r.Handler = CommandHandler.Create<NugetPackageReference, KernelPipelineContext>(async (package, pipelineContext) =>
            {
                pipelineContext.OnExecute(async invocationContext =>
                {
                    var refs = await _restoreContext.AddPackage(package.PackageName, package.PackageVersion);
                    if (refs != null)
                    {
                        kernel.AddMetatadaReferences(refs);
                    }

                    invocationContext.OnNext(new NuGetPackageAdded(package));
                    invocationContext.OnCompleted();
                });
            });

            kernel.AddDirective(r);

            return kernel;
        }
    }
}