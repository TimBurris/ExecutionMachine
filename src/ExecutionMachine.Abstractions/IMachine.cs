using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExecutionMachine.Abstractions
{
	/// <summary>
	/// This machine is built to allow the caller to say what he wants to do (TaskProvider property) and then simply call Run over and over.
	/// The Machine will use an executing and a pending task to ensure that the task is only run again if there is not another one queue up to run.
	/// 
	/// This is for when you need to execute the EXACT same thing over and over (like refreshing some count).  If the refresh takes a few hundred milliseconds, the need to refresh could be call 100 times in that time frame.  they don't ALL need to refresh
	/// Example:
	/// A call to run is made, while the executing task is running, 30 more calls to run are made.
	/// once the first run is complete, it only need to run once more, not 29 more... because they are all doing the exact same thing
	/// </summary>
	/// <remarks></remarks>
	public interface IMachine
	{
		/// <summary>
		/// specifies whether or not there is currently a task pending execution 
		/// </summary>
		/// <remarks>can be used to help performance by not doing any work on your end if you know there's already a task that will run
		/// Of course that would only make sense if ReplacePendingTaskOnRun is false
		/// </remarks>
		bool HasPendingTask { get; }

		/// <summary>
		/// the amount of time (milliseconds) to delay between tasks (gives breathing room for the UI)
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks>Default is 300 milliseconds</remarks>
		int PendingTaskExecutionDelay { get; set; }

		/// <summary>
		/// used to specifiy whether or not the existing pending task should be replaced when Run is called and there is already a pending task
		///    This would be useful if the Lambda represented by TaskProvider has changed 
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks>Default is false</remarks>
		bool ReplacePendingTaskOnRun { get; set; }

		TaskScheduler Scheduler { get; set; }
		Func<Task> TaskProvider { get; set; }

		void Run();
	}
}
