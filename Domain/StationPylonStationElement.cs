using Colossal.Serialization.Entities;
using Unity.Entities;

namespace StationPylon.Domain
{
    public struct StationPylonStationElement : IBufferElementData, ISerializable
    {
        public Entity stationEntity;

        public StationPylonStationElement(Entity stationEntity)
        {
            this.stationEntity = stationEntity;
        }

        public void Serialize<TWriter>(TWriter writer) where TWriter : IWriter
        {
            writer.Write(stationEntity);
        }

        public void Deserialize<TReader>(TReader reader) where TReader : IReader
        {
            reader.Read(out stationEntity);
        }
    }
}
