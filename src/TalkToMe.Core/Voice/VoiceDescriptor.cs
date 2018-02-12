using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace TalkToMe.Core.Voice
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    [DebuggerDisplay("Provider = {Provider}, Name = {VoiceName}")]
    public class VoiceDescriptor : IEquatable<VoiceDescriptor>
    {
        public VoiceDescriptor(VoiceProvider provider, string voiceName)
        {
            this.provider = provider;
            this.voiceName = voiceName;
        }

        public static VoiceDescriptor Empty => empty;
        public VoiceProvider Provider => this.provider;

        public string VoiceName => this.voiceName;

        public override bool Equals(object obj)
        {
            return obj is VoiceDescriptor vd
                ? this.Equals(vd)
                : false;
        }

        public bool Equals(VoiceDescriptor other)
        {
            return this.provider == other.provider && this.voiceName == other.voiceName;
        }

        public override int GetHashCode()
        {
            var hashCode = 847152782;
            hashCode = hashCode * -1521134295 + this.provider.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.voiceName);
            return hashCode;
        }

        private static readonly VoiceDescriptor empty = new VoiceDescriptor(VoiceProvider.None, string.Empty);

        [JsonProperty]
        private readonly VoiceProvider provider;

        [JsonProperty]
        private readonly string voiceName;
    }
}