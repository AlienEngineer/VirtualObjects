using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace VirtualObjects
{
    public static class Diagnostic
    {

        class DiagnosticTimeSpan
        {
            public DiagnosticTimeSpan()
            {
                Reset();
            }

            public TimeSpan Time { get; private set; }
            public int Count { get; private set; }

            public void Add(TimeSpan time)
            {
                // Removes the first timer.
                // The first time will always be greater than the rest.
                // This is meaningless since this system resuses resources all the time.
                // the second time as it would be the first.
                if (Count == 1)
                {
                    Time = new TimeSpan();
                    Time = Time.Add(time);
                }

                Time = Time.Add(time);
                Count++;
            }

            private void Reset()
            {
                Count = 0;
                Time = new TimeSpan();
            }
        }

        static readonly IDictionary<String, DiagnosticTimeSpan> Times = new Dictionary<string, DiagnosticTimeSpan>();

        static void Add(TimeSpan time, String name = "DEFAULT")
        {
            DiagnosticTimeSpan record;
            if ( !Times.TryGetValue(name, out record) )
            {
                Times[name] = record = new DiagnosticTimeSpan();
            }

            record.Add(time);
        }


        public static void Timed(Action func, String mask = null, String name = "DEFAULT")
        {
            Timed<Object>(() =>
            {
                func();
                return null;
            }, mask, name);
        }

        /// <summary>
        /// Timeds the specified function.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="func">The function.</param>
        /// <param name="mask">For the mask use [{0} for millis, {1} for TimeSpan, {2} for Ticks]. Or null to hide.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static TResult Timed<TResult>(Func<TResult> func, String mask = null, String name = "DEFAULT")
        {
            var timer = Stopwatch.StartNew();
            try
            {
                return func();
            }
            finally
            {
                timer.Stop();

                if ( mask != null )
                {
                    Trace.WriteLine(String.Format(mask, timer.ElapsedMilliseconds, timer.Elapsed, timer.ElapsedTicks));
                }

                Add(timer.Elapsed, name);
            }
        }


        /// <summary>
        /// Prints the average time.
        /// </summary>
        /// <param name="mask">For the mask use [{0} for millis, {1} for TimeSpan, {2} for Ticks].</param>
        /// <param name="name">The name.</param>
        public static void PrintAverageTime(String mask, String name = "DEFAULT")
        {
            if ( !Times.ContainsKey(name) )
            {
                return;
            }

            var acum = Times[name];
            var timer = acum.Time;

            timer = new TimeSpan(timer.Ticks / acum.Count);
            Trace.WriteLine(String.Format(mask, timer.TotalMilliseconds, timer, timer.Ticks));

            Times.Remove(name);
        }

        /// <summary>
        /// Prints the time.
        /// </summary>
        /// <param name="mask">For the mask use [{0} for millis, {1} for TimeSpan, {2} for Ticks].</param>
        /// <param name="name">The name.</param>
        public static void PrintTime(String mask, String name = "DEFAULT")
        {
            if ( !Times.ContainsKey(name) )
            {
                return;
            }

            var acum = Times[name];
            var timer = acum.Time;

            Trace.WriteLine(String.Format(mask, timer.TotalMilliseconds, timer, timer.Ticks));

            Times.Remove(name);
        }

    }
}
