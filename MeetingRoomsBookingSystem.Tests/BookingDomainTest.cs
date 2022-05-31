using Booking.Domain;
using Moq;
using NUnit.Framework;

namespace MeetingRoomsBookingSystem.Tests
{
    public class BookingDomainTest
    {

        private readonly Mock<IBookingDomainContract> _bookingModel;

        [SetUp]
        public void Setup()
        {
            _bookingModel = new Mock<IBookingDomainContract>();
        }

        [Test]
        public void BookMeetingRoomWithProperInput()
        {


        }
    }
}
