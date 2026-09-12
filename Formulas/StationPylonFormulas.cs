using System.Collections.Generic;
using Colossal.Entities;
using StationPylon.Domain;
using Unity.Entities;

namespace StationPylon.Formulas
{
    public static class StationPylonFormulas
    {
        private const string MOD_PATH = "StationPylon:";
        
        public static Entity GetStation(Entity entity, Dictionary<string, string> vars)
        {
            var pylonData = GetPylonData(entity);
            if (pylonData == null)
            {
                return Entity.Null;
            }

            vars["lineType"] = pylonData.Value.lineType switch
            {
                TransportLineType.All => "All",
                TransportLineType.Subway => "Subway",
                TransportLineType.Train => "Train",
                TransportLineType.Bus => "Bus",
                TransportLineType.Tram => "Tram",
                _ => "All"
            };

            return entity;
        }
        
        public static string GetName(Entity entity, Dictionary<string, string> vars)
        {
            var pylonData = GetPylonData(entity);
            if (pylonData == null)
            {
                return BuildingName.GetMainBuildingName(entity);
            }
            if (pylonData.Value.usePylonCustomName)
            {
                return BuildingName.GetMainBuildingName(entity);
            }
            
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            if (entityManager.HasBuffer<StationPylonStationElement>(entity))
            {
                var buffer = entityManager.GetBuffer<StationPylonStationElement>(entity);
                for (int i = 0; i < buffer.Length; i++)
                {
                    var stEntity = buffer[i].stationEntity;
                    if (stEntity != Entity.Null && entityManager.Exists(stEntity) && !entityManager.HasComponent<Game.Common.Deleted>(stEntity))
                    {
                        return BuildingName.GetMainBuildingName(stEntity);
                    }
                }
            }
            
            if (pylonData.Value.stationEntity == Entity.Null || !entityManager.Exists(pylonData.Value.stationEntity) || entityManager.HasComponent<Game.Common.Deleted>(pylonData.Value.stationEntity))
            {
                return BuildingName.GetMainBuildingName(entity);
            }
            
            return BuildingName.GetMainBuildingName(pylonData.Value.stationEntity);
        }
        
        public static string GetWheelchairIcon(Entity entity, Dictionary<string, string> vars)
        {
            var pylonData = GetPylonData(entity);
            if (pylonData == null)
            {
                return "WheelchairSquare";
            }

            return (pylonData.Value.showWheelchair ? "WheelchairSquare" : "Transparent");
        }
        
        public static int ShowSmallIcons(Entity entity, Dictionary<string, string> vars)
        {
            var pylonData = GetPylonData(entity);
            if (pylonData == null)
            {
                return 1;
            }

            return pylonData.Value.smallIcons ? 1 : 0;
        }
        
        public static int ShowRectangleIcons(Entity entity, Dictionary<string, string> vars)
        {
            var pylonData = GetPylonData(entity);
            if (pylonData == null)
            {
                return 0;
            }

            return pylonData.Value.smallIcons ? 0 : 1;
        }

        private static StationPylonData? GetPylonData(Entity entity)
        {
            return World.DefaultGameObjectInjectionWorld.EntityManager.TryGetComponent<StationPylonData>(entity, out var pylonData) ? pylonData : null;
        }
    }   
}