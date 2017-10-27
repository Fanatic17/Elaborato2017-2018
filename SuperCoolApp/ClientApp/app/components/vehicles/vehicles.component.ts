import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'vehicles',
    templateUrl: './vehicles.component.html'
})
export class VehiclesComponent {
    public vehicles: Vehicle[];

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        http.get(baseUrl + 'api/vehicles').subscribe(result => {
            this.vehicles = result.json() as Vehicle[];
        }, error => console.error(error));
    }
}

interface Vehicle {
    id: string;
	brand: string;
    model: string;
	plate: string;
	price: string;
}
