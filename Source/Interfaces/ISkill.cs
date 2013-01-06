using System;

namespace Conquer_Online_Server.Interfaces
{
    public interface ISkill
    {
        uint Experience { get; set; }
        ushort ID { get; set; }
        byte Level { get; set; }
        byte PreviousLevel { get; set; }
        bool Available { get; set; }
        void Send(Client.GameState client);
    }
}
