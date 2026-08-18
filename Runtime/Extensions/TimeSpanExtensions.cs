using System;

namespace Mane.DotNet
{
	/// <summary>
	/// Formats a <see cref="TimeSpan"/> as a clock string. Negative and zero values format as zeros.
	/// </summary>
	public static class TimeSpanExtensions
	{
		/// <summary>
		/// Formats as HH:MM:SS. Negative and zero values become 00:00:00.
		/// Hours are the full duration, so values over 24 hours are not wrapped.
		/// </summary>
		public static string ToHHMMSS(this TimeSpan ts) => 
			ts.TotalSeconds > 0
				? $"{(int)ts.TotalHours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}"
				: "00:00:00";

		/// <summary>
		/// Formats as HH:MM. Negative and zero values become 00:00.
		/// Hours are the full duration, so values over 24 hours are not wrapped.
		/// </summary>
		public static string ToHHMM(this TimeSpan ts) => 
			ts.TotalSeconds > 0
				? $"{(int)ts.TotalHours:D2}:{ts.Minutes:D2}"
				: "00:00";

		/// <summary>
		/// Formats as MM:SS. Negative and zero values become 00:00.
		/// Minutes are the full duration, so values over 60 minutes are not wrapped.
		/// </summary>
		public static string ToMMSS(this TimeSpan ts) => 
			ts.TotalSeconds > 0
				? $"{(int)ts.TotalMinutes:D2}:{ts.Seconds:D2}"
				: "00:00";
	}
}