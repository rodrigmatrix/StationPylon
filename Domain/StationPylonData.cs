using Colossal.Serialization.Entities;
using Unity.Entities;

namespace StationPylon.Domain
{
    public struct StationPylonData : IComponentData, ISerializable
    {
        public TransportLineType lineType;
        public Entity stationEntity;
        public bool showWheelchair;
        public bool smallIcons;
        public bool usePylonCustomName;

        public StationPylonData(
            TransportLineType lineType,
            Entity stationEntity,
            bool showWheelchair = true,
            bool smallIcons = true,
            bool usePylonCustomName = false
        )
        {
            this.lineType = lineType;
            this.stationEntity = stationEntity;
            this.showWheelchair = showWheelchair;
            this.smallIcons = smallIcons;
            this.usePylonCustomName = usePylonCustomName;
        }

        public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
        {
            writer.Write((uint) lineType);
            writer.Write(stationEntity);
            writer.Write(showWheelchair);
            writer.Write(smallIcons);
            writer.Write(usePylonCustomName);
        }

        public void Deserialize<TReader>(TReader reader) where TReader : IReader
        {
            reader.Read(out uint readLineType);
            reader.Read(out stationEntity);
            reader.Read(out showWheelchair);
            reader.Read(out smallIcons);
            reader.Read(out usePylonCustomName);
            lineType = (TransportLineType)readLineType;
        }
    }
}