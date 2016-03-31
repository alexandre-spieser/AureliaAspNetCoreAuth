import {inject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-fetch-client';
import 'fetch';

let movieUrl = "/movies";

@inject(HttpClient)
export class Users {
    heading = 'My secret movies';
    users = [];

    constructor(http) {
        this.http = http;
    }

    activate() {
        return this.http.fetch(movieUrl)
          .then(users => this.movies = users);
    }
}