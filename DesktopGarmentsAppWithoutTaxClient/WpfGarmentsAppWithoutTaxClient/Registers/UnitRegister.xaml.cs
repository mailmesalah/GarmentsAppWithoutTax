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
    /// Interaction logic for UnitRegister.xaml
    /// </summary>
    public partial class UnitRegister : Window
    {

        private ObservableCollection<TreeViewItem> mUnitTreeContents = new ObservableCollection<TreeViewItem>() ;
        private string mUnitID;
        private string mUnitType;
        private CUnitRegister mCurrentUnit;
        public ProductRegister mPR = new ProductRegister();

        private static string mLastSearchCode = "";

        public UnitRegister()
        {
            InitializeComponent();
            loadInitialDetails();
        }

        private void loadInitialDetails()
        {            
            getGroupUnits();
            
            cleanTheForm();
        }

        private void getGroupUnits()
        {
            try
            {
                using (ChannelFactory<IUnit> UnitProxy = new ChannelFactory<ServerServiceInterface.IUnit>("UnitEndpoint"))
                {
                    UnitProxy.Open();
                    IUnit UnitService = UnitProxy.CreateChannel();
                    List<CUnit> Units = UnitService.ReadAllGroupUnits();
                    mComboUnitGroups.ItemsSource = Units;
                    mComboUnitGroups.DisplayMemberPath = "Unit";
                    mComboUnitGroups.SelectedValuePath = "UnitCode";
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void getUnitsForTree()
        {
            try
            {
                using (ChannelFactory<IUnit> UnitProxy = new ChannelFactory<ServerServiceInterface.IUnit>("UnitEndpoint"))
                {
                    UnitProxy.Open();
                    IUnit UnitService = UnitProxy.CreateChannel();
                    ObservableCollection<CUnit> Units = UnitService.ReadAllUnitsWithGroupAsTree();
                    mUnitTreeContents= convertCUnitToTreeViewItem(Units);
                    mTreeUnitRegister.ItemsSource = mUnitTreeContents;                                        
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private ObservableCollection<TreeViewItem> convertCUnitToTreeViewItem(ObservableCollection<CUnit> Units)
        {
            ObservableCollection<TreeViewItem> treeContents = new ObservableCollection<TreeViewItem>();
            foreach (var item1 in Units)
            {
                TreeViewItem aTItem = new TreeViewItem();
                foreach (var item2 in item1.MemberList)
                {
                    TreeViewItem bTItem = new TreeViewItem();
             
                    bTItem.Header = item2.Unit;
                    bTItem.Tag = item2;
                    aTItem.Items.Add(bTItem);
                }

                
                aTItem.Header = item1.Unit;
                aTItem.Tag = item1;
                treeContents.Add(aTItem);
            }

            return treeContents;
        }

        private void cleanTheForm()
        {
            mUnitID = "";
            mUnitType = "";
            getUnitsForTree();
            getGroupUnits();
            clearEditBoxes();
            mComboUnitGroups.Focus();
        }

        private void clearEditBoxes()
        {           
            mComboUnitGroups.Text = "";
            mTextBoxUnit.Text = "";
            mTextBoxUnitValue.Text = "";                       
        }

        private void selectDataToEditBoxes(CUnit l)
        {
            try
            {
                if (l != null)
                {
                    using (ChannelFactory<IUnit> UnitProxy = new ChannelFactory<ServerServiceInterface.IUnit>("UnitEndpoint"))
                    {
                        UnitProxy.Open();
                        IUnit UnitService = UnitProxy.CreateChannel();

                        mCurrentUnit = UnitService.ReadUnitRegister(l.UnitCode);

                        if (mCurrentUnit!= null)
                        {
                            mUnitID = mCurrentUnit.Id.ToString();
                            mTextBoxUnit.Text = mCurrentUnit.Unit;
                            mComboUnitGroups.SelectedValue = mCurrentUnit.GroupCode;
                            mTextBoxUnitValue.Text = mCurrentUnit.UnitValue.ToString();                                                        
                            mUnitType = mCurrentUnit.UnitType;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        private void createNewGroup()
        {
            mUnitID = "";
            mUnitType = "AGroup";
            mComboUnitGroups.Text = "";
            mTextBoxUnit.Text = "";
            mTextBoxUnitValue.Text = "";
            mTextBoxUnit.Focus();            
        }

        private void createNewUnit()
        {
            if (mCurrentUnit == null)
            {
                return;
            }

            mUnitID = "";
            mUnitType = "BUnit";
            mComboUnitGroups.SelectedValue = mCurrentUnit.UnitCode;
            mTextBoxUnit.Text = "";
            mTextBoxUnitValue.Text = "";
            mTextBoxUnit.Focus();            
        }

        private void deleteFromDatabase()
        {
            if (mUnitID != "")
            {
                try
                {
                    using (ChannelFactory<IUnit> UnitProxy = new ChannelFactory<ServerServiceInterface.IUnit>("UnitEndpoint"))
                    {
                        UnitProxy.Open();
                        IUnit UnitService = UnitProxy.CreateChannel();

                        bool success = UnitService.DeleteUnitRegister(mCurrentUnit.UnitCode);


                        if (success)
                        {                            
                            cleanTheForm();
                        }else
                        {
                            MessageBox.Show("Deletion Failed");
                        }
                    }
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        private void saveDataToDatabase()
        {
            try
            {
                if (mComboUnitGroups.SelectedIndex==-1 && mUnitType=="BUnit")
                {
                    MessageBox.Show("Basic Unit is Not Selected");
                    mComboUnitGroups.Focus();
                    return;
                }

                if (mTextBoxUnit.Text.Trim().Length==0)
                {
                    MessageBox.Show("Unit name is not given");
                    mTextBoxUnit.Focus();
                    return;
                }

                decimal unitVal = decimal.Parse(mTextBoxUnitValue.Text.Trim());

                using (ChannelFactory<IUnit> UnitProxy = new ChannelFactory<ServerServiceInterface.IUnit>("UnitEndpoint"))
                {
                    UnitProxy.Open();
                    IUnit UnitService = UnitProxy.CreateChannel();
                    
                    CUnitRegister ccr = new CUnitRegister();
                    string groupCode= mUnitType=="AGroup"?"":mComboUnitGroups.SelectedValue.ToString();
                    ccr.GroupCode = groupCode;
                    ccr.Unit = mTextBoxUnit.Text.Trim();
                    ccr.UnitValue = unitVal;                    
                    ccr.UnitType = mUnitType;
                    
                    bool success = false;
                    if (mUnitID != "")
                    {
                        CUnit cl = (mTreeUnitRegister.SelectedItem as TreeViewItem).Tag as CUnit;
                        ccr.UnitCode = cl.UnitCode;
                        success = UnitService.UpdateUnitRegister(ccr);
                    }
                    else
                    {
                        success = UnitService.CreateUnitRegister(ccr);
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
            foreach (TreeViewItem item in mTreeUnitRegister.Items)
            {                
                if (item != null)
                {                    
                    CUnit obj = (item.Tag as CUnit);
                    if (obj.Unit.Contains(searchText)&&(obj.UnitCode== mLastSearchCode||start))
                    {
                        if (start)
                        {
                            mLastSearchCode = obj.UnitCode;
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
                    CUnit obj = (item.Tag as CUnit);
                    if (obj.Unit.Contains(searchText) && (obj.UnitCode == mLastSearchCode || start))
                    {
                        if (start)
                        {
                            mLastSearchCode = obj.UnitCode;
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

        private void mTreeUnitRegister_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            selectDataToEditBoxes(((sender as TreeView).SelectedItem as TreeViewItem).Tag as CUnit);
        }

        private void mButtonAddGroup_Click(object sender, RoutedEventArgs e)
        {
            createNewGroup();
        }

        private void mButtonAddItem_Click(object sender, RoutedEventArgs e)
        {
            createNewUnit();
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

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            mPR.getAllUnits();
        }
    }
}
