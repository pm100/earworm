"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
class PitchDetectWorklet {
    constructor(cb) {
        this.running = false;
        this.start = () => __awaiter(this, void 0, void 0, function* () {
            try {
                const AudioContextConstructor = window.AudioContext || window.webkitAudioContext;
                this.audioContext = new AudioContextConstructor();
                yield this.audioContext.audioWorklet.addModule('js/worklet_pitcher.js');
                var stream = yield navigator.mediaDevices.getUserMedia({ audio: true });
                var mediaStreamSource = this.audioContext.createMediaStreamSource(stream);
                this.pitchWorklet = new AudioWorkletNode(this.audioContext, 'pitch-processor');
                this.setParam('buffersize', 10);
                this.setParam('lockcount', 3);
                this.setParam('silence', 0.01);
                this.setParam('threshold', 0.2);
                this.pitchWorklet.port.onmessage = (ev) => {
                    this.cb(ev.data);
                };
                mediaStreamSource.connect(this.pitchWorklet);
                this.running = true;
            }
            catch (e) {
                console.log(e);
            }
        });
        this.stop = () => {
            if (this.running) {
                this.audioContext.close();
                this.running = false;
            }
        };
        this.cb = cb;
    }
    setParam(name, val) {
        var param = this.pitchWorklet.parameters.get(name);
        if (param)
            param.setValueAtTime(val, 0);
        else
            console.log("error setting " + name);
    }
}
