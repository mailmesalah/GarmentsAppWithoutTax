using ServerServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfServerApp.Services
{
    class BillNoService : IBillNo
    {
        private static readonly object @billNoLock = new object();

        public bool DeleteFinancialYear(string financialCode)
        {
            bool success = true;

            try
            {
                throw new NotImplementedException();
            }
            catch
            {
                success = false;
            }

            return success;
        }

        public List<string> ReadAllFinancialCodes()
        {
            List<string> fcodes = new List<string>();
            try
            {
                
                using (var dataB = new Database9006Entities())
                {
                    var vals = dataB.bill_nos.Select(e=>e.financial_code);
                    foreach (var item in vals)
                    {
                        fcodes.Add(item);
                    }
                }                
            }
            catch
            {

            }
            return fcodes;
        }
        
        public int ReadNextProductRegisterBillNo()
        {
            int billNo = 1;
            try
            {
                using (var dataB = new Database9006Entities())
                {
                    var val = dataB.bill_nos.Where(e =>e.bill_type == "PR").FirstOrDefault();

                    if (val == null)//Create a row for new Financial year
                    {
                        lock (@billNoLock)
                        {
                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = 1;
                            bn.bill_type = "PR";
                            bn.financial_code = "";

                            dataB.bill_nos.Add(bn);
                            dataB.SaveChanges();
                        }
                    }
                    else
                    {
                        billNo = val.bill_no;
                    }
                }
            }
            catch
            {

            }
            return billNo;
        }

        public int ReadNextUnitRegisterBillNo()
        {
            int billNo = 1;
            try
            {
                using (var dataB = new Database9006Entities())
                {
                    var val = dataB.bill_nos.Where(e => e.bill_type == "UR").FirstOrDefault();

                    if (val == null)//Create a row for new Financial year
                    {
                        lock (@billNoLock)
                        {
                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = 1;
                            bn.bill_type = "UR";
                            bn.financial_code = "";

                            dataB.bill_nos.Add(bn);
                            dataB.SaveChanges();
                        }
                    }
                    else
                    {
                        billNo = val.bill_no;
                    }
                }
            }
            catch
            {

            }
            return billNo;
        }

        public int ReadNextBankDepositBillNo(string financialCode)
        {
            int billNo = 1;
            try
            {
                using (var dataB = new Database9006Entities())
                {
                    var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == "BD").FirstOrDefault();

                    if (val == null)//Create a row for new Financial year
                    {
                        lock (@billNoLock)
                        {
                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = 1;
                            bn.bill_type = "BD";
                            bn.financial_code = financialCode;

                            dataB.bill_nos.Add(bn);
                            dataB.SaveChanges();
                        }
                    }
                    else
                    {
                        billNo = val.bill_no;
                    }
                }
            }
            catch
            {

            }
            return billNo;
        }

        public int ReadNextBankWithdrawalBillNo(string financialCode)
        {
            int billNo = 1;
            try
            {
                using (var dataB = new Database9006Entities())
                {
                    var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == "BW").FirstOrDefault();

                    if (val == null)//Create a row for new Financial year
                    {
                        lock (@billNoLock)
                        {
                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = 1;
                            bn.bill_type = "BW";
                            bn.financial_code = financialCode;

                            dataB.bill_nos.Add(bn);
                            dataB.SaveChanges();
                        }
                    }
                    else
                    {
                        billNo = val.bill_no;
                    }
                }
            }
            catch
            {

            }
            return billNo;
        }

        public int ReadNextCashPaymentBillNo(string financialCode)
        {
            int billNo = 1;
            try
            {
                using (var dataB = new Database9006Entities())
                {
                    var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == "CP").FirstOrDefault();

                    if (val == null)//Create a row for new Financial year
                    {
                        lock (@billNoLock)
                        {
                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = 1;
                            bn.bill_type = "CP";
                            bn.financial_code = financialCode;

                            dataB.bill_nos.Add(bn);
                            dataB.SaveChanges();
                        }
                    }
                    else
                    {
                        billNo = val.bill_no;
                    }
                }
            }
            catch
            {

            }
            return billNo;
        }

        public int ReadNextCashReceiptBillNo(string financialCode)
        {
            int billNo = 1;
            try
            {
                using (var dataB = new Database9006Entities())
                {
                    var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == "CR").FirstOrDefault();

                    if (val == null)//Create a row for new Financial year
                    {
                        lock (@billNoLock)
                        {
                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = 1;
                            bn.bill_type = "CR";
                            bn.financial_code = financialCode;

                            dataB.bill_nos.Add(bn);
                            dataB.SaveChanges();
                        }
                    }
                    else
                    {
                        billNo = val.bill_no;
                    }
                }
            }
            catch
            {

            }
            return billNo;
        }

        public int ReadNextJournalVoucherBillNo(string financialCode)
        {
            int billNo = 1;
            try
            {
                using (var dataB = new Database9006Entities())
                {
                    var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == "JV").FirstOrDefault();

                    if (val == null)//Create a row for new Financial year
                    {
                        lock (@billNoLock)
                        {
                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = 1;
                            bn.bill_type = "JV";
                            bn.financial_code = financialCode;

                            dataB.bill_nos.Add(bn);
                            dataB.SaveChanges();
                        }
                    }
                    else
                    {
                        billNo = val.bill_no;
                    }
                }
            }
            catch
            {

            }
            return billNo;
        }

        public int ReadNextLedgerRegisterBillNo()
        {
            int billNo = 1;
            try
            {
                using (var dataB = new Database9006Entities())
                {
                    var val = dataB.bill_nos.Where(e => e.bill_type == "LR").FirstOrDefault();

                    if (val == null)//Create a row for new Financial year
                    {
                        lock (@billNoLock)
                        {
                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = 1;
                            bn.bill_type = "LR";
                            bn.financial_code = "";

                            dataB.bill_nos.Add(bn);
                            dataB.SaveChanges();
                        }
                    }
                    else
                    {
                        billNo = val.bill_no;
                    }
                }
            }
            catch
            {

            }
            return billNo;
        }

        public int ReadNextLedgerTransactionBillNo(string financialCode)
        {
            int billNo = 1;
            try
            {
                using (var dataB = new Database9006Entities())
                {
                    var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == "LT").FirstOrDefault();

                    if (val == null)//Create a row for new Financial year
                    {
                        lock (@billNoLock)
                        {
                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = 1;
                            bn.bill_type = "LT";
                            bn.financial_code = financialCode;

                            dataB.bill_nos.Add(bn);
                            dataB.SaveChanges();
                        }
                    }
                    else
                    {
                        billNo = val.bill_no;
                    }
                }
            }
            catch
            {

            }
            return billNo;
        }

        public int ReadNextOpeningBalanceBillNo(string financialCode)
        {
            int billNo = 1;
            try
            {
                using (var dataB = new Database9006Entities())
                {
                    var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == "OB").FirstOrDefault();

                    if (val == null)//Create a row for new Financial year
                    {
                        lock (@billNoLock)
                        {
                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = 1;
                            bn.bill_type = "OB";
                            bn.financial_code = financialCode;

                            dataB.bill_nos.Add(bn);
                            dataB.SaveChanges();
                        }
                    }
                    else
                    {
                        billNo = val.bill_no;
                    }
                }
            }
            catch
            {

            }
            return billNo;
        }

        public int ReadNextPurchaseBillNo(string financialCode, string billType)
        {
            int billNo = 1;
            try
            {
                using (var dataB = new Database9006Entities())
                {
                    var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == billType).FirstOrDefault();

                    if (val == null)//Create a row for new Financial year
                    {
                        lock (@billNoLock)
                        {
                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = 1;
                            bn.bill_type = billType;
                            bn.financial_code = financialCode;

                            dataB.bill_nos.Add(bn);
                            dataB.SaveChanges();
                        }
                    }
                    else
                    {
                        billNo = val.bill_no;
                    }
                }
            }
            catch
            {

            }
            return billNo;
        }

        public int ReadNextPurchaseReturnBillNo(string financialCode, string billType)
        {
            int billNo = 1;
            try
            {
                using (var dataB = new Database9006Entities())
                {
                    var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == billType).FirstOrDefault();

                    if (val == null)//Create a row for new Financial year
                    {
                        lock (@billNoLock)
                        {
                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = 1;
                            bn.bill_type = billType;
                            bn.financial_code = financialCode;

                            dataB.bill_nos.Add(bn);
                            dataB.SaveChanges();
                        }
                    }
                    else
                    {
                        billNo = val.bill_no;
                    }
                }
            }
            catch
            {

            }
            return billNo;
        }

        public int ReadNextSalesBillNo(string financialCode, string billType)
        {
            int billNo = 1;
            try
            {
                using (var dataB = new Database9006Entities())
                {
                    var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == billType).FirstOrDefault();

                    if (val == null)//Create a row for new Financial year
                    {
                        lock (@billNoLock)
                        {
                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = 1;
                            bn.bill_type = billType;
                            bn.financial_code = financialCode;

                            dataB.bill_nos.Add(bn);
                            dataB.SaveChanges();
                        }
                    }
                    else
                    {
                        billNo = val.bill_no;
                    }
                }
            }
            catch
            {

            }
            return billNo;
        }

        public int ReadNextSalesReturnBillNo(string financialCode, string billType)
        {
            int billNo = 1;
            try
            {
                using (var dataB = new Database9006Entities())
                {
                    var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == billType).FirstOrDefault();

                    if (val == null)//Create a row for new Financial year
                    {
                        lock (@billNoLock)
                        {
                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = 1;
                            bn.bill_type = billType;
                            bn.financial_code = financialCode;

                            dataB.bill_nos.Add(bn);
                            dataB.SaveChanges();
                        }
                    }
                    else
                    {
                        billNo = val.bill_no;
                    }
                }
            }
            catch
            {

            }
            return billNo;
        }

        public bool UpdateProductRegisterBillNo(int billNo)
        {
            bool success = true;
            try
            {
                lock (@billNoLock)
                {
                    using (var dataB = new Database9006Entities())
                    {
                        var val = dataB.bill_nos.Where(e => e.bill_type == "PR").FirstOrDefault();

                        if (val == null)//Create a row for new Financial year
                        {

                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = billNo;
                            bn.bill_type = "PR";
                            bn.financial_code = "";
                        }
                        else//Or Edit
                        {
                            val.bill_no = billNo;
                        }

                        dataB.SaveChanges();
                    }
                }
            }
            catch
            {
                success = false;
            }
            return success;
        }

        public bool UpdateUnitRegisterBillNo(int billNo)
        {
            bool success = true;
            try
            {
                lock (@billNoLock)
                {
                    using (var dataB = new Database9006Entities())
                    {
                        var val = dataB.bill_nos.Where(e => e.bill_type == "UR").FirstOrDefault();

                        if (val == null)//Create a row for new Financial year
                        {

                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = billNo;
                            bn.bill_type = "UR";
                            bn.financial_code = "";
                        }
                        else//Or Edit
                        {
                            val.bill_no = billNo;
                        }

                        dataB.SaveChanges();
                    }
                }
            }
            catch
            {
                success = false;
            }
            return success;
        }

        public bool UpdateBankDepositBillNo(string financialCode, int billNo)
        {
            bool success = true;
            try
            {
                lock (@billNoLock)
                {
                    using (var dataB = new Database9006Entities())
                    {
                        var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == "BD").FirstOrDefault();

                        if (val == null)//Create a row for new Financial year
                        {

                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = billNo;
                            bn.bill_type = "BD";
                            bn.financial_code = financialCode;
                        }
                        else//Or Edit
                        {
                            val.bill_no = billNo;
                        }

                        dataB.SaveChanges();
                    }
                }
            }
            catch
            {
                success = false;
            }
            return success;
        }

        public bool UpdateBankWithdrawalBillNo(string financialCode, int billNo)
        {
            bool success = true;
            try
            {
                lock (@billNoLock)
                {
                    using (var dataB = new Database9006Entities())
                    {
                        var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == "BW").FirstOrDefault();

                        if (val == null)//Create a row for new Financial year
                        {

                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = billNo;
                            bn.bill_type = "BW";
                            bn.financial_code = financialCode;
                        }
                        else//Or Edit
                        {
                            val.bill_no = billNo;
                        }

                        dataB.SaveChanges();
                    }
                }
            }
            catch
            {
                success = false;
            }
            return success;
        }

        public bool UpdateCashPaymentBillNo(string financialCode, int billNo)
        {
            bool success = true;
            try
            {
                lock (@billNoLock)
                {
                    using (var dataB = new Database9006Entities())
                    {
                        var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == "CP").FirstOrDefault();

                        if (val == null)//Create a row for new Financial year
                        {

                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = billNo;
                            bn.bill_type = "CP";
                            bn.financial_code = financialCode;
                        }
                        else//Or Edit
                        {
                            val.bill_no = billNo;
                        }

                        dataB.SaveChanges();
                    }
                }
            }
            catch
            {
                success = false;
            }
            return success;
        }

        public bool UpdateCashReceiptBillNo(string financialCode, int billNo)
        {
            bool success = true;
            try
            {
                lock (@billNoLock)
                {
                    using (var dataB = new Database9006Entities())
                    {
                        var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == "CR").FirstOrDefault();

                        if (val == null)//Create a row for new Financial year
                        {

                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = billNo;
                            bn.bill_type = "CR";
                            bn.financial_code = financialCode;
                        }
                        else//Or Edit
                        {
                            val.bill_no = billNo;
                        }

                        dataB.SaveChanges();
                    }
                }
            }
            catch
            {
                success = false;
            }
            return success;
        }

        public bool UpdateJournalVoucherBillNo(string financialCode, int billNo)
        {
            bool success = true;
            try
            {
                lock (@billNoLock)
                {
                    using (var dataB = new Database9006Entities())
                    {
                        var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == "JV").FirstOrDefault();

                        if (val == null)//Create a row for new Financial year
                        {

                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = billNo;
                            bn.bill_type = "JV";
                            bn.financial_code = financialCode;
                        }
                        else//Or Edit
                        {
                            val.bill_no = billNo;
                        }

                        dataB.SaveChanges();
                    }
                }
            }
            catch
            {
                success = false;
            }
            return success;
        }

        public bool UpdateLedgerRegisterBillNo(int billNo)
        {
            bool success = true;
            try
            {
                lock (@billNoLock)
                {
                    using (var dataB = new Database9006Entities())
                    {
                        var val = dataB.bill_nos.Where(e => e.bill_type == "LR").FirstOrDefault();

                        if (val == null)//Create a row for new Financial year
                        {

                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = billNo;
                            bn.bill_type = "LR";
                            bn.financial_code = "";
                        }
                        else//Or Edit
                        {
                            val.bill_no = billNo;
                        }

                        dataB.SaveChanges();
                    }
                }
            }
            catch
            {
                success = false;
            }
            return success;
        }

        public bool UpdateLedgerTransactionBillNo(string financialCode, int billNo)
        {
            bool success = true;
            try
            {
                lock (@billNoLock)
                {
                    using (var dataB = new Database9006Entities())
                    {
                        var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == "LT").FirstOrDefault();

                        if (val == null)//Create a row for new Financial year
                        {

                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = billNo;
                            bn.bill_type = "LT";
                            bn.financial_code = financialCode;
                        }
                        else//Or Edit
                        {
                            val.bill_no = billNo;
                        }

                        dataB.SaveChanges();
                    }
                }
            }
            catch
            {
                success = false;
            }
            return success;
        }

        public bool UpdateOpeningBalanceBillNo(string financialCode, int billNo)
        {
            bool success = true;
            try
            {
                lock (@billNoLock)
                {
                    using (var dataB = new Database9006Entities())
                    {
                        var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == "OB").FirstOrDefault();

                        if (val == null)//Create a row for new Financial year
                        {

                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = billNo;
                            bn.bill_type = "OB";
                            bn.financial_code = financialCode;
                        }
                        else//Or Edit
                        {
                            val.bill_no = billNo;
                        }

                        dataB.SaveChanges();
                    }
                }
            }
            catch
            {
                success = false;
            }
            return success;
        }

        public bool UpdatePurchaseBillNo(string financialCode, int billNo, string billType)
        {
            bool success = true;
            try
            {
                lock (@billNoLock)
                {
                    using (var dataB = new Database9006Entities())
                    {
                        var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == billType).FirstOrDefault();

                        if (val == null)//Create a row for new Financial year
                        {

                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = billNo;
                            bn.bill_type = billType;
                            bn.financial_code = financialCode;
                        }
                        else//Or Edit
                        {
                            val.bill_no = billNo;
                        }

                        dataB.SaveChanges();
                    }
                }
            }
            catch
            {
                success = false;
            }
            return success;
        }

        public bool UpdatePurchaseReturnBillNo(string financialCode, int billNo, string billType)
        {
            bool success = true;
            try
            {
                lock (@billNoLock)
                {
                    using (var dataB = new Database9006Entities())
                    {
                        var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == billType).FirstOrDefault();

                        if (val == null)//Create a row for new Financial year
                        {

                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = billNo;
                            bn.bill_type = billType;
                            bn.financial_code = financialCode;
                        }
                        else//Or Edit
                        {
                            val.bill_no = billNo;
                        }

                        dataB.SaveChanges();
                    }
                }
            }
            catch
            {
                success = false;
            }
            return success;
        }

        public bool UpdateSalesBillNo(string financialCode, int billNo, string billType)
        {
            bool success = true;
            try
            {
                lock (@billNoLock)
                {
                    using (var dataB = new Database9006Entities())
                    {
                        var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == billType).FirstOrDefault();

                        if (val == null)//Create a row for new Financial year
                        {

                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = billNo;
                            bn.bill_type = billType;
                            bn.financial_code = financialCode;
                        }
                        else//Or Edit
                        {
                            val.bill_no = billNo;
                        }

                        dataB.SaveChanges();
                    }
                }
            }
            catch
            {
                success = false;
            }
            return success;
        }

        public bool UpdateSalesReturnBillNo(string financialCode, int billNo, string billType)
        {
            bool success = true;
            try
            {
                lock (@billNoLock)
                {
                    using (var dataB = new Database9006Entities())
                    {
                        var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == billType).FirstOrDefault();

                        if (val == null)//Create a row for new Financial year
                        {

                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = billNo;
                            bn.bill_type = billType;
                            bn.financial_code = financialCode;
                        }
                        else//Or Edit
                        {
                            val.bill_no = billNo;
                        }

                        dataB.SaveChanges();
                    }
                }
            }
            catch
            {
                success = false;
            }
            return success;
        }

        public int ReadNextStockAdditionBillNo(string financialCode)
        {
            int billNo = 1;
            try
            {
                using (var dataB = new Database9006Entities())
                {
                    var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == "SA").FirstOrDefault();

                    if (val == null)//Create a row for new Financial year
                    {
                        lock (@billNoLock)
                        {
                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = 1;
                            bn.bill_type = "SA";
                            bn.financial_code = financialCode;

                            dataB.bill_nos.Add(bn);
                            dataB.SaveChanges();
                        }
                    }
                    else
                    {
                        billNo = val.bill_no;
                    }
                }
            }
            catch
            {

            }
            return billNo;
        }

        public int ReadNextStockDeletionBillNo(string financialCode)
        {
            int billNo = 1;
            try
            {
                using (var dataB = new Database9006Entities())
                {
                    var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == "SD").FirstOrDefault();

                    if (val == null)//Create a row for new Financial year
                    {
                        lock (@billNoLock)
                        {
                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = 1;
                            bn.bill_type = "SD";
                            bn.financial_code = financialCode;

                            dataB.bill_nos.Add(bn);
                            dataB.SaveChanges();
                        }
                    }
                    else
                    {
                        billNo = val.bill_no;
                    }
                }
            }
            catch
            {

            }
            return billNo;
        }

        public bool UpdateStockAdditionBillNo(string financialCode, int billNo)
        {
            bool success = true;
            try
            {
                lock (@billNoLock)
                {
                    using (var dataB = new Database9006Entities())
                    {
                        var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == "Sa").FirstOrDefault();

                        if (val == null)//Create a row for new Financial year
                        {

                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = billNo;
                            bn.bill_type = "SA";
                            bn.financial_code = financialCode;
                        }
                        else//Or Edit
                        {
                            val.bill_no = billNo;
                        }

                        dataB.SaveChanges();
                    }
                }
            }
            catch
            {
                success = false;
            }
            return success;
        }

        public bool UpdateStockDeletionBillNo(string financialCode, int billNo)
        {
            bool success = true;
            try
            {
                lock (@billNoLock)
                {
                    using (var dataB = new Database9006Entities())
                    {
                        var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == "SD").FirstOrDefault();

                        if (val == null)//Create a row for new Financial year
                        {

                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = billNo;
                            bn.bill_type = "SD";
                            bn.financial_code = financialCode;
                        }
                        else//Or Edit
                        {
                            val.bill_no = billNo;
                        }

                        dataB.SaveChanges();
                    }
                }
            }
            catch
            {
                success = false;
            }
            return success;
        }

        public int ReadNextPurchaseBillNo(string financialCode)
        {
            int billNo = 1;
            try
            {
                using (var dataB = new Database9006Entities())
                {
                    var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == "P").FirstOrDefault();

                    if (val == null)//Create a row for new Financial year
                    {
                        lock (@billNoLock)
                        {
                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = 1;
                            bn.bill_type = "P";
                            bn.financial_code = financialCode;

                            dataB.bill_nos.Add(bn);
                            dataB.SaveChanges();
                        }
                    }
                    else
                    {
                        billNo = val.bill_no;
                    }
                }
            }
            catch
            {

            }
            return billNo;
        }

        public int ReadNextPurchaseReturnBillNo(string financialCode)
        {
            int billNo = 1;
            try
            {
                using (var dataB = new Database9006Entities())
                {
                    var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == "PR").FirstOrDefault();

                    if (val == null)//Create a row for new Financial year
                    {
                        lock (@billNoLock)
                        {
                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = 1;
                            bn.bill_type = "PR";
                            bn.financial_code = financialCode;

                            dataB.bill_nos.Add(bn);
                            dataB.SaveChanges();
                        }
                    }
                    else
                    {
                        billNo = val.bill_no;
                    }
                }
            }
            catch
            {

            }
            return billNo;
        }

        public int ReadNextSalesBillNo(string financialCode)
        {
            int billNo = 1;
            try
            {
                using (var dataB = new Database9006Entities())
                {
                    var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == "S").FirstOrDefault();

                    if (val == null)//Create a row for new Financial year
                    {
                        lock (@billNoLock)
                        {
                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = 1;
                            bn.bill_type = "S";
                            bn.financial_code = financialCode;

                            dataB.bill_nos.Add(bn);
                            dataB.SaveChanges();
                        }
                    }
                    else
                    {
                        billNo = val.bill_no;
                    }
                }
            }
            catch
            {

            }
            return billNo;
        }

        public int ReadNextSalesReturnBillNo(string financialCode)
        {
            int billNo = 1;
            try
            {
                using (var dataB = new Database9006Entities())
                {
                    var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == "SR").FirstOrDefault();

                    if (val == null)//Create a row for new Financial year
                    {
                        lock (@billNoLock)
                        {
                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = 1;
                            bn.bill_type = "SR";
                            bn.financial_code = financialCode;

                            dataB.bill_nos.Add(bn);
                            dataB.SaveChanges();
                        }
                    }
                    else
                    {
                        billNo = val.bill_no;
                    }
                }
            }
            catch
            {

            }
            return billNo;
        }

        public bool UpdatePurchaseBillNo(string financialCode, int billNo)
        {
            bool success = true;
            try
            {
                lock (@billNoLock)
                {
                    using (var dataB = new Database9006Entities())
                    {
                        var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == "P").FirstOrDefault();

                        if (val == null)//Create a row for new Financial year
                        {

                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = billNo;
                            bn.bill_type = "P";
                            bn.financial_code = financialCode;
                        }
                        else//Or Edit
                        {
                            val.bill_no = billNo;
                        }

                        dataB.SaveChanges();
                    }
                }
            }
            catch
            {
                success = false;
            }
            return success;
        }

        public bool UpdatePurchaseReturnBillNo(string financialCode, int billNo)
        {
            bool success = true;
            try
            {
                lock (@billNoLock)
                {
                    using (var dataB = new Database9006Entities())
                    {
                        var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == "PR").FirstOrDefault();

                        if (val == null)//Create a row for new Financial year
                        {

                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = billNo;
                            bn.bill_type = "PR";
                            bn.financial_code = financialCode;
                        }
                        else//Or Edit
                        {
                            val.bill_no = billNo;
                        }

                        dataB.SaveChanges();
                    }
                }
            }
            catch
            {
                success = false;
            }
            return success;
        }

        public bool UpdateSalesBillNo(string financialCode, int billNo)
        {
            bool success = true;
            try
            {
                lock (@billNoLock)
                {
                    using (var dataB = new Database9006Entities())
                    {
                        var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == "S").FirstOrDefault();

                        if (val == null)//Create a row for new Financial year
                        {

                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = billNo;
                            bn.bill_type = "S";
                            bn.financial_code = financialCode;
                        }
                        else//Or Edit
                        {
                            val.bill_no = billNo;
                        }

                        dataB.SaveChanges();
                    }
                }
            }
            catch
            {
                success = false;
            }
            return success;
        }

        public bool UpdateSalesReturnBillNo(string financialCode, int billNo)
        {
            bool success = true;
            try
            {
                lock (@billNoLock)
                {
                    using (var dataB = new Database9006Entities())
                    {
                        var val = dataB.bill_nos.Where(e => e.financial_code == financialCode && e.bill_type == "SR").FirstOrDefault();

                        if (val == null)//Create a row for new Financial year
                        {

                            bill_nos bn = dataB.bill_nos.Create();
                            bn.bill_no = billNo;
                            bn.bill_type = "SR";
                            bn.financial_code = financialCode;
                        }
                        else//Or Edit
                        {
                            val.bill_no = billNo;
                        }

                        dataB.SaveChanges();
                    }
                }
            }
            catch
            {
                success = false;
            }
            return success;
        }
    }
}
