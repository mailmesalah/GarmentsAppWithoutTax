using ServerServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfServerApp.General;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace WpfServerApp.Services
{
    public class UnitService : IUnit
    {
        public bool CreateUnitRegister(CUnitRegister oUnitRegister)
        {
            bool returnValue = false;

            lock (Synchronizer.@lock)
            {
                using (var dataB = new Database9006Entities())
                {
                    var dataBTransaction = dataB.Database.BeginTransaction();
                    try
                    {
                        BillNoService bs = new BillNoService();

                        int cbillNo = bs.ReadNextUnitRegisterBillNo();
                        bs.UpdateUnitRegisterBillNo(cbillNo + 1);

                        unit_register lr = dataB.unit_register.Create();
                        lr.unit_code = cbillNo.ToString();
                        lr.unit = oUnitRegister.Unit;
                        lr.unit_value = oUnitRegister.UnitType == "AGroup" ? 1 : oUnitRegister.UnitValue;
                        lr.type = oUnitRegister.UnitType;
                        lr.group_code = oUnitRegister.GroupCode;
         
                        dataB.unit_register.Add(lr);

                        dataB.SaveChanges();
                        //Success
                        returnValue = true;

                        dataBTransaction.Commit();
                    }
                    catch
                    {
                        dataBTransaction.Rollback();
                    }
                    finally
                    {

                    }
                }
            }

            return returnValue;
        }

        public bool UpdateUnitRegister(CUnitRegister oUnitRegister)
        {            
            bool returnValue = false;

            if (IsUnitUsedInTransaction(oUnitRegister.UnitCode) == true)
            {
                return false;
            }

            lock (Synchronizer.@lock)
            {
                using (var dataB = new Database9006Entities())
                {
                    var dataBTransaction = dataB.Database.BeginTransaction();
                    try
                    {
                        var cpp = dataB.unit_register.Select(c => c).Where(x => x.unit_code == oUnitRegister.UnitCode);
                        dataB.unit_register.RemoveRange(cpp);

                        unit_register lr = dataB.unit_register.Create();
                        lr.unit_code = oUnitRegister.UnitCode;
                        lr.unit = oUnitRegister.Unit;
                        lr.unit_value = oUnitRegister.UnitType == "AGroup" ? 1 : oUnitRegister.UnitValue;
                        lr.type = oUnitRegister.UnitType;
                        lr.group_code = oUnitRegister.GroupCode;
            
                        dataB.unit_register.Add(lr);
                        dataB.SaveChanges();
                        //Success
                        returnValue = true;

                        dataBTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dataBTransaction.Rollback();
                    }
                    finally
                    {

                    }
                }
            }
            return returnValue;
        }

        public bool DeleteUnitRegister(string unitCode)
        {
            if (IsUnitHasChild(unitCode) == true)
            {
                return false;
            }

            if (IsUnitUsedInTransaction(unitCode) == true)
            {
                return false;
            }

            bool returnValue = true;

            lock (Synchronizer.@lock)
            {
                using (var dataB = new Database9006Entities())
                {
                    var dataBTransaction = dataB.Database.BeginTransaction();
                    try
                    {

                        var cr = dataB.unit_register.Select(c => c).Where(x => x.unit_code == unitCode);
                        dataB.unit_register.RemoveRange(cr);

                        dataB.SaveChanges();
                        dataBTransaction.Commit();
                    }
                    catch
                    {
                        returnValue = false;
                        dataBTransaction.Rollback();
                    }
                    finally
                    {

                    }
                }
            }
            return returnValue;
        }

        public CUnitRegister ReadUnitRegister(string unitCode)
        {
            CUnitRegister ccp = null;

            using (var dataB = new Database9006Entities())
            {
                var cps = dataB.unit_register.Select(c => c).Where(x => x.unit_code == unitCode);

                if (cps.Count() > 0)
                {
                    ccp = new CUnitRegister();

                    var cp = cps.FirstOrDefault();
                    ccp.Id = cp.id;
                    ccp.UnitCode = cp.unit_code;
                    ccp.Unit = cp.unit;
                    ccp.UnitType = cp.type;
                    ccp.GroupCode = cp.group_code;
                    ccp.UnitValue = (decimal)cp.unit_value;                    
                }

            }

            return ccp;
        }

        
        private bool IsUnitHasChild(string unitCode)
        {
            bool hasChild = false;
            using (var dataB = new Database9006Entities())
            {
                var data = dataB.unit_register.Select(e => e).Where(e => e.group_code == unitCode);
                if (data.Count() > 0)
                {
                    hasChild = true;
                }
            }
            return hasChild;
        }

        private bool IsUnitUsedInTransaction(string unitCode)
        {
            bool isUsed = false;
            using (var dataB = new Database9006Entities())
            {
                var data = dataB.product_transactions.Select(e => e).Where(e => e.purchase_unit_code == unitCode || e.sales_unit_code == unitCode);
                if (data.Count() > 0)
                {
                    isUsed = true;
                }
            }
            return isUsed;
        }
        

        public string ReadGroupCodeOf(string unitCode)
        {
            string groupCode = "";
            using (var dataB = new Database9006Entities())
            {
                var data = dataB.unit_register.Select(e => new { e.group_code, e.unit_code }).Where(e => e.unit_code == unitCode).FirstOrDefault();
                groupCode = data.group_code;
            }

            return groupCode;
        }

        public List<CUnit> ReadAllGroupUnits()
        {
            List<CUnit> units = new List<CUnit>();
            using (var dataB = new Database9006Entities())
            {
                var datas = dataB.unit_register.Select(e => new { e.unit_code, e.unit, e.type, e.unit_value }).Where(e => (e.type == "AGroup")).OrderBy(e => e.unit);
                foreach (var item in datas)
                {
                    units.Add(new CUnit() { Unit = item.unit, UnitCode = item.unit_code, UnitType = item.type, UnitValue=(decimal)item.unit_value });
                }
            }

            return units;
        }

        public List<CUnit> ReadAllUnits()
        {
            List<CUnit> units = new List<CUnit>();
            using (var dataB = new Database9006Entities())
            {
                var datas = dataB.unit_register.Select(e => new { e.unit_code, e.unit, e.type, e.unit_value }).OrderBy(e => e.unit);
                foreach (var item in datas)
                {
                    units.Add(new CUnit() { Unit = item.unit, UnitCode = item.unit_code, UnitType = item.type, UnitValue=(decimal) item.unit_value });
                }
            }

            return units;
        }

        public List<CUnit> ReadSubUnits(string unitCode)
        {
            List<CUnit> units = new List<CUnit>();
            try
            {
                string groupCode = "";
                decimal unitValue = 1;
                using (var dataB = new Database9006Entities())
                {
                    var datas = dataB.unit_register.Select(e => new { e.unit_code, e.unit, e.type,e.group_code,e.unit_value }).Where(e => e.unit_code == unitCode);
                    foreach (var item in datas)
                    {
                        units.Add(new CUnit() { Unit = item.unit, UnitCode = item.unit_code, UnitType = item.type, UnitValue=(decimal)item.unit_value });
                        groupCode = item.group_code;
                        unitValue = (decimal)item.unit_value;
                    }

                    if (units.ElementAt(0).UnitType == "AGroup")
                    {
                        using (var dataB1 = new Database9006Entities())
                        {
                            var datas1 = dataB1.unit_register.Select(e => new { e.unit_code, e.unit, e.group_code, e.type, e.unit_value }).Where(e => e.group_code == unitCode).OrderBy(e => e.unit_value);
                            foreach (var item in datas1)
                            {
                                units.Add(new CUnit() { Unit = item.unit, UnitCode = item.unit_code, UnitType = item.type, UnitValue=(decimal)item.unit_value });
                            }
                        }
                    }
                    else
                    {
                        using (var dataB1 = new Database9006Entities())
                        {
                            var datas1 = dataB1.unit_register.Select(e => new { e.unit_code, e.unit, e.group_code, e.type, e.unit_value }).Where(e => e.group_code == groupCode && e.unit_value>unitValue).OrderBy(e => e.unit_value);
                            foreach (var item in datas1)
                            {
                                units.Add(new CUnit() { Unit = item.unit, UnitCode = item.unit_code, UnitType = item.type, UnitValue=(decimal)item.unit_value });
                            }
                        }
                    }
                }
                    
            }
            catch 
            {

            }

            return units;
        }

        public ObservableCollection<CUnit> ReadAllUnitsWithGroupAsTree()
        {
            ObservableCollection<CUnit> units = new ObservableCollection<CUnit>();
            try
            {
                using (var dataB1 = new Database9006Entities())
                {
                    using (var dataB2 = new Database9006Entities())
                    {

                        var agroups = dataB1.unit_register.Select(e => new { e.unit_code, e.unit, e.type }).Where(e => (e.type == "AGroup")).OrderBy(e => e.unit);
                        foreach (var item1 in agroups)
                        {
                            CUnit aUnit = new CUnit() { Unit = item1.unit, UnitCode = item1.unit_code, UnitType = item1.type };
                            ObservableCollection<CUnit> aMembers = new ObservableCollection<CUnit>();

                            var bUnits = dataB2.unit_register.Select(e => new { e.unit_code, e.unit, e.type, e.group_code, e.unit_value }).Where(e => (e.group_code == aUnit.UnitCode)).OrderBy(e => e.unit_value);
                            foreach (var item2 in bUnits)
                            {
                                CUnit bUnit = new CUnit() { Unit = item2.unit, UnitCode = item2.unit_code, UnitType = item2.type };
                                ObservableCollection<CUnit> bMembers = new ObservableCollection<CUnit>();


                                bUnit.MemberList = bMembers;
                                aMembers.Add(bUnit);
                            }

                            aUnit.MemberList = aMembers;
                            units.Add(aUnit);

                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
            return units;
        }

        public decimal ReadLowestUnitValue(string unitCode)
        {
            decimal rUnitV = 1;
            CUnit currentUnit = new CUnit();
            try
            {
                string groupCode = "";
                
                using (var dataB = new Database9006Entities())
                {
                    var datas = dataB.unit_register.Select(e => new { e.unit_code, e.unit, e.type, e.group_code, e.unit_value }).Where(e => e.unit_code == unitCode);
                    foreach (var item in datas)
                    {
                        currentUnit= new CUnit() { Unit = item.unit, UnitCode = item.unit_code, UnitType = item.type, UnitValue = (decimal)item.unit_value };
                        groupCode = item.group_code;
                    }

                    if (currentUnit.UnitType == "AGroup")
                    {
                        using (var dataB1 = new Database9006Entities())
                        {
                            var datas1 = dataB1.unit_register.Where(e => e.group_code == unitCode).Max(e=>e.unit_value);
                            rUnitV = datas1.HasValue ? datas1.Value : 1;
                        }
                    }
                    else
                    {
                        using (var dataB1 = new Database9006Entities())
                        {
                            var datas1 = dataB1.unit_register.Where(e => e.group_code == groupCode).Max(e => e.unit_value);
                            rUnitV = datas1.HasValue ? datas1.Value : 1;
                        }
                    }
                }

            }
            catch
            {

            }

            return rUnitV;
        }

    }
}
