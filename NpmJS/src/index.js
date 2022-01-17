import * as  Tone from 'tone';
import NoSleep from 'nosleep.js';
import Vex from 'vexflow';
window.Vex = Vex;

window.drawStaffxx = (divName, noteList, noteColors) => {
    const VF = Vex.Flow;
    // Create an SVG renderer and attach it to the DIV element named "vf".
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
    stave.addClef("treble").addTimeSignature("4/4");

    // Connect it to the rendering context and draw!
    stave.setContext(context).draw();
    if (noteList.length == 0)
        return;
    var colorIdx = 0;
    var notes = noteList.map(n => {
        var nt = new VF.StaveNote({ clef: "treble", keys: [n], duration: "q" });
        console.log("color " + noteColors[colorIdx]);
        nt.setStyle({ fillStyle: noteColors[colorIdx++] });
        return nt;
    });
    // Create a voice in 4/4 and add above notes
    var voice = new VF.Voice({ num_beats: notes.length, beat_value: 4 });
    voice.addTickables(notes);

    // Format and justify the notes to 400 pixels.
    var formatter = new VF.Formatter().joinVoices([voice]).format([voice], 100 * notes.length);

    // Render voice
    voice.draw(context, stave);
}  



window.noSleep = () => {
    window.sleeper = new NoSleep();
    console.log("nosleep on");
    sleep.emable();

}
window.allowSleep = () => {
    sleeper.disable();
}
        window.convertArray = () => {
        Tone.start();
    //create a synth and connect it to the main output (your speakers)
    const synth = new Tone.Synth().toDestination();

    //play a middle 'C' for the duration of an 8th note
    synth.triggerAttackRelease("C4", "8n");

        };


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

