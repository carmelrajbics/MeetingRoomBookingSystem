using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Domain.Model
{
    public class BookingModel
    {
        public DateTime BookingDateTime { get; set; }

        public int StartTimeHours { get; set; }
        public int StartTimeMins { get; set; }
        public int EndTimeHours { get; set; }
        public int EndTimeMins { get; set; }
        public HashSet<MeetingRooms> MeetingRooms{ get; set; }
        public int NoOfAttendees { get; set; }
    }
}
