"use strict";
class PitchDetectWorklet {
    constructor(options, cb) {
        this.running = false;
        this.start = async () => {
            try {
                const AudioContextConstructor = window.AudioContext || window.webkitAudioContext;
                this.audioContext = new AudioContextConstructor();
                console.log(this.audioContext);
                console.log(this.audioContext.audioWorklet);
                await this.audioContext.audioWorklet.addModule('js/worklet_pitcher.js');
                console.log("after add worklet");
                this.stream = await navigator.mediaDevices.getUserMedia({ audio: true });
                console.log(this.stream);
                var mediaStreamSource = this.audioContext.createMediaStreamSource(this.stream);
                this.pitchWorklet = new AudioWorkletNode(this.audioContext, 'pitch-processor');
                console.log(this.pitchWorklet);
                this.setParam('buffersize', 10);
                this.setParam('lockcount', 3);
                this.setParam('silence', 0.01);
                this.setParam('threshold', 0.2);
                this.setParam('afreq', 440);
                this.setParam('samprate', this.audioContext.sampleRate);
                console.log(this.audioContext.sampleRate);
                this.pitchWorklet.port.onmessage = (ev) => {
                    this.cb(ev.data.n, ev.data.f);
                };
                mediaStreamSource.connect(this.pitchWorklet);
                this.running = true;
            }
            catch (e) {
                console.log(e);
            }
        };
        this.stop = () => {
            console.log("stop");
            if (this.running) {
                console.log("stop2");
                this.stream.getTracks().forEach((track) => track.stop());
                this.audioContext.close();
                this.running = false;
            }
        };
        this.cb = cb;
        this.options = options;
    }
    setParam(name, dflt) {
        var param = this.pitchWorklet.parameters.get(name);
        if (param) {
            const val = this.options[name] || dflt;
            param.setValueAtTime(val, 0);
        }
        else
            console.log("error setting " + name);
    }
}
