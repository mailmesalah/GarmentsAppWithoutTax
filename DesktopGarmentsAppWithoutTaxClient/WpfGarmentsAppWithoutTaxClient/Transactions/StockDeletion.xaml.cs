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
    /// Interaction logic for StockDeletion.xaml
    /// </summary>
    public partial class StockDeletion : Window
    {
        
        CStockDeletion mStockDeletion = new CStockDeletion();
        ObservableCollection<CStockDeletionDetails> mGridContent = new ObservableCollection<CStockDeletionDetails>();
        ObservableCollection<CStock> mStockContent = new ObservableCollection<CStock>();
        String mStockDeletionID = "";
        string mBarcode = "";
        string mBrandCode = "";
        string mSizeCode = "";
        string mColourCode = "";

        public StockDeletion(string billNo, DateTime billDate)
        {
            InitializeComponent();
            loadInitialDetails();

            mTextBoxBillNo.Text = billNo;
            mDTPDate.SelectedDate = billDate;

            showDataFromDatabase();
        }

        public StockDeletion()
        {
            InitializeComponent();
            loadInitialDetails();
                        
        }
        

        //Member methods
        private void loadInitialDetails()
        {
            getProducts();
            newBill();            
        }

        private void newBill()
        {
            mStockDeletionID = "";
            mStockDeletion = new CStockDeletion();
            mTextBoxBillNo.Text = getLastBillNo();
            mDTPDate.SelectedDate = DateTime.Now;
            mTextBoxNarration.Text = ""; 
            loadFinancialCodes();            
            mComboFinancialYear.Text = CommonMethods.getFinancialCode(DateTime.Now);
            mGridContent.Clear();
            mDataGridContent.ItemsSource = mGridContent;
            clearEditBoxes();           
        }

        private void clearEditBoxes(){
            mLabelSerialNo.Content = mDataGridContent.Items.Count+1;
            mComboProducts.Text = "";
            mComboUnits.Text = "";
            mTextBoxQuantity.Text = "";            
            mStockContent.Clear();
            mLabelInterstate.Content = "";
            mLabelWholesaleRate.Content = "";
            mLabelMRP.Content = "";
            mBarcode = "";
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
                using (ChannelFactory<IStockDeletion> StockDeletionProxy = new ChannelFactory<ServerServiceInterface.IStockDeletion>("StockDeletionEndpoint"))
                {
                    StockDeletionProxy.Open();
                    IStockDeletion StockService = StockDeletionProxy.CreateChannel();
                    billNo=StockService.ReadNextBillNo(CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value)).ToString();                    
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
                    string unitCode = (mComboProducts.SelectedItem as CProduct).StockOutUnitCode;

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
                    using (ChannelFactory<IStockDeletion> StockDeletionProxy = new ChannelFactory<ServerServiceInterface.IStockDeletion>("StockDeletionEndpoint"))
                    {
                        StockDeletionProxy.Open();
                        IStockDeletion stockDeletionService = StockDeletionProxy.CreateChannel();
                        List<CStock> stocks = stockDeletionService.ReadStockOfProduct(productCode,unitCode,unit,unitValue,billNo, fCode);

                        mStockContent.Clear();
                        foreach (var item in stocks)
                        {                            
                            decimal gridQ=mGridContent.Where(c=>c.Barcode==item.Barcode).Sum(c=>c.Quantity*(unitValue/c.StockDeletionUnitValue));
                            if (item.Quantity - gridQ > 0)
                            {
                                mStockContent.Add(new CStock() { Barcode = item.Barcode, Quantity = item.Quantity - gridQ, Unit = item.Unit, InterstateRate = item.InterstateRate, MRP=item.MRP, WholesaleRate=item.WholesaleRate, Brand=item.Brand, BrandCode=item.BrandCode, Colour= item.Colour, ColourCode=item.ColourCode, Size=item.Size, SizeCode=item.SizeCode });
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
                if (quantity <= 0 || quantity>stockQ)
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
            
            decimal mrp = 0;
            try
            {
                mrp = decimal.Parse(mLabelMRP.Content.ToString());
            }
            catch
            {

            }

            decimal iRate = 0;
            try
            {
                iRate = decimal.Parse(mLabelInterstate.Content.ToString());
            }
            catch
            {

            }

            decimal wRate = 0;
            try
            {
                wRate = decimal.Parse(mLabelWholesaleRate.Content.ToString());
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
                mGridContent.Remove(mDataGridContent.SelectedItem as CStockDeletionDetails);
                mGridContent.Insert(index, new CStockDeletionDetails() { SerialNo = serialNo, Product = mComboProducts.Text.ToString(), ProductCode = mComboProducts.SelectedValue.ToString(), StockDeletionUnit = mComboUnits.Text.ToString(), StockDeletionUnitCode = mComboUnits.SelectedValue.ToString(), Quantity = quantity, StockDeletionUnitValue = (mComboUnits.SelectedItem as CUnit).UnitValue, Barcode = mBarcode, MRP=mrp, InterstateRate= iRate, WholesaleRate=wRate, Brand = sBrand, BrandCode = sBCode, Colour = sColour, ColourCode = sCCode, Size = sSize, SizeCode = sSCode });
            }
            else
            {
                //Add
                CStockDeletionDetails crd = new CStockDeletionDetails() { SerialNo = serialNo, Product = mComboProducts.Text.ToString(), ProductCode = mComboProducts.SelectedValue.ToString(), StockDeletionUnit = mComboUnits.Text.ToString(), StockDeletionUnitCode = mComboUnits.SelectedValue.ToString(), Quantity = quantity, StockDeletionUnitValue = (mComboUnits.SelectedItem as CUnit).UnitValue, Barcode = mBarcode, MRP = mrp, InterstateRate = iRate, WholesaleRate = wRate, Brand = sBrand, BrandCode = sBCode, Colour = sColour, ColourCode = sCCode, Size = sSize, SizeCode = sSCode};
                mGridContent.Add(crd);
            }
            
            clearEditBoxes();
            mDataGridContent.ScrollIntoView(mDataGridContent.Items.GetItemAt(mDataGridContent.Items.Count-1));
            mComboProducts.Focus();

        }

        private void selectDataToEditBoxes()
        {
            if (mDataGridContent.SelectedIndex > -1)
            {
                CStockDeletionDetails crd=(CStockDeletionDetails)mDataGridContent.Items.GetItemAt(mDataGridContent.SelectedIndex);
                mLabelSerialNo.Content = crd.SerialNo;
                mComboProducts.Text = crd.Product;
                mComboUnits.Text = crd.StockDeletionUnit;
                mTextBoxQuantity.Text = crd.Quantity.ToString("N3");                
                mBarcode = crd.Barcode;
                mLabelInterstate.Content = crd.InterstateRate.ToString("N2");
                mLabelWholesaleRate.Content = crd.WholesaleRate.ToString("N2");
                mLabelMRP.Content = crd.MRP.ToString("N2");
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
                mGridContent.Remove(mDataGridContent.SelectedItem as CStockDeletionDetails);

                //Reseting the Serial Nos
                for(int i = 0; i < mGridContent.Count; i++)
                {
                    mGridContent.ElementAt(i).SerialNo = i + 1;                    
                }
                mDataGridContent.Items.Refresh();
                clearEditBoxes();
            }

        }

        private void showDataFromDatabase()
        {
            try
            {
                using (ChannelFactory<IStockDeletion> StockDeletionProxy = new ChannelFactory<ServerServiceInterface.IStockDeletion>("StockDeletionEndpoint"))
                {
                    StockDeletionProxy.Open();
                    IStockDeletion StockDeletionervice = StockDeletionProxy.CreateChannel();

                    CStockDeletion ccr= StockDeletionervice.ReadBill(mTextBoxBillNo.Text.Trim(),CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value));
                    
                    if (ccr != null)
                    {                        
                        mStockDeletionID = ccr.Id.ToString();                        
                        mDTPDate.SelectedDate = ccr.BillDateTime;
                        mTextBoxNarration.Text = ccr.Narration;
                                                 
                        mGridContent.Clear();
                        foreach (var item in ccr.Details)
                        {
                            mGridContent.Add(item);
                        }
                        mDataGridContent.Items.Refresh();
                    }                    
                }

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
                
                if (mDataGridContent.Items.Count==0)
                {
                    mComboProducts.Focus();
                    return;
                }

                using (ChannelFactory<IStockDeletion> StockDeletionProxy = new ChannelFactory<ServerServiceInterface.IStockDeletion>("StockDeletionEndpoint"))
                {
                    StockDeletionProxy.Open();
                    IStockDeletion StockDeletionService = StockDeletionProxy.CreateChannel();

                    CStockDeletion ccp = new CStockDeletion();
                    ccp.BillNo = mTextBoxBillNo.Text.Trim();
                    ccp.BillDateTime = mDTPDate.SelectedDate.Value;
                    ccp.Narration = mTextBoxNarration.Text.Trim();
                    ccp.FinancialCode = CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value);
                    foreach (var item in mGridContent)
                    {
                        ccp.Details.Add(item);
                    }

                    bool success = false;
                    if (mStockDeletionID != "")
                    { 
                        success = StockDeletionService.UpdateBill(ccp);
                    }
                    else
                    {                    
                        success = StockDeletionService.CreateBill(ccp);
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
                using (ChannelFactory<IStockDeletion> StockDeletionProxy = new ChannelFactory<ServerServiceInterface.IStockDeletion>("StockDeletionEndpoint"))
                {
                    StockDeletionProxy.Open();
                    IStockDeletion PurchaService = StockDeletionProxy.CreateChannel();
                    
                    bool success= PurchaService.DeleteBill(mTextBoxBillNo.Text.Trim(), CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value));

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
        
        private void selectStockToEditControls()
        {
            if(mDataGridStock.Items.Count>0 && mDataGridStock.SelectedItem != null)
            {
                CStock stock = mDataGridStock.SelectedItem as CStock;
                mBarcode = stock.Barcode;
                mTextBoxQuantity.Text = "1";
                mLabelInterstate.Content = stock.InterstateRate.ToString("N2");
                mLabelWholesaleRate.Content = stock.WholesaleRate.ToString("N2");
                mLabelMRP.Content = stock.MRP.ToString("N2");
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

        private void mPrint_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
