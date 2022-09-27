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
    /// </summary>
    public partial class CustomerRegisters : Window
    {

        private ObservableCollection<TreeViewItem> mLedgerTreeContents = new ObservableCollection<TreeViewItem>() ;
        private string mLedgerID;
        private string mLedgerType;
        private CLedgerRegister mCurrentLedger;

        private static string mLastSearchCode = "";

        public CustomerRegisters()
        {
            InitializeComponent();
            loadInitialDetails();
        }

        private void loadInitialDetails()
        {            
            getGroupLedgers();

            mComboStatus.Items.Add("Enabled");
            mComboStatus.Items.Add("Disabled");
            cleanTheForm();
        }

        private void getGroupLedgers()
        {
            try
            {
                using (ChannelFactory<ILedger> ledgerProxy = new ChannelFactory<ServerServiceInterface.ILedger>("LedgerEndpoint"))
                {
                    ledgerProxy.Open();
                    ILedger ledgerService = ledgerProxy.CreateChannel();
                    List<CLedger> ledgers = ledgerService.ReadCustomerGroupCode();
                    mComboLedgerGroups.ItemsSource = ledgers;
                    mComboLedgerGroups.DisplayMemberPath = "Ledger";
                    mComboLedgerGroups.SelectedValuePath = "LedgerCode";
                }
            }
            catch
            {

            }
        }

        private void getLedgersForTree()
        {
            try
            {
                using (ChannelFactory<ILedger> ledgerProxy = new ChannelFactory<ServerServiceInterface.ILedger>("LedgerEndpoint"))
                {
                    ledgerProxy.Open();
                    ILedger ledgerService = ledgerProxy.CreateChannel();
                    List<CLedger> ledgers = ledgerService.ReadCustomerLedgers();
                    mLedgerTreeContents= convertCLedgerToTreeViewItem(ledgers);
                    mTreeLedgerRegisters.ItemsSource = mLedgerTreeContents;                                        
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private ObservableCollection<TreeViewItem> convertCLedgerToTreeViewItem(List<CLedger> ledgers)
        {
            ObservableCollection<TreeViewItem> treeContents = new ObservableCollection<TreeViewItem>();
            foreach (var item1 in ledgers)
            {
                TreeViewItem aTItem = new TreeViewItem();
                foreach (var item2 in item1.MemberList)
                {
                    TreeViewItem bTItem = new TreeViewItem();
                    foreach (var item3 in item2.MemberList)
                    {
                        TreeViewItem cTItem = new TreeViewItem();
                        foreach (var item4 in item3.MemberList)
                        {
                            TreeViewItem dTItem = new TreeViewItem();
                            dTItem.Header = item4.Ledger;
                            dTItem.Tag = item4;
                            cTItem.Items.Add(dTItem);
                        }

                        cTItem.Header = item3.Ledger;
                        cTItem.Tag = item3;
                        bTItem.Items.Add(cTItem);
                    }

                    bTItem.Header = item2.Ledger;
                    bTItem.Tag = item2;
                    aTItem.Items.Add(bTItem);
                }

                
                aTItem.Header = item1.Ledger;
                aTItem.Tag = item1;
                treeContents.Add(aTItem);
            }

            return treeContents;
        }

        private void cleanTheForm()
        {
            mLedgerID = "";
            mLedgerType = "";
            getLedgersForTree();
            clearEditBoxes();
            mComboLedgerGroups.Focus();
        }

        private void clearEditBoxes()
        {           
            mComboLedgerGroups.Text = "";
            mTextBoxLedger.Text = "";
            mTextBoxAlternateName.Text = "";
            mTextBoxAddress1.Text = "";
            mTextBoxAddress2.Text = "";
            mTextBoxAddress3.Text = "";
            mTextBoxDetails1.Text = "";
            mTextBoxDetails2.Text = "";
            mTextBoxDetails3.Text = "";
            mTextBoxDetails4.Text = "";
            mTextBoxDetails5.Text = "";
            mTextBoxDetails6.Text = "";
            mComboStatus.SelectedIndex = 0;
        }

        private void selectDataToEditBoxes(CLedger l)
        {
            try
            {
                if (l != null)
                {
                    using (ChannelFactory<ILedger> LedgerProxy = new ChannelFactory<ServerServiceInterface.ILedger>("LedgerEndpoint"))
                    {
                        LedgerProxy.Open();
                        ILedger LedgerService = LedgerProxy.CreateChannel();

                        mCurrentLedger = LedgerService.ReadLedgerRegister(l.LedgerCode);

                        if (mCurrentLedger!= null)
                        {
                            mLedgerID = mCurrentLedger.Id.ToString();
                            mTextBoxLedger.Text = mCurrentLedger.Ledger;
                            mComboLedgerGroups.SelectedValue = mCurrentLedger.GroupCode;
                            mTextBoxAlternateName.Text = mCurrentLedger.AlternateName;
                            mTextBoxAddress1.Text = mCurrentLedger.Address1;
                            mTextBoxAddress2.Text = mCurrentLedger.Address2;
                            mTextBoxAddress3.Text = mCurrentLedger.Address3;
                            mTextBoxDetails1.Text = mCurrentLedger.Details1;
                            mTextBoxDetails2.Text = mCurrentLedger.Details2;
                            mTextBoxDetails3.Text = mCurrentLedger.Details3;
                            mTextBoxDetails4.Text = mCurrentLedger.Details4;
                            mTextBoxDetails5.Text = mCurrentLedger.Details5;
                            mTextBoxDetails6.Text = mCurrentLedger.Details6;
                            mComboStatus.SelectedIndex = mCurrentLedger.IsEnabled==true ? 0 : 1;
                            mLedgerType = mCurrentLedger.LedgerType;
                        }
                    }
                }
            }
            catch
            {

            }

        }

        private void createNewGroup()
        {
            if (mCurrentLedger == null)
            {
                return;
            }
            if (mCurrentLedger.LedgerType=="AGroup" || mCurrentLedger.LedgerType == "BGroup")
            {
                mLedgerID = "";
                mLedgerType = mCurrentLedger.LedgerType=="AGroup"?"BGroup":"CGroup";
                mComboLedgerGroups.SelectedValue = mCurrentLedger.LedgerCode;
                mTextBoxLedger.Text = "";
                mTextBoxAlternateName.Text = "";
                mTextBoxAddress1.Text = "";
                mTextBoxAddress2.Text = "";
                mTextBoxAddress3.Text = "";
                mTextBoxDetails1.Text = "";
                mTextBoxDetails2.Text = "";
                mTextBoxDetails3.Text = "";
                mTextBoxDetails4.Text = "";
                mTextBoxDetails5.Text = "";
                mTextBoxDetails6.Text = "";
                mComboStatus.SelectedIndex = 0;
                mTextBoxLedger.Focus();
            }
            else
            {
                MessageBox.Show("Please Select the Group under which the new Group need to be created");
            }

        }

        private void createNewLedger()
        {
            mLedgerID = "";
            mLedgerType = "DAccount";
            mComboLedgerGroups.SelectedIndex = 0;
            mTextBoxLedger.Text = "";
            mTextBoxAlternateName.Text = "";
            mTextBoxAddress1.Text = "";
            mTextBoxAddress2.Text = "";
            mTextBoxAddress3.Text = "";
            mTextBoxDetails1.Text = "";
            mTextBoxDetails2.Text = "";
            mTextBoxDetails3.Text = "";
            mTextBoxDetails4.Text = "";
            mTextBoxDetails5.Text = "";
            mTextBoxDetails6.Text = "";
            mComboStatus.SelectedIndex = 0;
            mTextBoxLedger.Focus();
        }

        private void deleteFromDatabase()
        {
            if (mLedgerID != "")
            {
                try
                {
                    using (ChannelFactory<ILedger> ledgerProxy = new ChannelFactory<ServerServiceInterface.ILedger>("LedgerEndpoint"))
                    {
                        ledgerProxy.Open();
                        ILedger ledgerService = ledgerProxy.CreateChannel();

                        bool success = ledgerService.DeleteLedgerRegister(mCurrentLedger.LedgerCode);


                        if (success)
                        {                            
                            cleanTheForm();                            
                        }
                    }
                }
                catch
                {

                }
            }
        }

        private void saveDataToDatabase()
        {
            try
            {
                if (mComboLedgerGroups.SelectedIndex==-1)
                {
                    mComboLedgerGroups.Focus();
                    return;
                }

                if (mTextBoxLedger.Text.Trim().Length==0)
                {
                    mTextBoxLedger.Focus();
                    return;
                }

                using (ChannelFactory<ILedger> LedgerProxy = new ChannelFactory<ServerServiceInterface.ILedger>("LedgerEndpoint"))
                {
                    LedgerProxy.Open();
                    ILedger LedgerService = LedgerProxy.CreateChannel();

                    CLedgerRegister ccr = new CLedgerRegister();
                    string groupCode = mComboLedgerGroups.SelectedValue.ToString();
                    ccr.GroupCode = groupCode;
                    ccr.Ledger = mTextBoxLedger.Text.Trim();
                    ccr.AlternateName = mTextBoxAlternateName.Text.Trim();
                    ccr.Address1 = mTextBoxAddress1.Text.Trim();
                    ccr.Address2 = mTextBoxAddress2.Text.Trim();
                    ccr.Address3 = mTextBoxAddress3.Text.Trim();
                    ccr.Details1 = mTextBoxDetails1.Text.Trim();
                    ccr.Details2 = mTextBoxDetails2.Text.Trim();
                    ccr.Details3 = mTextBoxDetails3.Text.Trim();
                    ccr.Details4 = mTextBoxDetails4.Text.Trim();
                    ccr.Details5 = mTextBoxDetails5.Text.Trim();
                    ccr.Details6 = mTextBoxDetails6.Text.Trim();
                    ccr.IsEnabled = mComboStatus.Text == "Enabled";
                    ccr.LedgerType = mLedgerType;

                    //Account
                    ccr.AGroupCode = LedgerService.ReadAGroupCodeOf(groupCode);
                    ccr.BGroupCode = LedgerService.ReadBGroupCodeOf(groupCode);
                    ccr.CGroupCode = groupCode;

                
                    bool success = false;
                    if (mLedgerID != "")
                    {
                        CLedger cl = (mTreeLedgerRegisters.SelectedItem as TreeViewItem).Tag as CLedger;
                        ccr.LedgerCode = cl.LedgerCode;
                        success = LedgerService.UpdateLedgerRegister(ccr);
                    }
                    else
                    {
                        success = LedgerService.CreateLedgerRegister(ccr);
                    }

                    if (success)
                    {
                        cleanTheForm();
                    }
                }
            }
            catch (Exception e)
            {

            }
        }

        private void searchTextInTree()
        {
            bool start = mLastSearchCode=="";
            string searchText = mTextBoxSearch.Text;
            foreach (TreeViewItem item in mTreeLedgerRegisters.Items)
            {                
                if (item != null)
                {                    
                    CLedger obj = (item.Tag as CLedger);
                    if (obj.Ledger.Contains(searchText)&&(obj.LedgerCode== mLastSearchCode||start))
                    {
                        if (start)
                        {
                            mLastSearchCode = obj.LedgerCode;
                            item.IsSelected = true;
                            return;
                        }
                        else
                        {
                            start = true;
                        }
                        
                    }

                    item.IsExpanded = true;
                    if(searchTestInNodes(item, searchText,start))
                    {
                        return;
                    }
                    
                    
                }
                
            }
            mLastSearchCode = "";
            MessageBox.Show("No More Items Found!","Search '"+searchText+"'");
        }
        private bool searchTestInNodes(TreeViewItem items, string searchText, bool start)
        {            
            foreach (TreeViewItem item in items.Items)
            {
                
                if (item != null)
                {                    
                    CLedger obj = (item.Tag as CLedger);
                    if (obj.Ledger.Contains(searchText) && (obj.LedgerCode == mLastSearchCode || start))
                    {
                        if (start)
                        {
                            mLastSearchCode = obj.LedgerCode;
                            item.IsSelected = true;
                            return true;
                        }
                        else
                        {
                            start = true;
                        }

                    }

                    item.IsExpanded = true;
                    if (searchTestInNodes(item, searchText,start))
                    {
                        return true;
                    }
                }                
            }

            return false;
        }

        private void mButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void mTreeLedgerRegisters_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            selectDataToEditBoxes(((sender as TreeView).SelectedItem as TreeViewItem).Tag as CLedger);
        }        

        private void mButtonAddItem_Click(object sender, RoutedEventArgs e)
        {
            createNewLedger();
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
    }
}
