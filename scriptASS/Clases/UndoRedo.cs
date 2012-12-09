using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace scriptASS
{

    public class UndoRedoSubtitleScript : SubtitleScript
    {
        private string actionName;

        public string ActionName
        {
            get { return actionName; }
        }

        public UndoRedoSubtitleScript(SubtitleScript script, string action)
        {
            actionName = action;

            SubtitleScript tmp = (SubtitleScript)script.Clone();
            this.Header = tmp.GetHeader();
            this.Styles = tmp.GetStyles();
            this.Lines = tmp.GetLineArrayList();
            this.FileName = tmp.FileName;

        }
    }

    public class UndoRedo
    {
        private Stack undoStack; 
        private Stack redoStack;

        public int UndoLevels
        {
            get { return undoStack.Count; }
        }

        public int RedoLevels
        {
            get { return redoStack.Count; }
        }

        public UndoRedo()
        {            
            undoStack = new Stack();
            redoStack = new Stack();
        }

        public void AddUndo(SubtitleScript script, string action)
        {
            if (script.IsMorbid) return;

            undoStack.Push(new UndoRedoSubtitleScript(script, action));
            redoStack.Clear();
        }

        public SubtitleScript GetUndo(SubtitleScript orig)
        {
            UndoRedoSubtitleScript tmp = (UndoRedoSubtitleScript)undoStack.Pop();
            redoStack.Push(new UndoRedoSubtitleScript(orig, "Deshacer (" + tmp.ActionName + ")"));
            return (SubtitleScript)tmp;
        }

        public SubtitleScript GetRedo(SubtitleScript orig)
        {
            UndoRedoSubtitleScript tmp = (UndoRedoSubtitleScript)redoStack.Pop();
            undoStack.Push(new UndoRedoSubtitleScript(orig, "Rehacer (" + tmp.ActionName + ")"));
            return (SubtitleScript)tmp;
        }

        // para consultar
        public UndoRedoSubtitleScript[] GetUndoArray()
        {            
            object[] objarray = undoStack.ToArray();
            UndoRedoSubtitleScript[] final = new UndoRedoSubtitleScript[objarray.Length];

            for (int i = 0; i < objarray.Length; i++)
                final[i] = (UndoRedoSubtitleScript)objarray[i];

            return final;
        }

        public UndoRedoSubtitleScript[] GetRedoArray()
        {
            object[] objarray = redoStack.ToArray();
            UndoRedoSubtitleScript[] final = new UndoRedoSubtitleScript[objarray.Length];

            for (int i = 0; i < objarray.Length; i++)
                final[i] = (UndoRedoSubtitleScript)objarray[i];

            return final;
        }

    }
}
