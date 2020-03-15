using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AEMPROJECT
{
    public class Model
    {
        public class LoginRequest
        {
            public string username { get; set; }
            public string password { get; set; }
        }
        public class ErrorDto
        {
            public string message { get; set; }
            public string stackTrace { get; set; }
            public string innerException { get; set; }
            public string columnName { get; set; }
            public string columnValue { get; set; }
        }
        public class DashboardDto
        {
            public bool success { get; set; }
            public List<ChartDashboardDto> chartDonut { get; set; }
            public List<ChartDashboardDto> chartBar { get; set; }
            public List<TableUserDto> tableUsers { get; set; }
        }
        public class ChartDashboardDto
        {
            public string name { get; set; }
            public double value { get; set; }
        }
        public class TableUserDto
        {
            public string firstName { get; set; }
            public string lastName { get; set; }
            public string username { get; set; }
        }
        public class PlatformDto
        {
            public Int32 id { get; set; }
            public string uniqueName { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public DateTime createdAt { get; set; }
            public DateTime updatedAt { get; set; }
            public List<WellDto> well { get; set; }
        }
        public class WellDto
        {
            public Int32 id { get; set; }
            public Int32 platformId { get; set; }
            public string uniqueName { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public DateTime createdAt { get; set; }
            public DateTime updatedAt { get; set; }
        }
        public class PlatfromDummyDto
        {
            public Int32 id { get; set; }
            public string uniqueName { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public DateTime lastUpdate { get; set; }
            public List<WellDummyDto> well { get; set; }
        }
        public class WellDummyDto
        {
            public Int32 id { get; set; }
            public Int32 platformId { get; set; }
            public string uniqueName { get; set; }
            public double latitude { get; set; }
            public double longitude { get; set; }
            public DateTime lastUpdate { get; set; }
        }
    }
}
