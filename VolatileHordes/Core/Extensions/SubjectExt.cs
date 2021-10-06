using System.Reactive;
using System.Reactive.Subjects;

namespace VolatileHordes
{
    public static class SubjectExt
    {
        public static void OnNext(this Subject<Unit> subject)
        {
            subject.OnNext(Unit.Default);
        }
    }
}