using System.Text.Json;

namespace EarWorm.Code
{
	public  class Settings
	{
		const string SETTINGS_KEY = "SETTINGS";
		record SettingsData {
			public string InstrumentKey { get; set; }
			};

		private SettingsData _settingsData;

		public Settings()
		{
			Load();
		}
		public async  void Load()
		{
			var json = await Util.ReadStorage(SETTINGS_KEY);
			if (json == null)
				_settingsData = new SettingsData
				{
					InstrumentKey = ""
				};
			else
				_settingsData = JsonSerializer.Deserialize<SettingsData>(json);
			Util.Log(_settingsData.ToString());
		}

		public void Save()
		{
			var json = JsonSerializer.Serialize(_settingsData);
			Util.WriteStorage(SETTINGS_KEY,json);
		}

		public  string InstrumentKey
        {
			get
			{
				return _settingsData.InstrumentKey;
			}
            set { _settingsData.InstrumentKey = value; }
		}
	}
}
