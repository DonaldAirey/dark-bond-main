// <copyright file="ColumnViewModel.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents a column that displays data.
    /// </summary>
    public class ColumnViewModel : ViewModel
    {
        /// <summary>
        /// The default width of a column.
        /// </summary>
        private const double DefaultMinWidth = 80.0;

        /// <summary>
        /// The actual width of the column.
        /// </summary>
        private double actualWidthField = double.NaN;

        /// <summary>
        /// The member in the data context to which the column is bound.
        /// </summary>
        private string displayMemberField;

        /// <summary>
        /// The template for the cell.
        /// </summary>
        private object cellTemplateField;

        /// <summary>
        /// The description of the column.
        /// </summary>
        private string descriptionField;

        /// <summary>
        /// The list of filters use to select the rows in the view.
        /// </summary>
        private ObservableCollection<FilterItemViewModel> filterItemViewModelCollection = new ObservableCollection<FilterItemViewModel>();

        /// <summary>
        /// An indication that the column has filters.
        /// </summary>
        private bool hasFiltersField;

        /// <summary>
        /// The column header.
        /// </summary>
        private object headerField;

        /// <summary>
        /// The horizontal alignment of the header.
        /// </summary>
        private object headerHorizontalAlignmentField = DarkBond.ViewModels.HorizontalAlignment.Left;

        /// <summary>
        /// The template for the header.
        /// </summary>
        private object headerTemplateField;

        /// <summary>
        /// The vertical alignment of the header.
        /// </summary>
        private object headerVerticalAlignmentField = DarkBond.ViewModels.VerticalAlignment.Top;

        /// <summary>
        /// The horizontal alignment of the column.
        /// </summary>
        private object horizontalAlignmentField = DarkBond.ViewModels.HorizontalAlignment.Left;

        /// <summary>
        /// An indication of whether the field is visible.
        /// </summary>
        private bool isVisibleField = true;

        /// <summary>
        /// The left edge of the header on the canvas.
        /// </summary>
        private double leftField;

        /// <summary>
        /// The sort direction.
        /// </summary>
        private SortDirection sortDirectionField = SortDirection.NotSorted;

        /// <summary>
        /// The path to the field used for sorting.
        /// </summary>
        private string sortPathField;

        /// <summary>
        /// The width of the field.
        /// </summary>
        private double widthField = double.NaN;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnViewModel"/> class.
        /// </summary>
        public ColumnViewModel()
        {
            // In order to keep the 'HasFilters' property reconciled to the list of filters, we need to monitor the collection for changes.
            this.filterItemViewModelCollection.CollectionChanged += (s, e) =>
            {
                this.HasFilters = this.filterItemViewModelCollection.Count != 0;
            };
        }

        /// <summary>
        /// Gets or sets the actual width of the column.
        /// </summary>
        public double ActualWidth
        {
            get
            {
                return this.actualWidthField;
            }

            set
            {
                if (this.actualWidthField != value)
                {
                    this.actualWidthField = value;
                    this.OnPropertyChanged("ActualWidth");
                }
            }
        }

        /// <summary>
        /// Gets or sets the template used by the cell in the column view.
        /// </summary>
        public object CellTemplate
        {
            get
            {
                return this.cellTemplateField;
            }

            set
            {
                if (this.cellTemplateField != value)
                {
                    this.cellTemplateField = value;
                    this.OnPropertyChanged("CellTemplate");
                }
            }
        }

        /// <summary>
        /// Gets or sets the description of the column.
        /// </summary>
        public string Description
        {
            get
            {
                return this.descriptionField;
            }

            set
            {
                if (this.descriptionField != value)
                {
                    this.descriptionField = value;
                    this.OnPropertyChanged("Description");
                }
            }
        }

        /// <summary>
        /// Gets or sets the data item to bind to for this column.
        /// </summary>
        public string DisplayMember
        {
            get
            {
                return this.displayMemberField;
            }

            set
            {
                if (this.displayMemberField != value)
                {
                    this.displayMemberField = value;
                    this.OnPropertyChanged("DisplayMember");
                }
            }
        }

        /// <summary>
        /// Gets the collection of FilterItems use to select the rows that will appear in the view.
        /// </summary>
        public ObservableCollection<FilterItemViewModel> Filters
        {
            get
            {
                return this.filterItemViewModelCollection;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the column contains filters.
        /// </summary>
        public bool HasFilters
        {
            get
            {
                return this.hasFiltersField;
            }

            set
            {
                if (this.hasFiltersField != value)
                {
                    this.hasFiltersField = value;
                    this.OnPropertyChanged("HasFilters");
                }
            }
        }

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        public object Header
        {
            get
            {
                return this.headerField;
            }

            set
            {
                if (this.headerField != value)
                {
                    this.headerField = value;
                    this.OnPropertyChanged("Header");
                }
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment used by the header.
        /// </summary>
        public object HeaderHorizontalAlignment
        {
            get
            {
                return this.headerHorizontalAlignmentField;
            }

            set
            {
                if (this.headerHorizontalAlignmentField != value)
                {
                    this.headerHorizontalAlignmentField = value;
                    this.OnPropertyChanged("HeaderHorizontalAlignment");
                }
            }
        }

        /// <summary>
        /// Gets or sets the template for the header.
        /// </summary>
        public object HeaderTemplate
        {
            get
            {
                return this.headerTemplateField;
            }

            set
            {
                if (this.headerTemplateField != value)
                {
                    this.headerTemplateField = value;
                    this.OnPropertyChanged("HeaderTemplate");
                }
            }
        }

        /// <summary>
        /// Gets or sets the vertical alignment used by the header.
        /// </summary>
        public object HeaderVerticalAlignment
        {
            get
            {
                return this.headerVerticalAlignmentField;
            }

            set
            {
                if (this.headerVerticalAlignmentField != value)
                {
                    this.headerVerticalAlignmentField = value;
                    this.OnPropertyChanged("HeaderVerticalAlignment");
                }
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment of the column content.
        /// </summary>
        public object HorizontalAlignment
        {
            get
            {
                return this.horizontalAlignmentField;
            }

            set
            {
                if (this.horizontalAlignmentField != value)
                {
                    this.horizontalAlignmentField = value;
                    this.OnPropertyChanged("HorizontalAlignment");
                }
            }
        }

        /// <summary>
        /// Gets or sets the left of the column.
        /// </summary>
        public double Left
        {
            get
            {
                return this.leftField;
            }

            set
            {
                if (this.leftField != value)
                {
                    this.leftField = value;
                    this.OnPropertyChanged("Left");
                }
            }
        }

        /// <summary>
        /// Gets or sets the sort direction.
        /// </summary>
        public SortDirection SortDirection
        {
            get
            {
                return this.sortDirectionField;
            }

            set
            {
                if (this.sortDirectionField != value)
                {
                    this.sortDirectionField = value;
                    this.OnPropertyChanged("SortDirection");
                }
            }
        }

        /// <summary>
        /// Gets or sets the path to the field used to sort the column.
        /// </summary>
        public string SortPath
        {
            get
            {
                return this.sortPathField;
            }

            set
            {
                if (this.sortPathField != value)
                {
                    this.sortPathField = value;
                    this.OnPropertyChanged("SortPath");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the column is visible.
        /// </summary>
        public bool IsVisible
        {
            get
            {
                return this.isVisibleField;
            }

            set
            {
                if (this.isVisibleField != value)
                {
                    this.isVisibleField = value;
                    this.OnPropertyChanged("IsVisible");
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the column.
        /// </summary>
        public double Width
        {
            get
            {
                return this.widthField;
            }

            set
            {
                if (this.widthField != value)
                {
                    this.widthField = value;
                    this.OnPropertyChanged("Width");
                }
            }
        }
    }
}