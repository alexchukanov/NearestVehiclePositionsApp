using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NearestVehiclePositions.Model;

namespace NearestVehiclePositions
{
    internal class Program
    {
        const string fileName = @"C:\Projects\FindTheBestDealApp\VehiclePositions.dat";

        static void Main(string[] args)
        {
            List<VehiclesPosition> vehicleList = new List<VehiclesPosition>();

            List<VehiclesPosition> nearestVehicleList = new List<VehiclesPosition>();

            VehiclesPosition vehiclePosition = null;
            double distPow = 0;

            Stopwatch stopwatchLoad = new Stopwatch();

            if (File.Exists(fileName))
            {
                //Load file
                stopwatchLoad.Start();

                using (var stream = File.Open(fileName, FileMode.Open))
                {
                    using (var reader = new BinaryReader(stream, Encoding.ASCII, false))
                    {
                        while (reader.PeekChar() > -1)
                        {
                            var vehicle = new VehiclesPosition();

                            vehicle.PositionId = reader.ReadInt32();

                            char[] chars = reader.ReadChars(10);

                            double latitude = reader.ReadSingle();
                            double longitude = reader.ReadSingle();

                            vehicle.Position = new GeoPosition(latitude, longitude);

                            vehicle.RecordedTimeUTC = reader.ReadUInt64();

                            vehicleList.Add(vehicle);
                        }
                    }
                }

                stopwatchLoad.Stop();
                Console.WriteLine("Load file time: {0} ms", stopwatchLoad.ElapsedMilliseconds);

                // search the nearest car for each position
                Stopwatch stopwatchCalc = new Stopwatch();
                stopwatchCalc.Start();

                foreach (var position in GetPositions())
                {
                    foreach (var vehicle in vehicleList)
                    {

                        double latPow = Math.Pow(position.Latitude - vehicle.Position.Latitude, 2);
                        double lonPow = Math.Pow(position.Longitude - vehicle.Position.Longitude, 2);

                        if (distPow == 0 || (latPow + lonPow) < distPow)
                        {
                            distPow = latPow + lonPow;
                            vehiclePosition = vehicle;
                        }
                    }

                    nearestVehicleList.Add(vehiclePosition);
                    distPow = 0;
                }

                stopwatchCalc.Stop();
                Console.WriteLine("Calc ten positions time: {0} ms", stopwatchCalc.ElapsedMilliseconds);
                Console.WriteLine("Total cala time: {0} ms", stopwatchLoad.ElapsedMilliseconds + stopwatchCalc.ElapsedMilliseconds);
                Console.WriteLine();

                //print results
                Console.WriteLine("The nearest cars for ten positions:");

                int i = 1;
                foreach (var vehicle in nearestVehicleList)
                {
                    Console.WriteLine($"position = {i++}, the nearest vehicle ID = {vehicle.PositionId}, Latitude= {vehicle.Position.Latitude}, Longitude = {vehicle.Position.Longitude}");
                }
            }
            else
            {
                Console.WriteLine($"can not find file: {fileName}");
            }

            Console.ReadLine();
        }

        static List<GeoPosition> GetPositions()
        {
            List<GeoPosition> geoPositions = new List<GeoPosition>();

            geoPositions.Add(new GeoPosition(34.544909, -102.100843));
            geoPositions.Add(new GeoPosition(32.345544, -99.123124));

            geoPositions.Add(new GeoPosition(33.234235, -99.123124));
            geoPositions.Add(new GeoPosition(35.195739, -95.348899));

            geoPositions.Add(new GeoPosition(31.895839, -97.789573));
            geoPositions.Add(new GeoPosition(32.895839, -101.789573));

            geoPositions.Add(new GeoPosition(34.115839, -100.225732));
            geoPositions.Add(new GeoPosition(32.335839, -99.992232));

            geoPositions.Add(new GeoPosition(33.535339, -94.792232));
            geoPositions.Add(new GeoPosition(32.234235, -100.222222));

            return geoPositions;
        }
    }
}
