using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Unity.Entities;
using StationPylon.Domain;

namespace StationPylon.System
{
    public static class LinesUtilsPatch
    {
        public static bool Prefix(EntityManager entityManager, Entity selectedEntity, bool iterateToOwner, ref object __result)
        {
            if (selectedEntity == Entity.Null) return true;

            // If selectedEntity is a pylon, we intercept and combine lines
            if (entityManager.HasComponent<StationPylonData>(selectedEntity))
            {
                var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "StationEntranceVisuals");
                if (assembly == null) return true;

                var linesUtilsType = assembly.GetType("StationEntranceVisuals.Formulas.LinesUtils");
                if (linesUtilsType == null) return true;

                var getFilteredLinesMethod = linesUtilsType.GetMethod("GetFilteredLinesList", BindingFlags.NonPublic | BindingFlags.Static);
                if (getFilteredLinesMethod == null) return true;

                // We must return a HashSet<LineDescriptor> from StationEntranceVisuals
                var lineDescriptorType = assembly.GetType("StationEntranceVisuals.Formulas.LineDescriptor");
                var hashSetType = typeof(HashSet<>).MakeGenericType(lineDescriptorType);
                var combinedHashSet = Activator.CreateInstance(hashSetType);
                var addMethod = hashSetType.GetMethod("Add");

                if (entityManager.HasBuffer<StationPylonStationElement>(selectedEntity))
                {
                    var buffer = entityManager.GetBuffer<StationPylonStationElement>(selectedEntity);
                    foreach (var element in buffer)
                    {
                        if (element.stationEntity != Entity.Null && entityManager.Exists(element.stationEntity) && !entityManager.HasComponent<Game.Common.Deleted>(element.stationEntity))
                        {
                            var stationLines = getFilteredLinesMethod.Invoke(null, new object[] { element.stationEntity, "All", true });
                            if (stationLines != null)
                            {
                                foreach (var line in (IEnumerable)stationLines)
                                {
                                    addMethod.Invoke(combinedHashSet, new object[] { line });
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Fallback to the single stationEntity for old saves
                    var pylonData = entityManager.GetComponentData<StationPylonData>(selectedEntity);
                    if (pylonData.stationEntity != Entity.Null && entityManager.Exists(pylonData.stationEntity) && !entityManager.HasComponent<Game.Common.Deleted>(pylonData.stationEntity))
                    {
                        var stationLines = getFilteredLinesMethod.Invoke(null, new object[] { pylonData.stationEntity, "All", true });
                        if (stationLines != null)
                        {
                            foreach (var line in (IEnumerable)stationLines)
                            {
                                addMethod.Invoke(combinedHashSet, new object[] { line });
                            }
                        }
                    }
                }

                __result = combinedHashSet;
                return false; // Skip the original GetLines call
            }
            return true; // Execute the original GetLines call
        }
    }
}
