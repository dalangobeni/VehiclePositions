using BAMCIS.GIS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehiclePositions
{
    public class DataRepository
    {
        private readonly StringBuilder _stringBuilder = new StringBuilder();
        private List<VehiclePosition> GetVehiclePositions()
        {
            var vehiclePositions = new List<VehiclePosition>();
            //reading from the file
            try
            {
                var binaryReader = new BinaryReader(new FileStream("VehiclePositions.dat", FileMode.Open));
                long length = binaryReader.BaseStream.Length;
                while (binaryReader.BaseStream.Position < length)
                {
                    var vehiclePosition = new VehiclePosition
                    {
                        PositionId = binaryReader.ReadInt32(),
                        VehicleRegistraton = ReadNullTerminatedAsciiString(binaryReader),
                        Latitude = binaryReader.ReadSingle(),
                        Longitude = binaryReader.ReadSingle(),
                        RecordedTimeUTC = DateTime.UnixEpoch.AddSeconds(binaryReader.ReadUInt64())
                    };

                    vehiclePositions.Add(vehiclePosition);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message + "\n Cannot read from file.");
                throw;
            }

            return vehiclePositions;
        }
        private GeoCoordinate[] GetCoordinates()
        {
            return new GeoCoordinate[]
            {
                new GeoCoordinate(34.544909f,-102.100843f),
                new GeoCoordinate(32.345544f,-99.123124f),
                new GeoCoordinate(33.234235f,-100.214124f),
                new GeoCoordinate(35.195739f,-95.348899f),
                new GeoCoordinate(31.895839f,-97.789573f),
                new GeoCoordinate(32.895839f,-101.789573f),
                new GeoCoordinate(34.115839f,-100.225732f),
                new GeoCoordinate(32.335839f,-99.992232f),
                new GeoCoordinate(33.535339f,-94.792232f),
                new GeoCoordinate(32.234235f,-100.222222f)
            };
        }
        public List<NearestVehicleResult> GetNearestVehiclePositions()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            List<VehiclePosition> vehiclePositions = GetVehiclePositions();

            stopwatch.Stop();
            long timeToLoadFile = stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();

            var nearestVehicles = new List<NearestVehicleResult>();
            var coordinates = GetCoordinates();
            foreach (var coordinate in coordinates)
            {
                nearestVehicles.Add(GetNearestVehicle(vehiclePositions, coordinate));
            }
            stopwatch.Stop();
            long closestPositionTime = stopwatch.ElapsedMilliseconds;

            Console.WriteLine($"Data file read execution time : {timeToLoadFile} ms");
            Console.WriteLine($"Closest position calculation execution time : {closestPositionTime} ms");
            Console.WriteLine($"Total execution time : {closestPositionTime + timeToLoadFile} ms");


            return nearestVehicles;


        }
        private NearestVehicleResult GetNearestVehicle(List<VehiclePosition> positions, GeoCoordinate geoCoordinate)
        {
            return positions.
                Select(e => new NearestVehicleResult { VehiclePosition = e, Distance = geoCoordinate.DistanceTo((double)e.Latitude, (double)e.Longitude, DistanceType.METERS) })
                .OrderBy(e => e.Distance)
                .First();

        }
        private string ReadNullTerminatedAsciiString(BinaryReader binaryReader)
        {
            var b = binaryReader.ReadByte();
            _stringBuilder.Clear();
            while (b != 0)
            {
                _stringBuilder.Append(Convert.ToChar(b));
                b = binaryReader.ReadByte();
            }
            return _stringBuilder.ToString();
        }

    }
}
