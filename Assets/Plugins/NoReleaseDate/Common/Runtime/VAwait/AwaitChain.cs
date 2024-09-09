using System.Threading;

namespace NoReleaseDate.Common.Runtime.VAwait
{
    public class AwaitChain
    {
        public CancellationTokenSource cts { get; set; }
        public bool completed { get; set; }
    }
}