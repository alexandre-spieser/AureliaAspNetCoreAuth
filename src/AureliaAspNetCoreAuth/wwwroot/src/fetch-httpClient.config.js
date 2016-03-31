import {inject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-fetch-client';
import 'fetch';
import {Storage} from './storage';
import {ApplicationState} from './applicationState';
import {AuthService} from './authSvc';


let _this;

@inject(HttpClient, Storage, ApplicationState, AuthService)
export class FetchConfig {
    constructor(httpClient, storage, applicationState, authService) {
        this.httpClient = httpClient;
        this.storage = storage;
        this.appState = applicationState;
        this.auth = authService;
        _this = this;
    }

    configure() {

        this.httpClient.configure(httpConfig => {
            httpConfig
              .withDefaults({
                  credentials: 'same-origin',
                  headers: {
                      'Content-Type': 'application/x-www-form-urlencoded',
                      'Accept': 'application/json',
                      'X-Requested-With': 'Fetch'
                  }
              })
              .withInterceptor({
                  request(request) {
                      if (_this.auth.isAuthenticated()) {
                          var token = _this.auth.getToken();
                          var header;
                          if (token) {
                              header ='Bearer ' + token;
                          }
                          request.headers.append('Authorization', header);
                      }
                      return request; // you can return a modified Request, or you can short-circuit the request by returning a Response
                  },
                  response(response) {
                      response = response.json();
                      return response; // you can return a modified Response
                  }
              });
        });

    }
}