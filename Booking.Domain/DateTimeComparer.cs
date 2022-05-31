using System;

namespace Booking.Domain
{
    public static class DateTimeComparer
    {
        public static bool HasOverlap(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        {
            return Min(start1, end1) < Max(start2, end2) && Max(start1, end1) > Min(start2, end2);
        }

        private static DateTime Max(DateTime d1, DateTime d2)
        {
            return d1 > d2 ? d1 : d2;
        }

        private static DateTime Min(DateTime d1, DateTime d2)
        {
            return d2 > d1 ? d1 : d2;
        }
    }
}
