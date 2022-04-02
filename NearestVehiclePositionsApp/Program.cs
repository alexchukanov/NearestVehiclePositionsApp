using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NearestVehiclePositions.Model;

/*
Your task is to write a program that can find the nearest vehicle position in the data file to each of the 10 co-ordinates 
provided below. In addition to being able to do this, however, your program must be able to complete all 10 lookups in less 
time than our benchmark. This benchmark is based on simply looping through each of the 2 million positions and keeping the 
closest to each given co-ordinate. This is simply repeated for each of the 10 provided co-ordinates.

Telematics telemetry result:
Data file read execution time : 172 ms
Closest position calculation execution time : 3917 ms
Total execution time : 4089 ms

My telemetry result:
Load file time: 2979 ms
Calc ten positions time: 656 ms
Total calc time: 3635 ms

The nearest cars for ten positions:
position = 1, the nearest vehicle ID = 1017652, Latitude= 34,5450325012207, Longitude = -102,10150909423828
position = 2, the nearest vehicle ID = 609817, Latitude= 32,34754180908203, Longitude = -99,12191009521484
position = 3, the nearest vehicle ID = 591880, Latitude= 33,23557662963867, Longitude = -99,12040710449219
position = 4, the nearest vehicle ID = 284382, Latitude= 35,19633102416992, Longitude = -95,34922790527344
position = 5, the nearest vehicle ID = 1724293, Latitude= 31,895187377929688, Longitude = -97,78909301757812
position = 6, the nearest vehicle ID = 1442878, Latitude= 32,89590835571289, Longitude = -101,78733825683594
position = 7, the nearest vehicle ID = 108054, Latitude= 34,11565399169922, Longitude = -100,22537231445312
position = 8, the nearest vehicle ID = 1247417, Latitude= 32,33456802368164, Longitude = -99,9914779663086
position = 9, the nearest vehicle ID = 87700, Latitude= 33,53670883178711, Longitude = -94,79186248779297
position = 10, the nearest vehicle ID = 282385, Latitude= 32,23622512817383, Longitude = -100,22180938720703
*/

namespace NearestVehiclePositions
{
    internal class Program
    {
        const string fileName = @"C:\Projects\NearestVehiclePositionsApp\NearestVehiclePositionsApp\VehiclePositions.dat";

        static void Main(string[] args)
        {
            List<VehiclesPosition> vehicleList = new List<VehiclesPosition>();

            List<VehiclesPosition> nearestVehicleList = new List<VehiclesPosition>();

            VehiclesPosition vehiclePosition = default;
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
                            var vehicle = new VehiclesPosition
                            (
                                reader.ReadInt32(),
                                reader.ReadChars(10).ToString(),
                                new GeoPosition(reader.ReadSingle(), reader.ReadSingle()),
                                reader.ReadUInt64()
                            );

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
                        double latDif = (position.Latitude - vehicle.Position.Latitude);
                        double latPow = latDif * latDif;

                        double lonDif = (position.Longitude - vehicle.Position.Longitude);
                        double lonPow = lonDif * lonDif;

                        double distDiff = latPow + lonPow;
                      
                        if (distPow == 0 || (distDiff) < distPow)
                        {
                            distPow = distDiff;
                            vehiclePosition = vehicle;
                        }
                    }

                    nearestVehicleList.Add(vehiclePosition);
                    distPow = 0;
                }

                stopwatchCalc.Stop();
                Console.WriteLine("Calc ten positions time: {0} ms", stopwatchCalc.ElapsedMilliseconds);
                Console.WriteLine("Total calc time: {0} ms", stopwatchLoad.ElapsedMilliseconds + stopwatchCalc.ElapsedMilliseconds);
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
            List<GeoPosition> geoPositions = new List<GeoPosition>()
            {
                new GeoPosition(34.544909, -102.100843),
                new GeoPosition(32.345544, -99.123124),
                new GeoPosition(33.234235, -99.123124),
                new GeoPosition(35.195739, -95.348899),
                new GeoPosition(31.895839, -97.789573),
                new GeoPosition(32.895839, -101.789573),
                new GeoPosition(34.115839, -100.225732),
                new GeoPosition(32.335839, -99.992232),
                new GeoPosition(33.535339, -94.792232),
                new GeoPosition(32.234235, -100.222222)
            };

            return geoPositions;
        }
    }
}
