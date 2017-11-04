import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'users',
    templateUrl: './users.component.html',
    styleUrls: ['./users.component.css']
})
export class UsersComponent {
    public users: User[];

    constructor(http: Http, @Inject('BASE_URL') baseUrl: string) {
        http.get(baseUrl + 'api/users').subscribe(result => {
            this.users = result.json() as User[];
        }, error => console.error(error));
    }
}

interface User {
    name: string;
	lastName: string;
    birthDate: Date;
	address: string;
	ownedVehicles: string; //expansion: create vehicle object and make this a vehicle array
}
