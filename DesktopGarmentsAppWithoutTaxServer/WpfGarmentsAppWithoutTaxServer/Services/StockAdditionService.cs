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
    public class StockAdditionService : IStockAddition
    {
        private string mBillType = "SA";

        public bool CreateBill(CStockAddition oStockAddition)
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
                        ProductPropertyService pps = new ProductPropertyService();


                        int cbillNo = bs.ReadNextStockAdditionBillNo(oStockAddition.FinancialCode);
                        bs.UpdateStockAdditionBillNo(oStockAddition.FinancialCode,cbillNo+1);

                        List<string> barcodes = new BarcodeService().ReadBarcodes(oStockAddition.Details.Count);
                        
                        for (int i = 0; i < oStockAddition.Details.Count; i++)
                        {
                            product_transactions pt = new product_transactions();

                            pt.bill_no= cbillNo.ToString();
                            pt.bill_type = mBillType;
                            pt.bill_date_time = oStockAddition.BillDateTime;
                            pt.narration = oStockAddition.Narration;
                            pt.financial_code = oStockAddition.FinancialCode;

                            pt.serial_no = oStockAddition.Details.ElementAt(i).SerialNo;
                            pt.product_code = oStockAddition.Details.ElementAt(i).ProductCode;
                            pt.product = oStockAddition.Details.ElementAt(i).Product;
                            pt.purchase_unit = oStockAddition.Details.ElementAt(i).StockAdditionUnit;
                            pt.purchase_unit_code = oStockAddition.Details.ElementAt(i).StockAdditionUnitCode;
                            pt.purchase_unit_value = oStockAddition.Details.ElementAt(i).StockAdditionUnitValue;
                            pt.quantity = oStockAddition.Details.ElementAt(i).Quantity;
                            pt.mrp = oStockAddition.Details.ElementAt(i).MRP;
                            pt.interstate_rate = oStockAddition.Details.ElementAt(i).InterstateRate;
                            pt.wholesale_rate = oStockAddition.Details.ElementAt(i).WholesaleRate;
                            pt.brand_code = oStockAddition.Details.ElementAt(i).BrandCode == "" ? pps.CreateProperty(ls.ReadGroupCodeOf(oStockAddition.Details.ElementAt(i).ProductCode), oStockAddition.Details.ElementAt(i).Brand, "1").PropertyCode : oStockAddition.Details.ElementAt(i).BrandCode;
                            pt.size_code = oStockAddition.Details.ElementAt(i).SizeCode == "" ? pps.CreateProperty(ls.ReadGroupCodeOf(oStockAddition.Details.ElementAt(i).ProductCode), oStockAddition.Details.ElementAt(i).Size, "2").PropertyCode : oStockAddition.Details.ElementAt(i).SizeCode;
                            pt.colour_code = oStockAddition.Details.ElementAt(i).ColourCode == "" ? pps.CreateProperty(ls.ReadGroupCodeOf(oStockAddition.Details.ElementAt(i).ProductCode), oStockAddition.Details.ElementAt(i).Colour, "3").PropertyCode : oStockAddition.Details.ElementAt(i).ColourCode;
                            pt.brand = oStockAddition.Details.ElementAt(i).Brand;
                            pt.size = oStockAddition.Details.ElementAt(i).Size;
                            pt.colour = oStockAddition.Details.ElementAt(i).Colour;
                            //get a barcode here
                            pt.barcode = barcodes.ElementAt(i);                            
                            pt.unit_code= oStockAddition.Details.ElementAt(i).StockAdditionUnitCode;
                            pt.unit_value= oStockAddition.Details.ElementAt(i).StockAdditionUnitValue;

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
                        //get barcodes already used in other transactions
                        BarcodeService bs = new BarcodeService();
                        List<string> usedBarcodes = ReadAlreadyUsedBarcodes(billNo,financialCode);
                        //Remove editable entries of transaction
                        var cpp = dataB.product_transactions.Select(c => c).Where(x => x.bill_no == billNo && x.financial_code == financialCode && x.bill_type == mBillType && !usedBarcodes.Contains<string>(x.barcode));
                        dataB.product_transactions.RemoveRange(cpp);

                        //Updating serial numbers of entires that are already used
                        int serialNo = 1;
                        var tr = dataB.product_transactions.Select(c => c).Where(x => x.bill_no == billNo && x.financial_code == financialCode && x.bill_type == mBillType && usedBarcodes.Contains<string>(x.barcode));
                        foreach (var item in tr)
                        {
                            item.serial_no = serialNo++;
                        }

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

        public CStockAddition ReadBill(string billNo,string financialCode)
        {
            CStockAddition ccp = null;

            using (var dataB = new Database9006Entities())
            {
                var cps = dataB.product_transactions.Select(c => c).Where(x => x.bill_no == billNo && x.financial_code == financialCode&&x.bill_type==mBillType).OrderBy(y=>y.serial_no);
                
                if (cps.Count() > 0)
                {
                    ccp = new CStockAddition();

                    var cp = cps.FirstOrDefault();
                    ccp.Id = cp.id;
                    ccp.BillNo = cp.bill_no;
                    ccp.BillDateTime = cp.bill_date_time;
                    ccp.Narration = cp.narration;
                    ccp.FinancialCode = cp.financial_code;

                    foreach (var item in cps)
                    {
                        ccp.Details.Add(new CStockAdditionDetails() { SerialNo=(int)item.serial_no,ProductCode=item.product_code,Product=item.product, StockAdditionUnit=item.purchase_unit, StockAdditionUnitCode=item.purchase_unit_code, StockAdditionUnitValue = (decimal)item.purchase_unit_value, Quantity=(decimal)item.quantity, MRP = (decimal)item.mrp, Barcode = item.barcode, InterstateRate=(decimal)item.interstate_rate, WholesaleRate=(decimal)item.wholesale_rate, BrandCode = item.brand_code, ColourCode = item.colour_code, SizeCode = item.size_code, Brand = item.brand, Size = item.size, Colour = item.colour });
                    }
                }
                
            }

            return ccp;
        }

        public int ReadNextBillNo(string financialCode)
        {
            
            BillNoService bns = new BillNoService();
            return bns.ReadNextStockAdditionBillNo(financialCode);
            
        }

        public bool UpdateBill(CStockAddition oStockAddition)
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
                        ProductPropertyService pps = new ProductPropertyService();
                        //get barcodes already used in other transactions
                        BarcodeService bs = new BarcodeService();                        
                        List<string> usedBarcodes =ReadAlreadyUsedBarcodes(oStockAddition.BillNo,oStockAddition.FinancialCode);
                        //Remove editable entries of transaction
                        var cpp = dataB.product_transactions.Select(c => c).Where(x => x.bill_no == oStockAddition.BillNo&& x.financial_code==oStockAddition.FinancialCode&&x.bill_type==mBillType && !usedBarcodes.Contains<string>(x.barcode));
                        dataB.product_transactions.RemoveRange(cpp);
                        
                        int serialNo = 1;
                        for (int i = 0; i < oStockAddition.Details.Count; i++)
                        {
                            if (!usedBarcodes.Contains<string>(oStockAddition.Details.ElementAt(i).Barcode))
                            {
                                product_transactions pt = new product_transactions();

                                pt.bill_no = oStockAddition.BillNo;
                                pt.bill_type = mBillType;
                                pt.bill_date_time = oStockAddition.BillDateTime;
                                pt.narration = oStockAddition.Narration;
                                pt.financial_code = oStockAddition.FinancialCode;

                                pt.serial_no = serialNo++;
                                pt.product_code = oStockAddition.Details.ElementAt(i).ProductCode;
                                pt.product = oStockAddition.Details.ElementAt(i).Product;
                                pt.purchase_unit = oStockAddition.Details.ElementAt(i).StockAdditionUnit;
                                pt.purchase_unit_code = oStockAddition.Details.ElementAt(i).StockAdditionUnitCode;
                                pt.purchase_unit_value = oStockAddition.Details.ElementAt(i).StockAdditionUnitValue;
                                pt.quantity = oStockAddition.Details.ElementAt(i).Quantity;
                                pt.mrp = oStockAddition.Details.ElementAt(i).MRP;
                                pt.interstate_rate = oStockAddition.Details.ElementAt(i).InterstateRate;
                                pt.wholesale_rate = oStockAddition.Details.ElementAt(i).WholesaleRate;
                                pt.brand_code = oStockAddition.Details.ElementAt(i).BrandCode == "" ? pps.CreateProperty(ls.ReadGroupCodeOf(oStockAddition.Details.ElementAt(i).ProductCode), oStockAddition.Details.ElementAt(i).Brand, "1").PropertyCode : oStockAddition.Details.ElementAt(i).BrandCode;
                                pt.size_code = oStockAddition.Details.ElementAt(i).SizeCode == "" ? pps.CreateProperty(ls.ReadGroupCodeOf(oStockAddition.Details.ElementAt(i).ProductCode), oStockAddition.Details.ElementAt(i).Size, "2").PropertyCode : oStockAddition.Details.ElementAt(i).SizeCode;
                                pt.colour_code = oStockAddition.Details.ElementAt(i).ColourCode == "" ? pps.CreateProperty(ls.ReadGroupCodeOf(oStockAddition.Details.ElementAt(i).ProductCode), oStockAddition.Details.ElementAt(i).Colour, "3").PropertyCode : oStockAddition.Details.ElementAt(i).ColourCode;
                                pt.brand = oStockAddition.Details.ElementAt(i).Brand;
                                pt.size = oStockAddition.Details.ElementAt(i).Size;
                                pt.colour = oStockAddition.Details.ElementAt(i).Colour;
                                //get a barcode here                                
                                pt.barcode = oStockAddition.Details.ElementAt(i).Barcode != "" ?  oStockAddition.Details.ElementAt(i).Barcode: bs.ReadNextBarcode();
                                pt.unit_code = oStockAddition.Details.ElementAt(i).StockAdditionUnitCode;
                                pt.unit_value = oStockAddition.Details.ElementAt(i).StockAdditionUnitValue;

                                dataB.product_transactions.Add(pt);
                            }
                        }

                        //Updating serial numbers of entires that are already used
                        var tr = dataB.product_transactions.Select(c => c).Where(x => x.bill_no == oStockAddition.BillNo && x.financial_code == oStockAddition.FinancialCode && x.bill_type == mBillType && usedBarcodes.Contains<string>(x.barcode));
                        foreach (var item in tr)
                        {
                            item.serial_no = serialNo++;
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

        private List<string> ReadAlreadyUsedBarcodes(string billNo,string fCode)
        {
            List<string> barcodes = new List<string>();
            List<string> usedBarcodes = new List<string>();
            using (var dataB = new Database9006Entities())
            {
                try
                {
                    var bar = dataB.product_transactions.Select(c => new { c.barcode, c.bill_type, c.bill_no, c.financial_code }).Where(x => x.bill_type == mBillType && x.bill_no == billNo && x.financial_code == fCode);
                    barcodes = bar.Select(e => e.barcode).ToList<string>();

                    var usedBar = dataB.product_transactions.Select(c => new { c.barcode, c.bill_type }).Where(x => x.bill_type != mBillType && barcodes.Contains<string>(x.barcode));
                    usedBarcodes = usedBar.Select(e => e.barcode).ToList<string>();
                }
                catch
                {

                }
            }
            return usedBarcodes;
        }

        //Reports
        public List<CStockAdditionReportDetailed> FindStockAdditionDetailed(DateTime startDate, DateTime endDate, string billType, string billNo, string productCode, string product, string brandCode, string brand, string colourCode, string colour, string sizeCode, string size, string narration, string financialCode)
        {
            List<CStockAdditionReportDetailed> report = new List<CStockAdditionReportDetailed>();

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

                var resData = dataB.Database.SqlQuery<CStockAdditionReportDetailed>("Select  ((bd.quantity * bd.purchase_rate)-bd.product_discount) As Total, (bd.quantity * bd.purchase_rate) As GrossValue, bd.quantity As Quantity, bd.colour As Colour, bd.size As Size, bd.brand As Brand, bd.mrp As MRP, bd.wholesale_rate As WholesaleRate, bd.interstate_rate As InterstateRate, bd.purchase_rate As PurchaseRate, bd.product_discount As ProductDiscount, bd.purchase_unit As PurchaseUnit, bd.product As Product, bd.serial_no As SerialNo, bd.extra_charges As Expense, bd.discounts As Discount, bd.advance As Advance, bd.bill_date_time As BillDateTime,bd.bill_no As BillNo, bd.narration As Narration, bd.financial_code As FinancialCode From product_transactions bd Where(bd.bill_date_time >= '" + startD + "' && bd.bill_date_time <='" + endD + "') " + subQ + " Order By bd.bill_date_time,bd.bill_no, bd.serial_no");

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
                report.Add(new CStockAdditionReportDetailed() { BillDateTime = null, SerialNo = null, Product = "" });
                report.Add(new CStockAdditionReportDetailed() { BillDateTime = null, SerialNo = null, Product = "Total", Quantity = quantity, GrossValue = grossValue, ProductDiscount = proDiscount, Total = total });

            }


            return report;
        }

        public List<CStockAdditionReportSummary> FindStockAdditionSummary(DateTime startDate, DateTime endDate, string billType, string billNo, string productCode, string product, string brandCode, string brand, string colourCode, string colour, string sizeCode, string size, string narration, string financialCode)
        {
            List<CStockAdditionReportSummary> report = new List<CStockAdditionReportSummary>();

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

                var resData = dataB.Database.SqlQuery<CStockAdditionReportSummary>("Select Sum(((bd.quantity * bd.purchase_rate) - bd.product_discount)) As BillAmount, bd.extra_charges As Expense, bd.discounts As Discount, bd.advance As Advance, bd.bill_date_time As BillDateTime,bd.bill_no As BillNo, bd.narration As Narration, bd.financial_code As FinancialCode From product_transactions bd Where(bd.bill_date_time >= '" + startD + "' && bd.bill_date_time <='" + endD + "') " + subQ + "Group By bd.extra_charges, bd.discounts, bd.advance, bd.bill_date_time, bd.bill_no, bd.narration, bd.financial_code Order By bd.bill_date_time,bd.bill_no");

                decimal? billAmount = 0;
                foreach (var item in resData)
                {

                    billAmount = billAmount + item.BillAmount;
                    report.Add(item);
                }
                //Total
                report.Add(new CStockAdditionReportSummary() { BillDateTime = null, BillNo = "" });
                report.Add(new CStockAdditionReportSummary() { BillDateTime = null, BillNo = "Total", BillAmount = billAmount });

            }


            return report;
        }
    }
}
