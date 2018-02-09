using System;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace TalkToMe.Core.Hook
{
    public interface IKeyMonitor
    {
        IObservable<KeyInfo> KeysObservable
        {
            get;
        }

        IDisposable Override(Func<KeyInfo, bool> onKey);
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class KeyInfo : IEquatable<KeyInfo>
    {
        public KeyInfo(Keys key, Keys modifier)
        {
            this.key = key;
            this.modifier = modifier;
        }

        public Keys Key => this.key;
        public Keys Modifier => this.modifier;

        public override int GetHashCode()
        {
            return this.Key.GetHashCode() ^ this.Modifier.GetHashCode();
        }

        public bool Equals(KeyInfo other)
        {
            return this.Key.Equals(other.Key) && this.Modifier.Equals(other.Modifier);
        }

        public override bool Equals(object obj)
        {
            return (obj is KeyInfo other)
                ? this.Equals(other)
                : false;
        }

        public static readonly KeyInfo Empty = new KeyInfo(Keys.None, Keys.None);

        [JsonProperty]
        private readonly Keys key;

        [JsonProperty]
        private readonly Keys modifier;
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class KeyInfoAndCommand
    {
        [JsonProperty]
        private readonly KeyInfo keyInfo;

        [JsonProperty]
        private readonly CommandType command;

        public KeyInfoAndCommand(KeyInfo keyInfo, CommandType command)
        {
            this.keyInfo = keyInfo;
            this.command = command;
        }

        public KeyInfo KeyInfo => this.keyInfo;
        public CommandType Command => this.command;
    }
}
