using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conquer_Online_Server.Database
{
    public class TransactionLog
    {
        public enum TransactionLogAction
        {
            Trade = 1,
            Booth = 2
        }
        
        public static void ProcessTransaction(Client.GameState owner, Client.GameState receiver, uint amount, TransactionLogAction action)
        {
            DateTime Date = DateTime.Now;
            MySqlCommand cmd = new MySqlCommand(MySqlCommandType.INSERT);
            cmd.Insert("transactionlog").
                Insert("owneruid", owner.Entity.UID).
                Insert("receiveruid", receiver.Entity.UID).
                Insert("amount", amount).
                Insert("ownerhadamount", owner.Entity.ConquerPoints).
                Insert("receiverhadamount", receiver.Entity.ConquerPoints).
                Insert("type", (byte)action).
                Insert("date", Date.ToString()).
                Execute();
        }
    }
}
