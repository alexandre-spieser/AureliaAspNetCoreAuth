export class ApplicationState { 
    
    constructor () {
        this.isAuthenticated = false;
        this.isConnected = false;
        this.baseUrl = "http://localhost:5001/";
        this.loginRoute = "login";
        this.loginRedirect = "movies";
        this.logoutRedirect = "login";
        this.clientId = "AureliaNetAuthApp";
    }

}