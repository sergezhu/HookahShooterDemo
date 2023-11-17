namespace _Game._Scripts.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public interface ICoroutineRunner
    {
        Coroutine Run(IEnumerator coroutine, int id);
        void Stop(Coroutine routine);
        void Stop(int id);
    }
    
    public class CoroutineRunner: MonoBehaviour, ICoroutineRunner
    {
        private List<(int, Coroutine)> _coroutines;
        
        public Coroutine Run(IEnumerator routine, int id)
        {
            if (routine == null)
                throw new NullReferenceException();
            
            if (_coroutines == null)
                _coroutines = new List<(int, Coroutine)>();

            CleanNullrefRoutines();

            var runRoutines = _coroutines
                .Where(tuple => tuple.Item1 == id)
                .ToList();

            runRoutines.ForEach( tuple => StopCoroutine(tuple.Item2));
            
            var coroutine = StartCoroutine(routine);
            _coroutines.Add((id, coroutine));

            return coroutine;
        }

        public void Stop(Coroutine routine)
        {
            if ( routine == null )
                return;
            
            if (_coroutines == null)
            {
                _coroutines = new List<(int, Coroutine)>();
                return;
            }

            CleanNullrefRoutines();
            
            var runRoutines = _coroutines
                .Where(tuple => tuple.Item2 == routine)
                .ToList();

            if (runRoutines.Count == 0)
            {
                Debug.LogWarning($"You tried stop coroutine but it is non be contains in list");
                return;
            }

            StopCoroutine(routine);
        }

        private void CleanNullrefRoutines()
        {
            _coroutines = _coroutines
                .Where( tuple => tuple.Item2 != null )
                .ToList();
        }

        public void Stop(int id)
        {
            if (_coroutines == null)
            {
                _coroutines = new List<(int, Coroutine)>();
                return;
            }

            var runRoutines = _coroutines
                .Where(tuple => tuple.Item1 == id && tuple.Item2 != null)
                .ToList();

            if (runRoutines.Count == 0)
            {
                Debug.LogWarning($"You tried stop coroutine but it is non be contains in list");
                return;
            }

            runRoutines.ForEach( tuple => StopCoroutine(tuple.Item2));
        }
    }
}