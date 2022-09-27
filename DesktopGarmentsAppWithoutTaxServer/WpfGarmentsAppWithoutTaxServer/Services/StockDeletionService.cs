using ServerServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Windows;
using WpfServerApp.General;

namespace WpfServerApp.Services
{   
    public class StockDeletionService : IStockDeletion
    {
        private string mBillType = "SD";

        public bool CreateBill(CStockDeletion oStockDeletion)
        {
            bool returnValue = false;

            lock (Synchronizer.@lock)
            {
                
                using (var dataB = new Database9006Entities())
                {
                    var dataBTransaction = dataB.Database.BeginTransaction();
                    try
                    {

                        ProductService ls = new ProductService();
                        BillNoService bs = new BillNoService();


                        int cbillNo = bs.ReadNextStockDeletionBillNo(oStockDeletion.FinancialCode);
                        bs.UpdateStockDeletionBillNo(oStockDeletion.FinancialCode,cbillNo+1);
                        
                        for (int i = 0; i < oStockDeletion.Details.Count; i++)
                        {
                            product_transactions pt = new product_transactions();

                            pt.bill_no= cbillNo.ToString();
                            pt.bill_type = mBillType;
                            pt.bill_date_time = oStockDeletion.BillDateTime;
                            pt.narration = oStockDeletion.Narration;
                            pt.financial_code = oStockDeletion.FinancialCode;

                            pt.serial_no = oStockDeletion.Details.ElementAt(i).SerialNo;
                            pt.product_code = oStockDeletion.Details.ElementAt(i).ProductCode;
                            pt.product = oStockDeletion.Details.ElementAt(i).Product;
                            pt.sales_unit = oStockDeletion.Details.ElementAt(i).StockDeletionUnit;
                            pt.sales_unit_code = oStockDeletion.Details.ElementAt(i).StockDeletionUnitCode;
                            pt.sales_unit_value = oStockDeletion.Details.ElementAt(i).StockDeletionUnitValue;
                            pt.quantity = oStockDeletion.Details.ElementAt(i).Quantity*-1;                            
                            pt.mrp = oStockDeletion.Details.ElementAt(i).MRP;
                            pt.interstate_rate = oStockDeletion.Details.ElementAt(i).InterstateRate;
                            pt.wholesale_rate = oStockDeletion.Details.ElementAt(i).WholesaleRate;
                            pt.brand_code = oStockDeletion.Details.ElementAt(i).BrandCode;
                            pt.size_code = oStockDeletion.Details.ElementAt(i).SizeCode;
                            pt.colour_code = oStockDeletion.Details.ElementAt(i).ColourCode;
                            pt.brand = oStockDeletion.Details.ElementAt(i).Brand;
                            pt.size = oStockDeletion.Details.ElementAt(i).Size;
                            pt.colour = oStockDeletion.Details.ElementAt(i).Colour;
                            //get a barcode here
                            pt.barcode = oStockDeletion.Details.ElementAt(i).Barcode;
                            pt.unit_code= oStockDeletion.Details.ElementAt(i).StockDeletionUnitCode;
                            pt.unit_value= oStockDeletion.Details.ElementAt(i).StockDeletionUnitValue;

                            dataB.product_transactions.Add(pt);                            
                        }

                        dataB.SaveChanges();
                        //Success
                        returnValue = true;

                        dataBTransaction.Commit();
                    }
                    catch(Exception e)
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

        public bool DeleteBill(string billNo,string financialCode)
        {
            bool returnValue = true;

            lock (Synchronizer.@lock)
            {
                using (var dataB = new Database9006Entities())
                {
                    var dataBTransaction = dataB.Database.BeginTransaction();
                    try
                    {
                        //Delete the transaction
                        var cpp = dataB.product_transactions.Select(c => c).Where(x => x.bill_no == billNo && x.financial_code == financialCode && x.bill_type == mBillType);
                        dataB.product_transactions.RemoveRange(cpp);
                        
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

        public CStockDeletion ReadBill(string billNo,string financialCode)
        {
            CStockDeletion ccp = null;

            using (var dataB = new Database9006Entities())
            {
                var cps = dataB.product_transactions.Select(c => c).Where(x => x.bill_no == billNo && x.financial_code == financialCode&&x.bill_type==mBillType).OrderBy(y=>y.serial_no);
                
                if (cps.Count() > 0)
                {
                    ccp = new CStockDeletion();

                    var cp = cps.FirstOrDefault();
                    ccp.Id = cp.id;
                    ccp.BillNo = cp.bill_no;
                    ccp.BillDateTime = cp.bill_date_time;
                    ccp.Narration = cp.narration;
                    ccp.FinancialCode = cp.financial_code;

                    foreach (var item in cps)
                    {
                        ccp.Details.Add(new CStockDeletionDetails() { SerialNo=(int)item.serial_no,ProductCode=item.product_code,Product=item.product, StockDeletionUnit=item.sales_unit, StockDeletionUnitCode=item.sales_unit_code, StockDeletionUnitValue = (decimal)item.sales_unit_value, Quantity=(decimal)item.quantity*-1, MRP = (decimal)item.mrp, Barcode = item.barcode, InterstateRate=(decimal)item.interstate_rate, WholesaleRate=(decimal)item.wholesale_rate, BrandCode = item.brand_code, ColourCode = item.colour_code, SizeCode = item.size_code, Brand = item.brand, Size = item.size, Colour = item.colour });
                    }
                }
                
            }

            return ccp;
        }

        public int ReadNextBillNo(string financialCode)
        {
            
            BillNoService bns = new BillNoService();
            return bns.ReadNextStockDeletionBillNo(financialCode);
            
        }
        
        public bool UpdateBill(CStockDeletion oStockDeletion)
        {
            bool returnValue = false;

            lock (Synchronizer.@lock)
            {
                using (var dataB = new Database9006Entities())
                {
                    var dataBTransaction = dataB.Database.BeginTransaction();
                    try
                    {
                        var cpp = dataB.product_transactions.Select(c => c).Where(x => x.bill_no == oStockDeletion.BillNo&& x.financial_code==oStockDeletion.FinancialCode&&x.bill_type==mBillType);
                        dataB.product_transactions.RemoveRange(cpp);

                        for (int i = 0; i < oStockDeletion.Details.Count; i++)
                        {

                            product_transactions pt = new product_transactions();

                            pt.bill_no = oStockDeletion.BillNo;
                            pt.bill_type = mBillType;
                            pt.bill_date_time = oStockDeletion.BillDateTime;
                            pt.narration = oStockDeletion.Narration;
                            pt.financial_code = oStockDeletion.FinancialCode;

                            pt.serial_no = oStockDeletion.Details.ElementAt(i).SerialNo;
                            pt.product_code = oStockDeletion.Details.ElementAt(i).ProductCode;
                            pt.product = oStockDeletion.Details.ElementAt(i).Product;
                            pt.sales_unit = oStockDeletion.Details.ElementAt(i).StockDeletionUnit;
                            pt.sales_unit_code = oStockDeletion.Details.ElementAt(i).StockDeletionUnitCode;
                            pt.sales_unit_value = oStockDeletion.Details.ElementAt(i).StockDeletionUnitValue;
                            pt.quantity = oStockDeletion.Details.ElementAt(i).Quantity*-1;                            
                            pt.mrp = oStockDeletion.Details.ElementAt(i).MRP;
                            pt.wholesale_rate = oStockDeletion.Details.ElementAt(i).WholesaleRate;
                            pt.interstate_rate = oStockDeletion.Details.ElementAt(i).InterstateRate;
                            pt.brand_code = oStockDeletion.Details.ElementAt(i).BrandCode;
                            pt.size_code = oStockDeletion.Details.ElementAt(i).SizeCode;
                            pt.colour_code = oStockDeletion.Details.ElementAt(i).ColourCode;
                            pt.brand = oStockDeletion.Details.ElementAt(i).Brand;
                            pt.size = oStockDeletion.Details.ElementAt(i).Size;
                            pt.colour = oStockDeletion.Details.ElementAt(i).Colour;
                            //get a barcode here
                            pt.barcode = oStockDeletion.Details.ElementAt(i).Barcode;
                            pt.unit_code = oStockDeletion.Details.ElementAt(i).StockDeletionUnitCode;
                            pt.unit_value = oStockDeletion.Details.ElementAt(i).StockDeletionUnitValue;

                            dataB.product_transactions.Add(pt);

                        }

                        
                        dataB.SaveChanges();

                        //Success
                        returnValue = true;

                        dataBTransaction.Commit();
                    }
                    catch(Exception e)
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

        public List<CStock> ReadStockOfProduct(string productCode,string unitCode,string unit, decimal unitValue,string billNo,string fCode)
        {
            List<CStock> stocks = new List<CStock>();
            string billType = mBillType;
            try
            {

                UnitService us = new UnitService();
                decimal lowestUnitVal = us.ReadLowestUnitValue(unitCode);

                using (var dataB = new Database9006Entities())
                {
                    using (var dataBSub = new Database9006Entities())
                    {

                        var cps = dataB.product_transactions.Where(c => c.product_code == productCode && !(c.bill_no == billNo && c.bill_type == billType && c.financial_code == fCode)).GroupBy(x => new { x.barcode }, x => new { x.unit_value, x.quantity, x.barcode, x.mrp })
                        .Select(y => new { Quantity = y.Sum(x => x.quantity * (unitValue / x.unit_value)), Barcode = y.FirstOrDefault().barcode });
                        foreach (var item in cps)
                        {
                            var rateNUnitValue = dataBSub.product_transactions.Where(c => c.barcode == item.Barcode && (c.bill_type == "SA" || c.bill_type == "PI" || c.bill_type == "PW")).Select(c => new { c.interstate_rate, c.mrp, c.wholesale_rate, c.unit_value, c.brand, c.size, c.colour, c.brand_code, c.colour_code, c.size_code }).FirstOrDefault();
                            CStock cs = new CStock() { Barcode = item.Barcode, Quantity = (decimal)item.Quantity, Unit = unit, MRP = (decimal)rateNUnitValue.mrp / unitValue / rateNUnitValue.unit_value, InterstateRate = (decimal)rateNUnitValue.interstate_rate / unitValue / rateNUnitValue.unit_value, WholesaleRate = (decimal)rateNUnitValue.wholesale_rate / unitValue / rateNUnitValue.unit_value, Brand = rateNUnitValue.brand, Size = rateNUnitValue.size, Colour = rateNUnitValue.colour, BrandCode = rateNUnitValue.brand_code, SizeCode = rateNUnitValue.size_code, ColourCode = rateNUnitValue.colour_code };
                            stocks.Add(cs);
                        }
                    }

                }

            }
            catch (Exception e)
            {

            }

            return stocks;
        }

        //Reports
        public List<CStockDeletionReportDetailed> FindStockDeletionDetailed(DateTime startDate, DateTime endDate, string billType, string billNo, string productCode, string product, string brandCode, string brand, string colourCode, string colour, string sizeCode, string size, string narration, string financialCode)
        {
            List<CStockDeletionReportDetailed> report = new List<CStockDeletionReportDetailed>();

            using (var dataB = new Database9006Entities())
            {
                string startD = startDate.Year + "-" + startDate.Month + "-" + startDate.Day;
                string endD = endDate.Year + "-" + endDate.Month + "-" + endDate.Day;

                string billTypeQuery = billType.Trim().Equals("") ? "" : " && (bd.bill_type='" + billType.Trim() + "') ";
                string billNoQuery = billNo.Trim().Equals("") ? "" : " && (bd.bill_no='" + billNo.Trim() + "') ";
                string productCodeQuery = productCode.Trim().Equals("") ? "" : " && (bd.product_code='" + productCode.Trim() + "') ";
                string productQuery = product.Trim().Equals("") ? "" : " && (bd.product Like '%" + product.Trim() + "%') ";
                string brandCodeQuery = brandCode.Trim().Equals("") ? "" : " && (bd.brand_code='" + brandCode.Trim() + "') ";
                string brandQuery = brand.Trim().Equals("") ? "" : " && (bd.brand Like '%" + brand.Trim() + "%') ";
                string colourCodeQuery = colourCode.Trim().Equals("") ? "" : " && (bd.colour_code='" + colourCode.Trim() + "') ";
                string colourQuery = colour.Trim().Equals("") ? "" : " && (bd.colour Like '%" + colour.Trim() + "%') ";
                string sizeCodeQuery = sizeCode.Trim().Equals("") ? "" : " && (bd.size_code='" + sizeCode.Trim() + "') ";
                string sizeQuery = size.Trim().Equals("") ? "" : " && (bd.size Like '%" + size.Trim() + "%') ";
                string narrationQuery = narration.Trim().Equals("") ? "" : " && (bd.narration Like '%" + narration.Trim() + "%') ";
                string financialCodeQuery = financialCode.Trim().Equals("") ? "" : " && (bd.financial_code='" + financialCode.Trim() + "') ";

                string subQ = billNoQuery + productCodeQuery + productQuery + narrationQuery + financialCodeQuery + billTypeQuery + colourQuery + brandQuery + sizeQuery + colourCodeQuery + brandCodeQuery + sizeCodeQuery;

                var resData = dataB.Database.SqlQuery<CStockDeletionReportDetailed>("Select  ((bd.quantity * bd.sales_rate)-bd.product_discount)*-1 As Total, (bd.quantity * bd.sales_rate)*-1 As GrossValue, bd.quantity*-1 As Quantity, bd.colour As Colour, bd.size As Size, bd.brand As Brand, bd.mrp As MRP, bd.wholesale_rate As WholesaleRate, bd.interstate_rate As InterstateRate, bd.sales_rate As SalesRate, bd.product_discount As ProductDiscount, bd.sales_unit As SalesUnit, bd.product As Product, bd.serial_no As SerialNo, bd.extra_charges As Expense, bd.discounts As Discount, bd.advance As Advance, bd.bill_date_time As BillDateTime,bd.bill_no As BillNo, bd.narration As Narration, bd.financial_code As FinancialCode From product_transactions bd Where(bd.bill_date_time >= '" + startD + "' && bd.bill_date_time <='" + endD + "') " + subQ + " Order By bd.bill_date_time,bd.bill_no, bd.serial_no");

                decimal? quantity = 0;
                decimal? grossValue = 0;
                decimal? proDiscount = 0;
                decimal? total = 0;
                foreach (var item in resData)
                {
                    quantity = quantity + item.Quantity;
                    grossValue = grossValue + item.GrossValue;
                    proDiscount = proDiscount + item.ProductDiscount;
                    total = total + item.Total;

                    report.Add(item);
                }
                //Total
                report.Add(new CStockDeletionReportDetailed() { BillDateTime = null, SerialNo = null, Product = "" });
                report.Add(new CStockDeletionReportDetailed() { BillDateTime = null, SerialNo = null, Product = "Total", Quantity = quantity, GrossValue = grossValue, ProductDiscount = proDiscount, Total = total });

            }


            return report;
        }

        public List<CStockDeletionReportSummary> FindStockDeletionSummary(DateTime startDate, DateTime endDate, string billType, string billNo, string productCode, string product, string brandCode, string brand, string colourCode, string colour, string sizeCode, string size, string narration, string financialCode)
        {
            List<CStockDeletionReportSummary> report = new List<CStockDeletionReportSummary>();

            using (var dataB = new Database9006Entities())
            {
                string startD = startDate.Year + "-" + startDate.Month + "-" + startDate.Day;
                string endD = endDate.Year + "-" + endDate.Month + "-" + endDate.Day;

                string billTypeQuery = billType.Trim().Equals("") ? "" : " && (bd.bill_type='" + billType.Trim() + "') ";
                string billNoQuery = billNo.Trim().Equals("") ? "" : " && (bd.bill_no='" + billNo.Trim() + "') ";
                string productCodeQuery = productCode.Trim().Equals("") ? "" : " && (bd.product_code='" + productCode.Trim() + "') ";
                string productQuery = product.Trim().Equals("") ? "" : " && (bd.product Like '%" + product.Trim() + "%') ";
                string brandCodeQuery = brandCode.Trim().Equals("") ? "" : " && (bd.brand_code='" + brandCode.Trim() + "') ";
                string brandQuery = brand.Trim().Equals("") ? "" : " && (bd.brand Like '%" + brand.Trim() + "%') ";
                string colourCodeQuery = colourCode.Trim().Equals("") ? "" : " && (bd.colour_code='" + colourCode.Trim() + "') ";
                string colourQuery = colour.Trim().Equals("") ? "" : " && (bd.colour Like '%" + colour.Trim() + "%') ";
                string sizeCodeQuery = sizeCode.Trim().Equals("") ? "" : " && (bd.size_code='" + sizeCode.Trim() + "') ";
                string sizeQuery = size.Trim().Equals("") ? "" : " && (bd.size Like '%" + size.Trim() + "%') ";
                string narrationQuery = narration.Trim().Equals("") ? "" : " && (bd.narration Like '%" + narration.Trim() + "%') ";
                string financialCodeQuery = financialCode.Trim().Equals("") ? "" : " && (bd.financial_code='" + financialCode.Trim() + "') ";

                string subQ = billNoQuery + productCodeQuery + productQuery + narrationQuery + financialCodeQuery + billTypeQuery + colourQuery + brandQuery + sizeQuery + colourCodeQuery + brandCodeQuery + sizeCodeQuery;

                var resData = dataB.Database.SqlQuery<CStockDeletionReportSummary>("Select Sum(((bd.quantity * bd.sales_rate) - bd.product_discount))*-1 As BillAmount, bd.extra_charges As Expense, bd.discounts As Discount, bd.advance As Advance, bd.bill_date_time As BillDateTime,bd.bill_no As BillNo, bd.narration As Narration, bd.financial_code As FinancialCode From product_transactions bd Where(bd.bill_date_time >= '" + startD + "' && bd.bill_date_time <='" + endD + "') " + subQ + "Group By bd.extra_charges, bd.discounts, bd.advance, bd.bill_date_time, bd.bill_no, bd.narration, bd.financial_code Order By bd.bill_date_time,bd.bill_no");

                decimal? billAmount = 0;
                foreach (var item in resData)
                {

                    billAmount = billAmount + item.BillAmount;
                    report.Add(item);
                }
                //Total
                report.Add(new CStockDeletionReportSummary() { BillDateTime = null, BillNo = "" });
                report.Add(new CStockDeletionReportSummary() { BillDateTime = null, BillNo = "Total", BillAmount = billAmount });

            }


            return report;
        }
    }
}
