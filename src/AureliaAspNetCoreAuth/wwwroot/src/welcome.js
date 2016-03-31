import {inject} from 'aurelia-framework';

export class Welcome {
    heading = 'Welcome to Aurelia!';
    firstName = 'John';
    lastName = 'Doe';

    constructor(){
    }

    activate(){
        console.log("activated welcome");
    }

    get fullName() {
        return `${this.firstName} ${this.lastName}`;
    }

    submit() {
        alert(`Welcome, ${this.fullName}!`);
    }
}