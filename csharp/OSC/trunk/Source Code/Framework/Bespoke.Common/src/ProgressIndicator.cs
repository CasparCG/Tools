using System;
using System.Windows.Forms;

namespace Bespoke.Common
{
	/// <summary>
	/// 
	/// </summary>
	public class ProgressIndicator
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="minimum"></param>
		/// <param name="maximum"></param>
		/// <param name="step"></param>
		/// <param name="value"></param>		
		private delegate void InitHandler(int minimum, int maximum, int step, int value);
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		private delegate void IncrementHandler(int value);

		/// <summary>
		/// 
		/// </summary>
		private delegate void PerformStepHandler();		

		/// <summary>
		/// 
		/// </summary>
		/// <param name="progressBar"></param>
		public ProgressIndicator(ProgressBar progressBar)
		{
			mProgressBar = progressBar;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="progressBar"></param>
		/// <param name="minimum"></param>
		/// <param name="maximum"></param>
		/// <param name="step"></param>
		/// <param name="value"></param>
		public ProgressIndicator(ProgressBar progressBar, int minimum, int maximum, int step, int value)
		{
			mProgressBar = progressBar;
			Init(minimum, maximum, step, value);
		}

		/// <summary>
		/// 
		/// </summary>
		public void PerformStep()
		{
			if (mProgressBar.InvokeRequired)
			{
				mProgressBar.Invoke(new PerformStepHandler(PerformStep));
			}
			else
			{
				lock (mProgressBar)
				{					
					mProgressBar.PerformStep();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		public void Increment(int value)
		{
			if (mProgressBar.InvokeRequired)
			{
				object[] args = { value };
				mProgressBar.Invoke(new IncrementHandler(Increment), args);
			}
			else
			{
				lock (mProgressBar)
				{
					mProgressBar.Increment(value);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="minimum"></param>
		/// <param name="maximum"></param>
		/// <param name="step"></param>
		/// <param name="value"></param>		
		public void Init(int minimum, int maximum, int step, int value)
		{
			if (mProgressBar.InvokeRequired)
			{
				object[] args = { minimum, maximum, step, value };
				mProgressBar.Invoke(new InitHandler(Init), args);
			}
			else
			{
				lock (mProgressBar)
				{
					mProgressBar.Minimum = minimum;
					mProgressBar.Maximum = maximum;
					mProgressBar.Step = step;
					mProgressBar.Value = value;
				}
			}
		}

		private ProgressBar mProgressBar;
	}	
}
