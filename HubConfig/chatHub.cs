using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;
using System;
using VedioChatApp_Server_.DTO_s;
using VedioChatApp_Server_.Models;

namespace VedioChatApp_Server_.HubConfig
{
    public class chatHub:Hub
    {
        private static Dictionary<string, List<string>> connectionIds = new Dictionary<string, List<string>>();


        private readonly chat_AppContext _appContext;

        public string? signalRiDG
        {
            get => Context.Items.TryGetValue("signalRiDG", out var value) ? value?.ToString() : null;
            set => Context.Items["signalRiDG"] = value;
        }


        public chatHub(chat_AppContext chat_AppContext)
        {
            _appContext = chat_AppContext;
   
        }

        public override async Task OnConnectedAsync()
        {
            var signalRi = Context.GetHttpContext().Request.Query["singlaRId"];
            signalRiDG = !StringValues.IsNullOrEmpty(signalRi)
            ? signalRi.ToString()?.Split(' ').Select(s => s.Trim()).Distinct().FirstOrDefault()
             : null;


            if (!string.IsNullOrEmpty(signalRiDG))
            {
                if(!connectionIds.ContainsKey(signalRiDG))
                {
                    connectionIds[signalRiDG] = new List<string>();
                }
                //add key as user id and value's are list of connection id for a particular user id
                connectionIds[signalRiDG].Add(Context.ConnectionId);
                
            }
            await base.OnConnectedAsync();
        }



        public override async Task OnDisconnectedAsync(Exception exception)
        {
    
            string userId = this.signalRiDG;
            if(!string.IsNullOrEmpty(userId)&&connectionIds.ContainsKey(userId))
            {
                connectionIds[userId].Remove(Context.ConnectionId);
                if (connectionIds[userId].Count==0)
                {
                    connectionIds.Remove(userId);
                }
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task getconn()
        {
            
            var connectionId = Context.ConnectionId;
            
            await Clients.Client(connectionId).SendAsync("reciveid", connectionId, this.signalRiDG);
        }


        public async Task SendMessage(string reciverConnId, int rcvUserId, int sndUserId, string message)
        {
            var senderconnectionId = Context.ConnectionId;

            var data = new Message
            {
                SenderId = sndUserId,
                ReceiverId = rcvUserId,
                Content = message,
                SentAt = DateTime.Now,
            };

            var messageResp = new MessageResp
            {
                MessageId = 0,
                SenderId = sndUserId,
                ReceiverId = rcvUserId,
                Content = message,
                SentAt = DateTime.Now,
                SenderUsername = GetUserUsername(sndUserId, _appContext),
                ReceiverUsername = GetUserUsername(rcvUserId, _appContext)
            };

            _appContext.Messages.Add(data);
            await _appContext.SaveChangesAsync();

            //getting signalRid using rcvUserId
            string receiverSignalRId = getRecvSid(rcvUserId, _appContext)?.Trim();

            if (!string.IsNullOrEmpty(receiverSignalRId))
            {
                //retrive the list of connection ID assosited  the userId
                List<string> receiverConneId = connectionIds[receiverSignalRId];

                foreach (string connId in receiverConneId)
                {
                    await Clients.Client(connId).SendAsync("ReceiveMessage", senderconnectionId, messageResp);
                }
            }
        }


        public static  string  getRecvSid(int? rcvUserId,chat_AppContext appContext)
        {
            return rcvUserId.HasValue ? appContext.Users.FirstOrDefault(u => u.UserId == rcvUserId)?.SignalRid : null;
        }

        public static string GetUserUsername(int? userId, chat_AppContext context)
        {
            return userId.HasValue
                ? context.Users.FirstOrDefault(u => u.UserId == userId)?.Username
                : null;
        }


        public async Task NotiyUser(string userId)
        {
            await Clients.Client(userId).SendAsync("RequestingUser", Context.ConnectionId);
        }

        public async  Task NotifyBackUser(string userId)
        {
            await Clients.Client(userId).SendAsync("userJoined", Context.ConnectionId);
        }


    }
}
