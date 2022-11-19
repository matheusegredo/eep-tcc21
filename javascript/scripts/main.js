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

    add (src, id) {

        this.audio = new Audio(src);
        this.documentFragment.appendChild(this.audio);
        this.audio.addEventListener('ended', this.endedEventListener.bind(event, this, id));        
        console.log("Adicionando audio na fila");      

        this.audioQueue.enqueue(this.audio);

        elements[id]['inQueue'] = true;

        if (!this.audioQueue.isEmpty()) {
            this.run()
        }
    }
    endedEventListener(e, id) {
        let audio = e.audioQueue.dequeue()
        e.documentFragment.removeChild(audio);
        console.log("Removendo audio da fila");      

        elements[id]['inQueue'] = false;
        
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
    $(document).on('click', function (event) {
        handle(event.target);
    });
});

const elements = [];

const uniqId = (() => {
    let i = 0;
    return () => {
        return i++;
    }
})();

const actions = {
    "button": function (target, information) {
        if (information['isFirstClick']) {
            target.preventDefault();

            information['isFirstClick'] = false;

            updateInteraction(information);
            
            request($(target).text(), $(target).attr('p_id'));
        }
    },
    "p": function (target, information) {

        updateInteraction(information);

        request($(target).text(), $(target).attr('p_id'));
    },
    "h1": null
}

function handle(target) {
    var id = $(target).attr('p_id');

    if (!id) {
        id = uniqId();
        $(target).attr('p_id', id);

        elements[id] = {
            'isFirstClick': true,
            'lastAudioRequest': new Date($.now()),
            'inQueue': false
        }
    }

    actions[target.localName](target, elements[id]);
}

function updateInteraction(information) {
    information['lastAudioRequest'] = new Date($.now());
}

function shouldRun(information, time) {
    return new Date($.now() - time) > information['lastAudioRequest'];
}

function request(text, id) {

    if (elements[id]['inQueue']) {
        if (shouldRun(elements[id], 5000)) {
            elements[id]['inQueue'] = false;
        }
        else {
            console.log('audio já está na fila');
            return;
        }
    }

    $.ajax({
        method: "POST",
        url: 'https://eep-tcc-api-2022.azurewebsites.net/api/v1/texttospeech', 
        data: JSON.stringify({ "text": text, }),
        contentType: 'application/json',        
    }).done(function(result) {
        soundHandler.add("data:audio/wav;base64," + result.content, id) 
    }).fail(function(jqXHR, textStatus, msg) {
        alert(msg);
    });
}


