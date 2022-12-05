import com.soap.ws.client.generated.ArrayOfstring;
import com.soap.ws.client.generated.IMyBiking;
import com.soap.ws.client.generated.MyBiking;

import java.util.List;
import java.util.Scanner;

public class Main {
    public static void main(String[] args) {
        MyBiking myBiking = new MyBiking();
        IMyBiking iMyBiking = myBiking.getBasicHttpBindingIMyBiking();

        Scanner scanner = new Scanner(System.in);

        System.out.println("Veuillez rentrer votre adresse de départ :");
        String adress1 = scanner.nextLine();
        System.out.println("Veuillez rentrer votre adresse de d'arrivée :");
        String adress2 = scanner.nextLine();

        ArrayOfstring s = iMyBiking.getTrajectory(adress1,adress2);

        //ArrayOfstring s = iMyBiking.getTrajectory("lyon", "paris");
        //ArrayOfstring s = iMyBiking.getTrajectory("rue pelisson villeurbanne", "rue tronchet lyon");
        //ArrayOfstring s = iMyBiking.getTrajectory("Rue du repos besancon", "Rue charles nodier besancon");
        //ArrayOfstring s = iMyBiking.getTrajectory("xhxblkxfblkjbwfblbk", "4");
        //ArrayOfstring s = iMyBiking.getTrajectory("Lyon Part-Dieu", "Faverges, 38510 Creys-Mepieu");
        //ArrayOfstring s = iMyBiking.getTrajectory("3 Place de la République, Mulhouse", "BOULEVARD CHARLES STOESSEL, Mulhouse");
        //ArrayOfstring s = iMyBiking.getTrajectory(null,null);
        //ArrayOfstring s = iMyBiking.getTrajectory("26 rue du faix aux chiens saint hilaire de riez", "Faverge 38510");
        List<String> strings = s.getString();
        if(strings.size() == 0){
            System.out.println("L'adresse de départ et/ou d'arrivée est non valable.");
        }else{
            for(String str: strings){
                System.out.println(str);
            }
        }
    }
}
