	
	 pitchStart = (asm, method) =>{
		var cb = (note) => {
			console.log(note);
			DotNet.invokeMethod(asm, method, note);

		 }
		 globalThis.detector = globalThis.detector || new PitchDetect(cb);
		 globalThis.detector.start();
	}
	pitchStop = () => {
		console.log("stop pitch detect");
		globalThis.detector.stop();
    }
window.drawStaff = (divName, staffDef) => {
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
    stave.addClef(staffDef.clef);

    // Connect it to the rendering context and draw!
    stave.setContext(context).draw();
    if (staffDef.notes.length == 0)
        return;
    var notes = staffDef.notes.map(n => {
        var nt = new VF.StaveNote({ clef: staffDef.clef, keys: [n.note], duration: "q" });
        console.log("color " + n.color);
        nt.setStyle({ fillStyle: n.color, strokeStyle: n.color});
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
