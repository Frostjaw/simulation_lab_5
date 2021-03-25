using System;
using System.Collections.Generic;

namespace simulation_lab_5
{
    public class ContinuousMarkovProcessGenerator
    {
        private readonly Random _baseRng;
        private readonly DiscreteRandomVariableGenerator _discreteRng;
        private readonly double[][] _transitionRateMatrix;
        private double _currentTime;
        private WeatherState _currentState;
        private Dictionary<WeatherState, double> _stateDurations;

        public ContinuousMarkovProcessGenerator(double[][] transitionRateMatrix, WeatherState weatherState)
        {
            _baseRng = new Random();
            _discreteRng = new DiscreteRandomVariableGenerator();
            _transitionRateMatrix = transitionRateMatrix;
            _currentTime = 0;
            _currentState = weatherState;
        }

        public void SetState(WeatherState state)
        {
            _currentState = state;
        }

        public (double, int) GetNextState()
        {
            var teta = Math.Log(_baseRng.NextDouble()) / _transitionRateMatrix[(int)_currentState][(int)_currentState];

            _currentTime += teta;

            var probMassFunc = new double[_transitionRateMatrix.Length];
            for (var i = 0; i < _transitionRateMatrix.Length; i++)
            {
                if (i == (int)_currentState)
                {
                    probMassFunc[i] = 0;

                    continue;
                }

                probMassFunc[i] = -_transitionRateMatrix[(int)_currentState][i] / _transitionRateMatrix[(int)_currentState][(int)_currentState];
            }

            return (teta, _discreteRng.GetRandomNumber(probMassFunc));
        }

        public List<(double, int)> SimulateAndGetInfo(double simulateTime)
        {
            var output = new List<(double, int)>()
            {
                (_currentTime, (int)_currentState)
            };

            while (_currentTime < simulateTime)
            {
                var teta = Math.Log(_baseRng.NextDouble()) / _transitionRateMatrix[(int)_currentState][(int)_currentState];

                _currentTime += teta;

                if (_currentTime >= simulateTime)
                {
                    continue;
                }

                var probMassFunc = new double[_transitionRateMatrix.Length];
                for (var i = 0; i < _transitionRateMatrix.Length; i++)
                {
                    if (i == (int)_currentState)
                    {
                        probMassFunc[i] = 0;

                        continue;
                    }

                    probMassFunc[i] = -_transitionRateMatrix[(int)_currentState][i] / _transitionRateMatrix[(int)_currentState][(int)_currentState];
                }
                _currentState = (WeatherState)_discreteRng.GetRandomNumber(probMassFunc);

                var tupleToAdd = (_currentTime, (int)_currentState);
                output.Add(tupleToAdd);
            }

            return output;
        }

        public List<double> GetStatisticalProcessingInfo(int sampleSize, double timeShift)
        {
            _stateDurations = new Dictionary<WeatherState, double>()
            {
                { WeatherState.Clear, 0d },
                { WeatherState.Cloudy, 0d },
                { WeatherState.Overcast, 0d },
            };

            // wait till stable state
            _ = SimulateAndGetInfo(timeShift);

            var k = 0;
            var sumOfDurations = 0d;

            while ( k < sampleSize)
            {
                var teta = Math.Log(_baseRng.NextDouble()) / _transitionRateMatrix[(int)_currentState][(int)_currentState];

                _currentTime += teta;

                var probMassFunc = new double[_transitionRateMatrix.Length];
                for (var i = 0; i < _transitionRateMatrix.Length; i++)
                {
                    if (i == (int)_currentState)
                    {
                        probMassFunc[i] = 0;

                        continue;
                    }

                    probMassFunc[i] = -_transitionRateMatrix[(int)_currentState][i] / _transitionRateMatrix[(int)_currentState][(int)_currentState];
                }

                _stateDurations[_currentState] += teta;
                sumOfDurations += teta;

                _currentState = (WeatherState)_discreteRng.GetRandomNumber(probMassFunc);

                k++;
            }

            var output = new List<double>();

            foreach (var duration in _stateDurations.Values)
            {
                output.Add(duration / sumOfDurations);
            }

            return output;
        }
    }
}
