using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExecutionMachine
{
	public class Machine : Abstractions.IMachine
	{
		public Machine()
		{
			this.Scheduler = System.Threading.Tasks.TaskScheduler.Default;
		}

		public Func<Task> TaskProvider { get; set; }
		public System.Threading.Tasks.TaskScheduler Scheduler { get; set; }

		public bool ReplacePendingTaskOnRun { get; set; }

		public int PendingTaskExecutionDelay { get; set; }

		public bool HasPendingTask { get { return (_pendingtask != null); } }

		private Task _executingTask;
		private Task _pendingtask;

		public virtual void Run()
		{
			//if the refresh task is not yet set, then set it.  else, if there is not already a pending refresh make it the pending refresh
			if (_executingTask == null)
			{
				_executingTask = this.BuildTask();
				this.StartExecutingTask();
			}
			else if (_pendingtask == null)
			{
				_pendingtask = this.BuildTask();
			}
			else
			{
				//there is already a task running, and another pending, so this one can be discarded 
				if (this.ReplacePendingTaskOnRun)
				{
					_pendingtask = this.BuildTask();
				}
			}
		}
		private Task BuildTask()
		{
			var t = TaskProvider.Invoke();
			return this.BuildTask(t);
		}
		private Task BuildTask(Task t)
		{
			t.ContinueWith((task) => this.TaskComplete(), scheduler: System.Threading.Tasks.TaskScheduler.Default);
			return t;
		}
		private void StartExecutingTask()
		{
			if (this.Scheduler == null)
			{
				_executingTask.Start();
			}
			else
			{
				_executingTask.Start(scheduler: this.Scheduler);
			}
		}

		private void TaskComplete()
		{
			//now that the task is complete we check to see if there is already another pending.  
			if (_pendingtask == null)
			{
				//there is no pending task, so simply clear the executing task so that it's available to be run again
				_executingTask = null;
			}
			else
			{
				//there is already a pending task so make the pending task the executing. clear pending and kick it off executing
				_executingTask = _pendingtask;
				_pendingtask = null;
				System.Threading.Thread.Sleep(this.PendingTaskExecutionDelay);

				this.StartExecutingTask();
			}
		}
	}

}
