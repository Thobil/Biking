import com.soap.ws.client.generated.IMyBiking;
import com.soap.ws.client.generated.MyBiking;
import org.apache.activemq.ActiveMQConnectionFactory;

import javax.jms.*;
import java.util.Scanner;

public class Main {
    public static void main(String[] args) {


        //ArrayOfstring s = iMyBiking.getTrajectory("lyon", "paris");
        //ArrayOfstring s = iMyBiking.getTrajectory("rue pelisson villeurbanne", "rue tronchet lyon");
        //ArrayOfstring s = iMyBiking.getTrajectory("Rue du repos besancon", "Rue charles nodier besancon");
        //ArrayOfstring s = iMyBiking.getTrajectory("xhxblkxfblkjbwfblbk", "4");
        //ArrayOfstring s = iMyBiking.getTrajectory("Lyon Part-Dieu", "Faverges, 38510 Creys-Mepieu");
        //ArrayOfstring s = iMyBiking.getTrajectory("3 Place de la République, Mulhouse", "BOULEVARD CHARLES STOESSEL, Mulhouse");
        //ArrayOfstring s = iMyBiking.getTrajectory(null,null);
        //ArrayOfstring s = iMyBiking.getTrajectory("26 rue du faix aux chiens saint hilaire de riez", "Faverge 38510");


        thread(new HelloWorldConsumer(), false);
    }

    public static void thread(Runnable runnable, boolean daemon) {
        Thread brokerThread = new Thread(runnable);
        brokerThread.setDaemon(daemon);
        brokerThread.start();
    }

    public static class HelloWorldConsumer implements Runnable, ExceptionListener {
        public void run() {
            MyBiking myBiking = new MyBiking();
            IMyBiking iMyBiking = myBiking.getBasicHttpBindingIMyBiking();

            Scanner scanner = new Scanner(System.in);

            System.out.println("Veuillez rentrer votre adresse de départ :");
            String adress1 = scanner.nextLine();
            System.out.println("Veuillez rentrer votre adresse de d'arrivée :");
            String adress2 = scanner.nextLine();

            String queueName = iMyBiking.getTrajectory(adress1,adress2);

            try {

                // Create a ConnectionFactory
                ActiveMQConnectionFactory connectionFactory = new ActiveMQConnectionFactory("tcp://localhost:61616");

                // Create a Connection
                Connection connection = connectionFactory.createConnection();
                connection.start();

                connection.setExceptionListener(this);

                // Create a Session
                Session session = connection.createSession(false, Session.AUTO_ACKNOWLEDGE);

                // Create the destination (Topic or Queue)
                Destination destination = session.createQueue(queueName);

                // Create a MessageConsumer from the Session to the Topic or Queue
                MessageConsumer consumer = session.createConsumer(destination);

                // Wait for a message
                Message message = consumer.receive(100000);

                if (message instanceof TextMessage) {
                    TextMessage textMessage = (TextMessage) message;
                    String text = textMessage.getText();
                    System.out.println("Received: " + text);
                } else {
                    System.out.println("Received: " + message);
                }

                consumer.close();
                session.close();
                connection.close();
            } catch (Exception e) {
                System.out.println("Caught: " + e);
                e.printStackTrace();
            }
        }

        public synchronized void onException(JMSException ex) {
            System.out.println("JMS Exception occured.  Shutting down client.");
        }
    }

}

