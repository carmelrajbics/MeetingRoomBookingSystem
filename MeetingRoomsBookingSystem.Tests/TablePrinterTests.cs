using MeetingRoomBookingSystem;
using NUnit.Framework;
using System;

namespace MeetingRoomsBookingSystem.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void PrintTableRowMisMatches()
        {
            string[] tableRowHeader = { "Meeting Rooms", "Capacity" };
            string[] tableRowValue = { "Oxgen" };

            TablePrinter printTable = new TablePrinter(tableRowHeader);
            Assert.Throws<ArgumentException>(() => printTable.AddRow(tableRowValue));

        }
    }
}