"use strict";
class PitchDetect {
    constructor(cb) {
        this.rafID = 0;
        this.running = false;
        this.fftSize = 2048;
        this.threshHold = 0.01;
        this.candidate = 0;
        this.same = 0;
        this.currentNote = 0;
        this.lock = false;
        this.cb = cb;
        this.buffer = new Float32Array(2048);
    }
    start() {
        var that = this;
        that.audioContext = new AudioContext();
        navigator.mediaDevices.getUserMedia({ audio: true }).then(that.gotStream.bind(that));
    }
    stop() {
        var that = this;
        that.running = false;
        window.cancelAnimationFrame(that.rafID);
    }
    noteFromPitch(frequency) {
        var noteNum = 12 * (Math.log(frequency / 440) / Math.log(2));
        return Math.round(noteNum) + 69;
    }
    gotStream(stream) {
        // Create an AudioNode from the stream.
        var that = this;
        var mediaStreamSource = that.audioContext.createMediaStreamSource(stream);
        //console.log("stream");
        // Connect it to the destination.
        this.analyser = this.audioContext.createAnalyser();
        this.analyser.fftSize = that.fftSize;
        //console.log("str->an");
        mediaStreamSource.connect(this.analyser);
        that.running = true;
        this.runDetect();
    }
    autoCorrelate(buf, sampleRate) {
        // Implements the ACF2+ algorithm
        var that = this;
        var size = buf.length;
        var rms = 0;
        for (var i = 0; i < size; i++) {
            var val = buf[i];
            rms += val * val;
        }
        rms = Math.sqrt(rms / size);
        if (rms < that.threshHold) // not enough signal
            return -1;
        var r1 = 0, r2 = size - 1, thres = 0.2;
        for (var i = 0; i < size / 2; i++)
            if (Math.abs(buf[i]) < thres) {
                r1 = i;
                break;
            }
        for (var i = 1; i < size / 2; i++)
            if (Math.abs(buf[size - i]) < thres) {
                r2 = size - i;
                break;
            }
        buf = buf.slice(r1, r2);
        size = buf.length;
        var c = new Array(size).fill(0);
        for (var i = 0; i < size; i++)
            for (var j = 0; j < size - i; j++)
                c[i] = c[i] + buf[j] * buf[j + i];
        var d = 0;
        while (c[d] > c[d + 1])
            d++;
        var maxval = -1, maxpos = -1;
        for (var i = d; i < size; i++) {
            if (c[i] > maxval) {
                maxval = c[i];
                maxpos = i;
            }
        }
        var T0 = maxpos;
        var x1 = c[T0 - 1], x2 = c[T0], x3 = c[T0 + 1];
        var a = (x1 + x3 - 2 * x2) / 2;
        var b = (x3 - x1) / 2;
        if (a)
            T0 = T0 - b / (2 * a);
        return sampleRate / T0;
    }
    runDetect() {
        var that = this;
        that.analyser.getFloatTimeDomainData(this.buffer);
        var ac = that.autoCorrelate(that.buffer, that.audioContext.sampleRate);
        var note = that.noteFromPitch(ac);
        that.processNote(note);
        if (that.running)
            that.rafID = window.requestAnimationFrame(that.runDetect.bind(that));
    }
    processNote(note) {
        var that = this;
        var noteStrings = ["C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"];
        if (note > 0) {
            if (!that.lock && that.currentNote != note) {
                //currentNote = note;
                if (note == that.candidate) {
                    that.same++;
                    if (that.same == 6) {
                        //	innote=true;
                        that.currentNote = note;
                        var noteStr = noteStrings[note % 12];
                        var octave = Math.floor(note / 12) - 1;
                        console.log(noteStr + octave + " " + note);
                        that.cb(note);
                        that.lock = true;
                        that.candidate = 0;
                    }
                }
                else {
                    that.candidate = note;
                    that.same = 0;
                }
            }
            else {
            }
        }
        else {
            that.currentNote = 0;
            that.same = 0;
            that.lock = false;
        }
    }
}
