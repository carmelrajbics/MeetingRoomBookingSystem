
using Booking.Domain.Enum;
/// <summary>
/// Objective of the class : This class is used to create number of meeting rooms instance configurad in the appsettings.json
/// </summary>
namespace Booking.Domain
{
    public class MeetingRooms
    {
        public string RoomName { get; set; }
        public int Capacity { get; set; }
        public bool IsBooked { get; set; } = false;
    }
}
