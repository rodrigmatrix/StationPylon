using System;
using Colossal.Entities;
using Colossal.Logging;
using Game.Areas;
using Game.Buildings;
using Game.Common;
using Game.Input;
using Game.Objects;
using Game.Prefabs;
using Game.Rendering;
using Game.Tools;
using StationPylon.Domain;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace StationPylon.System
{
	
    public partial class BuildingPickerToolSystem : ToolBaseSystem
	{
		private Entity m_PreviousRaycastedEntity;
		private Entity m_PreviousSelectedEntity;
		private EntityQuery m_HighlightedQuery;
		private ToolOutputBarrier m_Barrier;

		public override string toolID => "StationPylonPickerTool";
		private Entity m_TargetPylonEntity;

		protected override void OnCreate()
		{
			base.OnCreate();
			Enabled = false;
			m_Barrier = World.GetOrCreateSystemManaged<ToolOutputBarrier>();
            
			m_HighlightedQuery = GetEntityQuery(ComponentType.ReadOnly<Highlighted>());
		}

		public override void InitializeRaycast()
		{
			base.InitializeRaycast();
			m_ToolRaycastSystem.collisionMask = CollisionMask.Overground | CollisionMask.OnGround | CollisionMask.Underground;
			m_ToolRaycastSystem.typeMask |= TypeMask.StaticObjects;
			m_ToolRaycastSystem.areaTypeMask |= AreaTypeMask.Lots;
			m_ToolRaycastSystem.raycastFlags |= RaycastFlags.BuildingLots;
			m_ToolRaycastSystem.raycastFlags |= RaycastFlags.SubBuildings;
		}

		protected override void OnStartRunning()
		{
			base.OnStartRunning();
			applyAction.enabled = true;
		}

		protected override void OnStopRunning()
		{
			base.OnStopRunning();
			EntityManager.AddComponent<BatchesUpdated>(m_HighlightedQuery);
			EntityManager.RemoveComponent<Highlighted>(m_HighlightedQuery);
			m_PreviousRaycastedEntity = Entity.Null;
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
            EntityCommandBuffer buffer = m_Barrier.CreateCommandBuffer();
            
            if (!GetRaycastResult(out var currentRaycastEntity, out RaycastHit _) && !IsValidPrefab(ref currentRaycastEntity))
            {
                buffer.AddComponent<BatchesUpdated>(m_HighlightedQuery, EntityQueryCaptureMode.AtPlayback);
                buffer.RemoveComponent<Highlighted>(m_HighlightedQuery, EntityQueryCaptureMode.AtPlayback);
                m_PreviousRaycastedEntity = Entity.Null;
                return inputDeps;
            }
            
            if (currentRaycastEntity != m_PreviousRaycastedEntity)
            {
                m_PreviousRaycastedEntity = currentRaycastEntity;
                NativeArray<Entity> entities = m_HighlightedQuery.ToEntityArray(Allocator.Temp);
                buffer.AddComponent<BatchesUpdated>(entities);
                buffer.RemoveComponent<Highlighted>(entities);
            }
            
            if (m_HighlightedQuery.IsEmptyIgnoreFilter)
            {
                buffer.AddComponent<BatchesUpdated>(currentRaycastEntity);
                buffer.AddComponent<Highlighted>(currentRaycastEntity);
                m_PreviousRaycastedEntity = currentRaycastEntity;
            }
            
            if (!applyAction.WasReleasedThisFrame() || m_ToolSystem.selected == Entity.Null)
            {
                return inputDeps;
            }
            
            OnStationSelected(currentRaycastEntity, buffer);
            m_ToolSystem.activeTool = m_DefaultToolSystem;
            return base.OnUpdate(inputDeps);
		}

		private bool IsValidPrefab(ref Entity entity)
		{
			var owner = GetOwner(entity);
			Mod.log.Info("has building " + EntityManager.HasComponent<Building>(owner));
			Mod.log.Info("has transport " + EntityManager.HasComponent<TransportStationData>(owner));
			
			return EntityManager.HasComponent<Building>(entity) && EntityManager.HasComponent<TransportStationData>(entity);
		}

		private Entity GetOwner(Entity currentEntity)
		{
			if (World.DefaultGameObjectInjectionWorld.EntityManager.TryGetComponent<Owner>(currentEntity,
				    out var owner))
			{
				return GetOwner(owner.m_Owner);
			}
			return currentEntity;
		}

		public override bool TrySetPrefab(PrefabBase prefab)
		{
			return false;
		}

		public override PrefabBase GetPrefab()
		{
			return null;
		}
		
		public void StartPicking(Entity pylonEntity)
		{
			m_TargetPylonEntity = pylonEntity;
			m_ToolSystem.activeTool = this;
		}
		
		private void OnStationSelected(Entity selectedStation, EntityCommandBuffer buffer)
		{
			if (m_TargetPylonEntity == Entity.Null)
			{
				return;
			}
			
			if (EntityManager.HasComponent<StationPylonData>(m_TargetPylonEntity))
			{
				var pylonData = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<StationPylonData>(m_TargetPylonEntity);
				pylonData.stationEntity = selectedStation;
				buffer.SetComponent(m_TargetPylonEntity, pylonData);
			}
			else
			{
				var pylonData = new StationPylonData
				{
					stationEntity = selectedStation
				};
				buffer.AddComponent<StationPylonData>(m_TargetPylonEntity);
				buffer.SetComponent(m_TargetPylonEntity, pylonData);
			}
		}
	}
}
