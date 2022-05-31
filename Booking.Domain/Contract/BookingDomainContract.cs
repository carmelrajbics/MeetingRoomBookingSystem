using Booking.Domain.Model;
using System;
using System.Collections.Generic;

namespace Booking.Domain
{
    public interface IBookingDomainContract
    {
        BookingDomainModel BookMeetingRoom(BookingModel model);
        List<MeetingRooms> FindAvailableMeetingRooms(int capacity, DateTime meetingStarts,
            DateTime meetingEnds, HashSet<MeetingRooms> meetingRooms, out string filePath);
    }
}
