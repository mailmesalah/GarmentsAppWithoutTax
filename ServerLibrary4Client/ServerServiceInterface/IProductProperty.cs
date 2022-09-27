using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace ServerServiceInterface
{    
    [ServiceContract]
    public interface IProductProperty
    {
        [OperationContract]
        List<CProductProperty> ReadProductProperties(string productGroupCode,string propertyType);        
     
    }
    
    [DataContract]
    public class CProductProperty
    {
        int id;
        string propertyCode;
        string property;
        string productGroupCode;
        string propertyType;        
 
        [DataMember]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [DataMember]
        public string PropertyCode
        {
            get { return propertyCode; }
            set { propertyCode = value; }
        }
        
        [DataMember]
        public string Property
        {
            get { return property; }
            set { property = value; }
        }        

        [DataMember]
        public string ProductGroupCode
        {
            get { return productGroupCode; }
            set { productGroupCode = value; }
        }

        [DataMember]
        public string PropertyType
        {
            get { return propertyType; }
            set { propertyType = value; }
        }
        
    }
    
}
