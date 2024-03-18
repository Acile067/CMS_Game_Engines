using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS_Game_Engines.Common
{
    [Serializable]
    public class GameEngine
    {
        public bool IsSelected { get; set; }
        public int ActiveUsersField { get; set; } 
        public string DescriptionField { get; set; } 
        public string ImagePath { get; set; } 
        public string RtfFilePath { get; set; } 
        public DateTime DateAdded { get; set; } 

        public GameEngine(int activeUsersField, string descriptionField, string imagePath, string rtfFilePath)
        {
            IsSelected = false;
            ActiveUsersField = activeUsersField;
            DescriptionField = descriptionField;
            ImagePath = imagePath;
            RtfFilePath = rtfFilePath;
            DateAdded = DateTime.Now;
        }

        
        public GameEngine()
        {
        }
    }
}
