using ServerServiceInterface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using WpfClientApp.General;

namespace WpfClientApp.Transactions
{
    /// <summary>
    /// Interaction logic for Sales.xaml
    /// </summary>
    public partial class SalesWholesale : Window
    {
        
        CSales mSales = new CSales();
        ObservableCollection<CSalesDetails> mGridContent = new ObservableCollection<CSalesDetails>();
        ObservableCollection<CStock> mStockContent = new ObservableCollection<CStock>();
        String mSalesID = "";
        string mBarcode = "";
        decimal mRate = 0;
        string mBrandCode = "";
        string mSizeCode = "";
        string mColourCode = "";

        public SalesWholesale(string billNo, DateTime billDate)
        {
            InitializeComponent();
            loadInitialDetails();

            mTextBoxBillNo.Text = billNo;
            mDTPDate.SelectedDate = billDate;

            showDataFromDatabase();
        }

        public SalesWholesale()
        {
            InitializeComponent();
            loadInitialDetails();
                        
        }
        

        //Member methods
        private void loadInitialDetails()
        {
            getCustomers();
            getProducts();
            newBill();            
        }

        private void newBill()
        {
            mSalesID = "";
            mSales = new CSales();
            mTextBoxBillNo.Text = getLastBillNo();
            mDTPDate.SelectedDate = DateTime.Now;
            mComboCustomer.Text = "";
            mTextBoxAddress.Text = "";
            mTextBoxNarration.Text = ""; 
            loadFinancialCodes();            
            mComboFinancialYear.Text = CommonMethods.getFinancialCode(DateTime.Now);
            mTextBoxAdvance.Text = "";
            mTextBoxExpense.Text = "";
            mTextBoxDiscount.Text = "";
            mGridContent.Clear();
            mDataGridContent.ItemsSource = mGridContent;
            clearEditBoxes();
            setGrandTotalnBalance();
        }

        private void clearEditBoxes(){
            mLabelSerialNo.Content = mDataGridContent.Items.Count + 1;
            mComboProducts.Text = "";
            mComboUnits.Text = "";
            mTextBoxQuantity.Text = "";
            mTextBoxSalesRate.Text = "";
            mStockContent.Clear();
            mTextBoxProductDiscount.Text = "";
            mBarcode = "";
            mRate = 0;
            mBrandCode = "";
            mSizeCode = "";
            mColourCode = "";
            mLabelBrand.Content = "";
            mLabelColour.Content = "";
            mLabelSize.Content = "";
        }

        private void loadFinancialCodes()
        {
            try
            {
                using (ChannelFactory<IBillNo> billNoProxy = new ChannelFactory<ServerServiceInterface.IBillNo>("BillNoEndpoint"))
                {
                    billNoProxy.Open();
                    IBillNo billNoService = billNoProxy.CreateChannel();

                    List<String> fcodes = billNoService.ReadAllFinancialCodes();
                    mComboFinancialYear.ItemsSource = fcodes;                    
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private string getLastBillNo()
        {
            string billNo = "";
            try {
                using (ChannelFactory<ISales> SalesProxy = new ChannelFactory<ServerServiceInterface.ISales>("SalesEndpoint"))
                {
                    SalesProxy.Open();
                    ISales PurchaService = SalesProxy.CreateChannel();
                    billNo=PurchaService.ReadNextBillNo("SW",CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value)).ToString();                    
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return billNo;
        }

        private void getProducts()
        {
            try {
                using (ChannelFactory<IProduct> ledgerProxy = new ChannelFactory<ServerServiceInterface.IProduct>("ProductEndpoint"))
                {
                    ledgerProxy.Open();
                    IProduct ledgerService = ledgerProxy.CreateChannel();                    
                    List<CProduct> ledgers = ledgerService.ReadAllProducts();
                    mComboProducts.ItemsSource = ledgers;
                    mComboProducts.DisplayMemberPath = "Product";
                    mComboProducts.SelectedValuePath = "ProductCode";
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void getUnitsOfProduct()
        {
            try
            {
                if (mComboProducts.SelectedValue != null)
                {
                    string unitCode = (mComboProducts.SelectedItem as CProduct).StockInUnitCode;

                    using (ChannelFactory<IUnit> UnitProxy = new ChannelFactory<ServerServiceInterface.IUnit>("UnitEndpoint"))
                    {
                        UnitProxy.Open();
                        IUnit UnitService = UnitProxy.CreateChannel();
                        List<CUnit> Units = UnitService.ReadSubUnits(unitCode);
                        mComboUnits.ItemsSource = Units;
                        mComboUnits.DisplayMemberPath = "Unit";
                        mComboUnits.SelectedValuePath = "UnitCode";
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void getCustomers()
        {
            try
            {
                using (ChannelFactory<ILedger> ledgerProxy = new ChannelFactory<ServerServiceInterface.ILedger>("LedgerEndpoint"))
                {
                    ledgerProxy.Open();
                    ILedger ledgerService = ledgerProxy.CreateChannel();
                    List<CLedgerRegister> ledgers = ledgerService.ReadAllCustomerRegisters();
                    mComboCustomer.ItemsSource = ledgers;
                    mComboCustomer.DisplayMemberPath = "Ledger";
                    mComboCustomer.SelectedValuePath = "LedgerCode";
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }


        public void getStockOfProduct()
        {
            try
            {
                if (mComboProducts.SelectedValue != null && mComboUnits.SelectedValue!=null )
                {
                    string productCode = (mComboProducts.SelectedItem as CProduct).ProductCode;
                    string unitCode = (mComboUnits.SelectedItem as CUnit).UnitCode;
                    string unit = (mComboUnits.SelectedItem as CUnit).Unit;
                    decimal unitValue = (mComboUnits.SelectedItem as CUnit).UnitValue;
                    string billNo = mTextBoxBillNo.Text.Trim();
                    string fCode= CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value);
                    using (ChannelFactory<ISales> SalesProxy = new ChannelFactory<ServerServiceInterface.ISales>("SalesEndpoint"))
                    {
                        SalesProxy.Open();
                        ISales salesService = SalesProxy.CreateChannel();
                        List<CStock> stocks = salesService.ReadStockOfProduct(productCode,unitCode,unit,unitValue,billNo, "SW", fCode);

                        mStockContent.Clear();
                        foreach (var item in stocks)
                        {                            
                            decimal gridQ=mGridContent.Where(c=>c.Barcode==item.Barcode).Sum(c=>c.Quantity*(unitValue/c.SalesUnitValue));
                            if (item.Quantity - gridQ > 0)
                            {
                                mStockContent.Add(new CStock() { Barcode = item.Barcode, Quantity = item.Quantity - gridQ, Unit = item.Unit, WholesaleRate = item.WholesaleRate, Brand = item.Brand, BrandCode = item.BrandCode, Colour = item.Colour, ColourCode = item.ColourCode, Size = item.Size, SizeCode = item.SizeCode });
                            }
                        }
                        mDataGridStock.ItemsSource = mStockContent;
                        mDataGridStock.Items.Refresh();
                    }
                }
                else
                {                    
                    mStockContent.Clear();
                    mDataGridStock.Items.Refresh();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void addDataToGrid()
        {
            if (mComboProducts.SelectedIndex == -1)
            {
                MessageBox.Show("Product not given");
                mComboProducts.Focus();
                return;
            }

            if (mComboUnits.SelectedIndex == -1)
            {
                MessageBox.Show("Unit not given");
                mComboUnits.Focus();
                return;
            }
      
            decimal quantity = 0;
            try
            {

                quantity = decimal.Parse(mTextBoxQuantity.Text);

                decimal stockQ = mStockContent.Where(c => c.Barcode == mBarcode).Select(c => c.Quantity).FirstOrDefault();
                if (quantity <= 0 || quantity > stockQ)
                {
                    MessageBox.Show("Quantity not given");
                    mTextBoxQuantity.Focus();
                    return;
                }
            }
            catch
            {
                mTextBoxQuantity.Focus();
                return;
            }

            decimal salesRate = 0;
            try
            {

                salesRate = decimal.Parse(mTextBoxSalesRate.Text);

                if (salesRate < 0)
                {
                    MessageBox.Show("Sales rate not given");
                    mTextBoxSalesRate.Focus();
                    return;
                }
            }
            catch
            {
                mTextBoxSalesRate.Focus();
                return;
            }

            decimal discount = 0;
            try
            {

                discount = decimal.Parse(mTextBoxProductDiscount.Text);
            }
            catch
            {
            }

            decimal grossValue = 0;
            try
            {
                grossValue = decimal.Parse(mLabelGrossValue.Content.ToString());
            }
            catch
            {

            }

            decimal total = 0;
            try
            {
                total = decimal.Parse(mLabelTotal.Content.ToString());
            }
            catch
            {

            }

            string sBrand = mLabelBrand.Content.ToString();
            string sBCode = mBrandCode;
            string sSize = mLabelSize.Content.ToString();
            string sSCode = mColourCode;
            string sColour = mLabelColour.Content.ToString();
            string sCCode = mSizeCode;

            int serialNo = int.Parse(mLabelSerialNo.Content.ToString());
            if (serialNo <= mDataGridContent.Items.Count)
            {
                //Edit
                int index = mDataGridContent.SelectedIndex;
                mGridContent.Remove(mDataGridContent.SelectedItem as CSalesDetails);
                mGridContent.Insert(index, new CSalesDetails() { SerialNo = serialNo, Product = mComboProducts.Text.ToString(), ProductCode = mComboProducts.SelectedValue.ToString(), SalesUnit = mComboUnits.Text.ToString(), SalesUnitCode = mComboUnits.SelectedValue.ToString(), Quantity = quantity, SalesRate = salesRate, Rate = mRate, Total = total, SalesUnitValue = (mComboUnits.SelectedItem as CUnit).UnitValue, Barcode = mBarcode, ProductDiscount = discount, GrossValue = grossValue, Brand = sBrand, BrandCode = sBCode, Colour = sColour, ColourCode = sCCode, Size = sSize, SizeCode = sSCode });
            }
            else
            {
                //Add
                CSalesDetails crd = new CSalesDetails() { SerialNo = serialNo, Product = mComboProducts.Text.ToString(), ProductCode = mComboProducts.SelectedValue.ToString(), SalesUnit = mComboUnits.Text.ToString(), SalesUnitCode = mComboUnits.SelectedValue.ToString(), Quantity = quantity, SalesRate = salesRate, Rate = mRate, Total = total, SalesUnitValue = (mComboUnits.SelectedItem as CUnit).UnitValue, Barcode = mBarcode, ProductDiscount = discount, GrossValue = grossValue, Brand = sBrand, BrandCode = sBCode, Colour = sColour, ColourCode = sCCode, Size = sSize, SizeCode = sSCode };
                mGridContent.Add(crd);
            }

            clearEditBoxes();
            mDataGridContent.ScrollIntoView(mDataGridContent.Items.GetItemAt(mDataGridContent.Items.Count - 1));
            mComboProducts.Focus();

            setGrandTotalnBalance();
        }

        private void selectDataToEditBoxes()
        {
            if (mDataGridContent.SelectedIndex > -1)
            {
                CSalesDetails crd = (CSalesDetails)mDataGridContent.Items.GetItemAt(mDataGridContent.SelectedIndex);
                mLabelSerialNo.Content = crd.SerialNo;
                mComboProducts.Text = crd.Product;
                mComboUnits.Text = crd.SalesUnit;
                mTextBoxQuantity.Text = crd.Quantity.ToString("N3");
                mTextBoxSalesRate.Text = crd.SalesRate.ToString("N2");
                mBarcode = crd.Barcode;
                mTextBoxProductDiscount.Text = crd.ProductDiscount.ToString("N2");
                mRate = crd.Rate;
                mLabelBrand.Content = crd.Brand;
                mLabelSize.Content = crd.Size;
                mLabelColour.Content = crd.Colour;
                mBrandCode = crd.BrandCode;
                mColourCode = crd.ColourCode;
                mSizeCode = crd.ColourCode;
            }
        }

        private void removeFromGrid()
        {
            if (mDataGridContent.SelectedIndex > -1)
            {
                mGridContent.Remove(mDataGridContent.SelectedItem as CSalesDetails);

                //Reseting the Serial Nos
                for(int i = 0; i < mGridContent.Count; i++)
                {
                    mGridContent.ElementAt(i).SerialNo = i + 1;                    
                }
                mDataGridContent.Items.Refresh();
                clearEditBoxes();
            }

            setGrandTotalnBalance();
        }

        private void showDataFromDatabase()
        {
            try
            {
                using (ChannelFactory<ISales> SalesProxy = new ChannelFactory<ServerServiceInterface.ISales>("SalesEndpoint"))
                {
                    SalesProxy.Open();
                    ISales Saleservice = SalesProxy.CreateChannel();

                    CSales ccr= Saleservice.ReadBill(mTextBoxBillNo.Text.Trim(), "SW", CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value));
                    
                    if (ccr != null)
                    {                        
                        mSalesID = ccr.Id.ToString();                        
                        mDTPDate.SelectedDate = ccr.BillDateTime;
                        mComboCustomer.Text = ccr.Customer;
                        mTextBoxAddress.Text = ccr.CustomerAddress;
                        mTextBoxNarration.Text = ccr.Narration;
                        mTextBoxExpense.Text = ccr.Expense.ToString();
                        mTextBoxDiscount.Text = ccr.Discount.ToString();
                        mTextBoxAdvance.Text = ccr.Advance.ToString();                        
                                                 
                        mGridContent.Clear();
                        foreach (var item in ccr.Details)
                        {
                            mGridContent.Add(item);
                        }
                        mDataGridContent.Items.Refresh();
                    }                    
                }

                setGrandTotalnBalance();
                clearEditBoxes();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
     
        private void saveDataToDatabase()
        {
            try
            {                

                if (mComboCustomer.SelectedItem == null)
                {
                    mComboCustomer.Focus();
                    return;
                }

                decimal advance=0;
                try
                {

                    advance = decimal.Parse(mTextBoxAdvance.Text);
                    
                }
                catch
                {
                   
                }
                decimal expense = 0;
                try
                {

                    expense = decimal.Parse(mTextBoxExpense.Text);

                }
                catch
                {

                }
                decimal discount = 0;
                try
                {

                    discount = decimal.Parse(mTextBoxDiscount.Text);

                }
                catch
                {

                }

                if (mDataGridContent.Items.Count==0)
                {
                    mComboProducts.Focus();
                    return;
                }

                using (ChannelFactory<ISales> SalesProxy = new ChannelFactory<ServerServiceInterface.ISales>("SalesEndpoint"))
                {
                    SalesProxy.Open();
                    ISales SalesService = SalesProxy.CreateChannel();

                    CSales ccp = new CSales();
                    ccp.BillNo = mTextBoxBillNo.Text.Trim();
                    ccp.BillDateTime = mDTPDate.SelectedDate.Value;
                    ccp.CustomerCode = mComboCustomer.SelectedValue.ToString() ;
                    ccp.Customer = mComboCustomer.Text;
                    ccp.CustomerAddress = mTextBoxAddress.Text;
                    ccp.Narration = mTextBoxNarration.Text.Trim();
                    ccp.Advance = advance;
                    ccp.Expense = expense;
                    ccp.Discount = discount;
                    ccp.FinancialCode = CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value);
                    foreach (var item in mGridContent)
                    {
                        ccp.Details.Add(item);
                    }

                    bool success = false;
                    if (mSalesID != "")
                    { 
                        success = SalesService.UpdateBill(ccp, "SW");
                    }
                    else
                    {                    
                        success = SalesService.CreateBill(ccp, "SW");
                    }

                    if (success)
                    {
                        newBill();
                    }
                    else
                    {
                        MessageBox.Show("Saving Failed");
                    }                    
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void deleteDataFromDatabase()
        {
            try
            {
                using (ChannelFactory<ISales> SalesProxy = new ChannelFactory<ServerServiceInterface.ISales>("SalesEndpoint"))
                {
                    SalesProxy.Open();
                    ISales PurchaService = SalesProxy.CreateChannel();
                    
                    bool success= PurchaService.DeleteBill(mTextBoxBillNo.Text.Trim(), "SW", CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value));

                    if (success)
                    {
                        newBill();
                    }
                    else
                    {
                        MessageBox.Show("Deletion Failed");
                    }                   
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void setTotal()
        {
            decimal quantity = 0;
            try
            {
                quantity = decimal.Parse(mTextBoxQuantity.Text);
            }
            catch
            {

            }

            decimal salesRate = 0;
            try
            {
                salesRate = decimal.Parse(mTextBoxSalesRate.Text);
            }
            catch
            {

            }
          
            decimal discount = 0;
            try
            {
                discount = decimal.Parse(mTextBoxProductDiscount.Text);
            }
            catch
            {

            }

            decimal grossValue = (quantity * salesRate);
            decimal netValue = grossValue - discount;

            mLabelGrossValue.Content = grossValue.ToString("N2");
            mLabelTotal.Content = (netValue).ToString("N2");
        }

        private void setGrandTotalnBalance()
        {
            decimal gTotal = 0;
            try
            {
                gTotal = mGridContent.Sum(x => ((x.Quantity * x.SalesRate) - x.ProductDiscount));

            }
            catch
            {

            }

            try
            {
                gTotal += decimal.Parse(mTextBoxExpense.Text);
            }
            catch
            {

            }

            try
            {
                gTotal -= decimal.Parse(mTextBoxDiscount.Text);
            }
            catch
            {

            }

            mLabelGrandTotal.Content = gTotal.ToString("N2");

            try
            {
                gTotal -= decimal.Parse(mTextBoxAdvance.Text);
            }
            catch
            {

            }
            mLabelBalance.Content = gTotal.ToString("N2");

        }

        private void selectStockToEditControls()
        {
            if(mDataGridStock.Items.Count>0 && mDataGridStock.SelectedItem != null)
            {
                CStock stock = mDataGridStock.SelectedItem as CStock;
                mBarcode = stock.Barcode;
                mTextBoxQuantity.Text = "1";
                mTextBoxSalesRate.Text = stock.WholesaleRate.ToString("N2");  
                mRate= stock.WholesaleRate;
                mBrandCode = stock.BrandCode;
                mColourCode = stock.ColourCode;
                mSizeCode = stock.SizeCode;
                mLabelBrand.Content = stock.Brand;
                mLabelSize.Content = stock.Size;
                mLabelColour.Content = stock.Colour;
            }
        }


        //Events
        private void mButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
       
        private void mButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            addDataToGrid();
        }

        private void mDTPDate_LostFocus(object sender, RoutedEventArgs e)
        {
            mComboFinancialYear.Text= CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value);
        }

        private void mButtonNew_Click(object sender, RoutedEventArgs e)
        {
            newBill();
        }

        private void mButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            deleteDataFromDatabase();
        }

        private void mComboFinancialYear_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mComboFinancialYear.SelectedIndex > -1)
                {
                    int year = int.Parse(mComboFinancialYear.SelectedItem.ToString());
                    if (!mComboFinancialYear.SelectedItem.ToString().Equals(CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value)))
                    {
                        mDTPDate.SelectedDate = new DateTime(year, CommonMethods.FinancialStartDate.Month, CommonMethods.FinancialStartDate.Day);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }        

        private void mDataGridContent_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            selectDataToEditBoxes();
        }

        private void mButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            removeFromGrid();
        }        

        private void mTextBoxBillNo_LostFocus(object sender, RoutedEventArgs e)
        {
            showDataFromDatabase();
        }

        private void mButtonSave_Click(object sender, RoutedEventArgs e)
        {
            saveDataToDatabase();
        }

        private void mTextBoxExpense_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            setGrandTotalnBalance();
        }

        private void mTextBoxDiscount_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            setGrandTotalnBalance();
        }

        private void mTextBoxAdvance_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            setGrandTotalnBalance();
        }

        private void mTextBoxQuantity_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            setTotal();
        }

        private void mTextBoxSalesRate_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            setTotal();
        }

        private void mComboCustomer_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            mTextBoxAddress.Text = "";
            if (mComboCustomer.SelectedValue != null&& mComboCustomer.SelectedIndex > -1)
            {
                mTextBoxAddress.Text= (mComboCustomer.SelectedItem as CLedgerRegister).Address1;
            }
        }

        private void mComboProducts_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (mComboProducts.SelectedValue != null && mComboProducts.SelectedIndex > -1)
            {
                getUnitsOfProduct();
                mComboUnits.SelectedValue = (mComboProducts.SelectedItem as CProduct).StockOutUnitCode;
                getStockOfProduct();
            }
        }

        private void mComboUnits_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (mComboUnits.SelectedValue != null && mComboUnits.SelectedIndex > -1)
            {
                getStockOfProduct();
            }
        }

        private void mDataGridStock_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            selectStockToEditControls();
        }        

        private void mDataGridStock_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter && mDataGridStock.Items.Count > 0)
            {
                mTextBoxQuantity.Focus();
                e.Handled = true;
            }
        }

        private void mDataGridStock_GotFocus(object sender, RoutedEventArgs e)
        {
            if (mDataGridStock.Items.Count > 0)
            {
                mDataGridStock.SelectedItem = mDataGridStock.Items.GetItemAt(0);
            }
            selectStockToEditControls();
        }

        private void mTextBoxTax_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            setTotal(); ;
        }

        private void mTextBoxProductDiscount_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            setTotal();
        }
    }
}
