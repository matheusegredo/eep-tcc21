Queue = require('./queue.js');

class SoundHandler {
    constructor() {
        this.audioQueue = new Queue();
        this.audio = new Audio()

        this.documentFragment = document.createDocumentFragment();
    }

    add(src) {
        this.audio = new Audio(src);
        this.documentFragment.appendChild(this.audio);
        this.audio.addEventListener('ended', function () {
            df.removeChild(audio);
            console.log("Removendo audio da fila");
            this.audioQueue.dequeue()
            this.handler();
        });
        
        console.log("Adicionando audio na fila");
        this.audioQueue.enqueue(audio);

        if (!this.audioQueue.isEmpty()) {
            this.run()
        }            
    }
    handler() {
        console.log("Verificando próximos audios da fila");
        if (this.audioQueue.isEmpty()) {
            return;
        }   

        this.run();
    }
    run() {
        console.log("Executando próximo audio da fila");
        var audio = this.audioQueue.peek();
        audio.play();
    }
}

module.exports = Sound



