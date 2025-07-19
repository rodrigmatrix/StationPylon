using System;
using System.Collections.Generic;
using Colossal.Entities;
using Game.Common;
using Game.SceneFlow;
using Game.UI;
using StationPylon.Utils;
using Unity.Entities;

namespace StationPylon.Formulas
{
    
    internal static class BuildingName
    {

        private static NameSystem _nameSystem;

        private static readonly Func<Entity, string> GetMainBuildingNameBinding = (buildingRef) =>
        {
            _nameSystem ??= World.DefaultGameObjectInjectionWorld.GetOrCreateSystemManaged<NameSystem>();
            while (World.DefaultGameObjectInjectionWorld.EntityManager.TryGetComponent<Owner>(buildingRef, out var owner) && owner.m_Owner != Entity.Null)
            {
                buildingRef = owner.m_Owner;
            }
            return _nameSystem.GetName(buildingRef).Translate();
        };

        public static string GetMainBuildingName(Entity buildingRef) => GetMainBuildingNameBinding?.Invoke(buildingRef) ?? "<???>";
    }
}
