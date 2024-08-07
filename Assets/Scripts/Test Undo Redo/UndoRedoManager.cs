using UnityEngine;
using System.Collections.Generic;

public class UndoRedoManager : MonoBehaviour
{
    private Stack<Change> undoStack = new Stack<Change>();
    private Stack<Change> redoStack = new Stack<Change>();

    public static UndoRedoManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Register a change to be undone
    public void RegisterChange(Change change)
    {
        undoStack.Push(change);
        redoStack.Clear(); // Clear redo stack whenever a new change is registered
    }

    // Undo the last change
    public void Undo()
    {
        if (undoStack.Count > 0)
        {
            Change change = undoStack.Pop();
            change.Undo();
            redoStack.Push(change);
        }
    }

    // Redo the last undone change
    public void Redo()
    {
        if (redoStack.Count > 0)
        {
            Change change = redoStack.Pop();
            change.Redo();
            undoStack.Push(change);
        }
    }
}

// Interface for changes that can be undone and redone
public interface Change
{
    void Undo();
    void Redo();
}
