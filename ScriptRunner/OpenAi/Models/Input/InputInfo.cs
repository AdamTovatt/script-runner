﻿namespace ScriptRunner.OpenAi.Models.Input
{
    public class InputInfo
    {
        public string Id { get; set; }
        public string Message { get; private set; }
        public string Type { get; private set; }
        public string DetailedType { get; private set; }
        public string? SubType { get; private set; }
        public List<InputChoice>? Choices { get; set; }
        public bool HasChoices { get { return Choices != null && Choices.Any(); } }

        public InputInfo(Type type, string message, string? subType = null, List<InputChoice>? choices = null, string? id = null)
        {
            DetailedType = type.ToString();
            Message = message;
            SubType = subType;
            Choices = choices;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                Type = type.GenericTypeArguments[0].Name;
            else
                Type = type.Name;

            if (id == null)
                Id = GenerateId();
            else
                Id = id;
        }

        public InputInfo AddChoices(params InputChoice[] choices)
        {
            if (Choices == null)
                Choices = new List<InputChoice>();

            Choices.AddRange(choices);
            return this;
        }

        public InputChoice? GetInputChoiceByMessage(string message)
        {
            if (Choices == null) return null;

            return Choices.FirstOrDefault(c => c.DisplayValue.ToLower() == message.ToLower());
        }

        public static string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
