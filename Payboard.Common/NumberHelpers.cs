using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace Payboard.Common
{
    public static class NumberHelpers
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static void Times(this int times, Action<int> action)
        {
            for (var x = 0; x < times; x++)
            {
                action(x);
            }
        }

        public static async Task TimesAsync(this int times, Func<int, Task> action)
        {
            for (var x = 0; x < times; x++)
            {
                await action(x);
            }
        }

        public static List<T> Select<T>(this int times, Func<int, T> selector)
        {
            var list = new List<T>();
            for (var x = 0; x < times; x++)
            {
                list.Add(selector(x));
            }
            return list;
        }

        /// <summary>
        /// Try a given action 'n' number of times or until it succeeds.
        /// </summary>
        public static void Retries(this int times, Action<int> action, int pauseInMilliseconds = 0)
        {
            var attempt = 0;
            while (attempt < times)
            {
                try
                {
                    action(attempt);
                    break;
                }
                catch (Exception)
                {
                    attempt++;
                    if (attempt >= times)
                    {
                        throw;
                    }
                    if (pauseInMilliseconds > 0)
                    {
                        Thread.Sleep(pauseInMilliseconds * attempt);
                    }
                }
            }
        }

        /// <summary>
        /// Try a given async action 'n' number of times or until it succeeds.
        /// </summary>
        /// <param name="times">The number of times to retry the action</param>
        /// <param name="action">The action to retry</param>
        /// <param name="pauseInMilliseconds">The amount of time in milliseconds to pause between retries (defaults to 0)</param>
        public async static Task RetriesAsync(this int times, Func<int, Task> action, int pauseInMilliseconds = 0)
        {
            var attempt = 0;
            while (attempt < times)
            {
                try
                {
                    await action(attempt);
                    break;
                }
                catch (Exception)
                {
                    attempt++;
                    if (attempt >= times)
                    {
                        throw;
                    }
                }
                if (pauseInMilliseconds > 0)
                {
                    await Task.Delay(pauseInMilliseconds * attempt);
                }
            }
        }

        /// <summary>
        /// Try a given function 'n' times or until it succeeds.
        /// </summary>
        public static T Retries<T>(this int times, Func<int, T> action)
        {
            var attempt = 0;
            var result = default(T);
            while (attempt < times)
            {
                try
                {
                    result = action(attempt);
                    break;
                }
                catch (Exception)
                {
                    attempt++;
                    if (attempt >= times)
                    {
                        throw;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Try a given async action 'n' times or until it succeeds.
        /// </summary>
        /// <param name="times">The number of times to retry the action</param>
        /// <param name="action">The action to retry</param>
        /// <param name="pauseInMilliseconds">The amount of time in milliseconds to pause between retries (defaults to 0)</param>
        public async static Task<T> RetriesAsync<T>(this int times, Func<int, Task<T>> action, int pauseInMilliseconds = 0)
        {
            var attempt = 0;
            var result = default(T);
            while (attempt < times)
            {
                try
                {
                    result = await action(attempt);
                    break;
                }
                catch (Exception)
                {
                    attempt++;
                    if (attempt >= times)
                    {
                        throw;
                    }
                }
                if (pauseInMilliseconds > 0)
                {
                    await Task.Delay(pauseInMilliseconds);
                }
            }
            return result;
        }

        /// <summary>
        /// Execute the specified action only periodically, e.g., once every 100 or 1000 times
        /// </summary>
        public static void AtDebugIntervals(this int counter, Action<int> action)
        {
            try
            {
                int interval = counter < 10000 ? 100 : 1000;
                if (counter%interval == 0)
                {
                    action(interval);
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Error in AtDebugIntervals(): " + ex.Message);
            }
        }
    }
}