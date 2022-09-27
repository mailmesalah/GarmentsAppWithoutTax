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
    public class ProductPropertyService:IProductProperty
    {        

        public CProductProperty CreateProperty(string productGroupCode, string property, string propertyType)
        {
            CProductProperty cpp = new CProductProperty();
            cpp.ProductGroupCode = productGroupCode;
            cpp.Property = property;
            cpp.PropertyType = propertyType;

            lock (Synchronizer.@lock)
            {
                
                using (var dataB = new Database9006Entities())
                {
                    
                        var dataBTransaction = dataB.Database.BeginTransaction();
                    try
                    {
                        var propD = dataB.product_property_code.Select(c => c).FirstOrDefault();
                        if (propD == null)
                        {
                            product_property_code ppc = dataB.product_property_code.Create();
                            ppc.code = "0";
                            dataB.product_property_code.Add(ppc);
                            cpp.PropertyCode = "0";
                        }
                        else
                        {
                            int pCode = int.Parse(propD.code.ToString());
                            cpp.PropertyCode = (++pCode).ToString();
                            propD.code = pCode.ToString();
                        }


                        product_properties pt = new product_properties();
                        pt.product_group_code = productGroupCode;
                        pt.property = property;
                        pt.property_code = cpp.PropertyCode;
                        pt.property_type = propertyType;

                        dataB.product_properties.Add(pt);

                        dataB.SaveChanges();

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

            return cpp;
        }

        
        public List<CProductProperty> ReadProductProperties(string productCode, string propertyType)
        {
            List<CProductProperty> prodProperties = new List<CProductProperty>();

            ProductService ps = new ProductService();

            string gCode = ps.ReadGroupCodeOf(productCode);

            using (var dataB = new Database9006Entities())
            {
                var cps = dataB.product_properties.Select(c => c).Where(x => x.product_group_code==gCode && x.property_type==propertyType).OrderBy(y=>y.property);

                foreach (var item in cps)
                {
                    CProductProperty cpp = new CProductProperty { PropertyCode=item.property_code, Property=item.property, PropertyType=item.property_type, ProductGroupCode=item.product_group_code };
                    prodProperties.Add(cpp);
                }
                
            }

            return prodProperties;
        }
        
    }
}
