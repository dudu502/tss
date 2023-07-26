using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Task.Switch.Structure.HFSM
{
	/// <summary>
	/// Default timer that calculates the elapsed time based on Time.time.
	/// </summary>
	public class Timer
	{
		private float startTime;
		public float Elapsed => Time.time - startTime;

		public Timer()
		{
			startTime = Time.time;
		}

		public void Reset()
		{
			startTime = Time.time;
		}

		public static bool operator >(Timer timer, float duration)
			=> timer.Elapsed > duration;

		public static bool operator <(Timer timer, float duration)
			=> timer.Elapsed < duration;

		public static bool operator >=(Timer timer, float duration)
			=> timer.Elapsed >= duration;

		public static bool operator <=(Timer timer, float duration)
			=> timer.Elapsed <= duration;

		public static float operator /(Timer timer, float duration)
			=> timer.Elapsed / duration;
	}
}