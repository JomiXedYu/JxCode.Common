using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JxCode.Common
{
    public interface ICommand
    {
        void Undo();
        void Execute();
    }
    public class CommandExecuter
    {
        private SolidStack<ICommand> undoStack;
        private SolidStack<ICommand> redoStack;

        private int capicity;
        public int Capicity { get => this.capicity; }

        public int UndoListCount { get => this.undoStack.Count; }
        public int RedoListCount { get => this.redoStack.Count; }

        public CommandExecuter(int capicity)
        {
            this.capicity = capicity;
            this.undoStack = new SolidStack<ICommand>(capicity);
            this.redoStack = new SolidStack<ICommand>(capicity);
        }

        public ICommand Execute(ICommand item)
        {
            if (this.redoStack.Count != 0)
            {
                this.redoStack.Clear();
            }
            item.Execute();
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
            popobj.Execute();
        }
    }
}
