using System.Text.Json;

namespace EarWorm.Code
{
	public class Settings {
		const string SETTINGS_KEY = "SETTINGS";
		const string SETDEF_KEY = "SETDEFS";
		const string SCORE_KEY = "SCORES";
		record SettingsData {
			public string InstrumentKey { get; set; }
			public bool NoSleep { get; set; }	
		};


		record class SetDefData {
			public SetDef Current { get; set; }
			public List<SetDef> Saved { get; set; }
		 }
		private SettingsData _settingsData;
		private SetDefData _setDefData;

		public async Task Boot()
		{
			Util.Log("boot settings");
			await LoadSettings();
			await LoadSetDefs();
		}
		public async  Task LoadSettings()
		{
			var json = await Util.ReadStorage(SETTINGS_KEY);
			if (json == null)
				_settingsData = new SettingsData
				{
					InstrumentKey = "P",
					NoSleep=true,
				};
			else
				_settingsData = JsonSerializer.Deserialize<SettingsData>(json);
			Util.Log(_settingsData.ToString());
		}
		public async Task LoadSetDefs() {
			var json = await Util.ReadStorage(SETDEF_KEY);
			if (json == null)
				_setDefData = new SetDefData { Current = Defaults.DefaultSetDef };
			else
				_setDefData = JsonSerializer.Deserialize<SetDefData>(json);
			Util.Log(_setDefData.ToString());
		}
		public  void SaveSetDefs() {
			var json = JsonSerializer.Serialize(_setDefData);
			 Util.WriteStorage(SETDEF_KEY, json);

		}
		public void SaveSettings()
		{
			var json = JsonSerializer.Serialize(_settingsData);
			Util.WriteStorage(SETTINGS_KEY,json);
		}
		public SetDef CurrentSet  {
            get{
				return _setDefData.Current;

            }
            set{
				_setDefData.Current = value;
				SaveSetDefs();
			}
        }
		public  string InstrumentKey
        {
			get
			{
				Util.Log("get inst");
				return _settingsData.InstrumentKey;
			}
            set { _settingsData.InstrumentKey = value; }
		}
	}
}
