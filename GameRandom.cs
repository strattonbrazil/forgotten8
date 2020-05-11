using System;
namespace forgotten.Desktop
{
    public class GameRandom : Random
    {
        private static GameRandom instance;
        private Random rnd;

        private GameRandom()
        {
            rnd = new Random(37);
        }

        public T Choose<T>(T[] array)
        {
            return array[rnd.Next(array.Length)];
        }

        public float nextFloat()
        {
            return (float)rnd.NextDouble();
        }

        public static GameRandom Instance
        {
            get
            {
                if (instance == null)
                    instance = new GameRandom();
                return instance;
            }
        }
    }
}
