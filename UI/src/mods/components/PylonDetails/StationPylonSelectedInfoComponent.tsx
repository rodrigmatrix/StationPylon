import {Button, Dropdown, DropdownItem, DropdownToggle, Scrollable, Tooltip} from "cs2/ui";
import styles from "./StationPylonSelectedInfoComponent.module.scss";
import classNames from "classnames";
import {useValue} from "cs2/api";
import { getModule } from "cs2/modding";
import { Theme } from "cs2/bindings";
import {CheckBoxWithLine} from "../checkbox/checkbox";
import PickerIcon from "../../../images/Picker.png";
import trashIcon from "../../../images/Trash.svg";
import { FormLine } from "../form-line/form-line";
import {TransportType} from "../../domain/TransportType";
import {OnPylonDataChanged, SelectedPylon, OnOpenPicker} from "../../bindings";
import { VanillaComponentResolver } from "../vanilla-component/vanilla-components";
import { StationPylonData } from "mods/domain/StationPylonData";

interface InfoSectionComponent {
	group: string;
	tooltipKeys: Array<string>;
	tooltipTags: Array<string>;
}

const InfoSectionTheme: Theme | any = getModule(
	"game-ui/game/components/selected-info-panel/shared-components/info-section/info-section.module.scss",
	"classes"
);

const InfoRowTheme: Theme | any = getModule(
	"game-ui/game/components/selected-info-panel/shared-components/info-row/info-row.module.scss",
	"classes"
)

const InfoSection: any = getModule( 
    "game-ui/game/components/selected-info-panel/shared-components/info-section/info-section.tsx",
    "InfoSection"
)

const InfoRow: any = getModule(
    "game-ui/game/components/selected-info-panel/shared-components/info-row/info-row.tsx",
    "InfoRow"
)

const DropdownStyle: Theme | any = getModule("game-ui/menu/themes/dropdown.module.scss", "classes");


const StationPylonRow = () => {
	const selectedPylon = useValue(SelectedPylon);
	const profileOptions = Object.entries(TransportType)
		.map(([name, id]) => (
			<div key={id} className={ selectedPylon.TransportType == id ? styles.selectedDropdownProfileItem : styles.profileBox}>
				<DropdownItem
					theme={DropdownStyle}
					value={name}
					closeOnSelect={true}
					onChange={() => {
						const updatedPylon = { ...selectedPylon, TransportType: id };
						OnPylonDataChanged(updatedPylon)
					}}
					className={styles.dropdownName}>
					<div>
						{ name }
					</div>
				</DropdownItem>
			</div>
		))
    return (
		<div>
				<div className={styles.row} style={{ display: 'flex', alignItems: 'center', marginLeft: 24, marginRight: 24 }}>
						<span style={{ flex: 1 }}>{ "Line Type" }</span>
						<Dropdown
							theme={DropdownStyle}
							content={profileOptions}>
							<DropdownToggle>
								{ Object.keys(TransportType).find(key => TransportType[key as keyof typeof TransportType] === selectedPylon.TransportType) || "All" }
							</DropdownToggle>
						</Dropdown>
					</div>
					<FormLine title={"Select Building"}>
						<div style={{ display: 'flex'}}>
							<VanillaComponentResolver.instance.ToolButton
								selected={true}
								multiSelect={false}
								src={PickerIcon}
								tooltip={"Pick Building"}
								className={classNames(
									VanillaComponentResolver.instance.toolButtonTheme.button
								  )}
								onSelect={() => { OnOpenPicker() }}
							/>
						</div>
					</FormLine>
					
					{selectedPylon.Stations && selectedPylon.Stations.length > 0 ? (
						<div style={{ marginLeft: 26, marginRight: 24, marginBottom: 8 }}>
							<div style={{ fontWeight: 'bold', marginBottom: 4 }}>Selected Stations:</div>
							{selectedPylon.Stations.map((station, idx) => (
								<div key={idx} style={{ display: 'flex', alignItems: 'center', marginBottom: 4 }}>
									<span style={{ flex: 1, color: "green", fontWeight: 500 }}>{station.Name}</span>
									<Tooltip tooltip={"Remove Station"}>
										<img
											src={trashIcon}
											className={styles.removeButton}
											onClick={() => {
												const updatedStations = selectedPylon.Stations.filter((_, i) => i !== idx);
												const updatedPylon = {
													...selectedPylon,
													Stations: updatedStations,
													StationEntity: updatedStations.length > 0 ? updatedStations[0].Entity : { index: 0, version: 0 }
												};
												OnPylonDataChanged(updatedPylon);
											}}
										/>
									</Tooltip>
								</div>
							))}
						</div>
					) : (
						<div style={{ display: 'flex', alignItems: 'center', marginLeft: 26, marginRight: 24, marginBottom: 8 }}>
							<span style={{ flex: 1 }}>{"Selected building:"}</span>
							<span style={{ color: "red", fontWeight: 500 }}>{"None"}</span>
						</div>
					)}
					<CheckBoxWithLine
						title={"Show Wheelchair Icon"}
						isChecked={selectedPylon.ShowWheelchair}
						onValueToggle={(value) => {
							const updatedPylon = { ...selectedPylon, ShowWheelchair: value };
							OnPylonDataChanged(updatedPylon)
						}}/>
					<CheckBoxWithLine
						title={"Use Pylon Custom Name"}
						isChecked={selectedPylon.UsePylonCustomName}
						onValueToggle={(value) => {
							const updatedPylon = { ...selectedPylon, UsePylonCustomName: value };
							OnPylonDataChanged(updatedPylon)
						}}/>
					<CheckBoxWithLine
						title={"Use Small Icons"}
						isChecked={selectedPylon.SmallIcons}
						onValueToggle={(value) => {
							const updatedPylon = { ...selectedPylon, SmallIcons: value };
							OnPylonDataChanged(updatedPylon)
						}}/>
			 </div>
    );
}
export const StationPylonSelectedInfoComponent = (componentList: any): any => {
	componentList["StationPylon.System.SelectedBuildingUISystem"] = (e: InfoSectionComponent) => {
		return <StationPylonRow />
	}
	return componentList as any;
}
