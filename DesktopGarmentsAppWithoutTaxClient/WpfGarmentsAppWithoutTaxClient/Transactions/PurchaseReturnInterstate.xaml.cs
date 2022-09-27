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
    /// Interaction logic for PurchaseReturn.xaml
    /// </summary>
    public partial class PurchaseReturnInterstate : Window
    {
        
        CPurchaseReturn mPurchaseReturn = new CPurchaseReturn();
        ObservableCollection<CPurchaseReturnDetails> mGridContent = new ObservableCollection<CPurchaseReturnDetails>();
        String mPurchaseReturnID = "";
        string mBarcode = "";
        decimal mOldQuantity = 0;
        decimal mCurrentUnitValue = 0;
        string mBrandCode = "";
        string mSizeCode = "";
        string mColourCode = "";
        
        public PurchaseReturnInterstate(string billNo, DateTime billDate)
        {
            InitializeComponent();
            loadInitialDetails();

            mTextBoxBillNo.Text = billNo;
            mDTPDate.SelectedDate = billDate;

            showDataFromDatabase();
        }

        public PurchaseReturnInterstate()
        {
            InitializeComponent();
            loadInitialDetails();
                        
        }
        

        //Member methods
        private void loadInitialDetails()
        {
            getSuppliers();
            getProducts();
            newBill();            
        }

        private void newBill()
        {
            mPurchaseReturnID = "";
            mPurchaseReturn = new CPurchaseReturn();
            mTextBoxBillNo.Text = getLastBillNo();
            mDTPDate.SelectedDate = DateTime.Now;
            mTextBoxRefBillNo.Text = "";
            mDTPRefDate.SelectedDate= DateTime.Now;
            mComboSupplier.Text = "";
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
            mTextBoxPurchaseReturnRate.Text = "";
            mTextBoxInterstateRate.Text = "";
            mTextBoxWholesaleRate.Text = "";
            mTextBoxMRP.Text = "";
            mTextBoxProductDiscount.Text = "";
            mBarcode = "";
            mBrandCode = "";
            mSizeCode = "";
            mColourCode = "";
            mTextBoxBrand.Text = "";
            mTextBoxColour.Text = "";
            mTextBoxSize.Text = "";
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
                using (ChannelFactory<IPurchaseReturn> PurchaseReturnProxy = new ChannelFactory<ServerServiceInterface.IPurchaseReturn>("PurchaseReturnEndpoint"))
                {
                    PurchaseReturnProxy.Open();
                    IPurchaseReturn PurchaseService = PurchaseReturnProxy.CreateChannel();
                    billNo=PurchaseService.ReadNextBillNo("PRI",CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value)).ToString();                    
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

        private void getSuppliers()
        {
            try
            {
                using (ChannelFactory<ILedger> ledgerProxy = new ChannelFactory<ServerServiceInterface.ILedger>("LedgerEndpoint"))
                {
                    ledgerProxy.Open();
                    ILedger ledgerService = ledgerProxy.CreateChannel();
                    List<CLedgerRegister> ledgers = ledgerService.ReadAllSupplierRegisters();
                    mComboSupplier.ItemsSource = ledgers;
                    mComboSupplier.DisplayMemberPath = "Ledger";
                    mComboSupplier.SelectedValuePath = "LedgerCode";
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

            decimal purchaseRate = 0;
            try
            {

                purchaseRate = decimal.Parse(mTextBoxPurchaseReturnRate.Text);

                if (purchaseRate < 0)
                {
                    MessageBox.Show("Purchase rate not given");
                    mTextBoxPurchaseReturnRate.Focus();
                    return;
                }
            }
            catch
            {
                mTextBoxPurchaseReturnRate.Focus();
                return;
            }

            decimal iRate = 0;
            try
            {

                iRate = decimal.Parse(mTextBoxInterstateRate.Text);

                if (iRate < 0)
                {
                    MessageBox.Show("Interstate rate not given");
                    mTextBoxInterstateRate.Focus();
                    return;
                }
            }
            catch
            {
                mTextBoxInterstateRate.Focus();
                return;
            }

            decimal wRate = 0;
            try
            {

                wRate = decimal.Parse(mTextBoxWholesaleRate.Text);

                if (wRate < 0)
                {
                    MessageBox.Show("Wholesale rate not given");
                    mTextBoxWholesaleRate.Focus();
                    return;
                }
            }
            catch
            {
                mTextBoxWholesaleRate.Focus();
                return;
            }

            decimal mrp = 0;
            try
            {

                mrp = decimal.Parse(mTextBoxMRP.Text);

                if (mrp < 0)
                {
                    MessageBox.Show("MRP not given");
                    mTextBoxMRP.Focus();
                    return;
                }
            }
            catch
            {
                mTextBoxMRP.Focus();
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

            string sBrand = mTextBoxBrand.Text;
            string sBCode = mBrandCode;
            string sSize = mTextBoxSize.Text;
            string sSCode = mColourCode;
            string sColour = mTextBoxColour.Text;
            string sCCode = mSizeCode;

            int serialNo = int.Parse(mLabelSerialNo.Content.ToString());
            if (serialNo <= mDataGridContent.Items.Count)
            {
                //Edit
                int index = mDataGridContent.SelectedIndex;
                mGridContent.Remove(mDataGridContent.SelectedItem as CPurchaseReturnDetails);
                mGridContent.Insert(index, new CPurchaseReturnDetails() { SerialNo = serialNo, Product = mComboProducts.Text.ToString(), ProductCode = mComboProducts.SelectedValue.ToString(), PurchaseReturnUnit = mComboUnits.Text.ToString(), PurchaseReturnUnitCode = mComboUnits.SelectedValue.ToString(), Quantity = quantity, PurchaseReturnRate = purchaseRate, InterstateRate = iRate, WholesaleRate = wRate, MRP = mrp, ProductDiscount = discount, Total = total, PurchaseReturnUnitValue = (mComboUnits.SelectedItem as CUnit).UnitValue, Barcode = mBarcode, GrossValue = grossValue, OldQuantity=mOldQuantity, Brand = sBrand, BrandCode = sBCode, Colour = sColour, ColourCode = sCCode, Size = sSize, SizeCode = sSCode });
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
                CPurchaseReturnDetails crd = (CPurchaseReturnDetails)mDataGridContent.Items.GetItemAt(mDataGridContent.SelectedIndex);
                mLabelSerialNo.Content = crd.SerialNo;
                mComboProducts.Text = crd.Product;
                mComboUnits.Text = crd.PurchaseReturnUnit;
                mTextBoxQuantity.Text = crd.Quantity.ToString("N3");
                mTextBoxPurchaseReturnRate.Text = crd.PurchaseReturnRate.ToString("N2");
                mTextBoxInterstateRate.Text = crd.InterstateRate.ToString("N2");
                mTextBoxWholesaleRate.Text = crd.WholesaleRate.ToString("N2");
                mTextBoxMRP.Text = crd.MRP.ToString("N2");
                mBarcode = crd.Barcode;
                mTextBoxProductDiscount.Text = crd.ProductDiscount.ToString("N2");
                mCurrentUnitValue = crd.PurchaseReturnUnitValue;
                mOldQuantity = crd.OldQuantity;
                mTextBoxBrand.Text = crd.Brand;
                mTextBoxSize.Text = crd.Size;
                mTextBoxColour.Text = crd.Colour;
                mBrandCode = crd.BrandCode;
                mColourCode = crd.ColourCode;
                mSizeCode = crd.ColourCode;
            }
        }

        private void removeFromGrid()
        {
            if (mDataGridContent.SelectedIndex > -1)
            {
                mGridContent.Remove(mDataGridContent.SelectedItem as CPurchaseReturnDetails);

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
                using (ChannelFactory<IPurchaseReturn> PurchaseReturnProxy = new ChannelFactory<ServerServiceInterface.IPurchaseReturn>("PurchaseReturnEndpoint"))
                {
                    PurchaseReturnProxy.Open();
                    IPurchaseReturn PurchaseReturnervice = PurchaseReturnProxy.CreateChannel();

                    CPurchaseReturn ccr= PurchaseReturnervice.ReadBill(mTextBoxBillNo.Text.Trim(), "PRI", CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value));
                    
                    if (ccr != null)
                    {                        
                        mPurchaseReturnID = ccr.Id.ToString();                        
                        mDTPDate.SelectedDate = ccr.BillDateTime;
                        mTextBoxRefBillNo.Text = ccr.RefBillNo;
                        mDTPRefDate.SelectedDate = ccr.RefBillDateTime;
                        mComboSupplier.Text = ccr.Supplier;
                        mTextBoxAddress.Text = ccr.SupplierAddress;
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
                using (ChannelFactory<IPurchaseReturn> PurchaseProxy = new ChannelFactory<ServerServiceInterface.IPurchaseReturn>("PurchaseReturnEndpoint"))
                {
                    PurchaseProxy.Open();
                    IPurchaseReturn PurchaseService = PurchaseProxy.CreateChannel();

                    CPurchaseReturn ccr = PurchaseService.ReadPurchaseBill(mTextBoxRefBillNo.Text.Trim(), "PI", CommonMethods.getFinancialCode(mDTPRefDate.SelectedDate.Value));

                    if (ccr != null)
                    {
                        mDTPRefDate.SelectedDate = ccr.BillDateTime;
                        mComboSupplier.Text = ccr.Supplier;
                        mTextBoxAddress.Text = ccr.SupplierAddress;
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

                if (mComboSupplier.SelectedItem == null)
                {
                    mComboSupplier.Focus();
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

                using (ChannelFactory<IPurchaseReturn> PurchaseReturnProxy = new ChannelFactory<ServerServiceInterface.IPurchaseReturn>("PurchaseReturnEndpoint"))
                {
                    PurchaseReturnProxy.Open();
                    IPurchaseReturn PurchaseReturnService = PurchaseReturnProxy.CreateChannel();

                    CPurchaseReturn ccp = new CPurchaseReturn();
                    ccp.BillNo = mTextBoxBillNo.Text.Trim();
                    ccp.BillDateTime = mDTPDate.SelectedDate.Value;
                    ccp.RefBillNo = mTextBoxRefBillNo.Text.Trim();
                    ccp.RefBillDateTime = mDTPRefDate.SelectedDate.Value;
                    ccp.SupplierCode = mComboSupplier.SelectedValue.ToString() ;
                    ccp.Supplier = mComboSupplier.Text;
                    ccp.SupplierAddress = mTextBoxAddress.Text;
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
                    if (mPurchaseReturnID != "")
                    { 
                        success = PurchaseReturnService.UpdateBill(ccp, "PRI");
                    }
                    else
                    {                    
                        success = PurchaseReturnService.CreateBill(ccp, "PRI");
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
                using (ChannelFactory<IPurchaseReturn> PurchaseReturnProxy = new ChannelFactory<ServerServiceInterface.IPurchaseReturn>("PurchaseReturnEndpoint"))
                {
                    PurchaseReturnProxy.Open();
                    IPurchaseReturn PurchaService = PurchaseReturnProxy.CreateChannel();
                    
                    bool success= PurchaService.DeleteBill(mTextBoxBillNo.Text.Trim(), "PRI", CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value));

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

            decimal purchaseRate = 0;
            try
            {
                purchaseRate = decimal.Parse(mTextBoxPurchaseReturnRate.Text);
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

            decimal grossValue = (quantity * purchaseRate);
            decimal netValue = grossValue - discount;

            mLabelGrossValue.Content = grossValue.ToString("N2");
            mLabelTotal.Content = (netValue).ToString("N2");
        }

        private void setGrandTotalnBalance()
        {
            decimal gTotal = 0;
            try
            {
                gTotal = mGridContent.Sum(x => ((x.Quantity * x.PurchaseReturnRate) - x.ProductDiscount));

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

        private void mTextBoxPurchaseReturnRate_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            setTotal();
        }

        private void mComboSupplier_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            mTextBoxAddress.Text = "";
            if (mComboSupplier.SelectedValue != null&& mComboSupplier.SelectedIndex > -1)
            {
                mTextBoxAddress.Text= (mComboSupplier.SelectedItem as CLedgerRegister).Address1;
            }
        }

        private void mComboProducts_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (mComboProducts.SelectedValue != null && mComboProducts.SelectedIndex > -1)
            {
                getUnitsOfProduct();
                mComboUnits.SelectedValue = (mComboProducts.SelectedItem as CProduct).StockInUnitCode;
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

                    rate = decimal.Parse(mTextBoxPurchaseReturnRate.Text);                    
                    newUnitV = (mComboUnits.SelectedItem as CUnit).UnitValue;                    
                    rate /= (newUnitV / mCurrentUnitValue);

                    mTextBoxPurchaseReturnRate.Text = rate.ToString("N2");
                }
                catch
                {                 
                }

                try
                {
                    mOldQuantity = mOldQuantity / (mCurrentUnitValue / newUnitV);
                }
                catch
                {
                }

                try
                {

                    rate = decimal.Parse(mTextBoxInterstateRate.Text);
                    rate /= (newUnitV / mCurrentUnitValue);

                    mTextBoxInterstateRate.Text = rate.ToString("N2");
                }
                catch
                {
                }

                try
                {

                    rate = decimal.Parse(mTextBoxWholesaleRate.Text);
                    rate /= (newUnitV / mCurrentUnitValue);

                    mTextBoxWholesaleRate.Text = rate.ToString("N2");
                }
                catch
                {
                }

                try
                {

                    rate = decimal.Parse(mTextBoxMRP.Text);                    
                    rate /= (newUnitV / mCurrentUnitValue);

                    mTextBoxMRP.Text = rate.ToString("N2");
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
