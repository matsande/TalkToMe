using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using TalkToMe.Core;
using TalkToMe.Core.Hook;
using TalkToMe.UI.Common;

namespace TalkToMe.UI.ViewModel
{
    public class HotkeyViewModel : ViewModelBase
    {
        public HotkeyViewModel(ISpeechManager speechmanager, IKeyMonitor keyMonitor)
        {
            this.KeyMonitor = keyMonitor;
            this.speechManager = speechmanager;
            this.speechManager.StateChangeObservable.Subscribe(_ =>
            {
                this.SupportedCommands = CreateSupportedCommands(this.speechManager.Config);
                this.RaisePropertyChanged(nameof(this.SupportedCommands));
            });

            // TODO: Do we need to add all commands but with Keys.None to the config, or should we store a default list somewhere?
            this.SupportedCommands = CreateSupportedCommands(this.speechManager.Config);
            this.ApplyCommand = new DelegateCommand(this.OnApply, this.CanApply);
            this.ClearBindingCommand = new DelegateCommand<HotkeyItemViewModel>(this.OnClearBinding);
        }

        public IKeyMonitor KeyMonitor
        {
            get;
        }

        public IReadOnlyCollection<HotkeyItemViewModel> SupportedCommands
        {
            get;
            private set;
        }

        public DelegateCommand ApplyCommand
        {
            get;
        }

        public DelegateCommand<HotkeyItemViewModel> ClearBindingCommand
        {
            get;
        }

        private void OnClearBinding(HotkeyItemViewModel vm)
        {
            vm.Key = KeyInfo.Empty;
        }

        private void OnApply()
        {
            this.speechManager.UpdateConfig(this.speechManager.Config.With(hotKeys: this.SupportedCommands.ToDictionary(x => x.Key, x => x.Command)));
        }

        private bool CanApply()
        {
            var assignedKeys = this.SupportedCommands
                .Where(k => k.Key != KeyInfo.Empty)
                .ToArray();

            bool AllKeysDifferent(IEnumerable<HotkeyItemViewModel> keys)
            {
                var hash = new HashSet<KeyInfo>();
                foreach (var key in assignedKeys)
                {
                    if (!hash.Add(key.Key))
                    {
                        return false;
                    }
                }

                return true;
            }

            bool result;
            if (assignedKeys.Any())
            {
                result = AllKeysDifferent(assignedKeys);
            }
            else
            {
                result = true;
            }

            return result;
        }

        private IReadOnlyCollection<HotkeyItemViewModel> CreateSupportedCommands(Config config)
        {
            var commandViewModels = config.Hotkeys.Select(kvp => new HotkeyItemViewModel(kvp.Key, kvp.Value)).ToList();
            var subscriptions = commandViewModels
                .Select(vm => Observable.FromEventPattern(vm, nameof(vm.PropertyChanged))
                .Subscribe(_ => this.OnKeyChanged()));

            this.keyChangedSubscriptions?.Dispose();
            this.keyChangedSubscriptions = new CompositeDisposable(subscriptions);

            return commandViewModels;
        }

        private void OnKeyChanged()
        {
            this.ApplyCommand.RaiseCanExecuteChanged();
        }

        private readonly ISpeechManager speechManager;
        private IDisposable keyChangedSubscriptions;
    }
}