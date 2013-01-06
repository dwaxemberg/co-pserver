using System;

namespace Conquer_Online_Server.Interfaces
{
    public interface IKnownPerson
    {
        uint ID { get; set; }
        string Name { get; set; }
        bool IsOnline { get; }
        Client.GameState Client { get; }
    }
}
