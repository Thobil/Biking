using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace JCDecauxCache
{
    [ServiceContract]
    public interface ICacheJCDecaux
    {
        [OperationContract]
        string getContracts();

        [OperationContract]
        string getAllStationsOfContract(string contract);
    }
}
