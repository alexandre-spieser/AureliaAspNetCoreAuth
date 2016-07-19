import {inject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-fetch-client';
import {ApplicationState} from './applicationState';
import {Router} from 'aurelia-router';
import {Storage} from './storage';
import {SignalRService} from "./signalRSvc";
import utilsSvc from './utilsSvc';


@inject(HttpClient, ApplicationState, Router, Storage, SignalRService)
export class AuthService{

    constructor(httpClient, applicationState, router, storage, signalRService) {

        this.router = router;
        this.appState = applicationState;
        this.storage = storage;
        this.http = httpClient;
        this.signalRService = signalRService;
        this.tokenName = "token";
    }

    //#region login
    login(username, password) {
        let baseUrl = this.appState.baseUrl;

        let data = "client_id=" + this.appState.clientId
                   + "&grant_type=password"
                   + "&username=" + username
                   + "&password=" + password
                   + "&resource=" + encodeURIComponent(baseUrl);

        return this.http.fetch(baseUrl + 'connect/token', {
            method: 'post',
            body : data
        });
    }
    //#endregion

    //#region setInitialUrl
    setInitialUrl(url) {
        this.initialUrl = url;
    }
    //#endregion setInitialUrl

    //#region getToken
    getToken() {
        return this.storage.get(this.tokenName);
    }
    //#endregion

    //#region setToken
    setToken(response) {
        //access token handling             
        let accessToken = response && response["access_token"];
        let tokenToStore;

        if (accessToken) {
            tokenToStore = accessToken;
        }

        if (!tokenToStore) {
            console.log("could not find token!");
            return false;
        }

        if (tokenToStore) {
            this.storage.set(this.tokenName, tokenToStore);
            this.appState.isAuthenticated = true;
            return true;
        }
    }
    //#endregion

    //#region removeToken
    removeToken() {
        this.storage.remove(this.tokenName);
    }
    //#endregion removeToken

    //#region isAuthenticated
    isAuthenticated() {
        let token = this.storage.get(this.tokenName);

        // There's no token, so user is not authenticated.
        if (!token) {
            this.appState.isAuthenticated = false;
            return false;
        }

        // There is a token, but in a different format. Return true.
        if (token.split('.').length !== 3) {
            this.appState.isAuthenticated = true;
            this.signalRService.connectToSignalR(token);
            return true;
        }

        let exp;
        try {
            let base64Url = token.split('.')[1];
            let base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
            exp = JSON.parse(window.atob(base64)).exp;
        } catch (error) {
            this.appState.isAuthenticated = false;
            return false;
        }

        if (exp) {
            let isNotExpired = Math.round(new Date().getTime() / 1000) <= exp;
            this.appState.isAuthenticated = isNotExpired;
            if(isNotExpired && !this.appState.isConnected) {
                var connection = this.signalRService.connectToSignalR(token);
                connection.start().then(
                    () => {}, 
                    () => {
                        console.log("Could not start connection, logging out.");
                        this.logout();
                    }
                );
            }
            return isNotExpired;
        }

        this.appState.isAuthenticated = true;
        return true;
    }
    //#endregion

    //#region logout
    logout(redirect) {
        var logoutRedirect = this.appState.logoutRedirect;
        return new Promise(resolve => {

            this.storage.remove(this.tokenName);
            this.appState.isAuthenticated = false;
            this.signalRService.disconnectFromSignalR();

            if (logoutRedirect && !redirect) {
                this.router.navigate(logoutRedirect);
            } else if (utilsSvc.isString(redirect)) {
                this.router.navigate(redirect);
            }

            resolve();
        });
    }
    //#endregion
}