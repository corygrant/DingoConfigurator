﻿using CanDevices;
using CanDevices.CanBoard;
using CanDevices.CanMsgLog;
using CanDevices.DingoDash;
using CanDevices.DingoPdm;
using CanDevices.dingoPdmMax;
using CanDevices.SoftButtonBox;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DingoConfigurator.Config
{
    public class DevicesConfig
    {
        public List<DingoPdmCan> pdm { get; set; }
        public List<dingoPdmMaxCan> pdmMax { get; set; }
        public List<CanBoardCan> canBoard { get; set; }
        public List<DingoDashCan> dash { get; set; }
        public List<SoftButtonBox> sbb { get; set; }
        public List<CanMsgLog> log { get; set; }

        public DevicesConfig()
        {
            pdm = new List<DingoPdmCan>();
            pdmMax = new List<dingoPdmMaxCan>();
            canBoard = new List<CanBoardCan>();
            dash = new List<DingoDashCan>();
            sbb = new List<SoftButtonBox>();
            log = new List<CanMsgLog>();
        }
    }

    public static class ConfigFile
    {   
        public static DevicesConfig CollectionToConfig(ObservableCollection<ICanDevice> devices)
        {
            DevicesConfig config = new DevicesConfig();
            foreach (var device in devices)
            {
                if (device is dingoPdmMaxCan)
                {
                    config.pdmMax.Add((dingoPdmMaxCan)device);
                }
                else if (device is DingoPdmCan)
                {
                    config.pdm.Add((DingoPdmCan)device);
                }
                else if (device is DingoDashCan)
                {
                    config.dash.Add((DingoDashCan)device);
                }
                else if (device is SoftButtonBox)
                {
                    config.sbb.Add((SoftButtonBox)device);
                }
                else if (device is CanBoardCan)
                {
                    config.canBoard.Add((CanBoardCan)device);
                }
                else if (device is CanMsgLog)
                {
                    config.log.Add((CanMsgLog)device);
                }
            }
            return config;
        }

        public static ObservableCollection<ICanDevice> ConfigToCollection(DevicesConfig config)
        {
            ObservableCollection<ICanDevice> devices = new ObservableCollection<ICanDevice>();
            foreach (var pdm in config.pdm)
            {
                pdm.InitializeAfterDeserialization();
                devices.Add(pdm);
            }
            foreach (var pdmMax in config.pdmMax)
            {
                devices.Add(pdmMax);
            }
            foreach (var dash in config.dash)
            {
                devices.Add(dash);
            }
            foreach (var sbb in config.sbb)
            {
                devices.Add(sbb);
            }
            foreach (var board in config.canBoard)
            {
                devices.Add(board);
            }
            foreach (var log in config.log)
            {
                devices.Add(log);
            }
            return devices;
        }

        public static bool Save(string filename, DevicesConfig devices)
        {
            var options = new JsonSerializerOptions 
            { 
                WriteIndented = true
            };
            var jsonString = JsonSerializer.Serialize(devices, options);
            File.WriteAllText(filename, jsonString);

            //Catch file exceptions
            return true;
        }

        public static bool Open(string filename, out DevicesConfig devices)
        {
            var jsonString = File.ReadAllText(filename);
            devices = new DevicesConfig();
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                devices = JsonSerializer.Deserialize<DevicesConfig>(jsonString, options);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Deserialization error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                // This will give you more specific error details
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            //Catch file exceptions
            return true;
        }
    }
}
