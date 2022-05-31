using System;
using System.Collections.Generic;
using System.Text;

namespace Booking.Contract
{
    public interface IBookingDomainContract
    {
        public void BookMeetingRoom(BookingDomainModel);
    }
}
