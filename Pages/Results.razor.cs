using EarWorm.Code;

namespace EarWorm.Pages {
    public partial class Results {
        public bool ShowResultTable {
            get {
                return _saver.ResultsDB.Results.Count > 0;
            }
        }
        public ResultsDB ResultsDB => _saver.ResultsDB;
        public void Dispose() {

        }
    }
}
