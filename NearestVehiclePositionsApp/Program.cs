﻿using System;
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
