	
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
