	
	 pitchStart = (asm, method, buff, sil, thr, lock) =>{
		var cb = (note) => {
			console.log(note);
			DotNet.invokeMethod(asm, method, note);

         }
         try {
             var options = {
                 buffersize: buff,
                 silence: sil,
                 threshold: thr,
                 lockcount:lock
             };
             globalThis.detector = globalThis.detector || new PitchDetectWorklet(options,cb);
             globalThis.detector.start();
         }
         catch (e) {
             console.log(e);
         }
	}
	pitchStop = () => {
		console.log("stop pitch detect");
		globalThis.detector.stop();
}

window.MyCacheClear = async () => {
    const cacheKeys = await caches.keys();
    await Promise.all(cacheKeys.map(key => caches.delete(key)));

}
//window.drawStaff = (divName, staffDef) => {
//    const VF = Vex.Flow;
//    // Create an SVG renderer and attach it to the DIV element named in divName.
//    const div = document.getElementById(divName);
//    div.innerHTML = "";
//    var renderer = new VF.Renderer(div, VF.Renderer.Backends.SVG);

//    // Configure the rendering context.
//    renderer.resize(500, 200);
//    var context = renderer.getContext();
//    context.setFont("Arial", 10, "").setBackgroundFillStyle("#eed");

//    // Create a stave of width 400 at position 10, 40 on the canvas.
//    var stave = new VF.Stave(10, 40, 400);

//    // Add a clef and time signature.
//    stave.addClef(staffDef.clef);
//    stave.addKeySignature(staffDef.key);
//    console.log("keysig=" + staffDef.key);
//    // Connect it to the rendering context and draw!
//    stave.setContext(context).draw();
//    if (staffDef.notes.length == 0)
//        return;
//    var notes = staffDef.notes.map(n => {
//        console.log("acc=", n.accidental);
//        console.log("note=", n.note);

//        var nt = new VF.StaveNote({ clef: staffDef.clef, keys: [n.note], duration: "q" ,auto_stem:true});

//        nt.setStyle({ fillStyle: n.color, strokeStyle: n.color });
//        if (n.accidental == 1)
//            nt.addAccidental(0, new VF.Accidental("#"));
//        if (n.accidental == 2)
//            nt.addAccidental(0, new VF.Accidental("b"));
//        if (n.accidental == 3)
//            nt.addAccidental(0, new VF.Accidental("n"));

//        return nt;
//    });
//    // Create a voice in x/4 and add above notes
//    var voice = new VF.Voice({ num_beats: notes.length, beat_value: 4 });
//    voice.addTickables(notes);

//    // Format and justify the notes to 400 pixels.
//    var formatter = new VF.Formatter().joinVoices([voice]).format([voice], 50 * notes.length);

//    // Render voice
//    voice.draw(context, stave);
//}  
