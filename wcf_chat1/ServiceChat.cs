using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;

namespace wcf_chat1
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "ServiceUser" в коде и файле конфигурации.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ServiceChat : IServiceChat
    {
        List<ServerUser> users = new List<ServerUser>();
        int nextId = 1;
        //Список ответов пользователя на текущую игру

        public int Connect(string name)
        {

            ServerUser user = new ServerUser()
            {
                ID = nextId,
                Name = name,
                MSG = "Камень",
                operationContext = OperationContext.Current
            };
            nextId++;

            SendMsg(": " + user.Name + " подключился к чату!", user.ID);
            users.Add(user);
            return user.ID;
        }

        public void Disconnect(int id)
        {
            var user = users.FirstOrDefault(i => i.ID == id);
            if (user != null)
            {
                users.Remove(user);
                SendMsg(": " + user.Name + " покинул чат!", 0);
            }
        }

        public void SendMsg(string msg, int id)
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
                item.operationContext.GetCallbackChannel<IServerChatCallback>().MsgCallback(answer);
            }
        }
        //--------------------------------Реализация интерфейсов для игры-------------------------------------
        public void SendStartGame(string msg, int id)
        {
            try
            {
                foreach (var item in users)
                {
                    item.MSG = "";
                }
                foreach (var item in users)
                {
                    string answer = " ";
                    answer += msg;
                    answer += 10 + " секунд";
                    item.operationContext.GetCallbackChannel<IServerChatCallback>().GameCallBack(answer);
                }
            }
            catch (Exception)
            {
                SendMsg("Error in Send StartGame", 1);
            }
        }
        // Пока не реализовано
        public void ResultGame(string msg, int id)
        {

            try
            {
                string answer = "";
                var user = users.FirstOrDefault(i => i.ID == id);
                if (user != null)
                {
                    if(msg == "")
                    {
                        foreach (var item in users)
                        {
                            item.MSG = "Камень";
                        }
                    }
                    else if(msg != "")
                    { 
                        foreach (var item in users)
                        {
                            item.MSG = msg;
                        }
                    }
                }
                foreach (var item in users)
                {
                    answer = "Игра закончилась!";
                    item.operationContext.GetCallbackChannel<IServerChatCallback>().ResultOfGameCallbak(answer);
                }

            }
            catch (Exception)
            {
                SendMsg("Error in StartGame CallBack", 1);
            }
        }

        public void PickingWinner()
        {
            try
            {
                int rock = 0;
                int paper = 0;
                int scissors = 0;
                string answer = "";
                foreach (var item in users) { 
                    if (item.MSG == "Камень")
                    {
                        rock++;
                    }
                    else if (item.MSG == "Бумага")
                    {
                        paper++;
                    }
                    else if (item.MSG == "Ножницы")
                    {
                        scissors++;
                    }
                }

                if (rock == 0 && paper == 0 && scissors !=0)
                {
                    answer = "Ничья,все выкинули ножницы";
                }
                else if (scissors == 0 && paper == 0 && rock != 0)
                {
                    answer = "Ничья,все выкинули камень";
                }
                else if (rock == 0 && scissors == 0 && paper != 0)
                {
                    answer = "Ничья,все выкинули бумагу";

                }
                else if (paper == 0 && rock != 0 && scissors!=0)
                {
                    answer = "Победители выкинули Камень: ";
                    foreach (var item in users)
                    {
                        if (item.MSG == "Камень")
                        {
                            answer += item.Name + " ";
                        }
                    }
                }
                else if (rock == 0 && paper != 0 && scissors != 0)
                {
                    answer = "Победители: выкинули Ножницы";
                    foreach (var item in users)
                    {
                        if (item.MSG == "Ножницы")
                        {
                            answer += item.Name + " ";
                        }
                    }
                }
                else if (scissors == 0 && rock != 0 && paper!= 0)
                {
                    answer = "Победители выкинули Бумагу: ";
                    foreach (var item in users)
                    {
                        if (item.MSG == "Бумага")
                        {
                            answer += item.Name + " ";
                        }
                    }
                }
                else if(scissors != 0 && rock != 0 && paper != 0)
                {
                    answer = "Победителя нет полная каша " + rock + " " + paper + " " + scissors;
                }

                foreach (var item in users)
                {
                    item.operationContext.GetCallbackChannel<IServerChatCallback>().PickingWinnerCallback(answer);
                }
            }
            catch (Exception)
            {
                SendMsg("Error in StartGame CallBack", 1);
            }
        }
        // Вроде работает, осталось сделать блок кнопок и проверку нажатия на кнопку
        public void SendTimer(int timer)
        {
            try
            {
                int counter = timer;
                while (counter >= 0)
                {
                    foreach (var item in users)
                    {
                        string answer = "";
                        answer += "До окончания игры осталось: " + counter;
                        item.operationContext.GetCallbackChannel<IServerChatCallback>().TimerCallback(answer,counter);
                    }
                    Thread.Sleep(1000);
                    counter--;
                }
            }
            catch (Exception)
            {
                SendMsg("Error in Send StartGame Timer", 1);
            }
        }
    }
}
