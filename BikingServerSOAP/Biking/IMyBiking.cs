using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Biking
{
    [ServiceContract]
    public interface IMyBiking
    {
        [OperationContract]
        string getTrajectory(string fromAdress, string toAdress);

    }
}
