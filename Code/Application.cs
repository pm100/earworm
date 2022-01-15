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
        public static async Task Boot()
        {
            s_app = new Application();
            s_app.m_settings = new Settings();
           
            s_app.m_musicEngine = new MusicEngine();
            await s_app.m_settings.Boot();
            return;// Task.CompletedTask;
        }
        public static event EventHandler OnNavigate;
        public static void OnMenuNavigate() {
            if (OnNavigate != null)
                OnNavigate.Invoke(s_app, null);

        }
    }
}
