# AureliaAspNetCoreAuth

This single page application is an example of token based authentication implementation using the Aurelia front end framework and ASP.NET core.

It uses OpenId to generate tokens after a successful login.

The Token is then used to connect to a SignalR persistent connection, therefore establishing an authenticated real-time connection. The status of the user is broadcasted and if another client connects, it is communicated to all the online clients.

## Limitations
* There is no database implementation, this project is supposed to be used as a boiler plate for you to build on top of using the database technology you find the most suitable for your application.
* There is no refresh token logic.

## Credits and references
* Toastr https://github.com/CodeSeven/toastr
* OpenIdConnect Samples https://github.com/aspnet-contrib/AspNet.Security.OpenIdConnect.Server
* SignalR-Server samples https://github.com/aspnet/SignalR-Server
* Aurelia Auth https://github.com/paulvanbladel/aurelia-auth
* Building Applications with Aurelia by Scott Allen https://app.pluralsight.com/library/courses/building-applications-aurelia/table-of-contents

## Copyright
Copyright © 2016

## License
AureliaAspNetCoreAuth is under MIT license - http://www.opensource.org/licenses/mit-license.php

## Author
**Alexandre Spieser**

## Donations

Feeling like my answer is worth a coffee? BTC Donations: 1Qc5ZpNA7g66KEEMcz7MXxwNyyoRyKJJZ
Greatly appreciated!




