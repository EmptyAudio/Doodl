//-----------------------------------------------------------------------
// <copyright file="DelegateCommand.cs" company="EmptyAudio">
//     Copyright EmptyAudio (c) 2016
// </copyright>
//-----------------------------------------------------------------------

namespace Doodl.ViewModel
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// A command that delegates execution to parameters.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="text">The text description of the command.</param>
        /// <param name="execute">The delegate to invoke when the command is executed.</param>
        /// <param name="canExecute">The optional delegate to invoke to determine whether or not the command can be executed.</param>
        public DelegateCommand(string text, Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.Text = text;
            this.execute = execute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Raised when the result of the <see cref="CanExecute"/> method has changed.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Gets a string describing this command.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Executes this command.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        public void Execute(object parameter)
        {
            this.execute(parameter);
        }

        /// <summary>
        /// Determines if this command can be executed.
        /// </summary>
        /// <param name="parameter">The command parameter.</param>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c>.</returns>
        public bool CanExecute(object parameter)
        {
            return this.canExecute?.Invoke(parameter) ?? true;
        }

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
