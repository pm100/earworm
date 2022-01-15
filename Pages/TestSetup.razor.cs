using EarWorm.Code;

namespace EarWorm.Pages {
    public partial class TestSetup {
        public static List<(SetGenerator.Scale, String)> s_scaleTable = new List<(SetGenerator.Scale, string)> {
        (SetGenerator.Scale.Ionian, "Major"),
        (SetGenerator.Scale.Mixolydian, "Dominant"),
        (SetGenerator.Scale.Dorian, "Dorian"),
        (SetGenerator.Scale.Aeolian,"Minor"),
        (SetGenerator.Scale.Lydian,"Lydian"),
        (SetGenerator.Scale.Phrygian,"Phrygian"),
        (SetGenerator.Scale.Locrian,"Locrian")
    };

        public static List<(SetGenerator.Key, String)> s_keyTable = new List<(SetGenerator.Key, string)> {
        (SetGenerator.Key.C, "C"),
        (SetGenerator.Key.CSharp, "C# / Db"),
        (SetGenerator.Key.D, "D"),
        (SetGenerator.Key.DSharp, "D# / Eb"),
        (SetGenerator.Key.E, "E"),
        (SetGenerator.Key.F, "F"),
        (SetGenerator.Key.FSharp, "F# / Gb"),
        (SetGenerator.Key.G, "G"),
        (SetGenerator.Key.GSharp, "G# / Ab"),
        (SetGenerator.Key.A, "A"),
        (SetGenerator.Key.ASharp, "A# / Bb"),
        (SetGenerator.Key.B, "D"),
    };
        private int SetSize { get { return s_currentSet.TestCount; } set { s_currentSet.TestCount = value; } }
        private int Retries { get { return s_currentSet.Retries; } set { s_currentSet.Retries = value; } }
        private List<string> ScaleItems {
            get {
                return s_scaleTable.Select(x => x.Item2).ToList();
            }
        }
        private void ScaleChanged(int sel) {
            s_currentSet.Scale = s_scaleTable[sel].Item1;
        }

        private int SelectedScale {
            get {
                return s_scaleTable.FindIndex(x => x.Item1 == s_currentSet.Scale);
                //return -1;
            }

        }
        private IList<string> KeyItems {
            get {
                return s_keyTable.Select(x => x.Item2).ToList();
            }
        }
        private void KeyChanged(int sel) {
            s_currentSet.Key = s_keyTable[sel].Item1;
        }

        private int SelectedKey {
            get {
                return s_keyTable.FindIndex(x => x.Item1 == s_currentSet.Key);
            }
        }

        static SetGenerator.SetDef s_currentSet;
        public  static SetGenerator.SetDef CurrentSet {
             get {

                return s_currentSet;
            }
        }
        public  TestSetup() {
            // we want to get called whenever there is a page change so we can save our state
            Application.OnNavigate += (_, _) => {
                Application.Settings.CurrentSet = s_currentSet;
            };
            // init load from storage            
            s_currentSet = Application.Settings.CurrentSet;
        }

    }
}
