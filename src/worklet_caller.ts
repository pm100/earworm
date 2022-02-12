class PitchDetectWorklet {
    audioContext!: AudioContext;
    running: boolean = false;
    pitchWorklet!: AudioWorkletNode;
    cb: (note: number) => void;

    constructor(cb: (note: number) => void) {
        this.cb = cb;
    }
    private setParam(name: string, val: number) {
        var param = this.pitchWorklet.parameters.get(name);
        if (param)
            param.setValueAtTime(val, 0);
        else
            console.log("error setting " + name);
    }
    start = async () => {
        try {
            const AudioContextConstructor =
                window.AudioContext || window.webkitAudioContext;
            this.audioContext = new AudioContextConstructor();

            await this.audioContext.audioWorklet.addModule('js/worklet_pitcher.js');
            var stream = await navigator.mediaDevices.getUserMedia({ audio: true });

            var mediaStreamSource = this.audioContext.createMediaStreamSource(stream);
            this.pitchWorklet = new AudioWorkletNode(this.audioContext, 'pitch-processor');

            this.setParam('buffersize', 10);
            this.setParam('lockcount', 3);
            this.setParam('silence', 0.01);
            this.setParam('threshold', 0.2);

            this.pitchWorklet.port.onmessage = (ev) => {
                this.cb(ev.data);
            }
            mediaStreamSource.connect(this.pitchWorklet);
            this.running = true;
        }
        catch (e) {
            console.log(e);
        }
    }
    stop = (): void => {
        if (this.running) {
            this.audioContext.close();
            this.running = false;
        }
    }
}





