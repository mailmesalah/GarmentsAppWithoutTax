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
    /// Interaction logic for StockAddition.xaml
    /// </summary>
    public partial class StockAddition : Window
    {
        
        CStockAddition mStockAddition = new CStockAddition();
        ObservableCollection<CStockAdditionDetails> mGridContent = new ObservableCollection<CStockAdditionDetails>();
        String mStockAdditionID = "";
        string mBarcode = "";
        
        public StockAddition(string billNo, DateTime billDate)
        {
            InitializeComponent();
            loadInitialDetails();

            mTextBoxBillNo.Text = billNo;
            mDTPDate.SelectedDate = billDate;

            showDataFromDatabase();
        }

        public StockAddition()
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
            mStockAdditionID = "";
            mStockAddition = new CStockAddition();
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
            mTextBoxInterstateRate.Text = "";
            mTextBoxWholesaleRate.Text = "";
            mTextBoxMRP.Text = "";
            mBarcode = "";
            mComboBrand.Text = "";
            mComboSize.Text = "";
            mComboColour.Text = "";
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
                using (ChannelFactory<IStockAddition> StockAdditionProxy = new ChannelFactory<ServerServiceInterface.IStockAddition>("StockAdditionEndpoint"))
                {
                    StockAdditionProxy.Open();
                    IStockAddition StockAdditionService = StockAdditionProxy.CreateChannel();
                    billNo=StockAdditionService.ReadNextBillNo(CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value)).ToString();                    
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

        public void getBrandsOfProduct()
        {
            try
            {
                if (mComboProducts.SelectedValue != null)
                {
                    string pCode = (mComboProducts.SelectedItem as CProduct).ProductCode;

                    using (ChannelFactory<IProductProperty> propertyProxy = new ChannelFactory<ServerServiceInterface.IProductProperty>("ProductPropertyEndpoint"))
                    {
                        propertyProxy.Open();
                        IProductProperty propertyService = propertyProxy.CreateChannel();
                        List<CProductProperty> properties = propertyService.ReadProductProperties(pCode, "1");
                        mComboBrand.ItemsSource = properties;
                        mComboBrand.DisplayMemberPath = "Property";
                        mComboBrand.SelectedValuePath = "PropertyCode";
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void getSizesOfProduct()
        {
            try
            {
                if (mComboProducts.SelectedValue != null)
                {
                    string pCode = (mComboProducts.SelectedItem as CProduct).ProductCode;

                    using (ChannelFactory<IProductProperty> propertyProxy = new ChannelFactory<ServerServiceInterface.IProductProperty>("ProductPropertyEndpoint"))
                    {
                        propertyProxy.Open();
                        IProductProperty propertyService = propertyProxy.CreateChannel();
                        List<CProductProperty> properties = propertyService.ReadProductProperties(pCode, "2");
                        mComboSize.ItemsSource = properties;
                        mComboSize.DisplayMemberPath = "Property";
                        mComboSize.SelectedValuePath = "PropertyCode";
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void getColoursOfProduct()
        {
            try
            {
                if (mComboProducts.SelectedValue != null)
                {
                    string pCode = (mComboProducts.SelectedItem as CProduct).ProductCode;

                    using (ChannelFactory<IProductProperty> propertyProxy = new ChannelFactory<ServerServiceInterface.IProductProperty>("ProductPropertyEndpoint"))
                    {
                        propertyProxy.Open();
                        IProductProperty propertyService = propertyProxy.CreateChannel();
                        List<CProductProperty> properties = propertyService.ReadProductProperties(pCode, "3");
                        mComboColour.ItemsSource = properties;
                        mComboColour.DisplayMemberPath = "Property";
                        mComboColour.SelectedValuePath = "PropertyCode";
                    }
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

                if (quantity <= 0)
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

            decimal mrp=0;
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

            string sBrand = mComboBrand.Text;
            string sBCode = mComboBrand.SelectedValue != null ? mComboBrand.SelectedValue.ToString() : "";
            string sSize = mComboSize.Text;
            string sSCode = mComboSize.SelectedValue != null ? mComboSize.SelectedValue.ToString() : "";
            string sColour = mComboColour.Text;
            string sCCode = mComboColour.SelectedValue != null ? mComboColour.SelectedValue.ToString() : "";

            int serialNo = int.Parse(mLabelSerialNo.Content.ToString());
            if (serialNo <= mDataGridContent.Items.Count)
            {
                //Edit
                int index = mDataGridContent.SelectedIndex;
                mGridContent.Remove(mDataGridContent.SelectedItem as CStockAdditionDetails);
                mGridContent.Insert(index, new CStockAdditionDetails() { SerialNo = serialNo, Product = mComboProducts.Text.ToString(), ProductCode = mComboProducts.SelectedValue.ToString(), StockAdditionUnit = mComboUnits.Text.ToString(), StockAdditionUnitCode = mComboUnits.SelectedValue.ToString(), Quantity=quantity,InterstateRate=iRate, WholesaleRate=wRate ,MRP=mrp, StockAdditionUnitValue= (mComboUnits.SelectedItem as CUnit).UnitValue  ,Barcode=mBarcode, Brand = sBrand, BrandCode = sBCode, Colour = sColour, ColourCode = sCCode, Size = sSize, SizeCode = sSCode });
            }
            else
            {
                //Add
                CStockAdditionDetails crd = new CStockAdditionDetails() { SerialNo = serialNo, Product = mComboProducts.Text.ToString(), ProductCode = mComboProducts.SelectedValue.ToString(), StockAdditionUnit = mComboUnits.Text.ToString(), StockAdditionUnitCode = mComboUnits.SelectedValue.ToString(), Quantity = quantity, InterstateRate = iRate, WholesaleRate = wRate, MRP = mrp, StockAdditionUnitValue = (mComboUnits.SelectedItem as CUnit).UnitValue, Barcode = "", Brand = sBrand, BrandCode = sBCode, Colour = sColour, ColourCode = sCCode, Size = sSize, SizeCode = sSCode};
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
                CStockAdditionDetails crd=(CStockAdditionDetails)mDataGridContent.Items.GetItemAt(mDataGridContent.SelectedIndex);
                mLabelSerialNo.Content = crd.SerialNo;
                mComboProducts.Text = crd.Product;
                mComboUnits.Text = crd.StockAdditionUnit;
                mTextBoxQuantity.Text = crd.Quantity.ToString("N3");                
                mTextBoxInterstateRate.Text = crd.InterstateRate.ToString("N2");
                mTextBoxWholesaleRate.Text = crd.WholesaleRate.ToString("N2");
                mTextBoxMRP.Text = crd.MRP.ToString("N2");
                mBarcode = crd.Barcode;
                mComboBrand.Text = crd.Brand;
                mComboSize.Text = crd.Size;
                mComboColour.Text = crd.Colour;
            }
        }

        private void removeFromGrid()
        {
            if (mDataGridContent.SelectedIndex > -1)
            {
                mGridContent.Remove(mDataGridContent.SelectedItem as CStockAdditionDetails);

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
                using (ChannelFactory<IStockAddition> StockAdditionProxy = new ChannelFactory<ServerServiceInterface.IStockAddition>("StockAdditionEndpoint"))
                {
                    StockAdditionProxy.Open();
                    IStockAddition StockAdditionService = StockAdditionProxy.CreateChannel();

                    CStockAddition ccr= StockAdditionService.ReadBill(mTextBoxBillNo.Text.Trim(),CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value));
                    
                    if (ccr != null)
                    {                        
                        mStockAdditionID = ccr.Id.ToString();                        
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
                    MessageBox.Show("Data not given");
                    mComboProducts.Focus();
                    return;
                }

                using (ChannelFactory<IStockAddition> StockAdditionProxy = new ChannelFactory<ServerServiceInterface.IStockAddition>("StockAdditionEndpoint"))
                {
                    StockAdditionProxy.Open();
                    IStockAddition StockAdditionService = StockAdditionProxy.CreateChannel();

                    CStockAddition ccp = new CStockAddition();
                    ccp.BillNo = mTextBoxBillNo.Text.Trim();
                    ccp.BillDateTime = mDTPDate.SelectedDate.Value;
                    ccp.Narration = mTextBoxNarration.Text.Trim();
                    ccp.FinancialCode = CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value);
                    foreach (var item in mGridContent)
                    {
                        ccp.Details.Add(item);
                    }

                    bool success = false;
                    if (mStockAdditionID != "")
                    { 
                        success = StockAdditionService.UpdateBill(ccp);
                    }
                    else
                    {                    
                        success = StockAdditionService.CreateBill(ccp);
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
                using (ChannelFactory<IStockAddition> StockAdditionProxy = new ChannelFactory<ServerServiceInterface.IStockAddition>("StockAdditionEndpoint"))
                {
                    StockAdditionProxy.Open();
                    IStockAddition PurchaService = StockAdditionProxy.CreateChannel();
                    
                    bool success= PurchaService.DeleteBill(mTextBoxBillNo.Text.Trim(),CommonMethods.getFinancialCode(mDTPDate.SelectedDate.Value));

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
                getBrandsOfProduct();
                getSizesOfProduct();
                getColoursOfProduct();
                mComboUnits.SelectedValue = (mComboProducts.SelectedItem as CProduct).StockInUnitCode;
            }
        }
        
    }
}
