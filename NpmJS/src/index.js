import * as  Tone from 'tone';
import NoSleep from 'nosleep.js';
import Vex from 'vexflow';

// Vex engine directly exposed

window.Vex = Vex;

// no-sleep functions

window.noSleep = () => {
    window.sleeper = new NoSleep();
   
    sleeper.enable();
    console.log("nosleep on");

}
window.allowSleep = () => {
    sleeper.disable();
    console.log("nosleep off");
}


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

window.playToneChord = (notes, length) => {
    Tone.start();
    const synth = new Tone.PolySynth(Tone.Synth).toDestination();
    const now = Tone.now();
    notes.forEach(note => {
        synth.triggerAttack(note);

    });
    synth.triggerRelease(notes, now + length);
};


window.initSampler =async ()=> {
    if (!window.toneSampler) {
        window.toneSampler = new Tone.Sampler({
            urls: {
                "C4": "assets/C4.mp3",
                "D#4": "assets/Ds4.mp3",
                "F#4": "assets/Fs4.mp3",
                "A4": "assets/A4.mp3",
            },
            release: 1

        }).toDestination();
        await Tone.loaded();
    }
}
// play seq with piano
window.playPianoSeq = async (notes, interval) => {
    
    await window.initSampler();
    const now = Tone.now();
    console.log(now);
    var offset = 0;
    notes.forEach(note => {
        window.toneSampler.triggerAttackRelease(note, 0.85, now + offset, 0.75);
        offset = offset + interval;
    });
}
// play chord with piano
window.playPianoChord = async (notes, interval) => {
    await window.initSampler();
    const now = Tone.now();
    notes.forEach(note => {
        window.toneSampler.triggerAttackRelease(note, 1.5, now, 0.5);
   });
}
window.drawStaff = (divName, staffDef) => {
    const VF = Vex.Flow;
    // Create an SVG renderer and attach it to the DIV element named in divName.
    const div = document.getElementById(divName);
    div.innerHTML = "";
    var renderer = new VF.Renderer(div, VF.Renderer.Backends.SVG);

    // Configure the rendering context.
    renderer.resize(500, 200);
    var context = renderer.getContext();
    context.setFont("Arial", 10, "").setBackgroundFillStyle("#eed");

    // Create a stave of width 400 at position 10, 40 on the canvas.
    var stave = new VF.Stave(10, 40, 400);

    // Add a clef and time signature.
    stave.addClef(staffDef.clef);
    stave.addKeySignature(staffDef.key);
    console.log("keysig=" + staffDef.key);
    // Connect it to the rendering context and draw!
    stave.setContext(context).draw();
    if (staffDef.notes.length == 0)
        return;
    var notes = staffDef.notes.map(n => {
        console.log("acc=", n.accidental);
        console.log("note=", n.note);

        var nt = new VF.StaveNote({ clef: staffDef.clef, keys: [n.note], duration: "q", auto_stem: true });

        nt.setStyle({ fillStyle: n.color, strokeStyle: n.color });
        if (n.accidental == 1)
            nt.addAccidental(0, new VF.Accidental("#"));
        if (n.accidental == 2)
            nt.addAccidental(0, new VF.Accidental("b"));
        if (n.accidental == 3)
            nt.addAccidental(0, new VF.Accidental("n"));

        return nt;
    });
    // Create a voice in x/4 and add above notes
    var voice = new VF.Voice({ num_beats: notes.length, beat_value: 4 });
    voice.addTickables(notes);

    // Format and justify the notes to 400 pixels.
    var formatter = new VF.Formatter().joinVoices([voice]).format([voice], 50 * notes.length);

    // Render voice
    voice.draw(context, stave);
}  
