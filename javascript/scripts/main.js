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

        this.audioQueue.enqueue({ "content": this.audio, "id": id });

        elements[id]['inQueue'] = true;

        if (!this.audioQueue.isEmpty()) {
            this.run()
        }
    }
    endedEventListener(e, id) {
        let audio = e.audioQueue.dequeue()
        e.documentFragment.removeChild(audio.content);
        console.log("Removendo audio da fila");      

        elements[id]['inQueue'] = false;
        
        e.handler(id);
    }
    handler() {
        console.log("Verificando próximos audios da fila");
        if (this.audioQueue.isEmpty()) {
            return;
        }   

        let next = this.audioQueue.peek();

        if (isValid(next.id)) {
            console.log("Audio executado recentemente");
            this.audioQueue.dequeue();
            
            this.handler();
        } else {
            this.run();
        }
    }
    run() {
        console.log("Executando próximo audio da fila");
        var audio = this.audioQueue.peek();
        audio.content.play();
    }
}

const soundHandler = new SoundHandler();
const actionQueue = new Queue();

const clickElements = [ 'h1', 'h2', 'h3', 'p', 'address', 'footer', 'section', 'li', 'hr', 'ol', 'ul', 'td', 'tr', 'span' ]
const hoverElements = [ 'a', 'nav', 'button', 'img', 'span', 'input' ]

$(document).ready(function () {
    
    clickElements.forEach(function (v) {
        createClickAction(v);
    }); 

    hoverElements.forEach(function (v) {
        createHoverAction(v);
    });     
});

function createHoverAction(type) {
    var hoverTimeout;

    $(type).hover(function (event) {
        hoverTimeout = setTimeout(function(event) {
            handle(event.target);            
        }, 1500, event);
    }, function() {
        clearTimeout(hoverTimeout);        
    });
}

function createClickAction(type) {
    $(type).on('click', function (event) {
        handle(event.target);
    });
}

const elements = [];

const uniqId = (() => {
    let i = 0;
    return () => {
        return i++;
    }
})();

const actions = {    
    "img": function (target) {
        let text = $(target).attr('alt');

        if (!isValid(id, 10)) {
            console.log('Audio executado a menos de 10seg atrás');
            return;
        }

        request(text, $(target).attr('p_id'));
    },
    "input": function (target) {

        if (!isValid(id, 15)) {
            console.log('Audio executado a menos de 15seg atrás');
            return;
        }

        request($(target).val(), $(target).attr('p_id'));
    },
    "default": function (target) {

        var id = $(target).attr('p_id');

        if (!isValid(id)) {
            console.log('Audio executado a menos de 60seg atrás');
            return;
        }

        if ($(target).is(":parent")) {
            actions['parent'](target);
        } else {
            request($(target).text(), $(target).attr('p_id'));
        }
    },
    "parent": function (target) {
        if ($(target).text() !== '') {
            request($(target).text(), $(target).attr('p_id'));
        }
        else if ($(target).children('li').length > 0) {
            $(target).find('li').each(function (i, li) {
                handle(li);
            })
        }        
        else {            
            let text = '';
            let t = target;

            counter = 0;

            while (counter < 5) {                        
                text = $(t).next().text();

                if (text !== '') {
                    break;
                }

                t = $(t).next();

                counter++;
            }

            if (text !== '') {
                handle(t);
            }
        }
    }
}

function performAction(type, target) {
    if (elements[$(target).attr('p_id')]['inQueue']) {        
        console.log('audio já está na fila');        
    }   
    else if (actions[type]) {
        actions[type](target);
    }
    else {
        actions['default'](target);
    }
}

function isValid(id, seconds = 60) {
    return elements[id]['lastInteraction'].setSeconds(elements[id]['lastInteraction'].getSeconds() + seconds) > new Date($.now());
}

function handle(target) {
    var id = $(target).attr('p_id');

    if (!id) {
        id = uniqId();
        $(target).attr('p_id', id);

        elements[id] = {
            'inQueue': false,
            'lastInteraction': new Date($.now())
        }
    }
    else {
        elements[id]['lastInteraction'] = new Date($.now())
    }

    actionQueue.enqueue(performAction);    
    handleQueue(target.localName, target);
}

var queueIsRunning = false;

async function handleQueue(type, target) {
    if (!queueIsRunning) {    
        queueIsRunning = true;

        var f = actionQueue.dequeue();

        f(type, target);
        await new Promise(resolve => setTimeout(resolve, 1000));
        queueIsRunning = false;

        if (!actionQueue.isEmpty()) {
            handleQueue(type, target);
        }
    }
}

function request(text, id) {

    if (text === '') {
        return;
    }    

    $.ajax({
        method: "POST",
        url: 'https://eep-tcc-api-2022.azurewebsites.net/api/v1/texttospeech', 
        data: JSON.stringify({ "text": text, }),
        contentType: 'application/json'        
    }).done(function(result) {
        soundHandler.add("data:audio/wav;base64," + result.content, id) 
    }).fail(function(jqXHR, textStatus, msg) {
        alert(msg);
    });
}