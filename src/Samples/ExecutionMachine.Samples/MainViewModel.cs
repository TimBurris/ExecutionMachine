using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExecutionMachine.Samples
{
	public class MainViewModel : NinjaMvvm.Wpf.WpfViewModelBase
	{
		private ExecutionMachine.Abstractions.IMachineDefferer _machineDeferrer;
		public MainViewModel()
		{
			this.BuildSearchMachine();
		}

		public ObservableCollection<string> LineItems { get; } = new ObservableCollection<string>();

		public string SearchText
		{
			get { return GetField<string>(); }
			set
			{
				//if text changes do a search
				if (SetField(value))
				{
					this.NotifyTyping();
					_machineDeferrer.Input();
				}
			}
		}

		private void BuildSearchMachine()
		{
			//obviously we should use an IoC and DI this thing
			_machineDeferrer = new ExecutionMachine.MachineDefferer();
			_machineDeferrer.DelayMilliseconds = 500;
			_machineDeferrer.Machine = new Machine();

			var searchMachine = _machineDeferrer.Machine;

			//task does nto need to be replaced, because the task will read from the NewProjectRoleUser property, which will always be accurate
			searchMachine.ReplacePendingTaskOnRun = false;

			searchMachine.PendingTaskExecutionDelay = 500;

			var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
			var dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;

			searchMachine.TaskProvider = () =>
			{
				//just call the method that already builds the filters and gets data from the server on a new task
				var loadItemsTask = new Task(() =>
				{
					dispatcher.Invoke(() => NotifyBeginningSearch());
					//wait 5 seconds, simulating a search that is taking a while to come back
					Task.Delay(5000).Wait();

				});

				loadItemsTask.ContinueWith((t) =>
				{
					this.LineItems.Add($"Results fetched {DateTime.Now.TimeOfDay}");

				}, scheduler: taskScheduler);

				return loadItemsTask;
			};
		}
		private void NotifyBeginningSearch()
		{
			this.LineItems.Add($"beginning a search that will take 5 seconds... {DateTime.Now.TimeOfDay}");
		}

		private void NotifyTyping()
		{
			//this.LineItems.Clear();
			this.LineItems.Add($"typing...  {DateTime.Now.TimeOfDay}");
		}
	}
}
