namespace EarWorm.Code {
    public class SetGenerator {
        SetDef _setdef;
        int _testCount;
        int _cycleCurrentKey;
        int _cycleStart;
        MusicEngine _musicEngine;
        Random _rng = new(Guid.NewGuid().GetHashCode());
        public SetGenerator(MusicEngine me) {
            _musicEngine = me;
        }

        public void Init(SetDef setDef) {
            _setdef = setDef;
            _testCount = 0;
            _cycleStart = _cycleCurrentKey = Array.IndexOf(Lookups.CycleKeys, setDef.Key);
        }
        public IEnumerable<TestDefinition> GetNextTest() {
            switch (_setdef.Style) {
                case Lookups.Style.ScaleRandom: {
                        for (; _testCount < _setdef.TestCount; _testCount++) {
                            yield return ScaleTD(_setdef.NoteCount, _setdef.Key);
                        }
                        yield break;
                    }
                case Lookups.Style.CycleOfFifthsRandom: {
                        for (; _testCount < _setdef.TestCount * Lookups.CycleKeys.Length;) {
                            for (int i = 0; i < _setdef.TestCount; i++) {
                                yield return ScaleTD(_setdef.NoteCount, Lookups.CycleKeys[_cycleCurrentKey]);
                                _testCount++;
                            }
                            _cycleCurrentKey = (++_cycleCurrentKey) % Lookups.CycleKeys.Length;
                        }
                        yield break;
                    }
                // break;
                default: {
                        throw new NotSupportedException();
                    }
            }

        }

        TestDefinition ScaleTD(int noteCount, Lookups.Key key) {
            var notes = ScaleNotes(noteCount, key);
            var td = new TestDefinition {
                Notes = notes.Select(x => x.Item2).ToList(),
                RelNotes = notes.Select(x => x.Item1).ToList(),
                Numtries = _setdef.Retries,
                TimeOut = notes.Count * _setdef.TimeAllowed,
                Key = _setdef.Key,
                SeqNumber = _testCount

            };
            return td;

        }

        // produce n random notes in the key given. 
        // the scale to use is taken from _setDef
        // the range is also in _setdef

        // returns the relative scale note and the actual note to play
        List<(int, int)> ScaleNotes(int noteCount, Lookups.Key key) {
            Util.Log($"key={key}");
            var ret = new List<int>(_setdef.NoteCount);
            var scale = Lookups.Scales[_setdef.Scale];

            // prevent 2 notes the same together
            int previous = -1;
            if (_setdef.RootMode == Lookups.RootMode.IncludeRoot) {
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
            return ret.Select(n => (n, RelToAbs(n, key))).ToList();
        }

        internal Lookups.Key GetCurrentKey() {
            return Lookups.CycleKeys[_cycleCurrentKey];
        }
       public  List<int> GenerateTriad(Lookups.Key key) {
            var scale = Lookups.Scales[_setdef.Scale];
            var ret = new List<int> { scale[0], scale[2], scale[4] };
            return GenerateChord(_setdef.Key, ret);
        }
        List<int> GenerateChord(Lookups.Key key, IList<int> rels) {
            // adjust for instrument transpose
            var offset = Lookups.KeyTable[key].Base - _musicEngine.GetCurrentInstrument().NoteOffset;
            // shunt into middle of the piano
            offset += (int)Lookups.ImportantNotes.A3 - (int)Lookups.ImportantNotes.A0;

            var ret = rels.Select(note=>note+offset).ToList();
            return ret;
        }
        int RelToAbs(int relNote, Lookups.Key key) {
            // convert a scale relative note to an actual note in range
            // and in the correct key
            // and transposed

            var offset = Lookups.KeyTable[key].Base - _musicEngine.GetCurrentInstrument().NoteOffset;
            var r = offset + relNote;
            var candidates = new List<int>();

            // generate the note in all octaves in range

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
