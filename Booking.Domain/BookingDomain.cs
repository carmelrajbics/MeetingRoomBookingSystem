using Booking.Domain.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Booking.Domain
{
    public class BookingDomain : IBookingDomainContract
    {

        /// <summary>
        /// Method to do the logics and do the actual bookings for the given date
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Returns the booking informations</returns>
        public BookingDomainModel BookMeetingRoom(BookingModel model)
        {
            string filePath;
            DateTime meetingStarts = model.BookingDateTime.AddHours(model.StartTimeHours).AddMinutes(model.StartTimeMins);
            DateTime meetingEnds = model.BookingDateTime.AddHours(model.EndTimeHours).AddMinutes(model.EndTimeMins);
            var bookingModel = new BookingDomainModel();
            var roomToBook = FindAvailableMeetingRooms(model.NoOfAttendees, meetingStarts, meetingEnds, model.MeetingRooms, out filePath).FirstOrDefault();

            if (roomToBook == null)
            {
                return bookingModel;
            }
            else
            {
                bookingModel = new BookingDomainModel
                {
                    RoomName = roomToBook.RoomName,
                    StartDate = meetingStarts,
                    EndDate = meetingEnds,
                    NoOfAttendees = model.NoOfAttendees
                };
                var json = JsonConvert.SerializeObject(bookingModel) + Environment.NewLine;

                File.AppendAllText(filePath, json);
            }
            return bookingModel;
        }

        /// <summary>
        /// Find the available meeting rooms to be booked for a given date
        /// </summary>
        /// <param name="capacity">No of participants</param>
        /// <param name="meetingStarts">Meeting start Date & Time</param>
        /// <param name="meetingEnds">Meeting ends datetime</param>
        /// <param name="meetingRooms">No of rooms availabe as per the configurations</param>
        /// <param name="filePath">File path to save the booked rooms</param>
        /// <returns>Returns only the available meeting rooms</returns>
        public List<MeetingRooms> FindAvailableMeetingRooms(int capacity, DateTime meetingStarts,
            DateTime meetingEnds, HashSet<MeetingRooms> meetingRooms, out string filePath)
        {
            var rooms = meetingRooms;
            var listOfBookedRooms = new HashSet<BookingDomainModel>();
            filePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName, "BookedRooms.json");
            using (var sr = new StreamReader(filePath))
            {
                string currentLine;
                while ((currentLine = sr.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(currentLine))
                    {
                        var bookedRooms = JsonConvert.DeserializeObject<BookingDomainModel>(currentLine);
                        listOfBookedRooms.Add(bookedRooms);
                    }
                }
            }


            //Getting all the overlapping rooms
            if (listOfBookedRooms.Any())
            {
                var overlapingRooms = listOfBookedRooms.Where(x => DateTimeComparer.HasOverlap(meetingStarts, meetingEnds, x.StartDate, x.EndDate)).ToList();
                //Removing all the overlapping rooms from list of available rooms for the given day
                foreach (var room in overlapingRooms)
                {
                    rooms.RemoveWhere(x => x.RoomName == room.RoomName);
                }
            }

            var availableMeetingRooms = rooms.Where(x => x.Capacity >= capacity && x.IsBooked == false).OrderBy(item => Math.Abs(capacity - item.Capacity)).ToList();

            return availableMeetingRooms;
        }
    }
}
