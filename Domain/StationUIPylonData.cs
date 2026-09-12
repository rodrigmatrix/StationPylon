using Unity.Entities;

namespace StationPylon.Domain
{
    public class StationUIPylonData
    {
        public int TransportType;
        public string SelectedBuildingName;
        public Entity StationEntity;
        public bool ShowWheelchair = true;
        public bool SmallIcons = true;
        public bool UsePylonCustomName = false;
        public StationUIElement[] Stations;
    }

    public class StationUIElement
    {
        public Entity Entity;
        public string Name;
    }
}