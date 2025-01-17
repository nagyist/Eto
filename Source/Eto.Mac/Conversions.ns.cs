using System;
using MonoMac.Foundation;

namespace Eto.Mac
{
	public static partial class Conversions
	{
		static readonly DateTime ReferenceDate = new DateTime (2001, 1, 1, 0, 0, 0);
		
		public static NSUrl ToNS (this Uri uri)
		{
			return uri == null ? null : new NSUrl(uri.AbsoluteUri);
		}

		public static NSDate ToNS (this DateTime date)
		{
			return NSDate.FromTimeIntervalSinceReferenceDate ((date.ToUniversalTime () - ReferenceDate).TotalSeconds);
		}
		
		public static NSDate ToNS (this DateTime? date)
		{
			return date == null ? null : date.Value.ToNS();
		}
		
		public static DateTime? ToEto (this NSDate date)
		{
			if (date == null) return null;
			return new DateTime ((long)(date.SecondsSinceReferenceDate * TimeSpan.TicksPerSecond + ReferenceDate.Ticks), DateTimeKind.Utc).ToLocalTime ();
		}
		
	}
}

