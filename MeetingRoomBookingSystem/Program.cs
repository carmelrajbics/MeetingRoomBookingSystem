using Booking.Domain;
using Booking.Domain.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MeetingRoomBookingSystem
{
    class Program
    {
        #region Variables
        private static IServiceProvider _serviceProvider;
        private static IConfiguration _configuration;
        static int _startTimeHours, _endtTimeHours, _startTimeMins, _endTimeMins;
        #endregion

        #region Main Method
        static void Main(string[] args)
        {
            Console.Title = "Meeting Room Booking System - By Carmel Raj M (PUID-wk29)";

            bool canExit;
            RegisterServices();
            try
            {
                do
                {
                    canExit = ReadUserInput();
                } while (!canExit);
                Console.ResetColor();
                DisposeServices();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


        }
        #endregion

        #region Validations
        private static bool ValidateUserInput(string[] values)
        {
            int noOfAttendees;
            bool isValid = false;
            if (values.Length == 4 && int.TryParse(values[3], out noOfAttendees) && !(string.IsNullOrEmpty(values[0]) && string.IsNullOrEmpty(values[1]) &&
                string.IsNullOrEmpty(values[2]) && string.IsNullOrEmpty(values[3])))
            {
                isValid = BusinessValidations(values, noOfAttendees);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("\n\n\t\tInvalid input\n\n");
                Console.ResetColor();
            }
            return isValid;
        }

        private static bool BusinessValidations(string[] values, int noOfAttendees)
        {
            //Mininum number of attendees 2 and Maxium no of attendees is 20
            bool isValid = false;
            if (noOfAttendees >= 2 && noOfAttendees <= 20)
            {
                var startTime = values[1].Split(":");
                var endTime = values[2].Split(":");
                _startTimeHours = Convert.ToInt32(startTime[0]);
                _endtTimeHours = Convert.ToInt32(endTime[0]);
                _startTimeMins = Convert.ToInt32(startTime[1]);
                _endTimeMins = Convert.ToInt32(endTime[1]);

                var startIntervalOfFives = _startTimeMins % 5;
                var endIntervalOfFives = _endTimeMins % 5;

                if (startIntervalOfFives == 0 && endIntervalOfFives == 0)
                {
                    //Validating the booking is not falling between the fixed buffer time
                    if ((_startTimeHours == 9 && Enumerable.Range(0, 15).Contains(_startTimeMins) ||
                        _startTimeHours == 13 && Enumerable.Range(15, 45).Contains(_startTimeMins) ||
                        _startTimeHours == 18 && Enumerable.Range(45, 59).Contains(_startTimeMins) ||
                        _startTimeHours == 19 && _startTimeMins == 0) ||
                       (_endtTimeHours == 9 && Enumerable.Range(1, 15).Contains(_endTimeMins) ||
                        _endtTimeHours == 13 && Enumerable.Range(15, 45).Contains(_endTimeMins) ||
                        _endtTimeHours == 18 && Enumerable.Range(45, 59).Contains(_endTimeMins) ||
                        _endtTimeHours == 19 && _endTimeMins == 0))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("\n\t\tBooking can not be made in during buffer time\n\n");
                        Console.ResetColor();
                    }
                    else
                    {
                        isValid = true;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("\n\t\tBooking can be made only in the interval of 5 mintues");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("\nNo of attendees should be greater than or equal 2 and less or equal to 20");
                Console.ResetColor();
            }
            return isValid;
        }
        #endregion

        #region Private Methods

        static void RegisterServices()
        {
            var collection = new ServiceCollection();
            collection.AddScoped<IBookingDomainContract, BookingDomain>();
            _serviceProvider = collection.BuildServiceProvider();

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            _configuration = builder.Build();
        }

        static void DisposeServices()
        {
            if (_serviceProvider == null)
            {
                return;
            }
            if (_serviceProvider is IDisposable)
            {
                ((IDisposable)_serviceProvider).Dispose();
            }
        }

        static bool ReadUserInput()
        {
            Console.WriteLine("----------------------------------Welcome to meeting room booking System----------------------------------\n\n");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\t\t\t1. Book a meeting Room");
            Console.WriteLine("\t\t\t2. View available meeting rooms");
            Console.WriteLine("\t\t\t3. Exit the application\n\n");
            Console.WriteLine("-----------------------------------------------------------------------------------------------------------\n\n");

            Console.ResetColor();
            var userChoice = Convert.ToInt32(Console.ReadLine());
            bool canExit = false;

            switch (userChoice)
            {
                case 1:
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("\nInput values separated by a space\n");
                        Console.WriteLine("Date(DD/MM/YYYY)\tStart Time (HH:MM)\tEnd time (HH:MM)\t# of attendees");
                        Console.WriteLine("\nEx:11/05/2022 10:00 11:30 11:\t");
                        Console.ForegroundColor = ConsoleColor.Green;

                        var values = Console.ReadLine().Split();
                        if (ValidateUserInput(values))
                        {
                            var model = new BookingModel
                            {
                                BookingDateTime = Convert.ToDateTime(values[0]),
                                NoOfAttendees = Convert.ToInt32(values[3]),
                                MeetingRooms = _configuration.GetSection("MeetingRooms").Get<HashSet<MeetingRooms>>(),
                                StartTimeHours = _startTimeHours,
                                StartTimeMins = _startTimeMins,
                                EndTimeHours = _endtTimeHours,
                                EndTimeMins = _endTimeMins
                            };
                            var service = _serviceProvider.GetService<IBookingDomainContract>();
                            //Calling the Model to do the booking
                            BookingDomainModel result = service.BookMeetingRoom(model);

                            //Print details to the users
                            if (result != null && !string.IsNullOrEmpty(result.RoomName))
                            {
                                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                                Console.WriteLine("\n\n\t\t\tMeeting room booked Sucessfully!!!\n\n");
                                var table = new TablePrinter("Room Name", "Booked On", "Capacity", "# of people you booked", "Status");
                                table.AddRow(result.RoomName, model.BookingDateTime.AddHours(model.StartTimeHours).AddMinutes(model.StartTimeMins), result.NoOfAttendees, model.NoOfAttendees, "Booked Sucessfully");
                                table.Print();
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.WriteLine("\n\n\t\t\tNo meeting rooms are available to book. Try for different date/time\n\n");
                                Console.ResetColor();
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("================================List of available meetin rooms==================================");
                        Console.WriteLine("\nInput values separated by a space\n");
                        Console.WriteLine("Date(DD/MM/YYYY)\nStart Time (HH:MM)\nEnd time (HH:MM)");
                        Console.WriteLine("\n[11/05/2022 10:00 11:30]:");
                        Console.WriteLine("\n\t");
                        Console.ForegroundColor = ConsoleColor.Green;
                        var values = Console.ReadLine().Split();
                        //Do the Input validation and business validationss
                        if (ValidateUserInput(values))
                        {
                            string filePath;

                            DateTime BookingDateTime = Convert.ToDateTime(values[0]);
                            DateTime meetingStarts = BookingDateTime.AddHours(_startTimeHours).AddMinutes(_startTimeMins);
                            DateTime meetingEnds = BookingDateTime.AddHours(_endtTimeHours).AddMinutes(_endTimeMins);
                            int noOfAttendees = Convert.ToInt32(values[3]);
                            var meetingRooms = _configuration.GetSection("MeetingRooms").Get<HashSet<MeetingRooms>>();

                            var service = _serviceProvider.GetService<IBookingDomainContract>();
                            var ListOfMeetingRooms = service.FindAvailableMeetingRooms(noOfAttendees, meetingStarts, meetingEnds, meetingRooms, out filePath);

                            if (ListOfMeetingRooms != null && ListOfMeetingRooms.Count > 0)
                            {
                                var table = new TablePrinter("Room Name", "Capacity", "Status");
                                foreach (var item in ListOfMeetingRooms)
                                {
                                    table.AddRow(item.RoomName, item.Capacity, "Available");
                                }
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                table.Print();
                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.WriteLine("\n=========================================================================================");
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                Console.WriteLine("\n\n\t\tNo Meeting rooms available at requested day\n\n");
                                Console.ResetColor();
                            }
                        }
                        break;
                    }
                case 3:
                    {
                        canExit = true;
                        break;
                    }
                default:
                    {
                        Console.WriteLine("\n\n\tPlease choose from the given options\n\n");
                        break;
                    }
            }
            return canExit;
        }
        #endregion
    }
}
