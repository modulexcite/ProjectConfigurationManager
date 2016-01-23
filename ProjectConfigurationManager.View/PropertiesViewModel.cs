﻿namespace tomenglertde.ProjectConfigurationManager.View
{
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using DataGridExtensions;

    using tomenglertde.ProjectConfigurationManager.Model;

    using TomsToolbox.Desktop;
    using TomsToolbox.Wpf;
    using TomsToolbox.Wpf.Composition;

    [DisplayName("Properties")]
    [VisualCompositionExport(GlobalId.ShellRegion, Sequence = 2)]
    class PropertiesViewModel : ObservableObject, IComposablePart
    {
        private readonly Solution _solution;

        [ImportingConstructor]
        public PropertiesViewModel(Solution solution)
        {
            Contract.Requires(solution != null);

            _solution = solution;
        }

        public Solution Solution
        {
            get
            {
                Contract.Ensures(Contract.Result<Solution>() != null);

                return _solution;
            }
        }

        public ICommand CopyCommand => new DelegateCommand<DataGrid>(CanCopy, Copy);

        private void Copy(DataGrid dataGrid)
        {
            Contract.Requires(dataGrid != null);
            dataGrid.GetCellSelection().SetClipboardData();
        }

        private bool CanCopy(DataGrid dataGrid)
        {
            if (dataGrid == null)
                return false;

            return dataGrid.HasRectangularCellSelection();
        }

        public ICommand PasteCommand => new DelegateCommand<DataGrid>(CanPaste, Paste);

        private void Paste(DataGrid dataGrid)
        {
            Contract.Requires(dataGrid != null);
            var data = ClipboardHelper.GetClipboardDataAsTable();
            if (data != null)
                dataGrid.PasteCells(data);
        }

        private bool CanPaste(DataGrid dataGrid)
        {
            return Clipboard.ContainsText() && (dataGrid?.SelectedCells?.Any(cell => cell.Column.IsReadOnly) == false);
        }

        public ICommand DeleteCommand => new DelegateCommand<DataGrid>(CanDelete, Delete);

        private void Delete(DataGrid dataGrid)
        {
            Contract.Requires(dataGrid != null);
            Contract.Requires(dataGrid.SelectedCells != null);

            foreach (var cell in dataGrid.SelectedCells)
            {
                var configuration = (ProjectConfiguration)cell.Item;
                var propertyName = (ProjectPropertyName)cell.Column.GetValue(ProperitesColumnsMananger.ProjectConfigurationProperty);
                if (propertyName != null)
                {
                    configuration.DeleteProperty(propertyName.Name);
                }
            }
        }

        private bool CanDelete(DataGrid dataGrid)
        {
            return dataGrid?.SelectedCells?.Any(cell => cell.Column.IsReadOnly) == false;
        }

        [ContractInvariantMethod]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(_solution != null);
        }
    }
}
