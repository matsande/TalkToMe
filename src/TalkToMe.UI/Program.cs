﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleInjector;
using TalkToMe.Core;
using TalkToMe.UI.View;
using TalkToMe.UI.ViewModel;

namespace TalkToMe.UI
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            var container = Bootstrap();
            RunApplication(container);
        }

        private static Container Bootstrap()
        {
            var container = new Container();

            var configPersistence = new LocalConfigPersistence();
            if (!configPersistence.TryLoad(out var config))
            {
                config = new Config(
                    true,
                    new Dictionary<KeyInfo, CommandType>
                    {
                        { new KeyInfo(Keys.A, Keys.LWin), CommandType.ToggleAutoMode },
                        { new KeyInfo(Keys.T, Keys.LWin), CommandType.Speak },
                        { new KeyInfo(Keys.M, Keys.LWin), CommandType.ToggleMute }
                    },
                    string.Empty,
                    string.Empty);
            }

            container.Register<IClipboardTextMonitor, ClipboardTextMonitor>(Lifestyle.Singleton);
            container.Register<IHookProvider, StaticHookProvider>(Lifestyle.Singleton);
            container.Register<IKeyMonitor, HookKeyMonitor>(Lifestyle.Singleton);

            container.RegisterSingleton<IConfigPersistence>(configPersistence);

            // Note: temporary, refactor HookKeyMonitor to be constructed in another way
            container.RegisterSingleton<IEnumerable<KeyInfo>>(config.Hotkeys.Keys);

            // Note: temporary, refactor SpeechManager to only use IConfigPersistence or something aggregating that interface.
            container.RegisterSingleton<Config>(config);

            container.Register<ISpeechManager, SpeechManager>(Lifestyle.Singleton);
            container.Register<ISpeech, SpeechSynth>(Lifestyle.Singleton);
            container.Register<MainWindow>();

#if DEBUG
            container.Verify();
#endif

            return container;
        }

        private static void RunApplication(Container container)
        {
            try
            {
                var app = new App();
                app.SetContainer(container);
                var mainWindow = container.GetInstance<MainWindow>();
                app.Run(mainWindow);
            }
            catch (Exception)
            {
                // TODO: Trace
            }
        }
    }
}