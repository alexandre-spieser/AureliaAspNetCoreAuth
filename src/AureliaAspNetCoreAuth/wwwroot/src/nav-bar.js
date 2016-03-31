import {inject} from 'aurelia-framework';
import {Router} from 'aurelia-router';
import {ApplicationState} from './applicationState';
import {AuthService} from './authSvc';

@inject(ApplicationState, Router, AuthService)
export class NavBar {

    constructor(applicationState, router, authService){
        this.state = applicationState;
        this.router = router;
        this.auth = authService;
    }

    logout(){
        this.auth.logout();
    }

}


