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
        public static T Let<T>(this T t, Action<T> action)
        {
            action(t);
            return t;
        }

        public static T Log<T>(this T t, String prefix)
        {
            Logger.Debug("{0}:{1}", prefix, t);
            return t;
        }
    }
}