using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace ServerServiceInterface
{    
    [ServiceContract]
    public interface ISales
    {
        [OperationContract]
        bool CreateBill(CSales oSales, string billType);
        [OperationContract]
        CSales ReadBill(string billNo, string billType, string financialCode);
        [OperationContract]
        bool UpdateBill(CSales oSales, string billType);
        [OperationContract]
        bool DeleteBill(string billNo, string billType, string financialCode);

        [OperationContract]
        int ReadNextBillNo(string billType, string financialCode);

        [OperationContract]
        List<CStock> ReadStockOfProduct(string productCode,string unitCode, string unit,decimal unitValue,string billNo, string billType, string fCode);


        [OperationContract]
        List<CSalesReportSummary> FindSalesSummary(DateTime startDate, DateTime endDate, string billType, string billNo, string customerCode, string customer, string productCode, string product, string brandCode, string brand, string colourCode, string colour, string sizeCode, string size, string narration, string financialCode);
        [OperationContract]
        List<CSalesReportDetailed> FindSalesDetailed(DateTime startDate, DateTime endDate, string billType, string billNo, string customerCode, string customer, string productCode, string product, string brandCode, string brand, string colourCode, string colour, string sizeCode, string size, string narration, string financialCode);
    }


    
    [DataContract]
    public class CSales
    {
        int id;
        string billNo;
        DateTime billDateTime = new DateTime();
        string customer;
        string customerCode;
        string customerAddress;
        string narration;
        decimal advance;
        decimal expense;
        decimal discount;
        string financialCode;
        List<CSalesDetails> details= new List<CSalesDetails>();
 
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
        public string Customer
        {
            get { return customer; }
            set { customer = value; }
        }

        [DataMember]
        public string CustomerCode
        {
            get { return customerCode; }
            set { customerCode = value; }
        }

        [DataMember]
        public string CustomerAddress
        {
            get { return customerAddress; }
            set { customerAddress = value; }
        }

        [DataMember]
        public string Narration
        {
            get { return narration; }
            set { narration = value; }
        }

        [DataMember]
        public decimal Advance
        {
            get { return advance; }
            set { advance = value; }
        }

        [DataMember]
        public decimal Expense
        {
            get { return expense; }
            set { expense = value; }
        }

        [DataMember]
        public decimal Discount
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
        public List<CSalesDetails> Details
        {
            get { return details; }
            set { details = value; }
        }
    }


    [DataContract]
    public class CSalesDetails
    {
        int serialNo;
        string productCode;
        string product;
        string salesUnit;
        string salesUnitCode;
        decimal salesUnitValue;
        decimal quantity;
        decimal grossValue;
        decimal productDiscount;
        decimal salesRate;
        string brandCode;
        string sizeCode;
        string colourCode;
        decimal rate;
        string brand;
        string size;
        string colour;
        string barcode;
        decimal total;

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
        public string SalesUnit
        {
            get { return salesUnit; }
            set { salesUnit = value; }
        }

        [DataMember]
        public string SalesUnitCode
        {
            get { return salesUnitCode; }
            set { salesUnitCode = value; }
        }

        [DataMember]
        public decimal SalesUnitValue
        {
            get { return salesUnitValue; }
            set { salesUnitValue = value; }
        }

        [DataMember]
        public decimal Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }
        
        [DataMember]
        public decimal ProductDiscount
        {
            get { return productDiscount; }
            set { productDiscount = value; }
        }

        [DataMember]
        public decimal SalesRate
        {
            get { return salesRate; }
            set { salesRate = value; }
        }

        [DataMember]
        public decimal Rate
        {
            get { return rate; }
            set { rate = value; }
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
        public decimal GrossValue
        {
            get { return grossValue; }
            set { grossValue = value; }
        }

        [DataMember]
        public string Barcode
        {
            get { return barcode; }
            set { barcode = value; }
        }

        [DataMember]
        public decimal Total
        {
            get { return total; }
            set { total = value; }
        }
    }

    [DataContract]
    public class CStock
    {
        string barcode;
        decimal quantity;
        string unit;
        decimal mrp;
        decimal interstateRate;
        decimal wholesaleRate;
        string brand;
        string size;
        string colour;
        string brandCode;
        string sizeCode;
        string colourCode;

        [DataMember]
        public string Barcode
        {
            get { return barcode; }
            set { barcode = value; }
        }

        [DataMember]
        public decimal Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        [DataMember]
        public string Unit
        {
            get { return unit; }
            set { unit = value; }
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

    }


    [DataContract]
    public class CSalesReportSummary
    {
        string billNo;
        DateTime? billDateTime = new DateTime();
        string customer;
        string customerAddress;
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
        public string Customer
        {
            get { return customer; }
            set { customer = value; }
        }

        [DataMember]
        public string CustomerAddress
        {
            get { return customerAddress; }
            set { customerAddress = value; }
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
    public class CSalesReportDetailed
    {
        string billNo;
        DateTime? billDateTime = new DateTime();
        string customer;
        string customerAddress;
        string narration;
        decimal? advance;
        decimal? expense;
        decimal? discount;
        string financialCode;

        int? serialNo;
        string product;
        string salesUnit;
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
        public string Customer
        {
            get { return customer; }
            set { customer = value; }
        }

        [DataMember]
        public string CustomerAddress
        {
            get { return customerAddress; }
            set { customerAddress = value; }
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
        public string SalesUnit
        {
            get { return salesUnit; }
            set { salesUnit = value; }
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
