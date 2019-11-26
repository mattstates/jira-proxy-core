namespace JiraProxyCore.Constants
{
    public static class CorsPolicies
    {
        private static string jiraProxyOriginPolicy = "Jira-Proxy-Origins-Policy";

        public static string JiraProxyOriginPolicy { get => jiraProxyOriginPolicy; set => jiraProxyOriginPolicy = value; }
    }
}
