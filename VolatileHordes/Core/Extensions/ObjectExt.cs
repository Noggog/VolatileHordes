using System;

namespace VolatileHordes.Utility
{
    
    public static class ObjectExt
    {
        /**
         * Scoping function which allow you to more easily use a variable in a scope.
         */
        public static T Also<T>(this T t, Action<T> action)
        {
            action(t);
            return t;
        }
        /**
         * Scoping function which allow you to more easily use a variable in a scope.
         */
        public static K Let<T, K>(this T t, Func<T, K> action)
        {
            return action(t);
        }

        public static T Log<T>(this T t, String prefix)
        {
            Logger.Temp("{0}:{1}", prefix, t);
            return t;
        }

        public static T PrintLn<T>(this T t, String prefix)
        {
            Console.WriteLine("{0}:{1}", prefix, t);
            return t;
        }
    }
}