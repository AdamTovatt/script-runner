using ScriptRunner;
using System;

namespace CustomScripts
{
    public class DayOfWeekAndDateScript : CompiledScript
    {
        public DayOfWeekAndDateScript(ScriptContext context) : base(context) { }

        /// <summary>
        /// This script tells the current day of the week and date.
        /// </summary>
        [ScriptStart]
        public string GetCurrentDayOfWeekAndDate()
        {
            string dayOfWeek = DateTime.Now.DayOfWeek.ToString();
            string date = DateTime.Now.ToString("MMMM dd, yyyy");

            return $"Today is {dayOfWeek} ({date}).";
        }
    }
}