import { Component, Inject } from '@angular/core';
import { Headers, Http, RequestOptions } from '@angular/http';
import { Observable } from "rxjs/Observable";
import 'rxjs/add/observable/forkJoin';

@Component({
    selector: 'vehicles',
    templateUrl: './vehicles.component.html',
    styleUrls: ['./vehicles.component.css']
})
export class VehiclesComponent {
    public vehicles: Vehicle[];
    public selectedVehicle: Vehicle;

    constructor(private http: Http, @Inject('BASE_URL') private baseUrl: string) {
        this.refreshData();
    }



    async refreshData() {
        this.http.get(this.baseUrl + 'api/vehicles').subscribe(result => {
            let vehicleList = [];

            for (let car of result.json() as Vehicle[]) {

                let vehicle = new Vehicle();
                vehicle.id = car.id;
                vehicle.brand = car.brand;
                vehicle.model = car.model;
                vehicle.plate = car.plate;
                vehicle.price = car.price;
                vehicle.hasChanges = false;
                vehicleList.push(vehicle);
            }

            console.log("ok");

            this.vehicles = vehicleList;

            this.selectVehicle();
        }, error => console.error(error));
    }
	//Questo è il costruttore
	//Refreshes the list of vehicles 

    selectVehicle(): void {

       // this.selectedVehicle = undefined;

        for (let car of this.vehicles) {
            if (car.deleted == false) {
                this.selectedVehicle = car;
                break;
            }

        }
    }


    async putData(): Promise<void> {
        let headers = new Headers({ 'Content-Type': 'application/json' });

        let serverCalls = [];

        for (let vehicle of this.vehicles) {
            if (vehicle.hasChanges == true || vehicle.deleted) {

                let json = JSON.stringify(vehicle.toJSON());

                if (!vehicle.id) { //create
                    if (!vehicle.deleted) {
                        let call = this.http.put(this.baseUrl + 'api/vehicles', json, { headers: headers });
                        serverCalls.push(call);
                    }
                }
                else {
                    if (vehicle.deleted) {//delete
                        let url = this.baseUrl + 'api/vehicles?id=' + vehicle.id;
                        let call = this.http.delete(url, { headers: headers });
                        serverCalls.push(call);
                    }
                    else {//modify
                        let call = this.http.post(this.baseUrl + 'api/vehicles', json, { headers: headers });
                        serverCalls.push(call);
                    }

                }
            }
        }
        Observable.forkJoin(serverCalls)
            .subscribe(data => {
                this.refreshData();
            }, error => console.error(error));


    }
	
	
	//Salva i cambiamenti 

    onSelect(vehicle: Vehicle): void {

        if (vehicle.deleted == false) {
            this.selectedVehicle = vehicle;
        }
    }

    addNewVehicle(): void {
        this.selectedVehicle = new Vehicle();
        this.selectedVehicle.hasChanges = true;
        this.vehicles.push(this.selectedVehicle);
    }

    async saveChanges(): Promise<void> {
        await this.putData();
        console.log("update completed");
        console.log(this.selectedVehicle.toJSON());
        //await this.refreshData();
    }

    delete(vehicle: Vehicle): void {
        console.log("CIAO");
        vehicle.deleted = true;
        this.selectVehicle();
    }
}

class Vehicle {
    id: number;

    private _brand: string = "";
    private _model: string = "";
	private _plate: string = "";
    private _price: string = "";
    public hasChanges: boolean;
    public deleted: boolean = false;


    //constructor() {
    //    this.brand = "brand";
    //    this.model = "model";
    //}

    get brand(): string {
        return this._brand;
    }
    set brand(b: string) {
        this._brand = b;
        this.hasChanges = true;
        console.log("set brand");
    }

    get model(): string {
        return this._model;
    }
    set model(m: string) {
        this._model = m;
        this.hasChanges = true;
        console.log("set model");
    }

	 get plate(): string {
        return this._plate;
    }
    set plate(p: string) {
        this._plate = p;
        this.hasChanges = true;
        console.log("set plate");
    }

    get price(): string {
        return this._price;
    }
    set price(p: string) {
        this._price = p;
        this.hasChanges = true;
        console.log("set price");
    }




    public toJSON() {
        return {
           "id": "this.id",
           "model": "this.model",
           "brand": "this.brand",
           "plate": "this.plate",
           "price": "this.price",
        };
    };
}
