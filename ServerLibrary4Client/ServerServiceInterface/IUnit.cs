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
    public interface IUnit
    {
        [OperationContract]
        string ReadGroupCodeOf(string unitCode);
        [OperationContract]
        List<CUnit> ReadAllUnits();
        [OperationContract]
        ObservableCollection<CUnit> ReadAllUnitsWithGroupAsTree();
        [OperationContract]
        List<CUnit> ReadAllGroupUnits();
        [OperationContract]
        List<CUnit> ReadSubUnits(string unitCode);

        [OperationContract]
        CUnitRegister ReadUnitRegister(string unitCode);
        [OperationContract]
        bool DeleteUnitRegister(string unitCode);
        [OperationContract]
        bool CreateUnitRegister(CUnitRegister unit);
        [OperationContract]
        bool UpdateUnitRegister(CUnitRegister unit);
        
    }

    
    [DataContract]
    public class CUnit
    {
        string unitCode;
        string unit;
        string type;
        decimal unitValue;
        ObservableCollection<CUnit> members = new ObservableCollection<CUnit>();
        bool isSelected = false;
        bool isExpanded = false;


        [DataMember]
        public string UnitCode
        {
            get { return unitCode; }
            set { unitCode = value; }
        }
        [DataMember]
        public string Unit
        {
            get { return unit; }
            set { unit = value; }
        }
        [DataMember]
        public string UnitType
        {
            get { return  type; }
            set { type = value; }
        }

        [DataMember]
        public ObservableCollection<CUnit> MemberList
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
        public decimal UnitValue
        {
            get { return unitValue; }
            set { unitValue = value; }
        }
    }

    [DataContract]
    public class CUnitRegister
    {
        int id;
        string unitCode;
        string unit;
        string type;        
        string groupCode;
        decimal unitValue;        
        
        [DataMember]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        [DataMember]
        public string UnitCode
        {
            get { return unitCode; }
            set { unitCode = value; }
        }
        [DataMember]
        public string Unit
        {
            get { return unit; }
            set { unit = value; }
        }
        [DataMember]
        public string UnitType
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
        public decimal UnitValue
        {
            get { return unitValue; }
            set { unitValue = value; }
        }                        
    }
}
