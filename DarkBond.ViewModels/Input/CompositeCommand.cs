// <copyright file="CompositeCommand.cs" company="Dark Bond, Inc.">
//     Copyright © 2016-2018 - Dark Bond, Inc.  All Rights Reserved.
// </copyright>
// <author>Donald Roy Airey</author>
namespace DarkBond.ViewModels.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;

    /// <summary>
    /// A composite command composed of one or more component commands.
    /// </summary>
    public partial class CompositeCommand : ICommand
    {
        /// <summary>
        /// The list of component commands.
        /// </summary>
        private readonly List<ICommand> componentCommands = new List<ICommand>();

        /// <summary>
        /// Invoked when the 'CanExecute' status of the composite command has changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Gets the list of all the registered commands.
        /// </summary>
        public IList<ICommand> RegisteredCommands
        {
            get
            {
                // Gets the list of child commands in a thread-safe way.
                lock (this.componentCommands)
                {
                    return this.componentCommands.ToList();
                }
            }
        }

        /// <summary>
        /// Determines if the command can execute with the given parameter.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <returns>true if all the component commands can execute with the given parameter, false otherwise.</returns>
        public virtual bool CanExecute(object parameter)
        {
            // Query each of the component commands to see if it can execute.
            bool canExecute = true;
            foreach (ICommand command in this.RegisteredCommands)
            {
                if (!command.CanExecute(parameter))
                {
                    canExecute = false;
                    break;
                }
            }

            // Only if all the component commands can execute can this composite command execute.
            return canExecute;
        }

        /// <summary>
        /// Executes all the component commands with the given parameter.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        public virtual void Execute(object parameter)
        {
            // Execute each of the component commands with the given parameter.
            foreach (ICommand command in this.RegisteredCommands)
            {
                command.Execute(parameter);
            }
        }

        /// <summary>
        /// Adds a command to the collection and signs up for the <see cref="ICommand.CanExecuteChanged"/> event of it.
        /// </summary>
        /// <param name="command">The command to register.</param>
        public virtual void RegisterCommand(ICommand command)
        {
            // Validate the command argument.
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (command == this)
            {
                throw new InvalidOperationException("Cannot Register Composite Command In Itself");
            }

            // This will add the command to the list of registered commands in a thread-safe way.
            lock (this.componentCommands)
            {
                // Insure that the same command isn't registered twice.
                if (this.componentCommands.Contains(command))
                {
                    throw new InvalidOperationException("Cannot Register Same Command Twice");
                }

                // The command is now registered.
                this.componentCommands.Add(command);
            }

            // This will relay the status change of every component command to the composite command.
            command.CanExecuteChanged += this.OnCommandCanExecuteChanged;

            // This makes sure that the composite command has been reconciled with all the registered commands.
            this.OnCanExecuteChanged();
        }

        /// <summary>
        /// Removes a command from the collection and removes itself from the <see cref="ICommand.CanExecuteChanged"/> event of it.
        /// </summary>
        /// <param name="command">The command to unregister.</param>
        public virtual void UnregisterCommand(ICommand command)
        {
            // Validate the command argument.
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            // Remove the command in a thread-safe way.
            bool removed;
            lock (this.componentCommands)
            {
                removed = this.componentCommands.Remove(command);
            }

            // If the command was successfully removed, then remove it from being used in the 'CanExuecuteChanged' chain of queries used to determine
            // if this composite command is enabled.  Also update the status of the 'CanExecute' now that the child command has been removed.
            if (removed)
            {
                command.CanExecuteChanged -= this.OnCommandCanExecuteChanged;
                this.OnCanExecuteChanged();
            }
        }

        /// <summary>
        /// Raises <see cref="ICommand.CanExecuteChanged"/> event on every component command.
        /// </summary>
        protected virtual void OnCanExecuteChanged()
        {
            // This event will be broadcast to every component command.
            if (this.CanExecuteChanged != null)
            {
                this.CanExecuteChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// Relays the change in execution state to the composite command.
        /// </summary>
        /// <param name="sender">The object that originated the event.</param>
        /// <param name="eventArgs">The event data.</param>
        private void OnCommandCanExecuteChanged(object sender, EventArgs eventArgs)
        {
            // When any of the component commands has changed its state that change is relayed to the composite command.
            this.OnCanExecuteChanged();
        }
    }
}