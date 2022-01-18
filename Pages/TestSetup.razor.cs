using EarWorm.Code;

namespace EarWorm.Pages {
    public partial class TestSetup {
        public TestSetup() {
            s_currentSet = Application.Settings.CurrentSet;
        }
        private int SetSize { get { return s_currentSet.TestCount; } set { s_currentSet.TestCount = value; } }
        private int Retries { get { return s_currentSet.Retries; } set { s_currentSet.Retries = value; } }
        private List<string> ScaleItems {
            get {
                return Lookups.ScaleNames.Select(x => x.Item2).ToList();
            }
        }
        private void ScaleChanged(int sel) {
            s_currentSet.Scale = Lookups.ScaleNames[sel].Item1;
        }

        private int SelectedScale {
            get {
                return Lookups.ScaleNames.FindIndex(x => x.Item1 == s_currentSet.Scale);
            }

        }
        private IList<string> KeyItems {
            get {
                return Lookups.KeyNames.Select(x => x.Item2).ToList();
            }
        }
        private void KeyChanged(int sel) {
            s_currentSet.Key = Lookups.KeyNames[sel].Item1;
        }

        private int SelectedKey {
            get {
                return Lookups.KeyNames.FindIndex(x => x.Item1 == s_currentSet.Key);
            }
        }

        static SetDef s_currentSet;
        public  static SetDef CurrentSet {
             get {

                return s_currentSet;
            }
        }

        public void Dispose() {
            Application.Settings.CurrentSet = s_currentSet;

        }

    }
}
