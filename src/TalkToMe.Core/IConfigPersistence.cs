using System;
using System.IO;
using System.Reflection;

namespace TalkToMe.Core
{
    /// <summary>
    /// Defines the <see cref="IConfigPersistence" />
    /// </summary>
    public interface IConfigPersistence
    {
        /// <summary>
        /// The Save
        /// </summary>
        /// <param name="config">The <see cref="Config"/></param>
        void Save(Config config);

        bool TryLoad(out Config config);
    }

    public class LocalConfigPersistence : IConfigPersistence
    {
        private readonly string configPath;

        public LocalConfigPersistence()
        {
            var localPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            this.configPath = Path.Combine(localPath, "talktome.json");
        }

        public bool TryLoad(out Config config)
        {
            config = null;
            try
            {
                using (var stream = File.OpenRead(this.configPath))
                {
                    config = Config.Deserialize(stream);
                }
            }
            catch (Exception)
            {
                // TODO: Trace
            }

            return config != null;
        }

        public void Save(Config config)
        {
            using (var stream = File.OpenWrite(this.configPath))
            {
                stream.SetLength(0);
                config.Serialize(stream);
            }
        }
    }
}
