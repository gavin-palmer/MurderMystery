using System;
using System.Collections.Generic;

namespace MurderMystery.Models
{
    /// <summary>
    /// Class to hold all information related to the security system
    /// </summary>
    public class SecurityInfo
    {
        // Basic security information
        public string PinCode { get; set; }
        public List<string> RoomsWithCameras { get; set; }
        public string SecurityRoom { get; set; }

        // Additional puzzle information
        public Dictionary<string, object> PuzzleData { get; set; }
        public string PuzzleName { get; set; }

        public SecurityInfo()
        {
            RoomsWithCameras = new List<string>();
            PuzzleData = new Dictionary<string, object>();
        }

        /// <summary>
        /// Store any puzzle-specific data
        /// </summary>
        public void StorePuzzleData(string key, object value)
        {
            PuzzleData[key] = value;
        }

        /// <summary>
        /// Get puzzle-specific data
        /// </summary>
        public T GetPuzzleData<T>(string key)
        {
            if (PuzzleData.ContainsKey(key) && PuzzleData[key] is T)
            {
                return (T)PuzzleData[key];
            }
            return default(T);
        }
    }
}