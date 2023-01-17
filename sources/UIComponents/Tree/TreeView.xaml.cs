﻿using System.Windows;
using System.Windows.Controls;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Tree
{
    public partial class TreeView : UserControl
    {
        public TreeView()
        {
            InitializeComponent();
        }


        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (this.DataContext is TreeVM treeView) 
            {
                if (e.NewValue is TreeViewItemVM treeViewItemVM)
                {
                    treeView.RaiseSelectedItemChanged(treeViewItemVM);
                }
            }
        }
    }
}