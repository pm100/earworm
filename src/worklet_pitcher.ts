import { Param } from "../node_modules/tone/build/esm/classes";

interface AudioWorkletProcessor {
    readonly port: MessagePort;
    process(
        inputs: Float32Array[][],
        outputs: Float32Array[][],
        parameters: Record<string, Float32Array>
    ): boolean;
}

declare var AudioWorkletProcessor: {
    prototype: AudioWorkletProcessor;
    new(options?: AudioWorkletNodeOptions): AudioWorkletProcessor;
};

declare function registerProcessor(
    name: string,
    processorCtor: (new (
        options?: AudioWorkletNodeOptions
    ) => AudioWorkletProcessor) & {
        parameterDescriptors?: { name: string }[];
    }
): undefined;



class PitchProcessor extends AudioWorkletProcessor {

    buffer!: Float32Array;
    bsize = 0;
    silence = 0.01;
    threshold = 0.2;
    lockCount = 3;
    candidate = 0;
    same = 0;
    currentNote = 0;
    lock = false;
    once = true;
    count = 0;
    anote = 440;
    samprate = 48000;

    static get parameterDescriptors() {
        return [
            { name: 'buffersize', defaultValue: 20, minValue: 1, maxValue: 1000 },
            { name: 'lockcount', defaultValue: 3, minValue: 1, maxValue: 1000 },
            { name: 'silence', defaultValue: 0.01, minValue: 0.001, maxValue: 1 },
            { name: 'threshold', defaultValue: 0.2, minValue: 0.001, maxValue: 1 },
            { name: 'afreq', defaultValue: 440, minValue: 1, maxValue: 10000 },
            { name: 'samprate', defaultValue: 48000, minValue: 1, maxValue: 1000000 },
        ];
    }

    constructor() {
        super();
        console.log("pitcher worklet");
    }

    process(inputs: Float32Array[][],
        outputs: Float32Array[][],
        parameters: Record<string, Float32Array>) :boolean {

        const input = inputs[0];
        const output = outputs[0];
        const left = input[0];
        const right = input[1];
        
        if (this.once) {
            this.once = false;
            this.bsize = parameters.buffersize[0] * 128;
            this.silence = parameters.silence[0];
            this.lockCount = parameters.lockcount[0];
            this.threshold = parameters.threshold[0];
            this.anote = parameters.afreq[0];
            this.samprate = parameters.samprate[0];
            this.buffer = new Float32Array(this.bsize);
            console.log(this.bsize, this.lockCount, this.silence, this.threshold);
        }

        for (let i = 0; i < 128; i++) { 
            var o = 0.5 * (left[i] + right[i]);
            this.buffer[this.count++] = o;
        }

        if (this.count >= this.bsize) {
            // we have enough data
            this.count = 0;
            var [fq, rms] = this.autoCorrelate(this.buffer, this.samprate);
            this.processNote(fq, rms);
        }
        return true;
    }

    private processNote(frequency: number, volume: number): void {
        var that = this;
        var noteStrings = ["C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"];
        // -1 means silence
        if (frequency > 0) {
            var note = this.noteFromPitch(frequency);
            //console.log(note);
            if (!that.lock && that.currentNote != note) {
                if (note == that.candidate) {
                    that.same++;
                    if (that.same == this.lockCount) {
                        that.currentNote = note;
                        var noteStr = noteStrings[note % 12];
                        var octave = Math.floor(note / 12) - 1;
                        console.log(noteStr + octave + " " + note + " " + volume);
                        that.lock = true;
                        that.candidate = 0;
                        // we have a note, tell the caller
                        this.port.postMessage({ n: note, f: frequency });
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
    // converts frequency to midi note number

    private noteFromPitch(frequency: number): number {
        var noteNum = 12 * (Math.log(frequency / this.anote) / Math.log(2));
        return Math.round(noteNum) + 69;
    }

    private autoCorrelate(buf: Float32Array, sampleRate: number): [number, number] {
        // Implements the ACF2+ algorithm
        var that = this;
        var size = buf.length;
        var rms = 0;
        for (var i = 0; i < size; i++) {
            var val = buf[i];
            rms += val * val;
        }
        rms = Math.sqrt(rms / size);
        if (rms < that.silence) // not enough signal
            return [0, -1];
        var r1 = 0, r2 = size - 1, thres = this.threshold;
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
        return [sampleRate / T0, rms];
    }

}

registerProcessor('pitch-processor', PitchProcessor);