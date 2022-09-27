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
    /// Interaction logic for SalesReturn.xaml
    /// </summary>
    public partial class SalesReturnLocal : Window
    {
        
        CSalesReturn mSalesReturn = new CSalesReturn();
        ObservableCollection<CSalesReturnDetails> mGridContent = new ObservableCollection<CSalesReturnDetails>();
        String mSalesReturnID = "";
        string mBarcode = "";
        decimal mOldQuantity = 0;
        decimal mCurrentUnitValue = 0;
        decimal mRate = 0;
        string mBrandCode = "";
        string mSizeCode = "";
        string mColourCode = "";

        public SalesReturnLocal(string billNo, DateTime billDate)
        {
            InitializeComponent();
            loadInitialDetails();

            mTextBoxBillNo.Text = billNo;
            mDTPDate.SelectedDate = billDate;

            showDataFromDatabase();
        }

        public SalesReturnLocal()
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
            mSalesReturnID = "";
            mSalesReturn = new CSalesReturn();
            mTextBoxBillNo.Text = getLastBillNo();
            mDTPDate.SelectedDate = DateTime.Now;
            mTextBoxRefBillNo.Text = "";
            mDTPRefDate.SelectedDate= DateTime.Now;
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
                using (ChannelFactory<ISalesReturn> SalesReturnProxy = new ChannelFactory<ServerServiceInterface.ISalesReturn>("SalesReturnEndpoint"))
                {
                    SalesReturnProxy.Open();
                    ISalesReturn SalesService = SalesReturnProxy.CreateChannel();
                    billNo=SalesService.ReadNextBillNo("SRL",CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value)).ToString();                    
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

                if (quantity <= 0 || quantity > mOldQuantity)
                {
                    MessageBox.Show("Quantity not given is incorrect");
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
                    MessageBox.Show("SalesReturn rate not given");
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
                mGridContent.Remove(mDataGridContent.SelectedItem as CSalesReturnDetails);
                mGridContent.Insert(index, new CSalesReturnDetails() { SerialNo = serialNo, Product = mComboProducts.Text.ToString(), ProductCode = mComboProducts.SelectedValue.ToString(), SalesReturnUnit = mComboUnits.Text.ToString(), SalesReturnUnitCode = mComboUnits.SelectedValue.ToString(), Quantity=quantity,SalesReturnRate=salesRate,Rate=mRate, Total=total, SalesReturnUnitValue= (mComboUnits.SelectedItem as CUnit).UnitValue  ,Barcode=mBarcode, OldQuantity = mOldQuantity, ProductDiscount=discount, GrossValue=grossValue, Brand = sBrand, BrandCode = sBCode, Colour = sColour, ColourCode = sCCode, Size = sSize, SizeCode = sSCode });
            }

            clearEditBoxes();
            mDataGridContent.ScrollIntoView(mDataGridContent.Items.GetItemAt(mDataGridContent.Items.Count-1));
            mComboProducts.Focus();

            setGrandTotalnBalance();
        }

        private void selectDataToEditBoxes()
        {
            if (mDataGridContent.SelectedIndex > -1)
            {
                CSalesReturnDetails crd=(CSalesReturnDetails)mDataGridContent.Items.GetItemAt(mDataGridContent.SelectedIndex);
                mLabelSerialNo.Content = crd.SerialNo;
                mComboProducts.Text = crd.Product;
                mComboUnits.Text = crd.SalesReturnUnit;
                mTextBoxQuantity.Text = crd.Quantity.ToString("N3");
                mTextBoxSalesRate.Text = crd.SalesReturnRate.ToString("N2");                
                mBarcode = crd.Barcode;
                mOldQuantity = crd.OldQuantity;
                mCurrentUnitValue = crd.SalesReturnUnitValue;                
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
                mGridContent.Remove(mDataGridContent.SelectedItem as CSalesReturnDetails);

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
                using (ChannelFactory<ISalesReturn> SalesReturnProxy = new ChannelFactory<ServerServiceInterface.ISalesReturn>("SalesReturnEndpoint"))
                {
                    SalesReturnProxy.Open();
                    ISalesReturn SalesReturnervice = SalesReturnProxy.CreateChannel();

                    CSalesReturn ccr= SalesReturnervice.ReadBill(mTextBoxBillNo.Text.Trim(), "SRL", CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value));
                    
                    if (ccr != null)
                    {                        
                        mSalesReturnID = ccr.Id.ToString();                        
                        mDTPDate.SelectedDate = ccr.BillDateTime;
                        mTextBoxRefBillNo.Text = ccr.RefBillNo;
                        mDTPRefDate.SelectedDate = ccr.RefBillDateTime;
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

        private void showRefBillDataFromDatabase()
        {
            try
            {
                using (ChannelFactory<ISalesReturn> SalesProxy = new ChannelFactory<ServerServiceInterface.ISalesReturn>("SalesReturnEndpoint"))
                {
                    SalesProxy.Open();
                    ISalesReturn SalesService = SalesProxy.CreateChannel();

                    CSalesReturn ccr = SalesService.ReadSalesBill(mTextBoxRefBillNo.Text.Trim(), "SL", CommonMethods.getFinancialCode(mDTPRefDate.SelectedDate.Value));

                    if (ccr != null)
                    {
                        mDTPRefDate.SelectedDate = ccr.BillDateTime;
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

                using (ChannelFactory<ISalesReturn> SalesReturnProxy = new ChannelFactory<ServerServiceInterface.ISalesReturn>("SalesReturnEndpoint"))
                {
                    SalesReturnProxy.Open();
                    ISalesReturn SalesReturnService = SalesReturnProxy.CreateChannel();

                    CSalesReturn ccp = new CSalesReturn();
                    ccp.BillNo = mTextBoxBillNo.Text.Trim();
                    ccp.BillDateTime = mDTPDate.SelectedDate.Value;
                    ccp.RefBillNo = mTextBoxRefBillNo.Text.Trim();
                    ccp.RefBillDateTime = mDTPRefDate.SelectedDate.Value;
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
                    if (mSalesReturnID != "")
                    { 
                        success = SalesReturnService.UpdateBill(ccp, "SRL");
                    }
                    else
                    {                    
                        success = SalesReturnService.CreateBill(ccp, "SRL");
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
                using (ChannelFactory<ISalesReturn> SalesReturnProxy = new ChannelFactory<ServerServiceInterface.ISalesReturn>("SalesReturnEndpoint"))
                {
                    SalesReturnProxy.Open();
                    ISalesReturn PurchaService = SalesReturnProxy.CreateChannel();
                    
                    bool success= PurchaService.DeleteBill(mTextBoxBillNo.Text.Trim(), "SRL", CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value));

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
                gTotal = mGridContent.Sum(x => ((x.Quantity * x.SalesReturnRate) - x.ProductDiscount));

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
            }
        }

        private void mTextBoxRefBillNo_LostFocus(object sender, RoutedEventArgs e)
        {
            showRefBillDataFromDatabase();
        }

        private void mComboUnits_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (mComboUnits.SelectedItem != null)
            {
                decimal rate = 0;
                decimal newUnitV = 0;
                try
                {

                    rate = decimal.Parse(mTextBoxSalesRate.Text);
                    newUnitV = (mComboUnits.SelectedItem as CUnit).UnitValue;
                    rate /= (newUnitV / mCurrentUnitValue);

                    mTextBoxSalesRate.Text = rate.ToString();
                }
                catch
                {
                }


                try
                {
                    mRate /= (newUnitV / mCurrentUnitValue);
                    mOldQuantity = mOldQuantity / (mCurrentUnitValue / newUnitV);
                }
                catch
                {
                }

                mCurrentUnitValue = newUnitV;
            }
        }

        private void mTextBoxTax_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            setTotal();
        }

        private void mTextBoxProductDiscount_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            setTotal();
        }
    }
}
