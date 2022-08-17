namespace VehiclePositions
{
    public class VehiclePosition
    {
        public int PositionId { get; set; }
        public string VehicleRegistraton { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public DateTime RecordedTimeUTC { get; set; }
    }
}
