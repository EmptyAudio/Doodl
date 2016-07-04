//-----------------------------------------------------------------------
// <copyright file="IUndoManager.cs" company="EmptyAudio">
//     Copyright EmptyAudio (c) 2016
// </copyright>
//-----------------------------------------------------------------------

namespace Doodl.Model
{
    using System;

    /// <summary>
    /// Provides methods for managing an undo/redo stack.
    /// </summary>
    public interface IUndoManager
    {
        /// <summary>
        /// Raised when the undo or redo state has changed.
        /// </summary>
        event EventHandler UndoStateChanged;

        /// <summary>
        /// Gets a value indicating whether or not there is anything to undo.
        /// </summary>
        bool CanUndo { get; }

        /// <summary>
        /// Gets a value indicating whether or not there is anything to redo.
        /// </summary>
        bool CanRedo { get; }

        /// <summary>
        /// Undoes the most recent operation.
        /// </summary>
        void Undo();

        /// <summary>
        /// Redoes the most recent operation.
        /// </summary>
        void Redo();

        /// <summary>
        /// Suspends adding new operations to the undo stack, but still collects operations.
        /// </summary>
        void BeginOperation();

        /// <summary>
        /// Adds all collected operations to the undo stack as a single operation, then resumes adding new operations.
        /// </summary>
        void EndOperation();

        /// <summary>
        /// Clears the undo and redo stacks.
        /// </summary>
        void Clear();
    }
}
