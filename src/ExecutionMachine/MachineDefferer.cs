using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExecutionMachine
{
	public class MachineDefferer : Abstractions.IMachineDefferer
	{
		private Task _t;
		private System.Threading.CancellationTokenSource _cts;

		public int DelayMilliseconds { get; set; } = 300;

		public Abstractions.IMachine Machine { get; set; }

		public void Input()
		{
			if (_cts != null)
				_cts.Cancel();

			_cts = new System.Threading.CancellationTokenSource();
			_t = Task.Factory.StartNew(() => StartDelay(_cts.Token), _cts.Token);
		}

		private void StartDelay(System.Threading.CancellationToken cancellationToken)
		{
			/*try
            {
                Task.Delay(this.DelayMilliseconds, cancellationToken)
                    .Wait();
            }
            catch (TaskCanceledException) { }
            catch (AggregateException) { }*/
			Task.Delay(this.DelayMilliseconds)
				  .Wait();

			if (cancellationToken.IsCancellationRequested)
				return;

			_t = null;

			this.Machine?.Run();
		}
	}

}
