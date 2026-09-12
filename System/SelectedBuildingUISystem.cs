using System.Collections.Generic;
using Colossal.Entities;
using Colossal.UI.Binding;
using Game.Prefabs;
using Game.Tools;
using Game.UI;
using StationPylon.Binding;
using StationPylon.Domain;
using StationPylon.Formulas;
using StationPylon.Utils;
using Unity.Entities;

namespace StationPylon.System
{
    public partial class SelectedBuildingUISystem : ExtendedInfoSectionBase
    {
        
        private NameSystem _nameSystem;
        private BuildingPickerToolSystem _buildingPickerToolSystem;
        private ValueBindingHelper<StationUIPylonData> _stationUiPylonDataBinding;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            m_InfoUISystem.AddMiddleSection(this);
            _nameSystem ??= World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<NameSystem>();
            _stationUiPylonDataBinding = CreateBinding("SelectedPylon", GetPylonData());
            CreateTrigger<StationUIPylonData>("OnPylonDataChanged", OnPylonDataChanged);
            CreateTrigger("OnOpenPicker", OnOpenPicker);
        }

        protected override string group => "StationPylon";

        public override void OnWriteProperties(IJsonWriter writer)
        {
        }

        protected override void OnProcess()
        {
        }

        protected override void Reset()
        {
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            visible = false;
            var buildingName = _nameSystem.GetDebugName(selectedEntity);
            if (buildingName.Contains("NewStationPylonPrism") || buildingName.Contains("NewStationPylonCube"))
            {
                visible = true;
                _stationUiPylonDataBinding.Value = GetPylonData();
            }
      
            RequestUpdate();
        }
        
        private StationUIPylonData GetPylonData()
        {
            if (selectedEntity != Entity.Null && EntityManager.TryGetComponent<StationPylonData>(selectedEntity, out var pylonData))
            {
                var stations = new List<StationUIElement>();
                if (EntityManager.HasBuffer<StationPylonStationElement>(selectedEntity))
                {
                    var buffer = EntityManager.GetBuffer<StationPylonStationElement>(selectedEntity);
                    foreach (var elem in buffer)
                    {
                        if (elem.stationEntity != Entity.Null && EntityManager.Exists(elem.stationEntity) && !EntityManager.HasComponent<Game.Common.Deleted>(elem.stationEntity))
                        {
                            stations.Add(new StationUIElement
                            {
                                Entity = elem.stationEntity,
                                Name = _nameSystem.GetName(elem.stationEntity).Translate()
                            });
                        }
                    }
                }
                else if (pylonData.stationEntity != Entity.Null && EntityManager.Exists(pylonData.stationEntity) && !EntityManager.HasComponent<Game.Common.Deleted>(pylonData.stationEntity))
                {
                    stations.Add(new StationUIElement
                    {
                        Entity = pylonData.stationEntity,
                        Name = _nameSystem.GetName(pylonData.stationEntity).Translate()
                    });
                }
                
                var selectedBuildingName = stations.Count > 0 ? stations[0].Name : "";
                
                return new StationUIPylonData
                {
                    TransportType = (int) pylonData.lineType,
                    SelectedBuildingName = selectedBuildingName,
                    ShowWheelchair = pylonData.showWheelchair,
                    UsePylonCustomName = pylonData.usePylonCustomName,
                    SmallIcons = pylonData.smallIcons,
                    StationEntity = pylonData.stationEntity,
                    Stations = stations.ToArray()
                };
            }
            return new StationUIPylonData();
        }

        private void OnPylonDataChanged(StationUIPylonData data)
        {
            DynamicBuffer<StationPylonStationElement> buffer;
            if (EntityManager.HasBuffer<StationPylonStationElement>(selectedEntity))
            {
                buffer = EntityManager.GetBuffer<StationPylonStationElement>(selectedEntity);
            }
            else
            {
                buffer = EntityManager.AddBuffer<StationPylonStationElement>(selectedEntity);
            }
            
            buffer.Clear();
            if (data.Stations != null)
            {
                foreach (var s in data.Stations)
                {
                    if (s.Entity != Entity.Null && EntityManager.Exists(s.Entity) && !EntityManager.HasComponent<Game.Common.Deleted>(s.Entity))
                    {
                        buffer.Add(new StationPylonStationElement(s.Entity));
                    }
                }
            }
            
            var primaryStation = buffer.Length > 0 ? buffer[0].stationEntity : Entity.Null;
            
            var pylonData = new StationPylonData
            {
                showWheelchair = data.ShowWheelchair,
                usePylonCustomName = data.UsePylonCustomName,
                smallIcons = data.SmallIcons,
                lineType = (TransportLineType) data.TransportType,
                stationEntity = primaryStation
            };
            
            if (EntityManager.HasComponent<StationPylonData>(selectedEntity))
            {
                EntityManager.SetComponentData(selectedEntity, pylonData);
            }
            else
            {
                EntityManager.AddComponent<StationPylonData>(selectedEntity);
                EntityManager.SetComponentData(selectedEntity, pylonData);
            }
            
            _stationUiPylonDataBinding.Value = GetPylonData();
        }

        private void OnOpenPicker()
        {
            _buildingPickerToolSystem ??= World.GetOrCreateSystemManaged<BuildingPickerToolSystem>();
            _buildingPickerToolSystem.StartPicking(selectedEntity);
        }
    }
}