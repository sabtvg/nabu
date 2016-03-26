function getPath(node) {
    var ret = [];

    while (node.id != arbolPersonal.raiz.id) {
        ret.push(node);
        node = node.parent;
    }
    return ret;
}

function showPanel() {
    //pido propuestas al servidor
    var node = selectedNode;
    if (node) {
        getHttp("doArbol.aspx?actn=getPropuestas&id=" + node.id + "&arbol=" + arbolPersonal.nombre,
            function (data) {
                recibirPropuestas(data);
                showPanel2();
            }
        );
    }
}

function showPanelIzq() {
    //activo el izq
    var panelIzq = document.getElementById("panelIzq");
    var panelIzq2 = document.getElementById("panelIzq2");
    if (panelIzq.style.visibility == "hidden") {
        panelIzq.style.visibility = "visible";
        efectoLeft(panelIzq, 0, -400 * scale, 20, TWEEN.Easing.Cubic.Out);
    }

    panelIzq.style.top = "220px";
    panelIzq.style.width = 400 * scale + "px";
    panelIzq2.style.width = 400 * scale + "px";
    panelIzq.style.height = 550 * scale + "px";

    fillPanel(panelIzq2);
}

function showPanelDer() {
    //activo el derecho
    var panelDer = document.getElementById("panelDer");
    var panelDer2 = document.getElementById("panelDer2");
    if (panelDer.style.visibility == "hidden") {
        panelDer.style.visibility = "visible";
        efectoLeft(panelDer, 0, window.innerWidth, window.innerWidth - 400 * scale - 40, TWEEN.Easing.Cubic.Out);
    }

    panelDer.style.top = "220px";
    panelDer.style.width = 400 * scale + "px";
    panelDer2.style.width = 400 * scale + "px";
    panelDer.style.height = 550 * scale + "px";

    fillPanel(panelDer2);
}

function fillPanel(panel) {
    //resuelvo el titulo del documento
    var node = selectedNode;
    var path = getPath(node);
    var titulo = '';
    if (path.length > 0) {
        var propuesta = propuestas[path[0].id];
        titulo = propuesta.titulo;
    }

    //escribo propuesta
    var s = "<div class='titulo1'>" + titulo + "</div><br>";
    var propuesta = propuestas[node.id];
    for (var i in propuesta.textos) {
        var textoTema = propuesta.textos[i];
        s += "<div class='tema'>" + textoTema.titulo + "</div><br>";
        s += "<div class='texto' style='width:" + (400 * scale - 30) + "px;'>" + textoTema.texto.replace(/\n/g, "<br>") + "</div><br><br>";
    }
    s += HTMLFlores(node);

    //escribo comentarios
    if (propuesta.comentarios.length > 0) s += "<br>Comentarios:"
    for (var t in propuesta.comentarios) {
        //escribo comentario de propuesta
        s += "<div class='comentario' style='overflow: auto;width:" + (window.innerWidth * 0.2 - 40) + "px'>" + propuesta.comentarios[t].replace(/\n/g, "<br>") + "</div>";
    }

    panel.innerHTML = s;
}

function showPanel2() {
    var panelIzq = document.getElementById("panelIzq");
    var panelDer = document.getElementById("panelDer");
    var node = selectedNode;
    var modelo = getModelo(node.modeloID);

    if (documentoModeOn && node.depth > 0) {
        //enseño panel
        if (tengoPropuestas(node)) {
            //puedo mostrar el documento

            if (arbolPersonal.simulacion) {
                //panel derecho
                showPanelDer();
            }
            else if (node.x > 90) {
                //panel izquierdo
                showPanelIzq();
                //desactivo el izq
                hidePanelDer();
            }
            else {
                //panel derecho
                showPanelDer();
                //desactivo el izq
                hidePanelIzq();
            }
        }
        else {

        }
    }
    else {
        //oculto
        hidePanelIzq();
        hidePanelDer();
    }
}

function hidePanelIzq() {
    var panelIzq = document.getElementById("panelIzq");
    if (panelIzq.style.visibility == "visible") {
        efectoLeft(panelIzq, 0, 20, -450 * scale, TWEEN.Easing.Cubic.Out, function () {
            document.getElementById("panelIzq").style.visibility = "hidden";
        });
    }
}

function hidePanelDer() {
    var panelDer = document.getElementById("panelDer");
    if (panelDer.style.visibility == "visible") {
        efectoLeft(panelDer, 0, window.innerWidth - 400 * scale - 40, window.innerWidth, TWEEN.Easing.Cubic.Out, function () {
            document.getElementById("panelDer").style.visibility = "hidden";
            document.getElementById("panelDer").style.left = "0px"; //evita que aparezcan las barras de scroll
        });
    }
}

function tengoPropuestas(node) {
    //veo si tengo las propuestas de este nodo y sus padres
    var parent = node;
    while (parent.id != arbolPersonal.raiz.id) {
        if (!propuestas[parent.id])
            return false; //me falta esta
        parent = parent.parent;
    }
    return true; //tengo todas
}

function recibirPropuestas(data) {
    var nuevasPropuestas = JSON.parse(data);
    for (var i in nuevasPropuestas) {
        var nop = nuevasPropuestas[i];
        propuestas[nop.nodoID] = nop;
    }
}

function HTMLFlores(node) {
    var ret;
    ret = "<div class='votos'>";
    ret += "<img src='res/icono.png'>";
    ret += "&nbsp;" + node.totalFlores;
    ret += "</div>";
    return ret;
}