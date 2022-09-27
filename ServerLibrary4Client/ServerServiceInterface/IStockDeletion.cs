using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace ServerServiceInterface
{    
    [ServiceContract]
    public interface IStockDeletion
    {
        [OperationContract]
        bool CreateBill(CStockDeletion oStockDeletion);
        [OperationContract]
        CStockDeletion ReadBill(string billNo,string financialCode);
        [OperationContract]
        bool UpdateBill(CStockDeletion oStockDeletion);
        [OperationContract]
        bool DeleteBill(string billNo,string financialCode);

        [OperationContract]
        int ReadNextBillNo(string financialCode);

        [OperationContract]
        List<CStock> ReadStockOfProduct(string productCode,string unitCode, string unit,decimal unitValue,string billNo,string fCode);

        [OperationContract]
        List<CStockDeletionReportSummary> FindStockDeletionSummary(DateTime startDate, DateTime endDate, string billType, string billNo, string productCode, string product, string brandCode, string brand, string colourCode, string colour, string sizeCode, string size, string narration, string financialCode);
        [OperationContract]
        List<CStockDeletionReportDetailed> FindStockDeletionDetailed(DateTime startDate, DateTime endDate, string billType, string billNo, string productCode, string product, string brandCode, string brand, string colourCode, string colour, string sizeCode, string size, string narration, string financialCode);
    }


    
    [DataContract]
    public class CStockDeletion
    {
        int id;
        string billNo;
        DateTime billDateTime = new DateTime();
        string narration;
        string financialCode;
        List<CStockDeletionDetails> details= new List<CStockDeletionDetails>();
 
        [DataMember]
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        [DataMember]
        public string BillNo
        {
            get { return billNo; }
            set { billNo = value; }
        }
        
        [DataMember]
        public DateTime BillDateTime
        {
            get { return billDateTime; }
            set { billDateTime = value; }
        }
   
        [DataMember]
        public string Narration
        {
            get { return narration; }
            set { narration = value; }
        }
        
        [DataMember]
        public string FinancialCode
        {
            get { return financialCode; }
            set { financialCode = value; }
        }

        [DataMember]
        public List<CStockDeletionDetails> Details
        {
            get { return details; }
            set { details = value; }
        }
    }


    [DataContract]
    public class CStockDeletionDetails
    {
        int serialNo;
        string productCode;
        string product;
        string stockDeletionUnit;
        string stockDeletionUnitCode;
        decimal stockDeletionUnitValue;
        decimal quantity;
        decimal mrp;
        decimal interstateRate;
        decimal wholesaleRate;
        string brand;
        string size;
        string colour;
        string brandCode;
        string sizeCode;
        string colourCode;        
        string barcode;       

        [DataMember]
        public int SerialNo
        {
            get { return serialNo; }
            set { serialNo = value; }
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
        public string StockDeletionUnit
        {
            get { return stockDeletionUnit; }
            set { stockDeletionUnit = value; }
        }

        [DataMember]
        public string StockDeletionUnitCode
        {
            get { return stockDeletionUnitCode; }
            set { stockDeletionUnitCode = value; }
        }

        [DataMember]
        public decimal StockDeletionUnitValue
        {
            get { return stockDeletionUnitValue; }
            set { stockDeletionUnitValue = value; }
        }

        [DataMember]
        public decimal Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }
        
        [DataMember]
        public decimal MRP
        {
            get { return mrp; }
            set { mrp = value; }
        }

        [DataMember]
        public decimal InterstateRate
        {
            get { return interstateRate; }
            set { interstateRate = value; }
        }

        [DataMember]
        public decimal WholesaleRate
        {
            get { return wholesaleRate; }
            set { wholesaleRate = value; }
        }

        [DataMember]
        public string Brand
        {
            get { return brand; }
            set { brand = value; }
        }
        [DataMember]
        public string Size
        {
            get { return size; }
            set { size = value; }
        }
        [DataMember]
        public string Colour
        {
            get { return colour; }
            set { colour = value; }
        }

        [DataMember]
        public string BrandCode
        {
            get { return brandCode; }
            set { brandCode = value; }
        }
        [DataMember]
        public string SizeCode
        {
            get { return sizeCode; }
            set { sizeCode = value; }
        }
        [DataMember]
        public string ColourCode
        {
            get { return colourCode; }
            set { colourCode = value; }
        }

        [DataMember]
        public string Barcode
        {
            get { return barcode; }
            set { barcode = value; }
        }
        
    }


    [DataContract]
    public class CStockDeletionReportSummary
    {
        string billNo;
        DateTime? billDateTime = new DateTime();
        string narration;
        decimal? advance;
        decimal? expense;
        decimal? discount;
        decimal? billAmount;
        string financialCode;

        [DataMember]
        public string BillNo
        {
            get { return billNo; }
            set { billNo = value; }
        }

        [DataMember]
        public DateTime? BillDateTime
        {
            get { return billDateTime; }
            set { billDateTime = value; }
        }

        [DataMember]
        public string Narration
        {
            get { return narration; }
            set { narration = value; }
        }

        [DataMember]
        public decimal? Advance
        {
            get { return advance; }
            set { advance = value; }
        }

        [DataMember]
        public decimal? Expense
        {
            get { return expense; }
            set { expense = value; }
        }

        [DataMember]
        public decimal? Discount
        {
            get { return discount; }
            set { discount = value; }
        }

        [DataMember]
        public decimal? BillAmount
        {
            get { return billAmount; }
            set { billAmount = value; }
        }

        [DataMember]
        public string FinancialCode
        {
            get { return financialCode; }
            set { financialCode = value; }
        }
    }

    [DataContract]
    public class CStockDeletionReportDetailed
    {
        string billNo;
        DateTime? billDateTime = new DateTime();
        string narration;
        decimal? advance;
        decimal? expense;
        decimal? discount;
        string financialCode;

        int? serialNo;
        string product;
        string purchaseUnit;
        decimal? quantity;
        decimal? grossValue;
        decimal? productDiscount;
        decimal? salesRate;
        decimal? interstateRate;
        decimal? wholesaleRate;
        decimal? mrp;
        string brand;
        string size;
        string colour;
        decimal? total;

        [DataMember]
        public string BillNo
        {
            get { return billNo; }
            set { billNo = value; }
        }

        [DataMember]
        public DateTime? BillDateTime
        {
            get { return billDateTime; }
            set { billDateTime = value; }
        }

        [DataMember]
        public string Narration
        {
            get { return narration; }
            set { narration = value; }
        }

        [DataMember]
        public decimal? Advance
        {
            get { return advance; }
            set { advance = value; }
        }

        [DataMember]
        public decimal? Expense
        {
            get { return expense; }
            set { expense = value; }
        }

        [DataMember]
        public decimal? Discount
        {
            get { return discount; }
            set { discount = value; }
        }

        [DataMember]
        public string FinancialCode
        {
            get { return financialCode; }
            set { financialCode = value; }
        }

        [DataMember]
        public int? SerialNo
        {
            get { return serialNo; }
            set { serialNo = value; }
        }

        [DataMember]
        public string Product
        {
            get { return product; }
            set { product = value; }
        }

        [DataMember]
        public string PurchaseUnit
        {
            get { return purchaseUnit; }
            set { purchaseUnit = value; }
        }

        [DataMember]
        public decimal? Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        [DataMember]
        public decimal? ProductDiscount
        {
            get { return productDiscount; }
            set { productDiscount = value; }
        }

        [DataMember]
        public decimal? SalesRate
        {
            get { return salesRate; }
            set { salesRate = value; }
        }

        [DataMember]
        public decimal? InterstateRate
        {
            get { return interstateRate; }
            set { interstateRate = value; }
        }

        [DataMember]
        public decimal? WholesaleRate
        {
            get { return wholesaleRate; }
            set { wholesaleRate = value; }
        }

        [DataMember]
        public decimal? MRP
        {
            get { return mrp; }
            set { mrp = value; }
        }

        [DataMember]
        public string Brand
        {
            get { return brand; }
            set { brand = value; }
        }
        [DataMember]
        public string Size
        {
            get { return size; }
            set { size = value; }
        }
        [DataMember]
        public string Colour
        {
            get { return colour; }
            set { colour = value; }
        }

        [DataMember]
        public decimal? GrossValue
        {
            get { return grossValue; }
            set { grossValue = value; }
        }

        [DataMember]
        public decimal? Total
        {
            get { return total; }
            set { total = value; }
        }
    }

}
