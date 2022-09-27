using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ServerServiceInterface
{
    [ServiceContract]
    public interface IProduct
    {
        [OperationContract]
        string ReadGroupCodeOf(string productCode);
        [OperationContract]
        List<CProduct> ReadAllProducts();
        [OperationContract]
        ObservableCollection<CProduct> ReadAllProductsWithGroupAsTree();
        [OperationContract]
        List<CProduct> ReadAllGroupProducts();

        [OperationContract]
        CProductRegister ReadProductRegister(string productCode);
        [OperationContract]
        bool DeleteProductRegister(string productCode);
        [OperationContract]
        bool CreateProductRegister(CProductRegister product);
        [OperationContract]
        bool UpdateProductRegister(CProductRegister product);
        
    }

    
    [DataContract]
    public class CProduct
    {
        string productCode;
        string product;
        string type;
        ObservableCollection<CProduct> members = new ObservableCollection<CProduct>();
        bool isSelected = false;
        bool isExpanded = false;
        string stockInUnitCode;
        string stockOutUnitCode;
        
        [DataMember]
        public string ProductCode
        {
            get { return productCode; }
            set { productCode = value; }
        }
        [DataMember]
        public string Product
        {
            get { return product; }
            set { product = value; }
        }
        [DataMember]
        public string ProductType
        {
            get { return  type; }
            set { type = value; }
        }

        [DataMember]
        public ObservableCollection<CProduct> MemberList
        {
            get { return members; }
            set { members = value; }
        }
        [DataMember]
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; }
        }
        [DataMember]
        public bool IsExpanded
        {
            get { return isExpanded; }
            set { isExpanded = value; }
        }
        [DataMember]
        public string StockInUnitCode
        {
            get { return stockInUnitCode; }
            set { stockInUnitCode = value; }
        }
        [DataMember]
        public string StockOutUnitCode
        {
            get { return stockOutUnitCode; }
            set { stockOutUnitCode = value; }
        }
        
    }

    [DataContract]
    public class CProductRegister
    {
        int id;
        string productCode;
        string product;
        string type;        
        string groupCode;
        string alternateName;
        bool? isEnabled;
        string stockInUnitCode;
        string stockOutUnitCode;
        
        [DataMember]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        [DataMember]
        public string ProductCode
        {
            get { return productCode; }
            set { productCode = value; }
        }
        [DataMember]
        public string Product
        {
            get { return product; }
            set { product = value; }
        }
        [DataMember]
        public string ProductType
        {
            get { return type; }
            set { type = value; }
        }
        [DataMember]
        public string GroupCode
        {
            get { return groupCode; }
            set { groupCode = value; }
        }        
        [DataMember]
        public string AlternateName
        {
            get { return alternateName; }
            set { alternateName = value; }
        }        
        [DataMember]
        public bool? IsEnabled
        {
            get { return isEnabled; }
            set { isEnabled = value; }
        }
        [DataMember]
        public string StockInUnitCode
        {
            get { return stockInUnitCode; }
            set { stockInUnitCode = value; }
        }
        [DataMember]
        public string StockOutUnitCode
        {
            get { return stockOutUnitCode; }
            set { stockOutUnitCode = value; }
        }        
    }
}
