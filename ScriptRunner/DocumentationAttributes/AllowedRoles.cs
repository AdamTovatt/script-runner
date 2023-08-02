namespace ScriptRunner.DocumentationAttributes
{
    public class AllowedRoles : Attribute
    {
        public string[] Roles { get; set; }

        public AllowedRoles(params string[] roles)
        {
            Roles = roles;
        }
    }
}
