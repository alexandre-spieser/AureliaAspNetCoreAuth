import {inject} from 'aurelia-dependency-injection';
import {Router, Redirect} from 'aurelia-router';
import {ApplicationState} from './applicationState';
import {AuthService} from './authSvc';

@inject(ApplicationState, AuthService)
export class AuthorizeStep {

    constructor(applicationState, authService) {
        this.appState = applicationState;
        this.auth = authService;
    }

    isLoggedIn(){
        return this.auth.isAuthenticated();
    }

    run(routingContext, next) {
        var isLoggedIn = this.auth.isAuthenticated();
        console.log("isLoggedIn", isLoggedIn);
        var loginRoute = this.appState.loginRoute;
        var loginRedirect = this.appState.loginRedirect;

        if (routingContext.getAllInstructions().some(i => i.config.auth)) {
            if (!isLoggedIn) {
                this.auth.setInitialUrl(window.location.href);
                return next.cancel(new Redirect(loginRoute));
            }
        } else if (isLoggedIn && routingContext.getAllInstructions().some(i => i.fragment) === loginRoute) {
            return next.cancel(new Redirect(loginRedirect));
        }

        return next();
    }
}