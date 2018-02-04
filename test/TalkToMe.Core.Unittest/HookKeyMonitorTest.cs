using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace TalkToMe.Core.Unittest
{
    public class HookKeyMonitorTest
    {
        [Fact]
        public void ShouldInstallHookOnConstruction()
        {
            var hookProvider = Substitute.For<IHookProvider>();
            var mon = new HookKeyMonitor(Enumerable.Empty<KeyInfo>(), hookProvider);
            hookProvider.Received().Install(Arg.Any<Func<KeyProcArgs, bool>>());
        }

        [Fact]
        public void ShouldUninstallHookOnDispose()
        {
            var hookProvider = Substitute.For<IHookProvider>();

            using (var mon = new HookKeyMonitor(Enumerable.Empty<KeyInfo>(), hookProvider))
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

            var mon = new HookKeyMonitor(new List<KeyInfo>{new KeyInfo(key, Keys.None)}, hookProvider);
            mon.KeysObservable.Subscribe(info => receivedKey = info);

            UseHookStructWith(key, ptr => keyProc(new KeyProcArgs(0, (IntPtr)NativeMethods.WM_KEYDOWN, ptr)));

            receivedKey.Should().Be(new KeyInfo(key, modifier));
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
