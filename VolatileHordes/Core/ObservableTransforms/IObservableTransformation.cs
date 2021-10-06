using System;

namespace VolatileHordes.Core.ObservableTransforms
{
    public interface IObservableTransformation<TIn, TOut>
    {
        IObservable<TOut> Transform(IObservable<TIn> obs);
    }
    
    public interface IObservableTransformation<TIn, TOut, T1>
    {
        IObservable<TOut> Transform(IObservable<TIn> obs, T1 param1);
    }
    
    public interface IObservableTransformation<TIn, TOut, T1, T2>
    {
        IObservable<TOut> Transform(IObservable<TIn> obs, T1 timeToShutOff, T2 componentName);
    }
    
    public interface IObservableTransformation<TIn, TOut, T1, T2, T3>
    {
        IObservable<TOut> Transform(IObservable<TIn> obs, T1 param1, T2 param2, T3 param3);
    }
    
    public interface IObservableTransformation<TIn, TOut, T1, T2, T3, T4>
    {
        IObservable<TOut> Transform(IObservable<TIn> obs, T1 param1, T2 param2, T3 param3, T4 param4);
    }
}