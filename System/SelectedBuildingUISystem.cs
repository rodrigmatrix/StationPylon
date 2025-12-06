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
                var selectedBuildingName = "";
                
                if (pylonData.stationEntity != Entity.Null)
                {
                    selectedBuildingName = _nameSystem.GetName(pylonData.stationEntity).Translate();
                }
                
                return new StationUIPylonData
                {
                    TransportType = (int) pylonData.lineType,
                    SelectedBuildingName = selectedBuildingName,
                    ShowWheelchair = pylonData.showWheelchair,
                    UsePylonCustomName = pylonData.usePylonCustomName,
                    SmallIcons = pylonData.smallIcons,
                    StationEntity = pylonData.stationEntity
                };
            }
            return new StationUIPylonData();
        }

        private void OnPylonDataChanged(StationUIPylonData data)
        {
            var pylonData = new StationPylonData
            {
                showWheelchair = data.ShowWheelchair,
                usePylonCustomName = data.UsePylonCustomName,
                smallIcons = data.SmallIcons,
                lineType = (TransportLineType) data.TransportType,
                stationEntity = data.StationEntity
            };
            if (EntityManager.TryGetComponent<StationPylonData>(selectedEntity, out var stationPylonData))
            {
                pylonData.stationEntity = stationPylonData.stationEntity;
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