// See https://aka.ms/new-console-template for more information
using VehiclePositions;

var repository = new DataRepository();
var nearestVehicleResults = repository.GetNearestVehiclePositions();

foreach (var item in nearestVehicleResults)
    Console.WriteLine($"Vehicle Registration: {item.VehiclePosition.VehicleRegistraton}, Distance: {item.Distance:N2} Meters");

Console.ReadLine();