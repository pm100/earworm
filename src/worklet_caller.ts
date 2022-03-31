interface PitchDetectOptions {
    buffersize?: number,
    lockcont?: number,
    silence?: number,
    threshold?: number,
    afeq?:number,
}
interface Window {
    webkitAudioContext: typeof AudioContext
}
class PitchDetectWorklet {
    audioContext!: AudioContext;
    running: boolean = false;
    pitchWorklet!: AudioWorkletNode;
    cb: (note: number, freq:number) => void;
    options: PitchDetectOptions;
    stream!: MediaStream;

    constructor(options: PitchDetectOptions, cb: (note: number, freq:number) => void) {
        this.cb = cb;
        this.options = options;
    }
    private setParam(name: string, dflt: number) {
        var param = this.pitchWorklet.parameters.get(name);
        if (param) {
            const val = this.options[name as keyof PitchDetectOptions] || dflt;
            param.setValueAtTime(val, 0);
        }
        else
            console.log("error setting " + name);
    }
    start = async () => {
        try {
            const AudioContextConstructor =
                window.AudioContext || window.webkitAudioContext;
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
            }
            mediaStreamSource.connect(this.pitchWorklet);
            this.pitchWorklet.connect(this.audioContext.destination);
            this.running = true;
        }
        catch (e) {
            console.log(e);
        }
    }
    stop = (): void => {
        console.log("stop");
        if (this.running) {
            console.log("stop2");
            this.stream.getTracks().forEach((track) => track.stop());
            this.audioContext.close();
            this.running = false;
        }
    }
}





