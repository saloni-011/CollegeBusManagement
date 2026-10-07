using CollegeBusManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace CollegeBusManagement.Data;

public static class DbInitializer
{
    public static void Seed(ApplicationDbContext context)
    {
        // Ensure database exists
        context.Database.EnsureCreated();

        // Check if database already has bus routes seeded
        if (context.BusRoutes.Any())
        {
            return; // DB has already been seeded
        }

        var routes = new List<BusRoute>
        {
            new BusRoute
            {
                RouteName = "Route 101 - Kalawad Road",
                StartPoint = "Kotecha Chowk",
                EndPoint = "College Campus Gate 1",
                MonthlyFee = 1200.00m,
                TotalSeats = 40,
                AvailableSeats = 40,
                BusStops = new List<BusStop>
                {
                    new BusStop { StopName = "Kotecha Chowk", PickupTime = "07:15 AM" },
                    new BusStop { StopName = "Indira Circle", PickupTime = "07:30 AM" },
                    new BusStop { StopName = "Atmiya University Junction", PickupTime = "07:45 AM" },
                    new BusStop { StopName = "Crystal Mall", PickupTime = "08:00 AM" },
                    new BusStop { StopName = "College Campus Gate 1", PickupTime = "08:20 AM" }
                }
            },
            new BusRoute
            {
                RouteName = "Route 102 - 150 Feet Ring Road",
                StartPoint = "Madhapar Chowk",
                EndPoint = "College Campus Gate 2",
                MonthlyFee = 1400.00m,
                TotalSeats = 45,
                AvailableSeats = 45,
                BusStops = new List<BusStop>
                {
                    new BusStop { StopName = "Madhapar Chowk", PickupTime = "07:10 AM" },
                    new BusStop { StopName = "Raiya Telephone Exchange", PickupTime = "07:25 AM" },
                    new BusStop { StopName = "Nanavati Chowk", PickupTime = "07:40 AM" },
                    new BusStop { StopName = "Mavdi Cross Roads", PickupTime = "07:55 AM" },
                    new BusStop { StopName = "College Campus Gate 2", PickupTime = "08:20 AM" }
                }
            },
            new BusRoute
            {
                RouteName = "Route 103 - University Road",
                StartPoint = "Jilla Panchayat",
                EndPoint = "Main Academic Block",
                MonthlyFee = 1100.00m,
                TotalSeats = 35,
                AvailableSeats = 35,
                BusStops = new List<BusStop>
                {
                    new BusStop { StopName = "Jilla Panchayat", PickupTime = "07:20 AM" },
                    new BusStop { StopName = "Kishanpara Chowk", PickupTime = "07:35 AM" },
                    new BusStop { StopName = "Akashvani Chowk", PickupTime = "07:50 AM" },
                    new BusStop { StopName = "Panchayat Nagar", PickupTime = "08:05 AM" },
                    new BusStop { StopName = "Main Academic Block", PickupTime = "08:25 AM" }
                }
            }
        };

        context.BusRoutes.AddRange(routes);
        context.SaveChanges();
    }
}
