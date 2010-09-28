using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMP.Reactive.Touch
{
    public struct VelocityAggregator
    {
        private float[] _vx;
        private float[] _vy;
        private float[] _dv;

        public VelocityAggregator(int pInstances)
        {
            _vx = new float[pInstances];
            _vy = new float[pInstances];
            _dv = new float[pInstances];
        }

        public Velocity GetVelocity
        {
            get 
            {
                return new Velocity(_vx.Average(), _vy.Average(), _dv.Average());
            } 
        }

        public Velocity VelocityNow
        {
            set
            {
                for (int i = 0; i < _vx.Length - 1; i++)
                {
                    _vx[i] = _vx[i + 1];
                    _vy[i] = _vy[i + 1];
                    _dv[i] = _dv[i + 1];
                }

                _vx[_vx.Length - 1] = value.VelocityX;
                _vy[_vy.Length - 1] = value.VelocityY;
                _dv[_dv.Length - 1] = value.VelocityD;

            }
        }
    }
}
