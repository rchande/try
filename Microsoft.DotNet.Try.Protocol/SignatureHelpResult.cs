﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.DotNet.Try.Protocol
{
    public class SignatureHelpResult
    {
        private IEnumerable<SignatureHelpItem> signatures ;

        public IEnumerable<SignatureHelpItem> Signatures
        {
            get => signatures ?? (signatures = Array.Empty<SignatureHelpItem>());
            set => signatures  = value;
        }

        public int ActiveSignature { get; set; }

        public int ActiveParameter { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string RequestId { get; set; }
        public IEnumerable<SerializableDiagnostic> Diagnostics { get; set; }


        public SignatureHelpResult(IEnumerable<SignatureHelpItem> signatures = null, IEnumerable<SerializableDiagnostic> diagnostics = null, string requestId = null)
        {
            RequestId = requestId;
            Signatures = signatures;
            Diagnostics = diagnostics;
        }
    }
}