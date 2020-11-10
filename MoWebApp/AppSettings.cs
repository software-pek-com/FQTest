﻿namespace MoWebApp
{
    public class AppSettings
    {
        public string DbUrl { get; set; }
        public string DbName { get; set; }
        public string EventBusUrl { get; set; }
        public string EventBusQueue { get; set; }
        public string LoginUrl {get; set; }
        public string WelcomeMessageFolder { get; set; }
    }
}
