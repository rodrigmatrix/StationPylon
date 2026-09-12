export interface StationPylonData {
    SelectedBuildingName: string;
    ShowWheelchair: boolean;
    SmallIcons: boolean;
    UsePylonCustomName: boolean;
    TransportType: number;
    StationEntity: any;
    Stations: StationUIElement[];
}

export interface StationUIElement {
    Entity: any;
    Name: string;
}