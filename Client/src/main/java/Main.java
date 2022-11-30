import com.soap.ws.client.generated.ArrayOfstring;
import com.soap.ws.client.generated.IMyBiking;
import com.soap.ws.client.generated.MyBiking;

import java.util.List;

public class Main {
    public static void main(String[] args) {
        MyBiking myBiking = new MyBiking();
        IMyBiking iMyBiking = myBiking.getBasicHttpBindingIMyBiking();
        //ArrayOfstring s = iMyBiking.getTrajectory("rue pelisson villeurbanne", "rue tronchet lyon");
        //ArrayOfstring s = iMyBiking.getTrajectory("Rue du repos besancon", "Rue charles nodier besancon");
        ArrayOfstring s = iMyBiking.getTrajectory("3 Place de la République, Mulhouse", "BOULEVARD CHARLES STOESSEL, Mulhouse");
        //ArrayOfstring s = iMyBiking.getTrajectory(null,null);

        List<String> strings = s.getString();
        for(String str: strings){
            System.out.println(str);
        }
    }
}
