namespace EarWorm.Code
{
    public class Application
    {
        static Application s_app;
        //public static Application Application{get{return s_app; } };
        public static Settings Settings { get { return s_app.m_settings; } }
        public static MusicEngine MusicEngine { get { return s_app.m_musicEngine; } }

        //public MusicEngine MusicEngine { get { return m_musicEngine; } }
        //public Settings Settings { get { return m_settings; } }
        Settings m_settings;
        MusicEngine m_musicEngine;
        public static Application GetApplicationSingleton()
        {
            return s_app;
        }
        public static void Boot()
        {
            s_app = new Application();
            s_app.m_settings = new Settings();
            s_app.m_musicEngine = new MusicEngine();
        }
    }
}
