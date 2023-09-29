using ExecutionMachine.Abstractions;
using System.Threading.Tasks;

namespace ExecutionMachine
{
    public class MachineDefferer : Abstractions.IMachineDefferer
    {
        private Task _t;
        private System.Threading.CancellationTokenSource _cts;

        public int DelayMilliseconds { get; set; } = 300;

        public IMachine Machine { get; set; }

        public void Input()
        {
            if (_cts != null)
                _cts.Cancel();

            _cts = new System.Threading.CancellationTokenSource();
            _t = StartDelayAsync(_cts.Token);
        }

        //i've switched to Async await because I am having issues with the old school task pattern when used in blazor wasm
        private async Task StartDelayAsync(System.Threading.CancellationToken cancellationToken)
        {
            await Task.Delay(this.DelayMilliseconds, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            _t = null;

            this.Machine?.Run();
        }
    }

}
