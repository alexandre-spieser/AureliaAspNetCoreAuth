import {inject} from 'aurelia-framework';
import {HttpClient} from 'aurelia-fetch-client';
import {ApplicationState} from './applicationState';
import {Router} from 'aurelia-router';
import {Storage} from './storage';

let connectionPath = "";
let connection;
let _this;

@inject(HttpClient, ApplicationState, Router, Storage)
export class SignalRService{

    constructor(httpClient, applicationState, router, storage) {
        this.router = router;
        this.appState = applicationState;
        this.storage = storage;
        this.http = httpClient;
        connectionPath = this.appState.baseUrl + "signalr";
        _this = this;
    }

    sayHello(data) {
        console.log("Say hello!", data);
        toastr.success(data.Content, "Yay");
    }

    connected(data) {
        toastr.info(data.Content, "Connected");
    }

    reconnected(data) {
        toastr.info(data.Content, "Reconnected");
    }

    disconnected(data) {
        toastr.warning(data.Content, "Disconnected");
    }

    disconnectFromSignalR() {
        connection.stop();
    }

    connectToSignalR (token) {
        var sayHello = this.sayHello;
        var disconnected = this.disconnected;
        var reconnected = this.reconnected;
        var connected = this.connected;
        connection = $.connection(connectionPath, "access_token=" + token);

        var log = function (data) { console.log(data); }

        connection.error(function (error) {
            _this.appState.isConnected = false;
            toastr.error("Connection error to real-time hub.", "Error");
            console.log('SignalR error: ' + error);
        });

        connection.received(function (data) {
            console.log("data", data);
            var response = window.JSON.parse(data);
            
            switch(response.Method){

                case "welcome":
                    sayHello(response);
                    break;

                case "connected":
                    connected(response);
                    break;

                case "reconnected":
                    reconnected(response);
                    break;

                case "disconnected":
                    disconnected(response);
                    break;

                default:
                    toastr.error("Something went wrong.", "Error");
            }
        });

        connection.stateChanged(function (change) {
            switch (change.newState) {
                case $.signalR.connectionState.reconnecting:
                    log('Re-connecting');
                    break;
                case $.signalR.connectionState.connected:
                    log("Connected to " + connection.url);
                    _this.appState.isConnected = true;
                    break;
                case $.signalR.connectionState.disconnected:
                    log("Disconnected");
                    _this.appState.isConnected = false;
                    break;
            }
        });

        return connection;
    }
}