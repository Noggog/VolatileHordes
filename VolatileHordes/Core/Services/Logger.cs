namespace VolatileHordes
{
    public enum LogLevel
    {
        None,
        Information,
        Debug,
        Verbose
    }
    
    public static class Logger
    {
        public static LogLevel Level = LogLevel.Information;
        
        public static void Error(string message, params object?[] objs)
        {
            Log.Error($"[{Constants.ModName}] {message}", objs);
        }
        
        public static void Warning(string message, params object?[] objs)
        {
            Log.Warning($"[{Constants.ModName}] {message}", objs);
        }
        
        private static void Write(string message)
        {
            Log.Out($"[{Constants.ModName}] {message}");
        }
        
        private static void Write<T1>(string message, T1 o1)
        {
            Log.Out($"[{Constants.ModName}] {message}", new[] { o1 });
        }
        
        private static void Write<T1, T2>(string message, T1 o1, T2 o2)
        {
            Log.Out($"[{Constants.ModName}] {message}", new object?[] { o1, o2 });
        }
        
        private static void Write<T1, T2, T3>(string message, T1 o1, T2 o2, T3 o3)
        {
            Log.Out($"[{Constants.ModName}] {message}", new object?[] { o1, o2, o3 });
        }
        
        private static void Write<T1, T2, T3, T4>(string message, T1 o1, T2 o2, T3 o3, T4 o4)
        {
            Log.Out($"[{Constants.ModName}] {message}", new object?[] { o1, o2, o3, o4 });
        }
        
        private static void Write<T1, T2, T3, T4, T5>(string message, T1 o1, T2 o2, T3 o3, T4 o4, T5 o5)
        {
            Log.Out($"[{Constants.ModName}] {message}", new object?[] { o1, o2, o3, o4, o5 });
        }
        
        public static void Info(string message)
        {
            if (LogLevel.Information > Level) return;
            Log.Out($"[{Constants.ModName}] {message}");
        }
        
        public static void Info<T1>(string message, T1 o1)
        {
            if (LogLevel.Information > Level) return;
            Log.Out($"[{Constants.ModName}] {message}", new object?[] { o1 });
        }
        
        public static void Info<T1, T2>(string message, T1 o1, T2 o2)
        {
            if (LogLevel.Information > Level) return;
            Log.Out($"[{Constants.ModName}] {message}", new object?[] { o1, o2 });
        }
        
        public static void Info<T1, T2, T3>(string message, T1 o1, T2 o2, T3 o3)
        {
            if (LogLevel.Information > Level) return;
            Log.Out($"[{Constants.ModName}] {message}", new object?[] { o1, o2, o3 });
        }
        
        public static void Info<T1, T2, T3, T4>(string message, T1 o1, T2 o2, T3 o3, T4 o4)
        {
            if (LogLevel.Information > Level) return;
            Log.Out($"[{Constants.ModName}] {message}", new object?[] { o1, o2, o3, o4 });
        }
        
        public static void Info<T1, T2, T3, T4, T5>(string message, T1 o1, T2 o2, T3 o3, T4 o4, T5 o5)
        {
            if (LogLevel.Information > Level) return;
            Log.Out($"[{Constants.ModName}] {message}", new object?[] { o1, o2, o3, o4, o5 });
        }
        
        public static void Debug(string message)
        {
            if (LogLevel.Debug > Level) return;
            Log.Out($"[{Constants.ModName}] {message}");
        }
        
        public static void Debug<T1>(string message, T1 o1)
        {
            if (LogLevel.Debug > Level) return;
            Log.Out($"[{Constants.ModName}] {message}", new object?[] { o1 });
        }
        
        public static void Debug<T1, T2>(string message, T1 o1, T2 o2)
        {
            if (LogLevel.Debug > Level) return;
            Log.Out($"[{Constants.ModName}] {message}", new object?[] { o1, o2 });
        }
        
        public static void Debug<T1, T2, T3>(string message, T1 o1, T2 o2, T3 o3)
        {
            if (LogLevel.Debug > Level) return;
            Log.Out($"[{Constants.ModName}] {message}", new object?[] { o1, o2, o3 });
        }
        
        public static void Debug<T1, T2, T3, T4>(string message, T1 o1, T2 o2, T3 o3, T4 o4)
        {
            if (LogLevel.Debug > Level) return;
            Log.Out($"[{Constants.ModName}] {message}", new object?[] { o1, o2, o3, o4 });
        }
        
        public static void Debug<T1, T2, T3, T4, T5>(string message, T1 o1, T2 o2, T3 o3, T4 o4, T5 o5)
        {
            if (LogLevel.Debug > Level) return;
            Log.Out($"[{Constants.ModName}] {message}", new object?[] { o1, o2, o3, o4, o5 });
        }
        
        public static void Verbose(string message)
        {
            if (LogLevel.Verbose > Level) return;
            Log.Out($"[{Constants.ModName}] {message}");
        }
        
        public static void Verbose<T1>(string message, T1 o1)
        {
            if (LogLevel.Verbose > Level) return;
            Log.Out($"[{Constants.ModName}] {message}", new object?[] { o1 });
        }
        
        public static void Verbose<T1, T2>(string message, T1 o1, T2 o2)
        {
            if (LogLevel.Verbose > Level) return;
            Log.Out($"[{Constants.ModName}] {message}", new object?[] { o1, o2 });
        }
        
        public static void Verbose<T1, T2, T3>(string message, T1 o1, T2 o2, T3 o3)
        {
            if (LogLevel.Verbose > Level) return;
            Log.Out($"[{Constants.ModName}] {message}", new object?[] { o1, o2, o3 });
        }
        
        public static void Verbose<T1, T2, T3, T4>(string message, T1 o1, T2 o2, T3 o3, T4 o4)
        {
            if (LogLevel.Verbose > Level) return;
            Log.Out($"[{Constants.ModName}] {message}", new object?[] { o1, o2, o3, o4 });
        }
        
        public static void Verbose<T1, T2, T3, T4, T5>(string message, T1 o1, T2 o2, T3 o3, T4 o4, T5 o5)
        {
            if (LogLevel.Verbose > Level) return;
            Log.Out($"[{Constants.ModName}] {message}", new object?[] { o1, o2, o3, o4, o5 });
        }

        public static void Temp(string message)
        {
            Log.Out($"[{Constants.ModName}] {message}");
        }

        public static void Temp<T1>(string message, T1 o1)
        {
            Log.Out($"[{Constants.ModName}] {message}", new object?[] { o1 });
        }

        public static void Temp<T1, T2>(string message, T1 o1, T2 o2)
        {
            Log.Out($"[{Constants.ModName}] {message}", new object?[] { o1, o2 });
        }

        public static void Temp<T1, T2, T3>(string message, T1 o1, T2 o2, T3 o3)
        {
            Log.Out($"[{Constants.ModName}] {message}", new object?[] { o1, o2, o3 });
        }

        public static void Temp<T1, T2, T3, T4>(string message, T1 o1, T2 o2, T3 o3, T4 o4)
        {
            Log.Out($"[{Constants.ModName}] {message}", new object?[] { o1, o2, o3, o4 });
        }

        public static void Temp<T1, T2, T3, T4, T5>(string message, T1 o1, T2 o2, T3 o3, T4 o4, T5 o5)
        {
            Log.Out($"[{Constants.ModName}] {message}", new object?[] { o1, o2, o3, o4, o5 });
        }
    }
}