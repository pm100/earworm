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

window.playPianoSeq = (notes, interval) => {
    console.log("piano");
    const sampler = new Tone.Sampler({
        urls: {
            "C4": "assets/C4.mp3",
            "D4": "assets/D4.mp3",
            "F4": "assets/F4.mp3",
            "A4": "assets/A4.mp3",
        },
        release: 1
      //  baseUrl: "https://tonejs.github.io/audio/salamander/",
    }).toDestination();

    Tone.loaded().then(() => {
        sampler.triggerAttackRelease(["Eb4", "G4", "Bb4"], 4);
    })
}

