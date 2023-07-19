using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAi.Models.Completion
{
    public class Model
    {
        public static string Default { get { return Gpt35Turbo; } }

        public const string Gpt35Turbo = "gpt-3.5-turbo";
    }
}
