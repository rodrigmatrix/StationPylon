import {bindValue, trigger} from "cs2/api";
import {StationPylonData} from "./domain/StationPylonData";
import mod from "mod.json";

export const SelectedPylon = bindValue<StationPylonData>(mod.id, "SelectedPylon", undefined);

export const OnPylonDataChanged = (pylonData: StationPylonData) => trigger(mod.id, "OnPylonDataChanged", pylonData);

export const OnOpenPicker = () => trigger(mod.id, "OnOpenPicker");
