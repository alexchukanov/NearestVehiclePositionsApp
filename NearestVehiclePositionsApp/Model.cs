using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearestVehiclePositions.Model
{   
    public class VehiclesPosition
    {
        public int PositionId { get; set; }
        public string VehicleRegistration { get; set; }
        public GeoPosition Position { get; set; }
        public ulong RecordedTimeUTC { get; set; }
    }

    public class GeoPosition
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public GeoPosition(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }
    }
}
