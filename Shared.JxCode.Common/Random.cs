using System;
using System.Collections.Generic;
using System.Linq;

namespace JxCode.Common
{
    public class Random
    {
        private static System.Random random = null;
        private static System.Random GetRandom
        {
            get
            {
                if (random == null)
                    random = new System.Random();
                return random;
            }
        }

        /// <summary>
        /// 根据权重随机对象
        /// </summary>
        public static T RandomObject<T>(Dictionary<T, int> rndPair)
        {
            double average = 100.0 / rndPair.Values.Sum();
            double[] rndarray = new double[rndPair.Count];
            double rndCount = 0;
            System.Random rnd = GetRandom;
            int rndnum = rnd.Next(100);

            foreach (KeyValuePair<T, int> item in rndPair)
            {
                rndCount += item.Value * average;
                if(rndnum < rndCount)
                {
                    return item.Key;
                }
            }
            return default;

        }
    }
}
