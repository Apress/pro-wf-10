//--------------------------------------------------------------------------------
// This file is part of the downloadable code for the Apress book:
// Pro WF: Windows Workflow in .NET 4.0
// Copyright (c) Bruce Bukovics.  All rights reserved.
//
// This code is provided as is without warranty of any kind, either expressed
// or implied, including but not limited to fitness for any particular purpose. 
// You may use the code for any commercial or noncommercial purpose, and combine 
// it with your own code, but cannot reproduce it in whole or in part for 
// publication purposes without prior approval. 
//--------------------------------------------------------------------------------      
using System;
using System.Workflow.Runtime;

namespace ActivityLibrary35
{
    public class GuessingGameService : IGuessingGame
    {
        #region IGuessingGame Members

        public void SendMessage(string message)
        {
            if (MessageReceived != null)
            {
                MessageReceivedEventArgs args
                    = new MessageReceivedEventArgs(
                        WorkflowEnvironment.WorkflowInstanceId,
                        message);
                MessageReceived(this, args);
            }
        }

        public event EventHandler<GuessReceivedEventArgs> GuessReceived;

        #endregion

        #region Public Members (not part of the service contract)

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public void OnGuessReceived(GuessReceivedEventArgs args)
        {
            if (GuessReceived != null)
            {
                //don't pass 'this' as the sender otherwise
                //the event cannot be delivered. Every part of
                //the event must be serializable, including the sender.
                GuessReceived(null, args);
            }
        }

        #endregion
    }
}
