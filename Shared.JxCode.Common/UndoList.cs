using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JxCode.Common
{
    public interface IUndoable
    {
        void Undo();
        void Redo();
    }
    public class UndoList
    {
        private SolidStack<IUndoable> undoStack;
        private SolidStack<IUndoable> redoStack;

        private int capicity;
        public int Capicity { get => this.capicity; }

        public int UndoListCount { get => this.undoStack.Count; }
        public int RedoListCount { get => this.redoStack.Count; }

        public UndoList(int capicity)
        {
            this.capicity = capicity;
            this.undoStack = new SolidStack<IUndoable>(capicity);
            this.redoStack = new SolidStack<IUndoable>(capicity);
        }

        public IUndoable Add(IUndoable item)
        {
            if (this.redoStack.Count != 0)
            {
                this.redoStack.Clear();
            }
            this.undoStack.Push(item);
            return item;
        }
        public bool IsUndoable()
        {
            return this.undoStack.Count > 0;
        }
        public bool IsRedoable()
        {
            return this.redoStack.Count > 0;
        }
        public void Undo()
        {
            if (this.undoStack.Count == 0)
            {
                throw new IndexOutOfRangeException();
            }
            var popobj = this.undoStack.Pop();
            this.redoStack.Push(popobj);
            popobj.Undo();
        }
        public void Redo()
        {
            if (this.redoStack.Count == 0)
            {
                throw new IndexOutOfRangeException();
            }
            var popobj = this.redoStack.Pop();
            this.undoStack.Push(popobj);
            popobj.Redo();
        }
    }
}
