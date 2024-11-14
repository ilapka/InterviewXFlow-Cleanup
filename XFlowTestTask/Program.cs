namespace Cleanup
{
    internal class Program
    {
        private const double TargetChangeTime = 1;

        private double _previousTargetSetTime;
        private bool _isTargetSet;
        
        private dynamic _lockedCandidateTarget;
        private dynamic _lockedTarget;
        private dynamic _target;
        private dynamic _previousTarget;
        private dynamic _activeTarget;
        private dynamic _targetInRangeContainer;

        public void CleanupTest(Frame frame)
        {
            try
            {
                CheckLockedTarget(_lockedCandidateTarget);
                CheckLockedTarget(_lockedTarget);

                _isTargetSet = false;

                // Sets _activeTarget field
                TrySetActiveTargetFromQuantum(frame);

                _isTargetSet = CanKeepTarget();
                _previousTarget = _target;

                if(_isTargetSet) return;
                
                if (TrySetTarget(_lockedTarget)) return;
                if (TrySetTarget(_activeTarget)) return;
                
                TrySetTargetFromContainer(_targetInRangeContainer);
            }
            finally
            {
                ApplyTarget();
            }
        }

        private void CheckLockedTarget(dynamic lockedTarget)
        {
            if (lockedTarget && !lockedTarget.CanBeTarget)
            {
                lockedTarget = null;
            }
        }

        /// <summary>
        /// If target exists and can be targeted, it should stay within Target Change Time since last target change
        /// </summary>
        /// <returns></returns>
        private bool CanKeepTarget() => _target && _target.CanBeTarget && IsWithinTimeSinceLastTargetChange();
        private bool IsWithinTimeSinceLastTargetChange() => Time.time - _previousTargetSetTime < TargetChangeTime;

        private bool TrySetTarget(dynamic fromTarget)
        {
            if (!fromTarget || !fromTarget.CanBeTarget) 
                return false;
            
            _target = fromTarget;
            _isTargetSet = true;
            return true;

        }

        private void TrySetTargetFromContainer(dynamic targetInRangeContainer)
        {
            _target = targetInRangeContainer.GetTarget();

            if (_target)
            {
                _isTargetSet = true;
            }
        }

        private void ApplyTarget()
        {
            if (_isTargetSet)
            {
                SavePreviousTargetTime();
            }
            else
            {
                _target = null;
            }

            TargetableEntity.Selected = _target;
        }

        private void SavePreviousTargetTime()
        {
            if (_previousTarget != _target)
            {
                _previousTargetSetTime = Time.time;
            }
        }
    }
}