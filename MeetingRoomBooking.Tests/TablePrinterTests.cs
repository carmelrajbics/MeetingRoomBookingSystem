using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MeetingRoomBooking.Tests
{
    [TestClass]
    public class TablePrinterTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PrintTableRowMisMatches()
        {
            string[] tableRowHeader = { "MeetingRoom", "Capacity" };
            string[] tableRowValue = { "Oxgen" };

            TablePrinter printTable = new TablePrinter(tableRowHeader);
            printTable.AddRow(tableRowValue);
        }
    }
}
