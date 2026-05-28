using System;
using System.Collections.Generic;
using System.Text;

namespace RetailERP.Core.Common
{
    public static class CommonMessage
    {
        static Dictionary<int, string> MessageList;

        private static void Message_Repository()
        {
            MessageList = new Dictionary<int, string>();
            // Integer Key Cannot Be Duplicate 

            // All Successful Messages
            MessageList.Add(1, "Save operation successful.");
            MessageList.Add(2, "Update operation successful.");
            MessageList.Add(3, "Delete operation successful.");
            MessageList.Add(4, "Transaction Successful.");
            MessageList.Add(5, "Send for approval operation successful.");
            MessageList.Add(6, "Approved operation successful.");
            MessageList.Add(7, "Acknowledgment operation successful.");
            MessageList.Add(8, "Revised operation successful.");
            MessageList.Add(9, "Reject operation successful.");
            MessageList.Add(10, "Mail Sent Successfully.");
            MessageList.Add(11, "Do you want to continue?");
            MessageList.Add(12, "Permission Is Assigned Successfully.");

            // All Warning Messages  
            MessageList.Add(-1, "Operation Failed.");
            MessageList.Add(-2, "Save Operation Failed.");
            MessageList.Add(-3, "Update Operation Failed.");
            MessageList.Add(-4, "Delete Operation Failed.");
            MessageList.Add(-5, "Transaction Failed.");
            MessageList.Add(-6, "Operation Failed. Please Contact With System Administrator.");
            MessageList.Add(-7, "Please fill-up mandatory field.");
            MessageList.Add(-8, "Numeric Value Only.");
            MessageList.Add(-9, "Round Number Only.");
            MessageList.Add(-10, "Decimal Value Only.");
            MessageList.Add(-11, "Data Not Found.");
            MessageList.Add(-12, "Duplicate Found.");
            MessageList.Add(-13, "Transaction Rollbacked.");
            MessageList.Add(-14, "Invalid User Account.");
            MessageList.Add(-15, "User Account Is Blocked.");
            MessageList.Add(-16, "You Have No Edit Permission.");
            MessageList.Add(-17, "You Have No Permission On This Page.");
            MessageList.Add(-18, "You Are Blocked By System Administrator.");
            MessageList.Add(-19, "You Account Is Suspended. Please Contact With System Administrator.");
            MessageList.Add(-20, "You Are Already Logged In On IP NO : XXXXXXXXXXXXXXXXX");
        }

        public static string Get_Message(int iKey)
        {
            if (MessageList == null)
            {
                Message_Repository();
            }

            string MessageText = "";
            foreach (var item in MessageList)
            {
                if (item.Key == iKey)
                {
                    MessageText = item.Value;

                    return MessageText;
                }
            }
            return MessageText;
        }
    }
}
