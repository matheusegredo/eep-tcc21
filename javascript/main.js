class Queue {
    constructor() {
        this.elements = [];
    }
    enqueue(e) {
        this.elements.push(e);
    }
    dequeue() {
        return this.elements.shift();
    }
    isEmpty() {
        return this.elements.length == 0;
    }
    peek() {
        return !this.isEmpty() ? this.elements[0] : undefined;
    };
}

class SoundHandler {
    constructor() {
        this.audioQueue = new Queue();
        this.audio = new Audio()

        this.documentFragment = document.createDocumentFragment();
    }

    add(src) {
        this.audio = new Audio(src);
        this.documentFragment.appendChild(this.audio);
        this.audio.addEventListener('ended', this.endedEventListener.bind(event, this));        
        console.log("Adicionando audio na fila");      

        if (this.audioQueue.isEmpty()) {
            console.log("Fila está vazia");
            this.audioQueue.enqueue(this.audio);
            this.run()
        } else {
            this.audioQueue.enqueue(this.audio);
        }

    }
    endedEventListener(e) {
        let audio = e.audioQueue.dequeue()
        e.documentFragment.removeChild(audio);
        console.log("Removendo audio da fila");      
        
        e.handler();
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

const soundHandler = new SoundHandler();

$(document).ready(function () {
    $(document).click(function(event) {
        TextToSpeechRequest($(event.target).text())
   });
});

function TextToSpeechRequest(text) {
    $.ajax({
        method: "POST",
        url: 'https://localhost:44377/api/v1/texttospeech', 
        data: JSON.stringify({ "text": text, }),
        contentType: 'application/json',        
    }).done(function(result) {
        soundHandler.add("data:audio/wav;base64," + result.base64Fila) 
    }).fail(function(jqXHR, textStatus, msg) {
        alert(msg);
    });
}


