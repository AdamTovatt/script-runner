﻿using ScriptRunner.Models;

namespace ScriptRunner.Providers
{
    public interface IScriptProvider
    {
        public Task<ScriptCode?> GetScriptAsync(string scriptName);
        public Task<List<ScriptCode>> GetAllScriptsAsync();
        public Task SaveScriptAsync(ScriptCode scriptCode);
        public List<string> GetAllScriptNames();
    }
}
