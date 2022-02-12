import * as  Tone from 'tone';
import NoSleep from 'nosleep.js';
import Vex from 'vexflow';

// Vex engine directly exposed

window.Vex = Vex;


// no sleep funuctions

window.noSleep = () => {
    window.sleeper = new NoSleep();
   
    sleeper.enable();
    console.log("nosleep on");

}
window.allowSleep = () => {
    sleeper.disable();
    console.log("nosleep off");
}
        window.convertArray = () => {
        Tone.start();
    //create a synth and connect it to the main output (your speakers)
    const synth = new Tone.Synth().toDestination();

    //play a middle 'C' for the duration of an 8th note
    synth.triggerAttackRelease("C4", "8n");

        };

// tone.js functions

// beep a sequence of tones
window.playToneSeq = (notes, interval) => {
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

// beep a chord

window.playChord = (notes, length) => {
    Tone.start();
    const synth = new Tone.PolySynth(Tone.Synth).toDestination();
    const now = Tone.now();
    notes.forEach(note => {
        synth.triggerAttack(note);

    });
    synth.triggerRelease(notes, now + length);
};

// play seq with piano
window.playSeq = async (notes, interval) => {
    
    if (!window.toneSampler) {
        window.toneSampler = new Tone.Sampler({
            urls: {
                "C4": "assets/C4v10.mp3",
                "D#4": "assets/Ds4v10.mp3",
                "F#4": "assets/Fs4v10.mp3",
                "A4": "assets/A4v10.mp3",
            },
            release: 1

        }).toDestination();
        await Tone.loaded()
    }
    const now = Tone.now();
    console.log(now);
    var offset = 0;
    notes.forEach(note => {
        window.toneSampler.triggerAttackRelease(note, 0.85, now + offset, 0.5);
        offset = offset + interval;
    });
}