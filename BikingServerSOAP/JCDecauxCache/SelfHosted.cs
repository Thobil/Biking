// add the WCF ServiceModel namespace 
using System.ServiceModel.Description;
using JCDecauxCache;

namespace System.ServiceModel
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create a URI to serve as the base address
            //Be careful to run Visual Studio as Admistrator or to allow VS to open new port netsh command. 
            // Example : netsh http add urlacl url=http://+:80/MyUri user=DOMAIN\user
            Uri httpUrl = new Uri("http://localhost:8733/Design_Time_Addresses/JCDecauxCache/JCDecauxCache/");

            //Create ServiceHost
            ServiceHost host = new ServiceHost(typeof(CacheJCDecaux), httpUrl);

            // Modify binding parameters
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = 1000000;
            binding.MaxBufferPoolSize = 1000000;
            binding.MaxBufferSize = 1000000;

            //Add a service endpoint
            host.AddServiceEndpoint(typeof(ICacheJCDecaux), binding, "");

            //Enable metadata exchange
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            host.Description.Behaviors.Add(smb);

            //Start the Service
            host.Open();

            Console.WriteLine("Service is host at " + DateTime.Now.ToString());
            Console.WriteLine("Host is running... Press <Enter> key to stop");
            Console.ReadLine();
        }
    }
}
