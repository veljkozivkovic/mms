using System;
using System.Collections.Generic;

namespace Aplikacija.Core.Imaging
{
    public class UndoRedoManager<T> where T : class
    {
        private readonly List<T> _states = new();
        private int _index = -1;

        // poziva se kad god neki state trajno izbacujemo iz liste (npr. prekidamo redo granu)
        public Action<T> OnDrop { get; set; }

        public bool CanUndo => _index > 0;
        public bool CanRedo => _index >= 0 && _index < _states.Count - 1;

        public void Reset(T initial)
        {
            // očisti stare i oslobodi resurse
            if (_states.Count > 0)
            {
                foreach (var s in _states) OnDrop?.Invoke(s);
                _states.Clear();
            }
            if (initial != null) { _states.Add(initial); _index = 0; }
            else _index = -1;
        }

        public void Push(T state)
        {
            // ako smo u sredini istorije
            if (_index >= 0 && _index < _states.Count - 1)
            {
                for (int i = _index + 1; i < _states.Count; i++)
                    OnDrop?.Invoke(_states[i]);
                _states.RemoveRange(_index + 1, _states.Count - (_index + 1));
            }

            _states.Add(state);
            _index = _states.Count - 1;
        }

        public T Undo()
        {
            if (!CanUndo) return null;
            _index--;
            return _states[_index];
        }

        public T Redo()
        {
            if (!CanRedo) return null;
            _index++;
            return _states[_index];
        }
    }
}
