using System.Text.Json;

namespace EarWorm.Code {
    public class SavedData {
        const string SETTINGS_KEY = "SETTINGS";
        const string SETDEF_KEY = "SETDEFS";
        const string RESULTS_HISTORY_KEY = "RESULTS_HISTORY";
        const string CURRENT_TEST_RESULT = "CURRENT_TEST_RESULT";

        private SettingsData _settingsData;
        private SetDefData _setDefData;
        TestSetResult _currentResults;

        public async Task Boot() {
            Util.Log("boot settings");
            await LoadSettings();
            await LoadSetDefs();
            await LoadTestResults();
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
        async Task LoadTestResults() {
            var json = await Util.ReadStorage(CURRENT_TEST_RESULT);
            if (json == null) {
                _currentResults = new TestSetResult {
                    Results = new()
                };
            }
            else {
                _currentResults = JsonSerializer.Deserialize<TestSetResult>(json);
            }
        }
        public async Task SaveCurrentResults() {
            _currentResults.DateTime = DateTime.Now;
            var json = JsonSerializer.Serialize(_currentResults);
            await Util.WriteStorage(CURRENT_TEST_RESULT, json);
        }
        public async void SaveSetDefs() {
            var json = JsonSerializer.Serialize(_setDefData);
            await Util.WriteStorage(SETDEF_KEY, json);
        }
        public async void SaveSettings() {
            var json = JsonSerializer.Serialize(_settingsData);
            await Util.WriteStorage(SETTINGS_KEY, json);
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

        public TestSetResult CurrentResults => _currentResults;

        internal async void WriteResult() {
            // save current result to DB

            //var json = JsonSerializer.Serialize(_currentResults);
            ResultsDB db;
            var json = await Util.ReadStorage(RESULTS_HISTORY_KEY);
            if (json == null) {
                db = new ResultsDB {
                    Results = new()
                };
            }
            else {
                db = JsonSerializer.Deserialize<ResultsDB>(json);

            }
            db.Results.Add(CurrentResults);
            json = JsonSerializer.Serialize(db);    
            await Util.WriteStorage(RESULTS_HISTORY_KEY, json);


        }
    }
}
