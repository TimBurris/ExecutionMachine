namespace ExecutionMachine.Abstractions
{

	/// <summary>
	/// Used to hold off the execution machine until all activity stops.
	/// A good example is type to search where you do not want to start searching until typing completely stops.  The normal machine would execute after X delay, even if the user is still typing
	/// </summary>
	public interface IMachineDefferer
	{

		/// <summary>
		/// The amout of time, in milliseconds, that must ellapse between <see cref="Input"/> before executing the <see cref="Machine"/>
		/// </summary>
		/// <value>Defaults to 300</value>
		int DelayMilliseconds { get; set; }

		/// <summary>
		/// The <see cref="IMachine"/> that should be run once no input has been received for <see cref="DelayMilliseconds"/>
		/// </summary>
		IMachine Machine { get; set; }

		/// <summary>
		/// informs that input has been recieved, meaning we should defer executing the <see cref="Machine"/> for <see cref="DelayMilliseconds"/>
		/// </summary>
		void Input();
	}
}