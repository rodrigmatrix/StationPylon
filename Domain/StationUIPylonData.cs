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
    }
}