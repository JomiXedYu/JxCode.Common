using System;

namespace JxCode.Common
{
    public interface ICommand
    {
        void Undo();
        void Execute();
    }
    public class CommandExecuter
    {
        private readonly SolidStack<ICommand> undoStack;
        private readonly SolidStack<ICommand> redoStack;

        private readonly int capacity;
        public int Capacity => this.capacity;

        public int UndoListCount => this.undoStack.Count;
        public int RedoListCount => this.redoStack.Count;

        public bool IsUndoable => this.undoStack.Count > 0;
        public bool IsRedoable => this.redoStack.Count > 0;

        public CommandExecuter(int capicity)
        {
            this.capacity = capicity;
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

        public void Undo()
        {
            if (this.undoStack.Count == 0)
            {
                throw new IndexOutOfRangeException("undo stack is empty");
            }
            var popobj = this.undoStack.Pop();
            this.redoStack.Push(popobj);
            popobj.Undo();
        }

        public void Redo()
        {
            if (this.redoStack.Count == 0)
            {
                throw new IndexOutOfRangeException("redo stack is empty");
            }
            var popobj = this.redoStack.Pop();
            this.undoStack.Push(popobj);
            popobj.Execute();
        }
    }

}
