import {inject, NewInstance} from 'aurelia-dependency-injection';
import {ValidationController, validateTrigger} from 'aurelia-validation';
import {required, email, ValidationRules} from 'aurelia-validatejs';

import {SignalRService} from "./signalRSvc";
import {AuthService} from './authSvc';
import {Router} from 'aurelia-router';
 



@inject(SignalRService, AuthService, Router, NewInstance.of(ValidationController))
export class Login {
    
    constructor(signalRService, authService, router, validationController) {

        this.signalRService = signalRService;
        this.authService = authService;
        this.router = router;

        // the default mode is validateTrigger.blur but 
        // you can change it:
        // validationController.validateTrigger = validateTrigger.manual;
        // validationController.validateTrigger = validateTrigger.change;
        // validationController.validateTrigger = validateTrigger.change;  
        this.validationController = validationController;
        this.email = "";
        this.password = "";
        this.message = "";
    }

    activate()
    {
    }

    login()
    {
        var _this = this;
        
        let errors = this.validationController.validate()
        this.authService
            .login(_this.email, _this.password)
            .then( 
            function(response){
                console.log("response", response);
                var res = _this.authService.setToken(response);
                if(!res){
                    _this.message = "Could not save token.";
                    return;
                }
                var connection = _this.signalRService
                                        .connectToSignalR(response.access_token);
                connection.start().done( 
                    function(){
                        _this.router.navigate("movies");
                    }
                );                           
            }, 
            function(error){
                console.log(error);
                _this.message = "An error occured at login.";
            });
    }
}

ValidationRules  
    .ensure("email")
        .email()
        .required()
        //.isNotEmpty()
        //.containsNoSpaces()
        //.hasMinLength(3)
        //.hasMaxLength(50)
     .ensure("password")
        .required()
        //.isNotEmpty()
        //.containsNoSpaces()
        //.hasMinLength(3)
        //.hasMaxLength(50)
        .on(Login);
