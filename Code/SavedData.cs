using System.Text.Json;

namespace EarWorm.Code {
    public class SavedData {
        const string SETTINGS_KEY = "SETTINGS";
        const string SETDEF_KEY = "SETDEFS";
        const string RESULTS_KEY = "RESULTS";

        private SettingsData _settingsData;
        private SetDefData _setDefData;
     

        public async Task Boot() {
            Util.Log("boot settings");
            await LoadSettings();
            await LoadSetDefs();
        }
        async Task LoadSettings() {
            var json = await Util.ReadStorage(SETTINGS_KEY);
            if (json == null)
                _settingsData = Defaults.DefaultSettings;
            else
                _settingsData = JsonSerializer.Deserialize<SettingsData>(json);
            Util.Log(_settingsData.ToString());
        }
        async Task LoadSetDefs() {
            var json = await Util.ReadStorage(SETDEF_KEY);
            if (json == null)
                _setDefData = new SetDefData { Current = Defaults.DefaultSetDef };
            else
                _setDefData = JsonSerializer.Deserialize<SetDefData>(json);
            Util.Log(_setDefData.ToString());
        }
        public void SaveSetDefs() {
            var json = JsonSerializer.Serialize(_setDefData);
            Util.WriteStorage(SETDEF_KEY, json);
        }
        public void SaveSettings() {
            var json = JsonSerializer.Serialize(_settingsData);
            Util.WriteStorage(SETTINGS_KEY, json);
        }
        public SetDef CurrentSet {
            get {
                return _setDefData.Current;
            }
            set {
                _setDefData.Current = value;
                SaveSetDefs();
            }
        }
        public SettingsData Settings {
            get {
                return _settingsData;
            }
            set { _settingsData = value;
            }
        }
    }
}
