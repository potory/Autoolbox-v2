namespace Autoolbox.App;

public static class Contracts
{
    public const string ConfigPath = "appconfig.json";
    
    public static class Config
    {
        public static class Directories
        {
            public const string Logs = "directories:logs";
        }

        public static class Defaults
        {
            public const string Request = "defaults:request";
        }
        
        public static class Injection
        {
            public const string PreviousResult = "injection:previousResult";
        }
        
        public static class ExternalDependencies
        {
            public const string SortingApplicationPath = "externalDependencies:sortingApplicationPath";
        }
    }
}