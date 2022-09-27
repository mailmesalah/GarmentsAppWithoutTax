using ServerServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using WpfServerApp.General;

namespace WpfServerApp.Services
{
    public class LedgerService : ILedger
    {
        public bool CreateLedgerRegister(CLedgerRegister oLedgerRegister)
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


                        int cbillNo = bs.ReadNextLedgerRegisterBillNo();
                        bs.UpdateLedgerRegisterBillNo(cbillNo + 1);

                        ledger_register lr = dataB.ledger_register.Create();
                        lr.ledger_code = cbillNo.ToString();
                        lr.ledger = oLedgerRegister.Ledger;
                        lr.alternate_name = oLedgerRegister.AlternateName;
                        lr.type = oLedgerRegister.LedgerType;
                        lr.group_code = oLedgerRegister.GroupCode;
                        lr.address1 = oLedgerRegister.Address1;
                        lr.address2 = oLedgerRegister.Address2;
                        lr.address3 = oLedgerRegister.Address3;
                        lr.details1 = oLedgerRegister.Details1;
                        lr.details2 = oLedgerRegister.Details2;
                        lr.details3 = oLedgerRegister.Details3;
                        lr.details4 = oLedgerRegister.Details4;
                        lr.details5 = oLedgerRegister.Details5;
                        lr.details6 = oLedgerRegister.Details6;
                        lr.a_group_code = oLedgerRegister.AGroupCode;
                        lr.b_group_code = oLedgerRegister.BGroupCode;
                        lr.c_group_code = oLedgerRegister.CGroupCode;
                        lr.is_editable = true;
                        lr.is_enabled = oLedgerRegister.IsEnabled;
                        lr.is_removable = true;

                        dataB.ledger_register.Add(lr);

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

        public bool UpdateLedgerRegister(CLedgerRegister oLedgerRegister)
        {

            if (IsLedgerEditable(oLedgerRegister.LedgerCode) != true)
            {
                return false;
            }

            bool returnValue = false;

            lock (Synchronizer.@lock)
            {
                using (var dataB = new Database9006Entities())
                {
                    var dataBTransaction = dataB.Database.BeginTransaction();
                    try
                    {
                        var cpp = dataB.ledger_register.Select(c => c).Where(x => x.ledger_code == oLedgerRegister.LedgerCode);
                        dataB.ledger_register.RemoveRange(cpp);

                        ledger_register lr = dataB.ledger_register.Create();
                        lr.ledger_code = oLedgerRegister.LedgerCode;
                        lr.ledger = oLedgerRegister.Ledger;
                        lr.alternate_name = oLedgerRegister.AlternateName;
                        lr.type = oLedgerRegister.LedgerType;
                        lr.group_code = oLedgerRegister.GroupCode;
                        lr.address1 = oLedgerRegister.Address1;
                        lr.address2 = oLedgerRegister.Address2;
                        lr.address3 = oLedgerRegister.Address3;
                        lr.details1 = oLedgerRegister.Details1;
                        lr.details2 = oLedgerRegister.Details2;
                        lr.details3 = oLedgerRegister.Details3;
                        lr.details4 = oLedgerRegister.Details4;
                        lr.details5 = oLedgerRegister.Details5;
                        lr.details6 = oLedgerRegister.Details6;
                        lr.a_group_code = oLedgerRegister.AGroupCode;
                        lr.b_group_code = oLedgerRegister.BGroupCode;
                        lr.c_group_code = oLedgerRegister.CGroupCode;
                        lr.is_editable = true;
                        lr.is_enabled = oLedgerRegister.IsEnabled;
                        lr.is_removable = true;

                        dataB.ledger_register.Add(lr);
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

        public bool DeleteLedgerRegister(string ledgerCode)
        {
            if (IsLedgerRemovable(ledgerCode) != true)
            {
                return false;
            }

            if (IsLedgerHasChild(ledgerCode) == true)
            {
                return false;
            }

            if (IsLedgerUsedInTransaction(ledgerCode) == true)
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

                        var cr = dataB.ledger_register.Select(c => c).Where(x => x.ledger_code == ledgerCode);
                        dataB.ledger_register.RemoveRange(cr);

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

        public CLedgerRegister ReadLedgerRegister(string ledgerCode)
        {
            CLedgerRegister ccp = null;

            using (var dataB = new Database9006Entities())
            {
                var cps = dataB.ledger_register.Select(c => c).Where(x => x.ledger_code == ledgerCode);

                if (cps.Count() > 0)
                {
                    ccp = new CLedgerRegister();

                    var cp = cps.FirstOrDefault();
                    ccp.Id = cp.id;
                    ccp.LedgerCode = cp.ledger_code;
                    ccp.Ledger = cp.ledger;
                    ccp.LedgerType = cp.type;
                    ccp.GroupCode = cp.group_code;
                    ccp.AlternateName = cp.alternate_name;
                    ccp.Address1 = cp.address1;
                    ccp.Address2 = cp.address2;
                    ccp.Address3 = cp.address3;
                    ccp.Details1 = cp.details1;
                    ccp.Details2 = cp.details2;
                    ccp.Details3 = cp.details3;
                    ccp.Details4 = cp.details4;
                    ccp.Details5 = cp.details5;
                    ccp.Details6 = cp.details6;
                    ccp.AGroupCode = cp.a_group_code;
                    ccp.BGroupCode = cp.b_group_code;
                    ccp.CGroupCode = cp.c_group_code;
                    ccp.IsEnabled = cp.is_enabled;
                }

            }

            return ccp;
        }

        private bool? IsLedgerRemovable(string ledgerCode)
        {
            bool? isRemovable = false;
            using (var dataB = new Database9006Entities())
            {
                var data = dataB.ledger_register.Select(e => e).Where(e => e.ledger_code == ledgerCode).FirstOrDefault();
                isRemovable = data.is_removable;
            }
            return isRemovable;
        }

        private bool IsLedgerHasChild(string ledgerCode)
        {
            bool hasChild = false;
            using (var dataB = new Database9006Entities())
            {
                var data = dataB.ledger_register.Select(e => e).Where(e => e.group_code == ledgerCode);
                if (data.Count() > 0)
                {
                    hasChild = true;
                }
            }
            return hasChild;
        }

        private bool IsLedgerUsedInTransaction(string ledgerCode)
        {
            bool isUsed = false;
            using (var dataB = new Database9006Entities())
            {
                var data = dataB.ledger_transactions.Select(e => e).Where(e => e.ledger_code == ledgerCode);
                if (data.Count() > 0)
                {
                    isUsed = true;
                }
            }
            return isUsed;
        }

        private bool? IsLedgerEditable(string ledgerCode)
        {
            bool? isEditable = false;
            using (var dataB = new Database9006Entities())
            {
                var data = dataB.ledger_register.Select(e => e).Where(e => e.ledger_code == ledgerCode);
                if (data.Count() > 0)
                {
                    foreach (var item in data)
                    {
                        isEditable = item.is_editable;
                        break;
                    }
                }
            }
            return isEditable;
        }



        public void LoadAllUniqueLedgers()
        {
            using (var dataB = new Database9006Entities())
            {
                var datas = dataB.ledger_register.Select(e => new { e.ledger_code, e.unique_id })
                    .Where(e => !e.unique_id.Equals(null));

                foreach (var item in datas)
                {
                    UniqueLedgers.LedgerCode[item.unique_id] = item.ledger_code;
                }
            }
        }

        public string ReadAGroupCodeOf(string ledgerCode)
        {
            string groupCode = "";
            using (var dataB = new Database9006Entities())
            {
                var data = dataB.ledger_register.Select(e => new { e.a_group_code, e.ledger_code }).Where(e => e.ledger_code == ledgerCode).FirstOrDefault();
                groupCode = data.a_group_code;
            }

            return groupCode;
        }

        public List<CLedger> ReadAllGroupLedgers()
        {
            List<CLedger> ledgers = new List<CLedger>();
            using (var dataB = new Database9006Entities())
            {
                var datas = dataB.ledger_register.Select(e => new { e.ledger_code, e.ledger, e.type }).Where(e => (e.type != "CAccount" && e.type != "DAccount")).OrderBy(e => e.ledger);
                foreach (var item in datas)
                {
                    ledgers.Add(new CLedger() { Ledger = item.ledger, LedgerCode = item.ledger_code, LedgerType = item.type });
                }
            }

            return ledgers;
        }

        public List<CLedger> ReadAllLedgers()
        {
            List<CLedger> ledgers = new List<CLedger>();
            using (var dataB = new Database9006Entities())
            {
                var datas = dataB.ledger_register.Select(e => new { e.ledger_code, e.ledger, e.type }).Where(e => (e.type == "CAccount" || e.type == "DAccount")).OrderBy(e => e.ledger);
                foreach (var item in datas)
                {
                    ledgers.Add(new CLedger() { Ledger = item.ledger, LedgerCode = item.ledger_code, LedgerType = item.type });
                }
            }

            return ledgers;
        }


        public ObservableCollection<CLedger> ReadAllLedgersWithGroupAsTree()
        {
            ObservableCollection<CLedger> ledgers = new ObservableCollection<CLedger>();
            try
            {
                using (var dataB1 = new Database9006Entities())
                {
                    using (var dataB2 = new Database9006Entities())
                    {
                        using (var dataB3 = new Database9006Entities())
                        {
                            using (var dataB4 = new Database9006Entities())
                            {

                                var agroups = dataB1.ledger_register.Select(e => new { e.ledger_code, e.ledger, e.type }).Where(e => (e.type == "AGroup")).OrderBy(e => e.ledger);
                                foreach (var item1 in agroups)
                                {
                                    CLedger aLedger = new CLedger() { Ledger = item1.ledger, LedgerCode = item1.ledger_code, LedgerType = item1.type };
                                    ObservableCollection<CLedger> aMembers = new ObservableCollection<CLedger>();

                                    var bLedgers = dataB2.ledger_register.Select(e => new { e.ledger_code, e.ledger, e.type, e.group_code }).Where(e => (e.group_code == aLedger.LedgerCode)).OrderBy(e => e.ledger);
                                    foreach (var item2 in bLedgers)
                                    {
                                        CLedger bLedger = new CLedger() { Ledger = item2.ledger, LedgerCode = item2.ledger_code, LedgerType = item2.type };
                                        ObservableCollection<CLedger> bMembers = new ObservableCollection<CLedger>();

                                        var cLedgers = dataB3.ledger_register.Select(e => new { e.ledger_code, e.ledger, e.type, e.group_code }).Where(e => (e.group_code == bLedger.LedgerCode)).OrderBy(e => e.ledger);
                                        foreach (var item3 in cLedgers)
                                        {
                                            CLedger cLedger = new CLedger() { Ledger = item3.ledger, LedgerCode = item3.ledger_code, LedgerType = item3.type };
                                            ObservableCollection<CLedger> cMembers = new ObservableCollection<CLedger>();

                                            var dLedgers = dataB4.ledger_register.Select(e => new { e.ledger_code, e.ledger, e.type, e.group_code }).Where(e => (e.group_code == cLedger.LedgerCode)).OrderBy(e => e.ledger);
                                            foreach (var item4 in dLedgers)
                                            {
                                                CLedger dLedger = new CLedger() { Ledger = item4.ledger, LedgerCode = item4.ledger_code, LedgerType = item4.type };
                                                cMembers.Add(dLedger);
                                            }

                                            cLedger.MemberList = cMembers;
                                            bMembers.Add(cLedger);
                                        }

                                        bLedger.MemberList = bMembers;
                                        aMembers.Add(bLedger);
                                    }

                                    aLedger.MemberList = aMembers;
                                    ledgers.Add(aLedger);

                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
            return ledgers;
        }


        public string ReadBGroupCodeOf(string ledgerCode)
        {
            string groupCode = "";
            using (var dataB = new Database9006Entities())
            {
                var data = dataB.ledger_register.Select(e => new { e.b_group_code, e.ledger_code }).Where(e => e.ledger_code == ledgerCode).FirstOrDefault();
                groupCode = data.b_group_code;
            }

            return groupCode;
        }

        public string ReadCGroupCodeOf(string ledgerCode)
        {
            string groupCode = "";
            using (var dataB = new Database9006Entities())
            {
                var data = dataB.ledger_register.Select(e => new { e.c_group_code, e.ledger_code }).Where(e => e.ledger_code == ledgerCode).FirstOrDefault();
                groupCode = data.c_group_code;
            }

            return groupCode;
        }

        public List<CLedger> ReadLedgersWithoutCash()
        {
            List<CLedger> ledgers = new List<CLedger>();
            using (var dataB = new Database9006Entities())
            {
                string cashLedgerCode = UniqueLedgers.LedgerCode["Cash"];
                var datas = dataB.ledger_register.Select(e => new { e.ledger_code, e.ledger, e.type }).Where(e => e.ledger_code != cashLedgerCode && (e.type == "CAccount" || e.type == "DAccount")).OrderBy(e => e.ledger);
                foreach (var item in datas)
                {
                    ledgers.Add(new CLedger() { Ledger = item.ledger, LedgerCode = item.ledger_code, LedgerType = item.type });
                }
            }

            return ledgers;
        }


        public List<CLedger> ReadSupplierGroupCode()
        {
            List<CLedger> ledgers = new List<CLedger>();
            using (var dataB = new Database9006Entities())
            {
                string groupCode = UniqueLedgers.LedgerCode["Sundry Creditors"];
                var datas = dataB.ledger_register.Select(e => new { e.ledger_code, e.ledger, e.type, e.group_code }).Where(e => e.ledger_code == groupCode).OrderBy(e => e.ledger);
                foreach (var item in datas)
                {
                    ledgers.Add(new CLedger() { Ledger = item.ledger, LedgerCode = item.ledger_code, LedgerType = item.type });
                }
            }

            return ledgers;
        }

        public List<CLedgerRegister> ReadAllSupplierRegisters()
        {
            List<CLedgerRegister> ledgers = new List<CLedgerRegister>();
            using (var dataB = new Database9006Entities())
            {
                string groupCode = UniqueLedgers.LedgerCode["Sundry Creditors"];
                var datas = dataB.ledger_register.Select(e => new { e.ledger_code, e.ledger, e.type, e.group_code, e.address1 }).Where(e => e.group_code == groupCode).OrderBy(e => e.ledger);
                foreach (var item in datas)
                {
                    ledgers.Add(new CLedgerRegister() { Ledger = item.ledger, LedgerCode = item.ledger_code, LedgerType = item.type, Address1 = item.address1 });
                }
            }

            return ledgers;
        }

        public List<CLedgerRegister> ReadAllCustomerRegisters()
        {
            List<CLedgerRegister> ledgers = new List<CLedgerRegister>();
            using (var dataB = new Database9006Entities())
            {
                string groupCode = UniqueLedgers.LedgerCode["Sundry Debtors"];
                var datas = dataB.ledger_register.Select(e => new { e.ledger_code, e.ledger, e.type, e.group_code, e.address1 }).Where(e => e.group_code == groupCode).OrderBy(e => e.ledger);
                foreach (var item in datas)
                {
                    ledgers.Add(new CLedgerRegister() { Ledger = item.ledger, LedgerCode = item.ledger_code, LedgerType = item.type, Address1 = item.address1 });
                }
            }

            return ledgers;
        }

        public List<CLedgerRegister> ReadAllEmployeeRegisters()
        {
            List<CLedgerRegister> ledgers = new List<CLedgerRegister>();
            using (var dataB = new Database9006Entities())
            {
                string groupCode = UniqueLedgers.LedgerCode["Staff Accounts"];
                var datas = dataB.ledger_register.Select(e => new { e.ledger_code, e.ledger, e.type, e.group_code, e.address1 }).Where(e => e.group_code == groupCode).OrderBy(e => e.ledger);
                foreach (var item in datas)
                {
                    ledgers.Add(new CLedgerRegister() { Ledger = item.ledger, LedgerCode = item.ledger_code, LedgerType = item.type, Address1 = item.address1 });
                }
            }

            return ledgers;
        }

        public List<CLedgerRegister> ReadAllBankRegisters()
        {
            List<CLedgerRegister> ledgers = new List<CLedgerRegister>();
            using (var dataB = new Database9006Entities())
            {
                string groupCode = UniqueLedgers.LedgerCode["Bank Accounts"];
                var datas = dataB.ledger_register.Select(e => new { e.ledger_code, e.ledger, e.type, e.group_code, e.address1 }).Where(e => e.group_code == groupCode).OrderBy(e => e.ledger);
                foreach (var item in datas)
                {
                    ledgers.Add(new CLedgerRegister() { Ledger = item.ledger, LedgerCode = item.ledger_code, LedgerType = item.type, Address1 = item.address1 });
                }
            }

            return ledgers;
        }

        public List<CLedger> ReadSupplierLedgers()
        {
            List<CLedger> ledgers = new List<CLedger>();
            using (var dataB = new Database9006Entities())
            {
                string groupCode = UniqueLedgers.LedgerCode["Sundry Creditors"];
                var datas = dataB.ledger_register.Select(e => new { e.ledger_code, e.ledger, e.type, e.group_code }).Where(e => e.group_code == groupCode).OrderBy(e => e.ledger);
                foreach (var item in datas)
                {
                    ledgers.Add(new CLedger() { Ledger = item.ledger, LedgerCode = item.ledger_code, LedgerType = item.type });
                }
            }

            return ledgers;
        }

        public List<CLedger> ReadCustomerLedgers()
        {
            List<CLedger> ledgers = new List<CLedger>();
            using (var dataB = new Database9006Entities())
            {
                string groupCode = UniqueLedgers.LedgerCode["Sundry Debtors"];
                var datas = dataB.ledger_register.Select(e => new { e.ledger_code, e.ledger, e.type, e.group_code }).Where(e => e.group_code == groupCode).OrderBy(e => e.ledger);
                foreach (var item in datas)
                {
                    ledgers.Add(new CLedger() { Ledger = item.ledger, LedgerCode = item.ledger_code, LedgerType = item.type });
                }
            }

            return ledgers;
        }

        public List<CLedger> ReadEmployeeLedgers()
        {
            List<CLedger> ledgers = new List<CLedger>();
            using (var dataB = new Database9006Entities())
            {
                string groupCode = UniqueLedgers.LedgerCode["Staff Accounts"];
                var datas = dataB.ledger_register.Select(e => new { e.ledger_code, e.ledger, e.type, e.group_code }).Where(e => e.group_code == groupCode).OrderBy(e => e.ledger);
                foreach (var item in datas)
                {
                    ledgers.Add(new CLedger() { Ledger = item.ledger, LedgerCode = item.ledger_code, LedgerType = item.type });
                }
            }

            return ledgers;
        }

        public List<CLedger> ReadBankLedgers()
        {
            List<CLedger> ledgers = new List<CLedger>();
            using (var dataB = new Database9006Entities())
            {
                string groupCode = UniqueLedgers.LedgerCode["Bank Accounts"];
                var datas = dataB.ledger_register.Select(e => new { e.ledger_code, e.ledger, e.type, e.group_code }).Where(e => e.group_code == groupCode).OrderBy(e => e.ledger);
                foreach (var item in datas)
                {
                    ledgers.Add(new CLedger() { Ledger = item.ledger, LedgerCode = item.ledger_code, LedgerType = item.type });
                }
            }

            return ledgers;
        }

        public List<CLedger> ReadCustomerGroupCode()
        {
            List<CLedger> ledgers = new List<CLedger>();
            using (var dataB = new Database9006Entities())
            {
                string groupCode = UniqueLedgers.LedgerCode["Sundry Debtors"];
                var datas = dataB.ledger_register.Select(e => new { e.ledger_code, e.ledger, e.type, e.group_code }).Where(e => e.ledger_code == groupCode).OrderBy(e => e.ledger);
                foreach (var item in datas)
                {
                    ledgers.Add(new CLedger() { Ledger = item.ledger, LedgerCode = item.ledger_code, LedgerType = item.type });
                }
            }

            return ledgers;
        }



        public List<CLedger> ReadEmployeeGroupCode()
        {
            List<CLedger> ledgers = new List<CLedger>();
            using (var dataB = new Database9006Entities())
            {
                string groupCode = UniqueLedgers.LedgerCode["Staff Accounts"];
                var datas = dataB.ledger_register.Select(e => new { e.ledger_code, e.ledger, e.type, e.group_code }).Where(e => e.ledger_code == groupCode).OrderBy(e => e.ledger);
                foreach (var item in datas)
                {
                    ledgers.Add(new CLedger() { Ledger = item.ledger, LedgerCode = item.ledger_code, LedgerType = item.type });
                }
            }

            return ledgers;
        }



        public List<CLedger> ReadBankGroupCode()
        {
            List<CLedger> ledgers = new List<CLedger>();
            using (var dataB = new Database9006Entities())
            {
                string groupCode = UniqueLedgers.LedgerCode["Bank Accounts"];
                var datas = dataB.ledger_register.Select(e => new { e.ledger_code, e.ledger, e.type, e.group_code }).Where(e => e.ledger_code == groupCode).OrderBy(e => e.ledger);
                foreach (var item in datas)
                {
                    ledgers.Add(new CLedger() { Ledger = item.ledger, LedgerCode = item.ledger_code, LedgerType = item.type });
                }
            }

            return ledgers;
        }

        public List<CTrialBalance> FindTrialBalance(string financialCode, DateTime startDate, DateTime endDate)
        {
            List<CTrialBalance> report = new List<CTrialBalance>();
            using (var dataB = new Database9006Entities())
            {
                int serNo = 0;
                decimal? totalDebit = 0;
                decimal? totalCredit = 0;

                string[] groupCodes = new string[] { UniqueLedgers.LedgerCode["Asset"], UniqueLedgers.LedgerCode["Liabilities"] };
                var datas1 = from lr in dataB.ledger_register
                             from lt in dataB.ledger_transactions
                             where lr.type == "BGroup"
                                 && groupCodes.Contains(lr.a_group_code)
                                 && lt.b_group_code == lr.ledger_code
                                 && lt.financial_code == financialCode
                             group new { lr.ledger, lr.ledger_code, lr.type, lt.debit, lt.credit, lr.group_code } by new { lr.ledger, lr.ledger_code, lr.group_code } into lrt
                             orderby lrt.FirstOrDefault().ledger
                             select new
                             {
                                 Ledger = lrt.FirstOrDefault().ledger,
                                 LedgerCode = lrt.FirstOrDefault().ledger_code,
                                 LedgerType = lrt.FirstOrDefault().type,
                                 Debit = lrt.Sum(l => l.debit),
                                 Credit = lrt.Sum(l => l.credit),
                                 GroupCode = lrt.FirstOrDefault().group_code
                             };
                foreach (var item in datas1)
                {
                    decimal? credit = (item.Credit - item.Debit) > 0 ? (decimal)(item.Credit - item.Debit) : 0;
                    decimal? debit = (item.Debit - item.Credit) > 0 ? (decimal)(item.Debit - item.Credit) : 0;

                    totalDebit += debit;
                    totalCredit += credit;

                    report.Add(new CTrialBalance() { SerialNo = (++serNo).ToString(), Ledger = item.Ledger, LedgerCode = item.LedgerCode, LedgerType = item.LedgerType, Debit = (decimal)debit, Credit = (decimal)credit, GroupCode = item.GroupCode });
                }



                groupCodes = new string[] { UniqueLedgers.LedgerCode["Income"], UniqueLedgers.LedgerCode["Expense"] };
                var datas2 = from lr in dataB.ledger_register
                             from lt in dataB.ledger_transactions
                             where lr.type == "BGroup"
                                 && groupCodes.Contains(lr.a_group_code)
                                 && lt.b_group_code == lr.ledger_code
                                 && lt.financial_code == financialCode
                             /*lr.type == "BGroup"
                                 && groupCodes.Contains(lr.a_group_code)
                                 && lt.b_group_code == lr.ledger_code
                                 && lt.bill_date_time >= startDate
                                 && lt.bill_date_time <= endDate*/
                             group new { lr.ledger, lr.ledger_code, lr.type, lt.debit, lt.credit, lr.group_code } by new { lr.ledger, lr.ledger_code, lr.group_code } into lrt
                             orderby lrt.FirstOrDefault().ledger
                             select new
                             {
                                 Ledger = lrt.FirstOrDefault().ledger,
                                 LedgerCode = lrt.FirstOrDefault().ledger_code,
                                 LedgerType = lrt.FirstOrDefault().type,
                                 Debit = lrt.Sum(l => l.debit),
                                 Credit = lrt.Sum(l => l.credit),
                                 GroupCode = lrt.FirstOrDefault().group_code
                             };


                /*var datas3 = from lr in dataB.ledger_register
                             from lt in dataB.ledger_transactions
                             where lr.type == "BGroup"
                                 && groupCodes.Contains(lr.a_group_code)
                                 && lt.b_group_code == lr.ledger_code
                                 && lt.bill_date_time < startDate
                             select new
                             {
                                 Debit = lt.debit,
                                 Credit = lt.credit
                             };*/


                foreach (var item in datas2)
                {
                    decimal? credit = (item.Credit - item.Debit) > 0 ? (decimal)(item.Credit - item.Debit) : 0;
                    decimal? debit = (item.Debit - item.Credit) > 0 ? (decimal)(item.Debit - item.Credit) : 0;

                    totalDebit += debit;
                    totalCredit += credit;

                    report.Add(new CTrialBalance() { SerialNo = (++serNo).ToString(), Ledger = item.Ledger, LedgerCode = item.LedgerCode, LedgerType = item.LedgerType, Debit = (decimal)debit, Credit = (decimal)credit, GroupCode = item.GroupCode });
                }

                /*if (datas3.Count() > 0)
                {
                    decimal? credit = datas3.Sum(l => l.Credit);
                    decimal? debit = datas3.Sum(l => l.Debit);

                    totalDebit += debit;
                    totalCredit += credit;

                    report.Add(new CTrialBalance() { SerialNo = (++serNo).ToString(), Ledger = "Profit n Loss", LedgerCode = "", LedgerType = "", Debit = (debit - credit) > 0 ? (decimal)(debit - credit) : 0, Credit = (credit - debit) > 0 ? (decimal)(credit - debit) : 0, GroupCode = "" });
                }*/

                //Total
                report.Add(new CTrialBalance() { SerialNo = "", Ledger = "Total", LedgerCode = "", LedgerType = "", Debit = (decimal)totalDebit, Credit = (decimal)totalCredit, GroupCode = "" });

            }

            return report;
        }

        public List<CTrialBalance> FindTrialBalanceOfBGroup(string groupCode, string financialCode, DateTime startDate, DateTime endDate)
        {
            List<CTrialBalance> report = new List<CTrialBalance>();
            using (var dataB = new Database9006Entities())
            {
                int serNo = 0;
                decimal? totalDebit = 0;
                decimal? totalCredit = 0;

                var datas1 = from lr in dataB.ledger_register
                             from lt in dataB.ledger_transactions
                             where lr.group_code == groupCode
                                 && (lr.type == "CGroup")
                                 && lt.c_group_code == lr.ledger_code
                                 && lt.financial_code == financialCode
                             group new { lr.ledger, lr.ledger_code, lr.type, lt.debit, lt.credit, lr.group_code } by new { lr.ledger, lr.ledger_code, lr.group_code } into lrt
                             orderby lrt.FirstOrDefault().ledger
                             select new
                             {
                                 Ledger = lrt.FirstOrDefault().ledger,
                                 LedgerCode = lrt.FirstOrDefault().ledger_code,
                                 LedgerType = lrt.FirstOrDefault().type,
                                 Debit = lrt.Sum(l => l.debit),
                                 Credit = lrt.Sum(l => l.credit),
                                 GroupCode = lrt.FirstOrDefault().group_code
                             };
                foreach (var item in datas1)
                {
                    decimal? credit = (item.Credit - item.Debit) > 0 ? (decimal)(item.Credit - item.Debit) : 0;
                    decimal? debit = (item.Debit - item.Credit) > 0 ? (decimal)(item.Debit - item.Credit) : 0;

                    totalDebit += debit;
                    totalCredit += credit;

                    report.Add(new CTrialBalance() { SerialNo = (++serNo).ToString(), Ledger = item.Ledger, LedgerCode = item.LedgerCode, LedgerType = item.LedgerType, Debit = (decimal)debit, Credit = (decimal)credit, GroupCode = item.GroupCode });
                }

                var datas2 = from lr in dataB.ledger_register
                             from lt in dataB.ledger_transactions
                             where lr.group_code == groupCode
                                 && (lr.type == "CAccount")
                                 && lt.ledger_code == lr.ledger_code
                                 && lt.financial_code == financialCode
                             group new { lr.ledger, lr.ledger_code, lr.type, lt.debit, lt.credit, lr.group_code } by new { lr.ledger, lr.ledger_code, lr.group_code } into lrt
                             orderby lrt.FirstOrDefault().ledger
                             select new
                             {
                                 Ledger = lrt.FirstOrDefault().ledger,
                                 LedgerCode = lrt.FirstOrDefault().ledger_code,
                                 LedgerType = lrt.FirstOrDefault().type,
                                 Debit = lrt.Sum(l => l.debit),
                                 Credit = lrt.Sum(l => l.credit),
                                 GroupCode = lrt.FirstOrDefault().group_code
                             };
                foreach (var item in datas2)
                {
                    decimal? credit = (item.Credit - item.Debit) > 0 ? (decimal)(item.Credit - item.Debit) : 0;
                    decimal? debit = (item.Debit - item.Credit) > 0 ? (decimal)(item.Debit - item.Credit) : 0;

                    totalDebit += debit;
                    totalCredit += credit;

                    report.Add(new CTrialBalance() { SerialNo = (++serNo).ToString(), Ledger = item.Ledger, LedgerCode = item.LedgerCode, LedgerType = item.LedgerType, Debit = (decimal)debit, Credit = (decimal)credit, GroupCode = item.GroupCode });
                }

                report.Add(new CTrialBalance() { SerialNo = "", Ledger = "Sub Total", LedgerCode = "", LedgerType = "", Debit = (decimal)totalDebit, Credit = (decimal)totalCredit, GroupCode = "" });

            }

            return report;
        }


        public List<CTrialBalance> FindTrialBalanceOfCGroup(string groupCode, string financialCode, DateTime startDate, DateTime endDate)
        {
            List<CTrialBalance> report = new List<CTrialBalance>();
            using (var dataB = new Database9006Entities())
            {
                int serNo = 0;
                decimal? totalDebit = 0;
                decimal? totalCredit = 0;

                var datas1 = from lr in dataB.ledger_register
                             from lt in dataB.ledger_transactions
                             where lr.group_code == groupCode
                                 && (lr.type == "DAccount")
                                 && lt.ledger_code == lr.ledger_code
                                 && lt.financial_code == financialCode
                             group new { lr.ledger, lr.ledger_code, lr.type, lt.debit, lt.credit, lr.group_code } by new { lr.ledger, lr.ledger_code, lr.group_code } into lrt
                             orderby lrt.FirstOrDefault().ledger
                             select new
                             {
                                 Ledger = lrt.FirstOrDefault().ledger,
                                 LedgerCode = lrt.FirstOrDefault().ledger_code,
                                 LedgerType = lrt.FirstOrDefault().type,
                                 Debit = lrt.Sum(l => l.debit),
                                 Credit = lrt.Sum(l => l.credit),
                                 GroupCode = lrt.FirstOrDefault().group_code
                             };
                foreach (var item in datas1)
                {
                    decimal? credit = (item.Credit - item.Debit) > 0 ? (decimal)(item.Credit - item.Debit) : 0;
                    decimal? debit = (item.Debit - item.Credit) > 0 ? (decimal)(item.Debit - item.Credit) : 0;

                    totalDebit += debit;
                    totalCredit += credit;

                    report.Add(new CTrialBalance() { SerialNo = (++serNo).ToString(), Ledger = item.Ledger, LedgerCode = item.LedgerCode, LedgerType = item.LedgerType, Debit = (decimal)debit, Credit = (decimal)credit, GroupCode = item.GroupCode });
                }

                report.Add(new CTrialBalance() { SerialNo = "", Ledger = "  Sub Total", LedgerCode = "", LedgerType = "", Debit = (decimal)totalDebit, Credit = (decimal)totalCredit, GroupCode = "" });

            }

            return report;
        }

        public List<CBalanceSheet> FindBalanceSheet(DateTime endDate)
        {
            //asset | amount | liabilities | amount | assetcode | liabilitycode
            List<CBalanceSheet> report = new List<CBalanceSheet>();
            using (var dataB = new Database9006Entities())
            {

                string aGroupCode = UniqueLedgers.LedgerCode["Asset"];
                var assetBGroupCodes = dataB.ledger_register.Where(e => e.group_code == aGroupCode).Select(e => e.ledger_code);

                var assetData = from lr in dataB.ledger_register
                                from lt in dataB.ledger_transactions
                                where lr.type == "BGroup"
                                    && assetBGroupCodes.Contains(lr.ledger_code)
                                    && lt.b_group_code == lr.ledger_code
                                    && lt.bill_date_time <= endDate
                                group new { lr.ledger, lr.ledger_code, lt.debit, lt.credit } by new { lr.ledger, lr.ledger_code, lr.b_group_code } into lrt
                                orderby lrt.FirstOrDefault().ledger
                                select new
                                {
                                    Ledger = lrt.FirstOrDefault().ledger,
                                    LedgerCode = lrt.FirstOrDefault().ledger_code,
                                    Amount = lrt.Sum(l => (l.debit - l.credit)),
                                };

                string lGroupCode = UniqueLedgers.LedgerCode["Liabilities"];
                var liabilityBGroupCodes = dataB.ledger_register.Where(e => e.group_code == lGroupCode).Select(e => e.ledger_code);

                var liabilityData = from lr in dataB.ledger_register
                                    from lt in dataB.ledger_transactions
                                    where lr.type == "BGroup"
                                        && liabilityBGroupCodes.Contains(lr.ledger_code)
                                        && lt.b_group_code == lr.ledger_code
                                        && lt.bill_date_time <= endDate
                                    group new { lr.ledger, lr.ledger_code, lt.debit, lt.credit } by new { lr.ledger, lr.ledger_code, lr.b_group_code } into lrt
                                    orderby lrt.FirstOrDefault().ledger
                                    select new
                                    {
                                        Ledger = lrt.FirstOrDefault().ledger,
                                        LedgerCode = lrt.FirstOrDefault().ledger_code,
                                        Amount = lrt.Sum(l => (l.debit - l.credit)) * -1,
                                    };


                //Finding Asset				
                decimal? total = 0;
                //report.Add(new CBalanceSheet() { Asset = "Asset", AssetAmount = null, Liabilities = "Liabilities", LiabilityAmount = null});
                foreach (var item in assetData)
                {
                    total = total + item.Amount;

                    report.Add(new CBalanceSheet() { Asset = item.Ledger, AssetAmount = item.Amount, Liabilities = "", LiabilityAmount = null, AssetCode = item.LedgerCode, LiabilityCode = "" });
                }
                // Blank Entry
                report.Add(new CBalanceSheet() { Asset = "", AssetAmount = null, Liabilities = "", LiabilityAmount = null, AssetCode = "", LiabilityCode = "" });
                //Total Asset
                report.Add(new CBalanceSheet() { Asset = "Total Asset", AssetAmount = total, Liabilities = "", LiabilityAmount = null, AssetCode = "", LiabilityCode = "" });
                decimal? assetTotal = total;
                //Finding Liabilities
                total = 0;
                int index = 1;
                foreach (var item in liabilityData)
                {
                    total = total + item.Amount;

                    if (index >= report.Count)
                    {
                        report.Add(new CBalanceSheet() { Asset = "", AssetAmount = null, Liabilities = item.Ledger, LiabilityAmount = item.Amount, AssetCode = "", LiabilityCode = item.LedgerCode });
                    }
                    else
                    {
                        report.ElementAt(index).Liabilities = item.Ledger;
                        report.ElementAt(index).LiabilityAmount = item.Amount;
                        report.ElementAt(index).LiabilityCode = item.LedgerCode;
                        ++index;
                    }

                }

                // Blank Entry                
                if (index >= report.Count)
                {
                    report.Add(new CBalanceSheet() { Asset = "", AssetAmount = null, Liabilities = "", LiabilityAmount = null, AssetCode = "", LiabilityCode = "" });
                }
                else
                {
                    ++index;
                }

                //Total Liabilities				
                if (index >= report.Count)
                {
                    report.Add(new CBalanceSheet() { Asset = "", AssetAmount = null, Liabilities = "Total Liabilities", LiabilityAmount = total, AssetCode = "", LiabilityCode = "" });
                }
                else
                {
                    report.ElementAt(index).Liabilities = "Total Liabilities";
                    report.ElementAt(index).LiabilityAmount = total;
                    report.ElementAt(index).LiabilityCode = "";
                    ++index;
                }

                // Blank Entry                
                if (index >= report.Count)
                {
                    report.Add(new CBalanceSheet() { Asset = "", AssetAmount = null, Liabilities = "", LiabilityAmount = null, AssetCode = "", LiabilityCode = "" });
                }
                else
                {
                    ++index;
                }

                //Find Equity (Asset - Liabilities)
                if (index >= report.Count)
                {
                    report.Add(new CBalanceSheet() { Asset = "", AssetAmount = null, Liabilities = "Total Equity (Capital)", LiabilityAmount = assetTotal - total, AssetCode = "", LiabilityCode = "" });
                }
                else
                {
                    report.ElementAt(index).Liabilities = "Total Equity (Capital)";
                    report.ElementAt(index).LiabilityAmount = assetTotal - total;
                    report.ElementAt(index).LiabilityCode = "";
                }

            }

            return report;
        }

        public List<CBalanceSheetDetails> FindBalanceSheetDetails(DateTime endDate, string ledgerCode)
        {
            //asset | amount | liabilities | amount | assetcode | liabilitycode
            List<CBalanceSheetDetails> report = new List<CBalanceSheetDetails>();
            using (var dataB = new Database9006Entities())
            {

                CLedgerRegister cl = ReadLedgerRegister(ledgerCode);
                if (cl.LedgerType.EndsWith("Group"))
                {
                    var groupCodes = dataB.ledger_register.Where(e => e.group_code == ledgerCode).Select(e => e.ledger_code);

                    var resData = from lr in dataB.ledger_register
                                  from lt in dataB.ledger_transactions
                                  where groupCodes.Contains(lr.ledger_code)
                                      && (lt.b_group_code == lr.ledger_code || lt.c_group_code == lr.ledger_code || lt.ledger_code == lr.ledger_code)
                                      && lt.bill_date_time <= endDate
                                  group new { lr.ledger, lr.ledger_code, lt.debit, lt.credit, lr.type } by new { lr.ledger, lr.ledger_code, lr.b_group_code } into lrt
                                  orderby lrt.FirstOrDefault().ledger
                                  select new CBalanceSheetDetails
                                  {
                                      Ledger = lrt.FirstOrDefault().ledger,
                                      LedgerCode = lrt.FirstOrDefault().ledger_code,
                                      Debit = lrt.Sum(l => l.debit),
                                      Credit = lrt.Sum(l => l.credit),
                                      LedgerType = lrt.FirstOrDefault().type

                                  };

                    foreach (var item in resData)
                    {
                        report.Add(item);
                    }
                }
            }

            return report;
        }

        public List<CIncomeStatement> FindIncomeStatement(DateTime startDate, DateTime endDate)
        {
            //asset | amount | liabilities | amount | assetcode | liabilitycode
            List<CIncomeStatement> report = new List<CIncomeStatement>();
            using (var dataB = new Database9006Entities())
            {

                string iGroupCode = UniqueLedgers.LedgerCode["Income"];
                string eGroupCode = UniqueLedgers.LedgerCode["Expense"];
                var incomeBGroupCodes = dataB.ledger_register.Where(e => e.group_code == iGroupCode).Select(e => e.ledger_code);
                var expenseBGroupCodes = dataB.ledger_register.Where(e => e.group_code == eGroupCode).Select(e => e.ledger_code);

                decimal? oIBalance = dataB.ledger_transactions.Where(e => e.a_group_code == iGroupCode && e.bill_date_time <= startDate).Sum(e => e.credit - e.debit);
                decimal? oEBalance = dataB.ledger_transactions.Where(e => e.a_group_code == eGroupCode && e.bill_date_time <= startDate).Sum(e => e.debit - e.credit);

                decimal val = oIBalance.HasValue ? oIBalance.Value : 0;
                val -= oEBalance.HasValue ? oEBalance.Value : 0;
                decimal oBalance = val;

                //Opening Balance
                report.Add(new CIncomeStatement() { LedgerCode = "", LedgerType = "", Ledger = "Opening Balance", Amount = null, Total = val });
                // Blank Entry
                report.Add(new CIncomeStatement() { LedgerCode = "", LedgerType = "", Ledger = "", Amount = null, Total = null });

                var incomeData = from lr in dataB.ledger_register
                                 from lt in dataB.ledger_transactions
                                 where lr.type == "BGroup"
                                     && incomeBGroupCodes.Contains(lr.ledger_code)
                                     && lt.b_group_code == lr.ledger_code
                                     && lt.bill_date_time > startDate
                                     && lt.bill_date_time <= endDate
                                 group new { lr.ledger, lr.ledger_code, lr.type, lt.debit, lt.credit } by new { lr.ledger, lr.ledger_code, lr.b_group_code } into lrt
                                 orderby lrt.FirstOrDefault().ledger
                                 select new
                                 {
                                     Ledger = lrt.FirstOrDefault().ledger,
                                     LedgerCode = lrt.FirstOrDefault().ledger_code,
                                     Amount = lrt.Sum(l => (l.credit - l.debit)),
                                     LedgerType = lrt.FirstOrDefault().type
                                 };

                var expenseData = from lr in dataB.ledger_register
                                  from lt in dataB.ledger_transactions
                                  where lr.type == "BGroup"
                                      && expenseBGroupCodes.Contains(lr.ledger_code)
                                      && lt.b_group_code == lr.ledger_code
                                      && lt.bill_date_time > startDate
                                      && lt.bill_date_time <= endDate
                                  group new { lr.ledger, lr.ledger_code, lr.type, lt.debit, lt.credit } by new { lr.ledger, lr.ledger_code, lr.b_group_code } into lrt
                                  orderby lrt.FirstOrDefault().ledger
                                  select new
                                  {
                                      Ledger = lrt.FirstOrDefault().ledger,
                                      LedgerCode = lrt.FirstOrDefault().ledger_code,
                                      Amount = lrt.Sum(l => (l.debit - l.credit)),
                                      LedgerType = lrt.FirstOrDefault().type
                                  };



                //Finding Income				
                decimal? total = 0;
                //Income Header
                report.Add(new CIncomeStatement() { LedgerCode = "", LedgerType = "", Ledger = "Income", Amount = null, Total = null });
                foreach (var item in incomeData)
                {
                    val = (decimal)item.Amount;
                    total = total + val;
                    report.Add(new CIncomeStatement() { Ledger = item.Ledger, LedgerType = item.LedgerType, LedgerCode = item.LedgerCode, Amount = val, Total = null });
                }
                // Blank Entry
                report.Add(new CIncomeStatement() { LedgerCode = "", LedgerType = "", Ledger = "", Amount = null, Total = null });
                //Total Income
                report.Add(new CIncomeStatement() { LedgerCode = "", LedgerType = "", Ledger = "Total Income", Amount = null, Total = total });
                decimal? incomeTotal = total;

                //Finding Expense
                total = 0;
                // Blank Entry
                report.Add(new CIncomeStatement() { LedgerCode = "", LedgerType = "", Ledger = "", Amount = null, Total = null });
                //Expense Header
                report.Add(new CIncomeStatement() { LedgerCode = "", LedgerType = "", Ledger = "Expense", Amount = null, Total = null });
                foreach (var item in expenseData)
                {
                    val = (decimal)item.Amount;
                    total = total + val;
                    report.Add(new CIncomeStatement() { Ledger = item.Ledger, LedgerType = item.LedgerType, LedgerCode = item.LedgerCode, Amount = val, Total = null });
                }
                // Blank Entry
                report.Add(new CIncomeStatement() { LedgerCode = "", LedgerType = "", Ledger = "", Amount = null, Total = null });
                //Total Expense
                report.Add(new CIncomeStatement() { LedgerCode = "", LedgerType = "", Ledger = "Total Expense", Amount = null, Total = total });

                // Blank Entry
                report.Add(new CIncomeStatement() { LedgerCode = "", LedgerType = "", Ledger = "", Amount = null, Total = null });
                //Net Income or Expense
                decimal netValue = (oBalance + (decimal)incomeTotal) - (decimal)total;
                report.Add(new CIncomeStatement() { LedgerCode = "", LedgerType = "", Ledger = netValue >= 0 ? "Net Profit" : "Net Loss", Amount = null, Total = netValue >= 0 ? netValue : netValue * -1 });

            }

            return report;
        }

        public List<CIncomeStatementDetails> FindIncomeStatementDetails(DateTime startDate, DateTime endDate, string ledgerCode)
        {
            //asset | amount | liabilities | amount | assetcode | liabilitycode
            List<CIncomeStatementDetails> report = new List<CIncomeStatementDetails>();
            using (var dataB = new Database9006Entities())
            {

                CLedgerRegister cl = ReadLedgerRegister(ledgerCode);
                if (cl.LedgerType.EndsWith("Group"))
                {
                    var groupCodes = dataB.ledger_register.Where(e => e.group_code == ledgerCode).Select(e => e.ledger_code);

                    var resData = from lr in dataB.ledger_register
                                  from lt in dataB.ledger_transactions
                                  where groupCodes.Contains(lr.ledger_code)
                                      && (lt.b_group_code == lr.ledger_code || lt.c_group_code == lr.ledger_code || lt.ledger_code == lr.ledger_code)
                                      && lt.bill_date_time <= endDate
                                  group new { lr.ledger, lr.ledger_code, lt.debit, lt.credit, lr.type } by new { lr.ledger, lr.ledger_code, lr.b_group_code } into lrt
                                  orderby lrt.FirstOrDefault().ledger
                                  select new CIncomeStatementDetails
                                  {
                                      Ledger = lrt.FirstOrDefault().ledger,
                                      LedgerCode = lrt.FirstOrDefault().ledger_code,
                                      Debit = lrt.Sum(l => l.debit),
                                      Credit = lrt.Sum(l => l.credit),
                                      LedgerType = lrt.FirstOrDefault().type

                                  };

                    foreach (var item in resData)
                    {
                        report.Add(item);
                    }
                }
            }

            return report;
        }

        public bool DeleteLedgerTransaction(string refBillNo, string refBillType, string financialCode)
        {
            bool returnValue = true;

            lock (Synchronizer.@lock)
            {
                using (var dataB = new Database9006Entities())
                {
                    var dataBTransaction = dataB.Database.BeginTransaction();
                    try
                    {

                        var cr = dataB.ledger_transactions.Select(c => c).Where(x => x.ref_bill_no == refBillNo && x.bill_type == refBillType && x.financial_code == financialCode);
                        dataB.ledger_transactions.RemoveRange(cr);

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

        public bool CreateLedgerTransaction(CLedgerTransaction ledgerT)
        {
            bool returnValue = false;

            lock (Synchronizer.@lock)
            {

                using (var dataB = new Database9006Entities())
                {
                    var dataBTransaction = dataB.Database.BeginTransaction();
                    try
                    {

                        var cr = dataB.ledger_transactions.Select(c => c).Where(x => x.ref_bill_no == ledgerT.RefBillNo && x.bill_type == ledgerT.RefBillType && x.financial_code == ledgerT.FinancialCode);
                        int cbillNo = int.Parse(cr.FirstOrDefault().bill_no);
                        dataB.ledger_transactions.RemoveRange(cr);



                        //Debit side
                        ledger_transactions lt1 = dataB.ledger_transactions.Create();
                        lt1.bill_no = cbillNo.ToString();
                        lt1.bill_date_time = ledgerT.BillDate;
                        lt1.serial_no = 1;
                        lt1.bill_type = ledgerT.RefBillType;
                        lt1.ref_bill_no = ledgerT.RefBillNo;
                        lt1.ref_bill_date_time = ledgerT.BillDate;
                        lt1.ledger = ledgerT.DebitLedger;
                        lt1.ledger_code = ledgerT.DebitLedgerCode;
                        lt1.co_ledger = ledgerT.CreditLedger;
                        lt1.financial_code = ledgerT.FinancialCode;
                        lt1.debit = ledgerT.Amount;
                        lt1.credit = 0;
                        lt1.a_group_code = ReadAGroupCodeOf(ledgerT.DebitLedgerCode);
                        lt1.b_group_code = ReadBGroupCodeOf(ledgerT.DebitLedgerCode);
                        lt1.c_group_code = ReadCGroupCodeOf(ledgerT.DebitLedgerCode);

                        //Credit side
                        ledger_transactions lt2 = dataB.ledger_transactions.Create();
                        lt2.bill_no = cbillNo.ToString();
                        lt2.bill_date_time = ledgerT.BillDate;
                        lt2.serial_no = 2;
                        lt2.bill_type = ledgerT.RefBillType;
                        lt2.ref_bill_no = ledgerT.RefBillNo;
                        lt2.ref_bill_date_time = ledgerT.BillDate;
                        lt2.ledger = ledgerT.CreditLedger;
                        lt2.ledger_code = ledgerT.CreditLedgerCode;
                        lt2.co_ledger = ledgerT.DebitLedger;
                        lt2.financial_code = ledgerT.FinancialCode;
                        lt2.debit = 0;
                        lt2.credit = ledgerT.Amount;
                        lt2.a_group_code = ReadAGroupCodeOf(ledgerT.CreditLedgerCode);
                        lt2.b_group_code = ReadBGroupCodeOf(ledgerT.CreditLedgerCode);
                        lt2.c_group_code = ReadCGroupCodeOf(ledgerT.CreditLedgerCode);

                        dataB.ledger_transactions.Add(lt1);
                        dataB.ledger_transactions.Add(lt2);
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

        public bool UpdateLedgerTransaction(CLedgerTransaction ledgerT)
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


                        int cbillNo = bs.ReadNextLedgerTransactionBillNo(ledgerT.FinancialCode);
                        bs.UpdateLedgerTransactionBillNo(ledgerT.FinancialCode, cbillNo + 1);

                        //Debit side
                        ledger_transactions lt1 = dataB.ledger_transactions.Create();
                        lt1.bill_no = cbillNo.ToString();
                        lt1.bill_date_time = ledgerT.BillDate;
                        lt1.serial_no = 1;
                        lt1.bill_type = ledgerT.RefBillType;
                        lt1.ref_bill_no = ledgerT.RefBillNo;
                        lt1.ref_bill_date_time = ledgerT.BillDate;
                        lt1.ledger = ledgerT.DebitLedger;
                        lt1.ledger_code = ledgerT.DebitLedgerCode;
                        lt1.co_ledger = ledgerT.CreditLedger;
                        lt1.financial_code = ledgerT.FinancialCode;
                        lt1.debit = ledgerT.Amount;
                        lt1.credit = 0;
                        lt1.a_group_code = ReadAGroupCodeOf(ledgerT.DebitLedgerCode);
                        lt1.b_group_code = ReadBGroupCodeOf(ledgerT.DebitLedgerCode);
                        lt1.c_group_code = ReadCGroupCodeOf(ledgerT.DebitLedgerCode);

                        //Credit side
                        ledger_transactions lt2 = dataB.ledger_transactions.Create();
                        lt2.bill_no = cbillNo.ToString();
                        lt2.bill_date_time = ledgerT.BillDate;
                        lt2.serial_no = 2;
                        lt2.bill_type = ledgerT.RefBillType;
                        lt2.ref_bill_no = ledgerT.RefBillNo;
                        lt2.ref_bill_date_time = ledgerT.BillDate;
                        lt2.ledger = ledgerT.CreditLedger;
                        lt2.ledger_code = ledgerT.CreditLedgerCode;
                        lt2.co_ledger = ledgerT.DebitLedger;
                        lt2.financial_code = ledgerT.FinancialCode;
                        lt2.debit = 0;
                        lt2.credit = ledgerT.Amount;
                        lt2.a_group_code = ReadAGroupCodeOf(ledgerT.CreditLedgerCode);
                        lt2.b_group_code = ReadBGroupCodeOf(ledgerT.CreditLedgerCode);
                        lt2.c_group_code = ReadCGroupCodeOf(ledgerT.CreditLedgerCode);

                        dataB.ledger_transactions.Add(lt1);
                        dataB.ledger_transactions.Add(lt2);
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

    }
}
