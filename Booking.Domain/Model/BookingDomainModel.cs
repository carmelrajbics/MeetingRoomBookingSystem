
using System;

namespace Booking.Domain.Model
{
    public sealed class BookingDomainModel
    {
        public string RoomName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NoOfAttendees { get; set; }
    }
}
