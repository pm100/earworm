namespace EarWorm.Code
{
    public static class Analyzer
    {
        public static void BadNotes(TestSetResult results) {
            foreach(var test in results.Results) {
                if(test.LR == Lookups.ListenResult.Failed) {
                    var note = test.TestDef.RelNotes[test.FailedNote];
                }
            }
        }
    }
}
