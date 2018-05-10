using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkToMe.Core.Plugin
{
    public interface IPluginManager
    {
        IReadOnlyCollection<ITextFormatPlugin> Plugins { get; }
    }

    public class SamplePluginManager : IPluginManager
    {
        private readonly List<ITextFormatPlugin> plugins;

        public SamplePluginManager()
        {
            this.plugins = new List<ITextFormatPlugin>
            {
                new SamplePlugin()
            };
        }

        public IReadOnlyCollection<ITextFormatPlugin> Plugins => this.plugins;
    }

    public interface ITextFormatPlugin
    {
        string Name { get; }
        string Description { get; }
        string FormatText(string source);
    }

    public class SamplePlugin : ITextFormatPlugin
    {
        public string Name => "Sample plugin";
        public string Description => "Just a sample";

        public string FormatText(string source)
        {
            return "Sample: " + source;
        }
    }
}
