using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

    public class PluginManager : IPluginManager
    {
        public IReadOnlyCollection<ITextFormatPlugin> Plugins { get; private set; }

        public bool Initialize(string path)
        {
            var result = false;
            try
            {
                var allPlugins = new List<ITextFormatPlugin>();
                foreach (var file in Directory.EnumerateFiles(path, "*.cs"))
                {
                    if (TryCompile(file, out var plugins))
                    {
                        allPlugins.AddRange(plugins);
                    }
                }

                this.Plugins = allPlugins;

                result = true;

            }
            catch (IOException)
            {
                // TODO: Trace
                result = false;
            }

            return result;
        }

        private static bool TryCompile(string path, out IReadOnlyCollection<ITextFormatPlugin> plugins)
        {
            var result = false;
            var output = new List<ITextFormatPlugin>();
            plugins = output;
            var source = File.ReadAllText(path);
            var syntaxTree = CSharpSyntaxTree.ParseText(source);
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var pluginSupport = MetadataReference.CreateFromFile(typeof(ITextFormatPlugin).Assembly.Location);
            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
            var name = Path.GetFileNameWithoutExtension(path);

            var compilation = CSharpCompilation.Create(
                name,
                new[] { syntaxTree },
                new[] { mscorlib, pluginSupport },
                options);

            using (var ms = new MemoryStream())
            {
                var emitResult = compilation.Emit(ms);
                if (emitResult.Success)
                {
                    var assembly = Assembly.Load(ms.ToArray());
                    var pluginTypes = assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(ITextFormatPlugin)));

                    foreach (var type in pluginTypes)
                    {
                        var instance = Activator.CreateInstance(type) as ITextFormatPlugin;
                        if (instance != null)
                        {
                            output.Add(instance);
                        }
                    }

                    result = output.Any();
                }
                else
                {
                    // TODO: Handle errors
                }
            }

            return result;
        }
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
