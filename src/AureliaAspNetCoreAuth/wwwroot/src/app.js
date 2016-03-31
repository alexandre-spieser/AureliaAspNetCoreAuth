import {inject} from 'aurelia-framework';
import {Router} from 'aurelia-router';
import {FetchConfig} from './fetch-httpClient.config';
import AppRouterConfig from './router.config';


@inject(FetchConfig, AppRouterConfig, Router)
export class App {

    constructor(fetchConfig, appRouterConfig, router) {
        this.fetchConfig = fetchConfig;
        this.appRouterConfig = appRouterConfig;
        this.router = router;
    }

    activate() {
        this.fetchConfig.configure();
        this.appRouterConfig.configure();
        //
    }
}
