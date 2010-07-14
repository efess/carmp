using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CarMp.Reactive.Touch
{
    public struct VelocityAggregator
    {
        private float[] _array;

        public VelocityAggregator(int pInstances)
        {
            _array = new float[pInstances];
        }

        public float GetVelocity
        {
            get 
            {
                return _array.Average();
            } 
        }

        public float VelocityNow
        {
            set
            {
                for (int i = 0; i < _array.Length - 1; i++)
                    _array[i] = _array[i + 1];

                _array[_array.Length - 1] = value;

            }
        }
    }
}
