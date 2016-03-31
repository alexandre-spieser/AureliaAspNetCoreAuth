import {inject} from 'aurelia-framework';
import {SignalRService} from "./signalRSvc";
import {AuthService} from './authSvc';
import {Router} from 'aurelia-router';
import {Validation} from 'aurelia-validation';

let _router;
let _signalRService;
let _authService;
let _validation;

@inject(SignalRService, AuthService, Router, Validation)
export class Login {
    data = {};
    message = "";

    constructor(signalRService, authService, router, validation) {
        _signalRService = signalRService;
        _authService = authService;
        _router = router;
        this.validation = validation.on(this)
                                    .ensure("email")
                                        .isEmail()
                                        .isNotEmpty()
                                        .containsNoSpaces()
                                        .hasMinLength(3)
                                        .hasMaxLength(50)
                                    .ensure("password")
                                        .isNotEmpty()
                                        .containsNoSpaces()
                                        .hasMinLength(3)
                                        .hasMaxLength(50);
        this.message = "";
        this.email = "";
        this.password = "";
    }

    activate()
    {
    }

    login()
    {
        var _this = this;
        this.validation.validate().then(() => {
            _authService.login(_this.email, _this.password)
                   .then( 
                   function(response){
                       console.log("response", response);
                       var res = _authService.setToken(response);
                       if(!res){
                           _this.message = "Could not save token.";
                           return;
                       }
                       var connection = 
                           _signalRService.connectToSignalR(response.access_token);
                       connection.start().done( 
                           function(){
                               _router.navigate("movies");
                           }
                       );                           
                   }, 
                   function(error){
                       console.log(error);
                       _this.message = "An error occured at login.";
                   });

        });

    }

}