namespace EarWorm.Code {
    public class SetGenerator {



        SetDef _setdef;
        Random _rng = new (Guid.NewGuid().GetHashCode());
        public SetGenerator(SetDef setDef) {
            Util.Log($"sd={setDef}");
            _setdef = setDef;
        }
        public IEnumerable<TestDefinition> GetNextTest() {
            switch (_setdef.Style) {
                case Lookups.Style.ScaleRandom: {
                        for (int j = 0; j < _setdef.TestCount; j++) {
                            var notes = ScaleNotes();
                            var td = new TestDefinition {
                                Notes = notes,
                                Numtries = _setdef.Retries,
                                TimeOut = notes.Count * 2,
                                Key = _setdef.Key,

                            };
                            yield return td;
                        }
                        yield break;
                    }
                // break;
                default: {
                        throw new NotSupportedException();
                    }
            }

        }
        List<int> ScaleNotes() {
            Util.Log($"sds={_setdef}");
            var noteCount = _setdef.NoteCount;
            var ret = new List<int>(_setdef.NoteCount);
            var scale = Lookups.Scales[_setdef.Scale];
            int previous = -1;
            Util.Log($"inc r=true");
            if (_setdef.RootMode == Lookups.RootMode.IncludeRoot) {
                Util.Log($"inc r=true");
                noteCount--;
                ret.Add(0);
                previous = 0;
            }
            for (int i = 0; i < noteCount; i++) {
                while (true) {
                    var noteIdx = _rng.Next(scale.Count);
                    var note = scale[noteIdx];
                    if (note == previous) continue;
                    previous = note;
                    ret.Add(note);
                    break;
                }
            }
            return ret.Select(n => RelToAbs(n)).ToList();
        }

        int RelToAbs(int relNote) {
            // convert a scale relative note to an actual note in range
            var r = (int)_setdef.Key + relNote;
            var candidates = new List<int>();
            for (int octave = 0; octave < 9; octave++) {
                var candidate = r + (octave * 12);
                if (candidate >= _setdef.RangeStart && candidate <= _setdef.RangeEnd) {
                    candidates.Add(candidate);
                }
                if (candidate > _setdef.RangeEnd)
                    break;
            }
            if (candidates.Count == 0)
                throw new NotSupportedException();

            // randomly choose a candidate note

            return candidates[_rng.Next(candidates.Count)];



        }
    }
}
