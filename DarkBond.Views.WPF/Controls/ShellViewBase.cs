// <copyright file="ShellViewBase.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2017 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.Views.Controls
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Windows;
    using System.Windows.Data;
    using DarkBond.Views.Properties;
    using Fluent;

    /// <summary>
    /// Used to provide a shell view around an application.
    /// </summary>
    public class ShellViewBase : Window, IRibbonWindow
    {
        /// <summary>
        /// A value indicating whether the application is in deign mode.
        /// </summary>
        private bool isInDesignMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewBase"/> class.
        /// </summary>
        public ShellViewBase()
        {
            // This keeps the content sized to the container.
            this.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            this.VerticalContentAlignment = VerticalAlignment.Stretch;

            // This will attach the settings to the current process of the domain and watch for the main process to exit.  The general idea is to
            // automatically save the settings when the application that called the settings has terminated.  It should be noted that the typical
            // 'Application.OnExit' can't be used because settings may be accessed in application constructors.
            AppDomain.CurrentDomain.ProcessExit += this.OnProcessExit;

            // Determine if the shell is being constructed in the design mode or normal operation.
            this.isInDesignMode = DesignerProperties.GetIsInDesignMode(this);

            // Older versions of Visual Studio do not like to have the boundaries of the window messed with while it is presenting the object on the
            // design surface.  This allows the ShellViewBase to be managed on the design surface during design time and load the properties from the
            // settings file during run time.
            if (!this.isInDesignMode)
            {
                // Bind the Main Window Height property to the settings so it will persist from one instance to the next.
                Binding shellViewBaseHeightBinding = new Binding();
                shellViewBaseHeightBinding.Mode = BindingMode.TwoWay;
                shellViewBaseHeightBinding.Path = new PropertyPath("ShellHeight");
                shellViewBaseHeightBinding.Source = Settings.Default;
                BindingOperations.SetBinding(this, ShellViewBase.HeightProperty, shellViewBaseHeightBinding);

                // Bind the Main Window Left property to the settings so it will persist from one instance to the next.
                Binding shellViewBaseLeftBinding = new Binding();
                shellViewBaseLeftBinding.Mode = BindingMode.TwoWay;
                shellViewBaseLeftBinding.Path = new PropertyPath("ShellLeft");
                shellViewBaseLeftBinding.Source = Settings.Default;
                BindingOperations.SetBinding(this, ShellViewBase.LeftProperty, shellViewBaseLeftBinding);

                // Bind the Main Window Top property to the settings so it will persist from one instance to the next.
                Binding shellViewBaseTopBinding = new Binding();
                shellViewBaseTopBinding.Mode = BindingMode.TwoWay;
                shellViewBaseTopBinding.Path = new PropertyPath("ShellTop");
                shellViewBaseTopBinding.Source = Settings.Default;
                BindingOperations.SetBinding(this, ShellViewBase.TopProperty, shellViewBaseTopBinding);

                // Bind the Main Window Width property to the settings so it will persist from one instance to the next.
                Binding shellViewBaseWidthBinding = new Binding();
                shellViewBaseWidthBinding.Mode = BindingMode.TwoWay;
                shellViewBaseWidthBinding.Path = new PropertyPath("ShellWidth");
                shellViewBaseWidthBinding.Source = Settings.Default;
                BindingOperations.SetBinding(this, ShellViewBase.WidthProperty, shellViewBaseWidthBinding);

                // Bind the Main Window WindowState property to the settings so it will persist from one instance to the next.
                Binding shellViewBaseWindowStateBinding = new Binding();
                shellViewBaseWindowStateBinding.Mode = BindingMode.TwoWay;
                shellViewBaseWindowStateBinding.Path = new PropertyPath("ShellWindowState");
                shellViewBaseWindowStateBinding.Source = Settings.Default;
                BindingOperations.SetBinding(this, ShellViewBase.WindowStateProperty, shellViewBaseWindowStateBinding);
            }
        }

        /// <inheritdoc/>
        public RibbonTitleBar TitleBar
        {
            get
            {
                // There is no support for a title bar but this prevents binding failure messages.
                return null;
            }
        }

        /// <summary>
        /// Resets the settings to their default values.
        /// </summary>
        protected static void ResetSettings()
        {
            // This will restore all the user preferences to the factory defaults.  Note that there is a bug with the 'Settings.Reset' function that
            // prevents it from being effective at restoring values that are bound to other properties.  It will leave some bound values such as the
            // Height and Top unchanged when the Width or Left property has changed.  Restoring the defaults manually here appears to avoid those
            // problems.
            foreach (SettingsProperty settingsProperty in Settings.Default.Properties)
            {
                TypeConverter typeConverter = TypeDescriptor.GetConverter(settingsProperty.PropertyType);
                Settings.Default[settingsProperty.Name] = typeConverter.ConvertFromInvariantString(settingsProperty.DefaultValue as string);
            }
        }

        /// <summary>
        /// Invoked when the main process of the calling application domain has exited.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="eventArgs">The event arguments.</param>
        private void OnProcessExit(object sender, EventArgs eventArgs)
        {
            // Just to be wholesome about this, we make sure that this routine is not available after the process has exited and before the garbage
            // collection has reclaimed it.
            AppDomain.CurrentDomain.ProcessExit -= this.OnProcessExit;

            // The Visual Studio designer doesn't like us messing around with the frame's dimensions or the settings file during design time.
            if (!this.isInDesignMode)
            {
                Settings.Default.Save();
            }
        }
    }
}