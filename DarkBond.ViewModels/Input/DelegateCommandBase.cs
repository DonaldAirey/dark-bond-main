// <copyright file="DelegateCommandBase.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels.Input
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// An <see cref="ICommand"/> whose delegates can be attached for <see cref="Execute"/> and <see cref="CanExecute"/>.
    /// </summary>
    public abstract class DelegateCommandBase : ICommand
    {
        /// <summary>
        /// The method to be executed for the command.
        /// </summary>
        private readonly Func<object, Task> executeMethod;

        /// <summary>
        /// The method that determines if the command can be executed.
        /// </summary>
        private readonly Func<object, bool> canExecuteMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommandBase"/> class.
        /// </summary>
        /// <param name="executeMethod">The <see cref="Action"/> to execute when <see cref="ICommand.Execute"/> is invoked.</param>
        /// <param name="canExecuteMethod">The function to invoked when <see cref="ICommand.CanExecute"/> is invoked.</param>
        protected DelegateCommandBase(Action<object> executeMethod, Func<object, bool> canExecuteMethod)
        {
            if (executeMethod == null || canExecuteMethod == null)
            {
                throw new ArgumentNullException(nameof(executeMethod));
            }

            this.executeMethod = (arg) =>
            {
                executeMethod(arg);
                return Task.Delay(0);
            };
            this.canExecuteMethod = canExecuteMethod;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommandBase"/> class.
        /// </summary>
        /// <param name="executeMethod">The function to execute when <see cref="ICommand.Execute"/> is invoked.</param>
        /// <param name="canExecuteMethod">The function to invoked when <see cref="ICommand.CanExecute"/> is invoked.</param>
        protected DelegateCommandBase(Func<object, Task> executeMethod, Func<object, bool> canExecuteMethod)
        {
            if (executeMethod == null || canExecuteMethod == null)
            {
                throw new ArgumentNullException(nameof(executeMethod));
            }

            // Initialize the object.
            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        /// <summary>
        /// Occurs when changes happen that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        async void ICommand.Execute(object parameter)
        {
            await this.Execute(parameter);
        }

        /// <summary>
        /// Determines if the command can execute.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <returns>True if the command can execute.</returns>
        bool ICommand.CanExecute(object parameter)
        {
            return this.CanExecute(parameter);
        }

        /// <summary>
        /// Raises <see cref="DelegateCommandBase.CanExecuteChanged"/> on the UI thread so every command invoker
        /// can re-query to check if the command can execute.
        /// <remarks>Note that this will trigger the execution of <see cref="DelegateCommandBase.CanExecute"/> once for each invoker.</remarks>
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate", Justification = "Reviewed")]
        public void RaiseCanExecuteChanged()
        {
            this.OnCanExecuteChanged();
        }

        /// <summary>
        /// Determines if the command can execute with the provided parameter by invoking the function supplied during construction.
        /// </summary>
        /// <param name="parameter">The parameter to use when determining if this command can execute.</param>
        /// <returns>Returns <see langword="true"/> if the command can execute.  <see langword="False"/> otherwise.</returns>
        protected bool CanExecute(object parameter)
        {
            return this.canExecuteMethod == null || this.canExecuteMethod(parameter);
        }

        /// <summary>
        /// Executes the command with the provided parameter by invoking the action supplied during construction.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <returns>An async task handle.</returns>
        protected async Task Execute(object parameter)
        {
            await this.executeMethod(parameter);
        }

        /// <summary>
        /// Raises <see cref="ICommand.CanExecuteChanged"/> on the UI thread so every
        /// command invoker can re-query <see cref="ICommand.CanExecute"/>.
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            var handlers = this.CanExecuteChanged;
            if (handlers != null)
            {
                handlers(this, EventArgs.Empty);
            }
        }
    }
}