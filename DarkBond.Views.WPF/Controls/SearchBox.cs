// <copyright file="SearchBox.cs" company="Dark Bond, Inc.">
//    Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Used to search for text in an application
    /// </summary>
    [TemplatePart(Name = SearchBox.WatermarkPartName, Type = typeof(TextBlock))]
    [TemplatePart(Name = SearchBox.TextBoxPartName, Type = typeof(TextBox))]
    public class SearchBox : TextBox
    {
        /// <summary>
        /// Identifies the IsSearching dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSearchingProperty = DependencyProperty.Register(
            "IsSearching",
            typeof(bool),
            typeof(SearchBox),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(SearchBox.OnIsSearchingPropertyChanged)));

        /// <summary>
        /// Identifies the Path dependency property.
        /// </summary>
        public static readonly DependencyProperty PathProperty = DependencyProperty.Register(
            "Path",
            typeof(string),
            typeof(SearchBox),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(SearchBox.OnPathPropertyChanged)));

        /// <summary>
        /// The character used to separate elements of the path.
        /// </summary>
        private const char SeparatorCharacter = '\\';

        /// <summary>
        /// The name of the TextBox part.
        /// </summary>
        private const string TextBoxPartName = "PART_TextBox";

        /// <summary>
        /// The name of the Watermark part.
        /// </summary>
        private const string WatermarkPartName = "PART_Watermark";

        /// <summary>
        /// The control where the search text is entered.
        /// </summary>
        private TextBox textBox;

        /// <summary>
        /// The read-only text displayed in the search box as a prompt.
        /// </summary>
        private TextBlock watermark;

        /// <summary>
        /// Initializes static members of the <see cref="SearchBox"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Reviewed")]
        static SearchBox()
        {
            // This allows the templates in resource files to be properly associated with this new class.  Without this override, the type of the base class would
            // be used as the key in any lookup involving resources dictionaries.
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchBox), new FrameworkPropertyMetadata(typeof(SearchBox)));

            // Even though the SearchBox is a compound control the keyboard navigation is limited.  We only want to pass through it once with the tab
            // keys.  Also note that the outer container is not itself a tab stop because it never wants or gets the keyboard focus.  The Tab Stop is
            // left up to the PART_TextBox to catch.
            KeyboardNavigation.ControlTabNavigationProperty.OverrideMetadata(typeof(SearchBox), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(SearchBox), new FrameworkPropertyMetadata(KeyboardNavigationMode.Once));
            KeyboardNavigation.IsTabStopProperty.OverrideMetadata(typeof(SearchBox), new FrameworkPropertyMetadata(false));

            // This is a complex control and will manage it's own focus scope.
            FocusManager.IsFocusScopeProperty.OverrideMetadata(typeof(SearchBox), new FrameworkPropertyMetadata(true));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchBox"/> class.
        /// </summary>
        public SearchBox()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether a search operation is in progress.
        /// </summary>
        public bool IsSearching
        {
            get
            {
                return (bool)this.GetValue(SearchBox.IsSearchingProperty);
            }

            set
            {
                this.SetValue(SearchBox.IsSearchingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the URI of the currently selected item.
        /// </summary>
        public string Path
        {
            get
            {
                return this.GetValue(SearchBox.PathProperty) as string;
            }

            set
            {
                this.SetValue(SearchBox.PathProperty, value);
            }
        }

        /// <summary>
        /// Called when the template generation for the visual tree is created.
        /// </summary>
        public override void OnApplyTemplate()
        {
            // The text box is where the search text is entered by the user.
            if (this.textBox != null)
            {
                this.textBox.TextChanged -= new TextChangedEventHandler(this.OnTextBoxTextChanged);
            }

            this.textBox = this.GetTemplateChild(SearchBox.TextBoxPartName) as TextBox;
            if (this.textBox != null)
            {
                this.textBox.TextChanged += new TextChangedEventHandler(this.OnTextBoxTextChanged);
            }

            // This is where the watermark for the searching is displayed.  The watermark is the read-only text that describes the scope of the search.
            this.watermark = this.GetTemplateChild(SearchBox.WatermarkPartName) as TextBlock;

            // The Path may have been loaded before the template and therefore had no control into which to deposit the information about the current directory that
            // is going to be searched.  This will insure that the watermark has had a chance to initialize.
            this.UpdateWaterMark();

            // Allow the base class to handle the rest of the initialization.
            base.OnApplyTemplate();
        }

        /// <summary>
        /// Invoked when a GotKeyboardFocus attached event reaches this element in its route.
        /// </summary>
        /// <param name="e">The KeyboardFocusChangedEventArgs that contains the event data.</param>
        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            // Validate parameters
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            // On initialization this will have the effect of moving the keyboard focus to the first element that can accept input.  In the case of most templates
            // this will be the PART_Textbox element.  Allowing the container control to accept the keyboard focus means that the frame that holds a control like
            // this can set the focus to the part it knows about (e.g. the SearchBox) and the search box will handle where the focus goes from there.  Since this
            // control has its own FocusScope, the focus will be remembered after the initial setting and there should be no need to repeat this logic for the
            // rest of this instance's lifetime.
            if (e.NewFocus == this)
            {
                DependencyObject focusScope = FocusManager.GetFocusScope(this);
                IInputElement iInputElement = FocusManager.GetFocusedElement(focusScope);
                if (iInputElement == null)
                {
                    this.MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
                }
            }

            // It's recommended to call the base class after handling this event.
            base.OnGotKeyboardFocus(e);
        }

        /// <summary>
        /// Called when the KeyDown occurs.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            // Validate the parameters.
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            // Pressing the escape key will terminate any pending searches.
            if (e.Key == Key.Escape)
            {
                this.IsSearching = false;
            }

            // The TextBoxBase class still has processing to be done on this event.
            base.OnPreviewKeyDown(e);
        }

        /// <summary>
        /// Invoked when the effective property value of the IsSearching property changes.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">Event data that is issued by any event that tracks changes to the effective value of this property.
        /// </param>
        private static void OnIsSearchingPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // Extract the specific arguments from the generic event arguments.
            SearchBox searchBox = dependencyObject as SearchBox;
            bool isSearching = (bool)dependencyPropertyChangedEventArgs.NewValue;

            // When the searching is canceled the control is restored to a state before the user started entering text for the search.
            if (!isSearching && searchBox.textBox != null)
            {
                searchBox.textBox.Text = string.Empty;
            }
        }

        /// <summary>
        /// Invoked when the effective property value of the Source property changes.
        /// </summary>
        /// <param name="dependencyObject">The DependencyObject on which the property has changed value.</param>
        /// <param name="dependencyPropertyChangedEventArgs">Event data that is issued by any event that tracks changes to the effective value of this property.
        /// </param>
        private static void OnPathPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            // Extract the specific arguments from the generic event arguments.
            SearchBox searchBox = dependencyObject as SearchBox;

            // The watermark shows which part of the hierarchy will be searched and is driven by the current path.
            searchBox.UpdateWaterMark();
        }

        /// <summary>
        /// Occurs when the search is to be halted.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="executedRoutedEventArgs">The event data.</param>
        private void OnCancel(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            // Turn the searching off on this control.  This will erase any text and restore the search icon and the watermark.  Then bubble the event up to any
            // frame controls that might be interested in the fact that we're not trying to search anymore.
            this.IsSearching = false;
        }

        /// <summary>
        /// Handles the TextBoxBase.TextChanged routed event.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="textChangedEventArgs">The event data.</param>
        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            // The main idea of this control is to bubble and event up to frame controls which then coordinate with this control to execute a global search on the
            // application.  Deleting all the text from the search box has the effect of cancelling a search (you can't search for nothing, you'll find it everywhere).  Any change
            // to the search text needs to be immediately transmitted to the frame controls so it can immediately alter the search if one is underway, or begin a
            // search if not.
            if (this.textBox.Text.Length == 0)
            {
                this.IsSearching = false;
            }
            else
            {
                this.IsSearching = true;
            }
        }

        /// <summary>
        /// Updates the watermark with the leaf element of the current path.
        /// </summary>
        private void UpdateWaterMark()
        {
            // Only the leaf part of the path will be displayed in the watermark as an indication of the part of the hierarchy that will be searched.
            // This will format a prompt that will appear as a water mark in the search box and will disappear once the focus is given to the
            // control.
            if (this.watermark != null && this.Path != null)
            {
                string[] pathParts = this.Path.Split(SeparatorCharacter);
                this.watermark.Text = string.Format(
                    CultureInfo.InvariantCulture,
                    Properties.Resources.WaterMarkFormat,
                    Properties.Resources.Search,
                    pathParts[pathParts.Length - 1]);
            }
        }
    }
}