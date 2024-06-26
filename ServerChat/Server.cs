﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ServerChat
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Server : IServer
    {
        List<ServerUser> users = new List<ServerUser>();
        int nextID = 1;
        public int Connect(string name)
        {
            ServerUser user = new ServerUser()
            {
                ID = nextID,
                Name = name,
                operationContext = OperationContext.Current,
            };
            nextID++;

            SendMessage(user.Name + " подключился к чату ",0);
            users.Add(user);
            return user.ID;
        }

        public void Disconnect(int id)
        {
            var user = users.FirstOrDefault(i =>  i.ID == id);
            if (user != null)
            {
                users.Remove(user);
                SendMessage(user.Name + " покинул чат ", 0);
            }
        }

        public void SendMessage(string msg, int id)
        {
            foreach (var item in users)
            {
                string answer = DateTime.Now.ToShortTimeString();
                var user = users.FirstOrDefault(i => i.ID == id);
                if (user != null)
                {
                    answer += ": " + user.Name + ": ";
                }

                answer += msg;

                item.operationContext.GetCallbackChannel<IServerChatCallBack>().MessageCallBack(answer);
            }
        }
    }   
}
