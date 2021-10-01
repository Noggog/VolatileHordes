namespace VolatileHordes.GameAbstractions
{
    public interface ILogger
    {
        void Error(string message, params object?[] objs);
        void Warning(string message, params object?[] objs);
        void Info(string message);
        void Info<T1>(string message, T1 o1);
        void Info<T1, T2>(string message, T1 o1, T2 o2);
        void Info<T1, T2, T3>(string message, T1 o1, T2 o2, T3 o3);
        void Info<T1, T2, T3, T4>(string message, T1 o1, T2 o2, T3 o3, T4 o4);
        void Info<T1, T2, T3, T4, T5>(string message, T1 o1, T2 o2, T3 o3, T4 o4, T5 o5);
        void Debug(string message);
        void Debug<T1>(string message, T1 o1);
        void Debug<T1, T2>(string message, T1 o1, T2 o2);
        void Debug<T1, T2, T3>(string message, T1 o1, T2 o2, T3 o3);
        void Debug<T1, T2, T3, T4>(string message, T1 o1, T2 o2, T3 o3, T4 o4);
        void Debug<T1, T2, T3, T4, T5>(string message, T1 o1, T2 o2, T3 o3, T4 o4, T5 o5);
        void Verbose(string message);
        void Verbose<T1>(string message, T1 o1);
        void Verbose<T1, T2>(string message, T1 o1, T2 o2);
        void Verbose<T1, T2, T3>(string message, T1 o1, T2 o2, T3 o3);
        void Verbose<T1, T2, T3, T4>(string message, T1 o1, T2 o2, T3 o3, T4 o4);
        void Verbose<T1, T2, T3, T4, T5>(string message, T1 o1, T2 o2, T3 o3, T4 o4, T5 o5);
    }

    public class LoggerWrapper : ILogger
    {
        public void Error(string message, params object?[] objs)
        {
            Logger.Error(message, objs);
        }

        public void Warning(string message, params object?[] objs)
        {
            Logger.Warning(message, objs);
        }

        public void Info(string message)
        {
            Logger.Info(message);
        }

        public void Info<T1>(string message, T1 o1)
        {
            Logger.Info(message, o1);
        }

        public void Info<T1, T2>(string message, T1 o1, T2 o2)
        {
            Logger.Info(message, o1, o2);
        }

        public void Info<T1, T2, T3>(string message, T1 o1, T2 o2, T3 o3)
        {
            Logger.Info(message, o1, o2, o3);
        }

        public void Info<T1, T2, T3, T4>(string message, T1 o1, T2 o2, T3 o3, T4 o4)
        {
            Logger.Info(message, o1, o2, o3, o4);
        }

        public void Info<T1, T2, T3, T4, T5>(string message, T1 o1, T2 o2, T3 o3, T4 o4, T5 o5)
        {
            Logger.Info(message, o1, o2, o3, o4, o5);
        }

        public void Debug(string message)
        {
            throw new System.NotImplementedException();
        }

        public void Debug<T1>(string message, T1 o1)
        {
            Logger.Debug(message, o1);
        }

        public void Debug<T1, T2>(string message, T1 o1, T2 o2)
        {
            Logger.Debug(message, o1, o2);
        }

        public void Debug<T1, T2, T3>(string message, T1 o1, T2 o2, T3 o3)
        {
            Logger.Debug(message, o1, o2, o3);
        }

        public void Debug<T1, T2, T3, T4>(string message, T1 o1, T2 o2, T3 o3, T4 o4)
        {
            Logger.Debug(message, o1, o2, o3, o4);
        }

        public void Debug<T1, T2, T3, T4, T5>(string message, T1 o1, T2 o2, T3 o3, T4 o4, T5 o5)
        {
            Logger.Debug(message, o1, o2, o3, o4, o5);
        }

        public void Verbose(string message)
        {
            Logger.Verbose(message);
        }

        public void Verbose<T1>(string message, T1 o1)
        {
            Logger.Verbose(message, o1);
        }

        public void Verbose<T1, T2>(string message, T1 o1, T2 o2)
        {
            Logger.Verbose(message, o1, o2);
        }

        public void Verbose<T1, T2, T3>(string message, T1 o1, T2 o2, T3 o3)
        {
            Logger.Verbose(message, o1, o2, o3);
        }

        public void Verbose<T1, T2, T3, T4>(string message, T1 o1, T2 o2, T3 o3, T4 o4)
        {
            Logger.Verbose(message, o1, o2, o3, o4);
        }

        public void Verbose<T1, T2, T3, T4, T5>(string message, T1 o1, T2 o2, T3 o3, T4 o4, T5 o5)
        {
            Logger.Verbose(message, o1, o2, o3, o4, o5);
        }
    }
}