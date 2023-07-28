using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptRunner.Helpers
{
    public interface IGithubApi
    {
        public Task CreateScript(string name, string code);
        public Task UpdateScript(string name, string code);
    }
}
