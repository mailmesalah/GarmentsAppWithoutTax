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
    public class ProductService : IProduct
    {
        public bool CreateProductRegister(CProductRegister oProductRegister)
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


                        int cbillNo = bs.ReadNextProductRegisterBillNo();
                        bs.UpdateProductRegisterBillNo(cbillNo + 1);

                        product_register lr = dataB.product_register.Create();
                        lr.product_code = cbillNo.ToString();
                        lr.product = oProductRegister.Product;
                        lr.alternate_name = oProductRegister.AlternateName;
                        lr.type = oProductRegister.ProductType;
                        lr.group_code = oProductRegister.GroupCode;
                        lr.is_enabled = oProductRegister.IsEnabled;
                        lr.stockin_unit_code = oProductRegister.StockInUnitCode;
                        lr.stockout_unit_code = oProductRegister.StockOutUnitCode;
                        dataB.product_register.Add(lr);

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

        public bool UpdateProductRegister(CProductRegister oProductRegister)
        {

            bool returnValue = false;

            lock (Synchronizer.@lock)
            {
                using (var dataB = new Database9006Entities())
                {
                    var dataBTransaction = dataB.Database.BeginTransaction();
                    try
                    {
                        var cpp = dataB.product_register.Select(c => c).Where(x => x.product_code == oProductRegister.ProductCode);
                        dataB.product_register.RemoveRange(cpp);

                        product_register lr = dataB.product_register.Create();
                        lr.product_code = oProductRegister.ProductCode;
                        lr.product = oProductRegister.Product;
                        lr.alternate_name = oProductRegister.AlternateName;
                        lr.type = oProductRegister.ProductType;
                        lr.group_code = oProductRegister.GroupCode;
                        lr.is_enabled = oProductRegister.IsEnabled;
                        lr.stockin_unit_code = oProductRegister.StockInUnitCode;
                        lr.stockout_unit_code = oProductRegister.StockOutUnitCode;
                        dataB.product_register.Add(lr);
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

        public bool DeleteProductRegister(string productCode)
        {            

            if (IsProductHasChild(productCode) == true)
            {
                return false;
            }

            if (IsProductUsedInTransaction(productCode) == true)
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

                        var cr = dataB.product_register.Select(c => c).Where(x => x.product_code == productCode);
                        dataB.product_register.RemoveRange(cr);

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

        public CProductRegister ReadProductRegister(string productCode)
        {
            CProductRegister ccp = null;

            using (var dataB = new Database9006Entities())
            {
                var cps = dataB.product_register.Select(c => c).Where(x => x.product_code == productCode);

                if (cps.Count() > 0)
                {
                    ccp = new CProductRegister();

                    var cp = cps.FirstOrDefault();
                    ccp.Id = cp.id;
                    ccp.ProductCode = cp.product_code;
                    ccp.Product = cp.product;
                    ccp.ProductType = cp.type;
                    ccp.GroupCode = cp.group_code;
                    ccp.AlternateName = cp.alternate_name;
                    ccp.IsEnabled = cp.is_enabled;
                    ccp.StockInUnitCode = cp.stockin_unit_code;
                    ccp.StockOutUnitCode = cp.stockout_unit_code;                    
                }

            }

            return ccp;
        }
        

        private bool IsProductHasChild(string productCode)
        {
            bool hasChild = false;
            using (var dataB = new Database9006Entities())
            {
                var data = dataB.product_register.Select(e => e).Where(e => e.group_code == productCode);
                if (data.Count() > 0)
                {
                    hasChild = true;
                }
            }
            return hasChild;
        }

        private bool IsProductUsedInTransaction(string productCode)
        {
            bool isUsed = false;
            using (var dataB = new Database9006Entities())
            {
                var data = dataB.product_transactions.Select(e => e).Where(e => e.product_code == productCode);
                if (data.Count() > 0)
                {
                    isUsed = true;
                }
            }
            return isUsed;
        }
        
        public List<CProduct> ReadAllGroupProducts()
        {
            List<CProduct> products = new List<CProduct>();
            using (var dataB = new Database9006Entities())
            {
                var datas = dataB.product_register.Select(e => new { e.product_code, e.product, e.type,e.stockin_unit_code,e.stockout_unit_code }).Where(e => e.type == "AGroup").OrderBy(e => e.product);
                foreach (var item in datas)
                {
                    products.Add(new CProduct() { Product = item.product, ProductCode = item.product_code, ProductType = item.type, StockInUnitCode = item.stockin_unit_code, StockOutUnitCode = item.stockout_unit_code });
                }
            }

            return products;
        }

        public List<CProduct> ReadAllProducts()
        {
            List<CProduct> products = new List<CProduct>();
            using (var dataB = new Database9006Entities())
            {
                var datas = dataB.product_register.Select(e => new { e.product_code, e.product, e.type,e.stockin_unit_code,e.stockout_unit_code }).Where(e => e.type == "BProduct").OrderBy(e => e.product);
                foreach (var item in datas)
                {
                    products.Add(new CProduct() { Product = item.product, ProductCode = item.product_code, ProductType = item.type, StockInUnitCode=item.stockin_unit_code,StockOutUnitCode=item.stockout_unit_code });
                }
            }

            return products;
        }


        public ObservableCollection<CProduct> ReadAllProductsWithGroupAsTree()
        {
            ObservableCollection<CProduct> products = new ObservableCollection<CProduct>();
            try
            {
                using (var dataB1 = new Database9006Entities())
                {
                    using (var dataB2 = new Database9006Entities())
                    {

                        var agroups = dataB1.product_register.Select(e => new { e.product_code, e.product, e.type }).Where(e => (e.type == "AGroup")).OrderBy(e => e.product);
                        foreach (var item1 in agroups)
                        {
                            CProduct aProduct = new CProduct() { Product = item1.product, ProductCode = item1.product_code, ProductType = item1.type };
                            ObservableCollection<CProduct> aMembers = new ObservableCollection<CProduct>();

                            var bProducts = dataB2.product_register.Select(e => new { e.product_code, e.product, e.type, e.group_code }).Where(e => (e.group_code == aProduct.ProductCode)).OrderBy(e => e.product);
                            foreach (var item2 in bProducts)
                            {
                                CProduct bProduct = new CProduct() { Product = item2.product, ProductCode = item2.product_code, ProductType = item2.type };
                                aMembers.Add(bProduct);
                            }

                            aProduct.MemberList = aMembers;
                            products.Add(aProduct);

                        }
                    }
                }
            }
            catch (Exception e)
            {

            }
            return products;
        }

        
        public string ReadGroupCodeOf(string productCode)
        {
            string groupCode = "";
            using (var dataB = new Database9006Entities())
            {
                var data = dataB.product_register.Select(e => new { e.group_code, e.product_code }).Where(e => e.product_code == productCode).FirstOrDefault();
                groupCode = data.group_code;
            }

            return groupCode;
        }
        

    }
}
