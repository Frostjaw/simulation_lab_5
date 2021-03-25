using System;

namespace simulation_lab_5
{
    public class DiscreteRandomVariableGenerator
    {
        private readonly Random _rng;

        public DiscreteRandomVariableGenerator()
        {
            _rng = new Random();
        }

        public int GetRandomNumber(double[] probMassFunc)
        {
            var a = _rng.NextDouble();

            for (var k = 0; k < probMassFunc.Length; k++)
            {
                a -= probMassFunc[k];

                if (a > 0)
                {
                    continue;
                }

                return k;
            }

            return default;
        }
    }
}
