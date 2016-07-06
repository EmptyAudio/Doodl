//-----------------------------------------------------------------------
// <copyright file="StrokeUndoManager.cs" company="EmptyAudio">
//     Copyright EmptyAudio (c) 2016
// </copyright>
//-----------------------------------------------------------------------

namespace Doodl.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Ink;

    /// <summary>
    /// Provides undo and redo functionality for a <see cref="StrokeCollection"/>.
    /// </summary>
    public class StrokeUndoManager : IUndoManager
    {
        private readonly StrokeCollection strokes;

        private Stack<UndoOperation> undoStack = new Stack<UndoOperation>();
        private Stack<UndoOperation> redoStack = new Stack<UndoOperation>();
        private bool undoing;
        private bool inOperation;
        private List<UndoRecord> inProgress = new List<UndoRecord>();

        /// <summary>
        /// Initializes a new instance of the <see cref="StrokeUndoManager"/> class.
        /// </summary>
        /// <param name="strokes">The strokes collection to bind to.</param>
        public StrokeUndoManager(StrokeCollection strokes)
        {
            this.strokes = strokes;

            this.strokes.StrokesChanged += this.StrokesChanged;
        }

        /// <summary>
        /// Raised when the undo or redo state has changed.
        /// </summary>
        public event EventHandler UndoStateChanged;

        /// <summary>
        /// Gets a value indicating whether or not there is anything to undo.
        /// </summary>
        public bool CanUndo
        {
            get
            {
                return this.undoStack.Count != 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not there is anything to redo.
        /// </summary>
        public bool CanRedo
        {
            get
            {
                return this.redoStack.Count != 0;
            }
        }

        /// <summary>
        /// Undoes the most recent operation.
        /// </summary>
        public void Undo()
        {
            this.undoing = true;

            this.redoStack.Push(this.undoStack.Pop());

            this.redoStack.Peek().Undo(this.strokes);

            this.undoing = false;

            this.OnUndoStateChanged();
        }

        /// <summary>
        /// Redoes the most recent operation.
        /// </summary>
        public void Redo()
        {
            this.undoing = true;

            this.undoStack.Push(this.redoStack.Pop());

            this.undoStack.Peek().Redo(this.strokes);

            this.undoing = false;

            this.OnUndoStateChanged();
        }

        /// <summary>
        /// Suspends adding new operations to the undo stack, but still collects operations.
        /// </summary>
        public void BeginOperation()
        {
            this.inOperation = true;
        }

        /// <summary>
        /// Adds all collected operations to the undo stack as a single operation, then resumes adding new operations.
        /// </summary>
        public void EndOperation()
        {
            this.AddInProgressToUndo();

            this.inOperation = false;
        }

        /// <summary>
        /// Clears the undo and redo stacks.
        /// </summary>
        public void Clear()
        {
            this.undoStack.Clear();
            this.redoStack.Clear();

            this.inProgress.Clear();

            this.OnUndoStateChanged();
        }

        /// <summary>
        /// Raises the <see cref="UndoStateChanged"/> event.
        /// </summary>
        protected virtual void OnUndoStateChanged()
        {
            this.UndoStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            if (!this.undoing)
            {
                this.inProgress.Add(new UndoRecord(e.Added, e.Removed));

                if (!this.inOperation)
                {
                    this.AddInProgressToUndo();
                }
            }
        }

        private void AddInProgressToUndo()
        {
            if (this.inProgress.Count > 0)
            {
                this.redoStack.Clear();
                this.undoStack.Push(new UndoOperation(this.inProgress));

                this.inProgress.Clear();

                this.OnUndoStateChanged();
            }
        }

        private class UndoOperation
        {
            private List<UndoRecord> records;

            public UndoOperation(IEnumerable<UndoRecord> records)
            {
                this.records = records.ToList();
            }

            public void Undo(StrokeCollection strokes)
            {
                foreach (var record in this.records.Reverse<UndoRecord>())
                {
                    record.Undo(strokes);
                }
            }

            public void Redo(StrokeCollection strokes)
            {
                foreach (var record in this.records)
                {
                    record.Redo(strokes);
                }
            }
        }

        private class UndoRecord
        {
            private StrokeCollection added;
            private StrokeCollection removed;

            public UndoRecord(StrokeCollection added, StrokeCollection removed)
            {
                this.added = added;
                this.removed = removed;
            }

            public void Undo(StrokeCollection strokes)
            {
                strokes.Remove(this.added);
                strokes.Add(this.removed);
            }

            public void Redo(StrokeCollection strokes)
            {
                strokes.Remove(this.removed);
                strokes.Add(this.added);
            }
        }
    }
}
