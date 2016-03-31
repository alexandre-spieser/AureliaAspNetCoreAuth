using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace AureliaAspNetCoreAuth.Connections
{

    public class MessageToClient
    {
        public string Method { get; set; }
        public string Content { get; set; }

        public MessageToClient(string method, string content)
        {
            Method = method;
            Content = content;
        }
    }


    public class RawConnection : PersistentConnection
    {
        private static readonly ConcurrentDictionary<string, string> _users = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<string, string> _clients = new ConcurrentDictionary<string, string>();


        protected override async Task OnConnected(HttpRequest request, string connectionId)
        {
            var identity = request.HttpContext.User.Identity;
            var status = identity.IsAuthenticated ? "Authenticated" : "Unauthenticated";

            Logger.LogInformation($"{status} connection {connectionId} has just connected.");

            var userName = identity.Name;
            if (!string.IsNullOrEmpty(userName))
            {
                _clients[connectionId] = userName;
                _users[userName] = connectionId;
            }

            string clientIp = GetClientIP(request);

            string user = GetUser(connectionId);

            await Groups.Add(connectionId, "AureliaAuthUsers");

            var data = new MessageToClient("welcome", $"Connection is {status}, hello {userName}!");

            var message = JsonConvert.SerializeObject(data);

            await Connection.Send(connectionId, message);

            data = new MessageToClient("connected", $"{DateTime.Now.ToString("MM-dd-HH-mm-ss")}: {user} connected");
            message = JsonConvert.SerializeObject(data);
            await Connection.Broadcast(message);
        }

        protected override Task OnReconnected(HttpRequest request, string connectionId)
        {
            string user = GetUser(connectionId);

            var data = new MessageToClient("reconnected", $"{DateTime.Now.ToString("MM-dd-HH-mm-ss")}: {user} reconnected");
            var message = JsonConvert.SerializeObject(data);
            return Connection.Broadcast(message);
        }

        protected override Task OnDisconnected(HttpRequest request, string connectionId, bool stopCalled)
        {
            string ignored;
            _users.TryRemove(connectionId, out ignored);
            string suffix = stopCalled ? "cleanly" : "uncleanly";
            string msg = DateTime.Now.ToString("MM-dd-HH-mm-ss") + ": " + GetUser(connectionId) + " disconnected " + suffix;
            var data = new MessageToClient("disconnected", msg);
            var message = JsonConvert.SerializeObject(data);
            return Connection.Broadcast(message);
        }

        private string GetUser(string connectionId)
        {
            string user;
            if (!_clients.TryGetValue(connectionId, out user))
            {
                return connectionId;
            }
            return user;
        }

        private string GetClient(string user)
        {
            string connectionId;
            if (_users.TryGetValue(user, out connectionId))
            {
                return connectionId;
            }
            return null;
        }

        private static string GetClientIP(HttpRequest request)
        {
            var conn = request.HttpContext.Features.Get<IHttpConnectionFeature>();
            return conn != null ? conn.RemoteIpAddress.ToString() : null;
        }
    }
}
