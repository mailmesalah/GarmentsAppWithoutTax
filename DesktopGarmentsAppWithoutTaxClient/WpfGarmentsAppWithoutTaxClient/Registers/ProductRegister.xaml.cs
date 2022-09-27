using ServerServiceInterface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using WpfClientApp.General;

namespace WpfClientApp.Registers
{
    /// <summary>
    /// Interaction logic for ProductRegister.xaml
    /// </summary>
    public partial class ProductRegister : Window
    {

        private ObservableCollection<TreeViewItem> mProductTreeContents = new ObservableCollection<TreeViewItem>() ;
        private string mProductID;
        private string mProductType;
        private CProductRegister mCurrentProduct;

        private static string mLastSearchCode = "";

        public ProductRegister()
        {
            InitializeComponent();
            loadInitialDetails();
        }

        private void loadInitialDetails()
        {
            getAllUnits();
            getGroupProducts();

            mComboStatus.Items.Add("Enabled");
            mComboStatus.Items.Add("Disabled");
            cleanTheForm();
        }

        private void getGroupProducts()
        {
            try
            {
                using (ChannelFactory<IProduct> ProductProxy = new ChannelFactory<ServerServiceInterface.IProduct>("ProductEndpoint"))
                {
                    ProductProxy.Open();
                    IProduct ProductService = ProductProxy.CreateChannel();
                    List<CProduct> Products = ProductService.ReadAllGroupProducts();
                    mComboProductGroups.ItemsSource = Products;
                    mComboProductGroups.DisplayMemberPath = "Product";
                    mComboProductGroups.SelectedValuePath = "ProductCode";
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void getAllUnits()
        {
            try
            {
                using (ChannelFactory<IUnit> UnitProxy = new ChannelFactory<ServerServiceInterface.IUnit>("UnitEndpoint"))
                {
                    UnitProxy.Open();
                    IUnit UnitService = UnitProxy.CreateChannel();
                    List<CUnit> Units = UnitService.ReadAllUnits();
                    mComboPurchaseUnit.ItemsSource = Units;
                    mComboPurchaseUnit.DisplayMemberPath = "Unit";
                    mComboPurchaseUnit.SelectedValuePath = "UnitCode";
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void getUnitsForSales()
        {
            try
            {
                if (mComboPurchaseUnit.SelectedValue!=null)
                {
                    string unitCode = mComboPurchaseUnit.SelectedValue.ToString();

                    using (ChannelFactory<IUnit> UnitProxy = new ChannelFactory<ServerServiceInterface.IUnit>("UnitEndpoint"))
                    {
                        UnitProxy.Open();
                        IUnit UnitService = UnitProxy.CreateChannel();
                        List<CUnit> Units = UnitService.ReadSubUnits(unitCode);
                        mComboSalesUnit.ItemsSource = Units;
                        mComboSalesUnit.DisplayMemberPath = "Unit";
                        mComboSalesUnit.SelectedValuePath = "UnitCode";
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void getProductsForTree()
        {
            try
            {
                using (ChannelFactory<IProduct> ProductProxy = new ChannelFactory<ServerServiceInterface.IProduct>("ProductEndpoint"))
                {
                    ProductProxy.Open();
                    IProduct ProductService = ProductProxy.CreateChannel();
                    ObservableCollection<CProduct> Products = ProductService.ReadAllProductsWithGroupAsTree();
                    mProductTreeContents= convertCProductToTreeViewItem(Products);
                    mTreeProductRegister.ItemsSource = mProductTreeContents;                                        
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private ObservableCollection<TreeViewItem> convertCProductToTreeViewItem(ObservableCollection<CProduct> Products)
        {
            ObservableCollection<TreeViewItem> treeContents = new ObservableCollection<TreeViewItem>();
            foreach (var item1 in Products)
            {
                TreeViewItem aTItem = new TreeViewItem();
                foreach (var item2 in item1.MemberList)
                {
                    TreeViewItem bTItem = new TreeViewItem();
                    
                    bTItem.Header = item2.Product;
                    bTItem.Tag = item2;
                    aTItem.Items.Add(bTItem);
                }

                
                aTItem.Header = item1.Product;
                aTItem.Tag = item1;
                treeContents.Add(aTItem);
            }

            return treeContents;
        }

        private void cleanTheForm()
        {
            mProductID = "";
            mProductType = "";
            getProductsForTree();
            getGroupProducts();
            clearEditBoxes();
            mComboProductGroups.Focus();
        }

        private void clearEditBoxes()
        {           
            mComboProductGroups.Text = "";
            mTextBoxProduct.Text = "";
            mTextBoxAlternateName.Text = "";
            mComboPurchaseUnit.Text = "";
            mComboSalesUnit.Text = "";
            mComboStatus.SelectedIndex = 0;
        }

        private void selectDataToEditBoxes(CProduct l)
        {
            try
            {
                if (l != null)
                {
                    using (ChannelFactory<IProduct> ProductProxy = new ChannelFactory<ServerServiceInterface.IProduct>("ProductEndpoint"))
                    {
                        ProductProxy.Open();
                        IProduct ProductService = ProductProxy.CreateChannel();

                        mCurrentProduct = ProductService.ReadProductRegister(l.ProductCode);

                        if (mCurrentProduct!= null)
                        {
                            mProductID = mCurrentProduct.Id.ToString();
                            mTextBoxProduct.Text = mCurrentProduct.Product;
                            mComboProductGroups.SelectedValue = mCurrentProduct.GroupCode;
                            mTextBoxAlternateName.Text = mCurrentProduct.AlternateName;                            
                            mComboStatus.SelectedIndex = mCurrentProduct.IsEnabled==true ? 0 : 1;
                            mProductType = mCurrentProduct.ProductType;
                            mComboPurchaseUnit.SelectedValue = mCurrentProduct.StockInUnitCode;
                            getUnitsForSales();
                            mComboSalesUnit.SelectedValue = mCurrentProduct.StockOutUnitCode;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        private void createNewGroup()
        {            
            mProductID = "";
            mProductType = "AGroup";
            mComboProductGroups.Text = "";
            mTextBoxProduct.Text = "";
            mTextBoxAlternateName.Text = "";
            mComboPurchaseUnit.Text = "";
            mComboSalesUnit.Text = "";            
            mComboStatus.SelectedIndex = 0;
            mTextBoxProduct.Focus();          
        }

        private void createNewProduct()
        {
            if (mCurrentProduct == null)
            {
                MessageBox.Show("Please Select a Group");
                return;
            }
            mProductID = "";
            mProductType = "BProduct";
            mComboProductGroups.SelectedValue = mCurrentProduct.ProductCode;
            mTextBoxProduct.Text = "";
            mTextBoxAlternateName.Text = "";
            mComboPurchaseUnit.Text = "";
            mComboSalesUnit.Text = "";
            mComboStatus.SelectedIndex = 0;
            mTextBoxProduct.Focus();
        }

        private void deleteFromDatabase()
        {
            if (mProductID != "")
            {
                try
                {
                    using (ChannelFactory<IProduct> ProductProxy = new ChannelFactory<ServerServiceInterface.IProduct>("ProductEndpoint"))
                    {
                        ProductProxy.Open();
                        IProduct ProductService = ProductProxy.CreateChannel();

                        bool success = ProductService.DeleteProductRegister(mCurrentProduct.ProductCode);


                        if (success)
                        {                            
                            cleanTheForm();
                        }else
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
        }

        private void saveDataToDatabase()
        {
            try
            {                
                if (mComboProductGroups.SelectedIndex==-1 && mProductType=="BProduct")
                {
                    MessageBox.Show("Group is not Selected");
                    mComboProductGroups.Focus();
                    return;
                }

                string groupCode = mProductType == "BProduct" ? mComboProductGroups.SelectedValue.ToString() : "";

                if (mTextBoxProduct.Text.Trim().Length==0)
                {
                    MessageBox.Show("Product name is not given");
                    mTextBoxProduct.Focus();
                    return;
                }

                if (mTextBoxAlternateName.Text.Trim().Length == 0)
                {
                    MessageBox.Show("Billing name is not given");
                    mTextBoxAlternateName.Focus();
                    return;
                }

                if (mComboPurchaseUnit.SelectedIndex == -1 && mProductType == "BProduct")
                {
                    MessageBox.Show("Purchase unit is not selected");
                    mComboPurchaseUnit.Focus();
                    return;
                }

                if (mComboSalesUnit.SelectedIndex == -1 && mProductType == "BProduct")
                {
                    MessageBox.Show("Sales unit is not selected");
                    mComboSalesUnit.Focus();
                    return;
                }

                using (ChannelFactory<IProduct> ProductProxy = new ChannelFactory<ServerServiceInterface.IProduct>("ProductEndpoint"))
                {
                    ProductProxy.Open();
                    IProduct ProductService = ProductProxy.CreateChannel();
                    
                    CProductRegister ccr = new CProductRegister();                    
                    ccr.GroupCode = groupCode;
                    ccr.Product = mTextBoxProduct.Text.Trim();
                    ccr.AlternateName = mTextBoxAlternateName.Text.Trim();
                    ccr.IsEnabled = mComboStatus.Text == "Enabled";
                    ccr.ProductType = mProductType;                    
                    if(mProductType != "AGroup")
                    {
                        ccr.StockInUnitCode = mComboPurchaseUnit.SelectedValue.ToString();
                        ccr.StockOutUnitCode = mComboSalesUnit.SelectedValue.ToString();
                    }
                    else
                    {
                        ccr.StockInUnitCode = "";
                        ccr.StockOutUnitCode = "";
                    }                    

                    bool success = false;
                    if (mProductID != "")
                    {
                        CProduct cl = (mTreeProductRegister.SelectedItem as TreeViewItem).Tag as CProduct;
                        ccr.ProductCode = cl.ProductCode;
                        success = ProductService.UpdateProductRegister(ccr);
                    }
                    else
                    {
                        success = ProductService.CreateProductRegister(ccr);
                    }

                    if (success)
                    {
                        cleanTheForm();
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

        private void searchTextInTree()
        {
            Boolean start = mLastSearchCode=="";
            string searchText = mTextBoxSearch.Text;
            foreach (TreeViewItem item in mTreeProductRegister.Items)
            {                
                if (item != null)
                {                    
                    CProduct obj = (item.Tag as CProduct);
                    if (obj.Product.Contains(searchText)&&(obj.ProductCode== mLastSearchCode||start))
                    {
                        if (start)
                        {
                            mLastSearchCode = obj.ProductCode;
                            item.IsSelected = true;
                            return;
                        }
                        else
                        {
                            start = true;
                        }
                        
                    }

                    item.IsExpanded = true;
                    if(searchTestInNodes(item, searchText,ref start))
                    {
                        return;
                    }
                    
                    
                }
                
            }
            mLastSearchCode = "";
            MessageBox.Show("No More Items Found!","Search '"+searchText+"'");
        }
        private bool searchTestInNodes(TreeViewItem items, string searchText,ref Boolean start)
        {            
            foreach (TreeViewItem item in items.Items)
            {
                
                if (item != null)
                {                    
                    CProduct obj = (item.Tag as CProduct);
                    if (obj.Product.Contains(searchText) && (obj.ProductCode == mLastSearchCode || start))
                    {
                        if (start)
                        {
                            mLastSearchCode = obj.ProductCode;
                            item.IsSelected = true;
                            return true;
                        }
                        else
                        {
                            start = true;
                        }

                    }

                }                
            }

            return false;
        }

        private void mButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void mTreeProductRegister_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            selectDataToEditBoxes(((sender as TreeView).SelectedItem as TreeViewItem).Tag as CProduct);
        }

        private void mButtonAddGroup_Click(object sender, RoutedEventArgs e)
        {
            createNewGroup();
        }

        private void mButtonAddItem_Click(object sender, RoutedEventArgs e)
        {
            createNewProduct();
        }

        private void mButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            deleteFromDatabase();
        }

        private void mButtonSave_Click(object sender, RoutedEventArgs e)
        {
            saveDataToDatabase();
        }

        private void mButtonFindNext_Click(object sender, RoutedEventArgs e)
        {
            searchTextInTree();
        }

        private void mTextBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            mLastSearchCode = "";
        }

        private void mComboPurchaseUnit_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.F2)
            {
                UnitRegister ur = new UnitRegister();
                ur.mPR = this;
                ur.Show();                
            }
        }

        private void mComboPurchaseUnit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            getUnitsForSales();
        }

    }
}
