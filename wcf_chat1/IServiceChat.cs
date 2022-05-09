using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace wcf_chat1
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IServiceChat" в коде и файле конфигурации.
    [ServiceContract(CallbackContract = typeof(IServerChatCallback))]
    
    public interface IServiceChat
    {
        [OperationContract]
        int Connect(string name);

        [OperationContract]
        void Disconnect(int id);
        [OperationContract(IsOneWay = true)]
        void SendMsg(string msg, int id);
        
// --------------------------------Методы для игры-------------------------------------
        
        [OperationContract(IsOneWay = true)]
        void SendStartGame(string msg, int id);
        
        [OperationContract(IsOneWay = true)]
        void ResultGame(string msg,int id);

        [OperationContract(IsOneWay = true)]
        void SendTimer(int timer);

        [OperationContract(IsOneWay = true)]
        void PickingWinner();
    }

    public interface IServerChatCallback
    {
        [OperationContract(IsOneWay = true)]
        void MsgCallback(string msg);

        [OperationContract(IsOneWay = true)]
        void GameCallBack(string msg);

        [OperationContract(IsOneWay = true)]
        void ResultOfGameCallbak(string msg);

        [OperationContract(IsOneWay = true)]
        void TimerCallback(string msg,int timer);

        [OperationContract(IsOneWay = true)]
        void PickingWinnerCallback(string msg);
    }
}
