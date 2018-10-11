﻿using System;
using MLS.Protocol.Execution;

namespace MLS.Protocol
{
    public class WorkspaceRequest
    {
        public Workspace Workspace { get; }

        public HttpRequest HttpRequest { get; }

        public BufferId ActiveBufferId { get; }

        public WorkspaceRequest(
            Workspace workspace,
            BufferId activeBufferId = null,
            HttpRequest httpRequest = null,
            int? position = null)
        {
            Workspace = workspace ?? throw new ArgumentNullException(nameof(workspace));

            HttpRequest = httpRequest;

            if (activeBufferId != null)
            {
                ActiveBufferId = activeBufferId;
            }
            else if (workspace.Buffers.Length == 1)
            {
                ActiveBufferId = workspace.Buffers[0].Id;
            }

            if (position != null)
            {
                var buffer = Workspace.GetBufferWithSpecifiedIdOrSingleBufferIfThereIsOnlyOne(ActiveBufferId);
                buffer.Position = position.Value;
            }
        }
    }
}