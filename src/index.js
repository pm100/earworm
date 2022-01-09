import * as  Tone from 'tone'



window.playSeq = (notes, interval) => {
    Tone.start();
    const synth = new Tone.Synth().toDestination();
    const now = Tone.now();
    var offset = 0;
    notes.forEach(note => {
        synth.triggerAttack(note, now + offset);
        offset = offset + 0.5;
        synth.triggerRelease(now + offset);
        offset = offset + 0.5;

    })
};
window.playChord = (notes, interval) => {
    Tone.start();
    const synth = new Tone.PolySynth(Tone.Synth).toDestination();
    const now = Tone.now();
    notes.forEach(note => {
        synth.triggerAttack(note);

    });
    synth.triggerRelease(notes, now + 1);
};

