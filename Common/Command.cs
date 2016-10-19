using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Common
{
    public abstract class Command
    {
        static Stack<Command> done = new Stack<Command>();
        static Stack<Command> undone = new Stack<Command>();
        bool executed = false;
        protected bool auto = false;

        public void Execute()
        {
            Execute(false);
        }

        private void Execute(bool isRedo)
        {
            if (executed)
                throw new Exception("This command " + this.ToString() + " is already executed!");
            done.Push(this);
            if (!isRedo)
                undone.Clear();
            executed = true;
            ExecuteInternal();
        }
        private void UnExecute()
        {
            if (!executed)
                throw new Exception("This command " + this.ToString() + " is not executed yet!");
            undone.Push(this);
            executed = false;
            UnExecuteInternal();
        }

        protected abstract void ExecuteInternal();
        protected abstract void UnExecuteInternal();

        public static void Undo()
        {
            if (done.Count == 0)
                return;
            Command comm = done.Pop();
            comm.UnExecute();
            while (comm.auto && done.Count > 0)
            {
                comm = done.Pop();
                comm.UnExecute();
            }
        }

        public static void Redo()
        {
            if (undone.Count == 0)
                return;
            undone.Pop().Execute(true);
            Command comm = null;
            while (undone.Count > 0 && (comm = undone.Peek()).auto)
            {
                comm = undone.Pop();
                comm.Execute(true);
            }
        }

        public static IEnumerable<Command> AllDoneCommands()
        {
            foreach (Command c in done)
                yield return c;
        }

        public static IEnumerable<Command> AllUndoneCommands()
        {
            foreach (Command c in undone)
                yield return c;
        }

        public static void Reset()
        {
            done.Clear();
            undone.Clear();
        }
    }

    public class PropertyChangedCommand : Command
    {
        CommandHandler executor;
        string property;
        object oldVal, newVal;

        public PropertyChangedCommand(CommandHandler executor, string property, object oldValue, object newValue)
        {
            this.executor = executor;
            this.property = property;
            this.oldVal = oldValue;
            this.newVal = newValue;
        }

        protected override void ExecuteInternal()
        {
            executor.ChangeProp(property, newVal);
        }

        protected override void UnExecuteInternal()
        {
            executor.ChangeProp(property, oldVal);
        }

        public override string ToString()
        {
            return "Property '" + property + "' of '" + executor.ToString() + "' changed from '" + oldVal + "' to '" + newVal + "'";
        }
    }

    public class ListChangedCommand : Command
    {
        public enum Type { ElementAdded, ElementRemoved };
        CommandHandler executor;
        string listName;
        object element;
        Type type;

        public ListChangedCommand(CommandHandler executor, string listName, object element, Type type)
        {
            this.executor = executor;
            this.element = element;
            this.listName = listName;
            this.type = type;
        }

        protected override void ExecuteInternal()
        {
            switch (type)
            {
                case Type.ElementAdded:
                    executor.ChangeLst(listName, element, Type.ElementAdded);
                    break;
                case Type.ElementRemoved:
                    executor.ChangeLst(listName, element, Type.ElementRemoved);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        protected override void UnExecuteInternal()
        {
            switch (type)
            {
                case Type.ElementAdded:
                    executor.ChangeLst(listName, element, Type.ElementRemoved);
                    break;
                case Type.ElementRemoved:
                    executor.ChangeLst(listName, element, Type.ElementAdded);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public override string ToString()
        {
            return "Element '" + element + "' " + (type == Type.ElementAdded ? "added" : "removed") + " from '" + listName + "' of '" + executor.ToString() + "'";
        }
    }

    public abstract class CommandHandler : INotifyPropertyChanged
    {
        protected void ExecutePropertyChangedCommand(string property, object oldValue, object newValue)
        {
            (new PropertyChangedCommand(this, property, oldValue, newValue)).Execute();
        }
        protected void ExecuteListChangedCommand(string list, object element, ListChangedCommand.Type type)
        {
            (new ListChangedCommand(this, list, element, type)).Execute();
        }

        INotifyPropertyChanged alternativeSender = null;

        protected void SetAlternativeSender(INotifyPropertyChanged sender)
        {
            alternativeSender = sender;
        }

        protected virtual void HandlePropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        protected abstract void ChangeProperty(string property, object newValue);
        protected abstract void ChangeList(string list, object element, ListChangedCommand.Type type);

        internal void ChangeProp(string property, object newValue)
        {
            ChangeProperty(property, newValue);
            HandlePropertyChanged(property);
        }

        internal void ChangeLst(string list, object element, ListChangedCommand.Type type)
        {
            ChangeList(list, element, type);
            HandlePropertyChanged(list);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
