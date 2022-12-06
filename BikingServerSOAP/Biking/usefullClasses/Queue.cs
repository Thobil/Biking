using System;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using ISession = Apache.NMS.ISession;

namespace ActivemqProducer
{
    class QueueJCDecaux
    {
        public string sendMessage(string[] msg)
        {
            // Create a Connection Factory.
            Uri connecturi = new Uri("activemq:tcp://localhost:61616");
            ConnectionFactory connectionFactory = new ConnectionFactory(connecturi);

            // Create a single Connection from the Connection Factory.
            IConnection connection = connectionFactory.CreateConnection();
            connection.Start();

            // Create a session from the Connection.
            ISession session = connection.CreateSession();

            // Use the session to target a queue.
            string name = "QueueJCDecault";
            IDestination destination = session.GetQueue(name);

            // Create a Producer targetting the selected queue.
            IMessageProducer producer = session.CreateProducer(destination);

            // You may configure everything to your needs, for instance:
            producer.DeliveryMode = MsgDeliveryMode.NonPersistent;

            // Finally, to send messages:
            string messages = "";
            foreach(string s in msg)
            {
                messages += s + "\n";
            }
            ITextMessage message = session.CreateTextMessage(messages);
            producer.Send(message);

            Console.WriteLine("Message sent, check ActiveMQ web interface to confirm.");

            // Don't forget to close your session and connection when finished.
            session.Close();
            connection.Close();

            return name;
        }
    }
}
