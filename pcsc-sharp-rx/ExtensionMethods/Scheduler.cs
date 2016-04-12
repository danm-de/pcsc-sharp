using System.Reactive.Concurrency;
using System.Threading;

namespace PCSC.Reactive
{
    static internal class Scheduler
    {
        public static IScheduler ForCurrentContext() {
            var current = SynchronizationContext.Current;
            return current != null
                ? (IScheduler)new SynchronizationContextScheduler(current, false)
                : ImmediateScheduler.Instance;
        }
    }
}
