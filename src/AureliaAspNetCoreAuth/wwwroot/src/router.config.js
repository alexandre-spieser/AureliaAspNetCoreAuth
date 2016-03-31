import {inject} from 'aurelia-framework';
import {AuthorizeStep} from './authorizeStep';
import {AuthService} from './authSvc';
import {Router} from 'aurelia-router';
import {ApplicationState} from './applicationState';


@inject(Router, AuthService, ApplicationState, AuthorizeStep)
export default class{

	constructor(router, authService, applicationState, authorizeStep){
	    this.router = router;
	    this.authService = authService;
	    this.appState = applicationState;
	    this.authorizeStep = authorizeStep;
	}

	configure(){
	    var appState = this.appState;
	    var authorizeStep = this.authorizeStep;

        var appRouterConfig = function(config){

            config.title = 'Aurelia ASP.NET Core';
            config.addPipelineStep('authorize', AuthorizeStep); // Add a route filter to the authorize extensibility point.
        
            config.map([
              { route: ['','welcome'], name: 'welcome', moduleId: './welcome', nav: true, title:'Welcome' },
              { route: 'movies', name: 'movies', moduleId: './movies', nav: true, title:'Movies', auth: true }, // set auth to true to trigger authentication
              { route: 'child-router',  name: 'child-router', moduleId: './child-router', nav: true, title:'Child Router' },
              { route: 'login',  name: 'login', moduleId: './login', nav: false, title:'Login' }
            ]);
        };

        this.router.configure(appRouterConfig);	
    }

}