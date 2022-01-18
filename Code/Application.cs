﻿namespace EarWorm.Code
{
    public class Application
    {
        static Application s_app;
        public static Settings Settings => s_app.m_settings; 
        public static MusicEngine MusicEngine => s_app.m_musicEngine;

        Settings m_settings;
        MusicEngine m_musicEngine;
        public static async Task Boot()
        {
            s_app = new Application();
            s_app.m_settings = new Settings();
           
            s_app.m_musicEngine = new MusicEngine();
            await s_app.m_settings.Boot();
            return;// Task.CompletedTask;
        }
    }
}
