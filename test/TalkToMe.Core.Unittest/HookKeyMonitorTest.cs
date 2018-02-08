using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FluentAssertions;
using NSubstitute;
using TalkToMe.Core.Hook;
using Xunit;

namespace TalkToMe.Core.Unittest
{
    public class HookKeyMonitorTest
    {
        [Fact]
        public void ShouldInstallHookOnConstruction()
        {
            var hookProvider = Substitute.For<IHookProvider>();
            var modChecker = Substitute.For<IModifierStateChecker>();
            var mon = new HookKeyMonitor(Enumerable.Empty<KeyInfo>(), hookProvider, modChecker);
            hookProvider.Received().Install(Arg.Any<Func<KeyProcArgs, bool>>());
        }

        [Fact]
        public void ShouldUninstallHookOnDispose()
        {
            var hookProvider = Substitute.For<IHookProvider>();
            var modChecker = Substitute.For<IModifierStateChecker>();

            using (var mon = new HookKeyMonitor(Enumerable.Empty<KeyInfo>(), hookProvider, modChecker))
            {
            }

            hookProvider.Received().Install(Arg.Any<Func<KeyProcArgs, bool>>());
            hookProvider.Received().Uninstall();
        }

        [Theory]
        [InlineData(Keys.A, Keys.None)]
        [InlineData(Keys.Z, Keys.None)]
        public void ShouldDispatchSubscribedKeys(Keys key, Keys modifier)
        {
            Func<KeyProcArgs, bool> keyProc = null;
            KeyInfo receivedKey = null;
            var hookProvider = Substitute.For<IHookProvider>();
            hookProvider.Install(Arg.Do<Func<KeyProcArgs, bool>>(x => keyProc = x));
            var modChecker = Substitute.For<IModifierStateChecker>();

            var mon = new HookKeyMonitor(new List<KeyInfo> { new KeyInfo(key, Keys.None) }, hookProvider, modChecker);
            mon.KeysObservable.Subscribe(info => receivedKey = info);

            UseHookStructWith(key, ptr => keyProc(new KeyProcArgs(0, (IntPtr)NativeMethods.WM_KEYDOWN, ptr)));

            receivedKey.Should().Be(new KeyInfo(key, modifier));
        }

        [Fact]
        public void ShouldOverrideSubscribedKeys()
        {
            Func<KeyProcArgs, bool> keyProc = null;
            KeyInfo sentKey = new KeyInfo(Keys.A, Keys.LShiftKey);

            KeyInfo receivedKey = null;
            KeyInfo receivedFromSubscription = null;
            var hookProvider = Substitute.For<IHookProvider>();
            hookProvider.Install(Arg.Do<Func<KeyProcArgs, bool>>(x => keyProc = x));
            var modChecker = Substitute.For<IModifierStateChecker>();
            modChecker.GetModifierState(Arg.Is<Keys>(k => k == Keys.LShiftKey)).Returns(Keys.LShiftKey);

            var mon = new HookKeyMonitor(new List<KeyInfo> { new KeyInfo(Keys.A, Keys.RControlKey) }, hookProvider, modChecker);
            mon.KeysObservable.Subscribe(info => receivedFromSubscription = info);

            mon.Override(keyInfo =>
            {
                receivedKey = keyInfo;
                return true;
            });

            UseHookStructWith(sentKey.Key, ptr => keyProc(new KeyProcArgs(0, (IntPtr)NativeMethods.WM_KEYDOWN, ptr)));

            receivedKey.Should().Be(sentKey);
            receivedFromSubscription.Should().BeNull();
        }

        [Fact]
        public void ShouldCancelOverrideOnTokenDispose()
        {
            Func<KeyProcArgs, bool> keyProc = null;
            KeyInfo sentKey = new KeyInfo(Keys.A, Keys.LShiftKey);

            var receivedKeys = new List<KeyInfo>();
            KeyInfo receivedFromSubscription = null;
            var hookProvider = Substitute.For<IHookProvider>();
            hookProvider.Install(Arg.Do<Func<KeyProcArgs, bool>>(x => keyProc = x));
            var modChecker = Substitute.For<IModifierStateChecker>();
            modChecker.GetModifierState(Arg.Is<Keys>(k => k == Keys.LShiftKey)).Returns(Keys.LShiftKey);

            var mon = new HookKeyMonitor(new List<KeyInfo> { new KeyInfo(Keys.A, Keys.RControlKey) }, hookProvider, modChecker);
            mon.KeysObservable.Subscribe(info => receivedFromSubscription = info);

            var token = mon.Override(keyInfo =>
            {
                receivedKeys.Add(keyInfo);
                return true;
            });

            UseHookStructWith(sentKey.Key, ptr => keyProc(new KeyProcArgs(0, (IntPtr)NativeMethods.WM_KEYDOWN, ptr)));
            UseHookStructWith(sentKey.Key, ptr => keyProc(new KeyProcArgs(0, (IntPtr)NativeMethods.WM_KEYDOWN, ptr)));

            token.Dispose();
            UseHookStructWith(sentKey.Key, ptr => keyProc(new KeyProcArgs(0, (IntPtr)NativeMethods.WM_KEYDOWN, ptr)));

            receivedKeys.Count.Should().Be(2);
            receivedFromSubscription.Should().BeNull();
        }

        private static void UseHookStructWith(Keys key, Action<IntPtr> hookStructPtrAction)
        {
            var hookStruct = new NativeMethods.KBDLLHOOKSTRUCT
            {
                vkCode = (uint)key
            };

            var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(hookStruct));
            Marshal.StructureToPtr(hookStruct, ptr, false);
            try
            {
                hookStructPtrAction(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}
