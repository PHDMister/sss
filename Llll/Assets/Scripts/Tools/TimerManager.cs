using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UnityTimer
{
    #region Manager Class (implementation detail, spawned automatically and updates all registered timers)
    public class TimerManager : MonoBehaviour
    {
        private List<Timer> _timers = new List<Timer>();
        // buffer adding timers so we don't edit a collection during iteration
        private List<Timer> _timersToAdd = new List<Timer>();

        public int _timersCount = 0;
        public int _timersToAddCount = 0;

        public void RegisterTimer(Timer timer)
        {
            this._timersToAdd.Add(timer);
        }

        public void CancelAllTimers()
        {
            foreach (Timer timer in this._timers)
            {
                timer.Cancel();
            }

            this._timers = new List<Timer>();
            this._timersToAdd = new List<Timer>();
        }

        public void PauseAllTimers()
        {
            foreach (Timer timer in this._timers)
            {
                timer.Pause();
            }
        }

        public void ResumeAllTimers()
        {
            foreach (Timer timer in this._timers)
            {
                timer.Resume();
            }
        }

        // update all the registered timers on every frame
        private void Update()
        {
            this.UpdateAllTimers();
            _timersCount = _timers.Count;
            _timersToAddCount = _timersToAdd.Count;
        }

        private void UpdateAllTimers()
        {
            if (this._timersToAdd.Count > 0)
            {
                this._timers.AddRange(this._timersToAdd);
                this._timersToAdd.Clear();
            }

            foreach (Timer timer in this._timers)
            {
                timer.Update();
            }

            this._timers.RemoveAll(t => t.isDone);
        }
    }

    #endregion
}
