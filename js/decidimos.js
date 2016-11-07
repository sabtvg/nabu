
function doDecidimos() {
    if (arbolPersonal.podado)
        //el arbol esta podado, lo pido de nuevo
        getHttp("doDecidimos.aspx?actn=getArbolPersonal&email=" + arbolPersonal.usuario.email + "&grupo=" + arbolPersonal.nombre,
            function (data) {
                recibirArbolPersonal(data);
                doDecidimos();
            });
    else {
        //menu ppal
        if (visual.level == 1)
            document.getElementById("menuppal").style.visibility = "hidden";
        else
            efectoOpacity(document.getElementById("menuppal"), 0, 1, 0, TWEEN.Easing.Cubic.Out, function () { document.getElementById("menuppal").style.visibility = "hidden"; });

        document.getElementById("menuppal").style.visibility = "hidden";
        document.getElementById("panelGrupo").style.visibility = 'hidden';

        //panel consenso
        document.getElementById("panelConsenso").style.visibility = 'visible';

        //joystick
        document.getElementById("joystick").style.visibility = 'visible';
        document.getElementById("joystick").style.top = (window.innerHeight - 160) + 'px';

        //objetivo
        var objetivo = document.getElementById("objetivo");
        objetivo.innerHTML = arbolPersonal.objetivo;
        objetivo.style.visibility = 'visible';

        //flores
        timerFlores = setInterval(rotarFlores, 200);

        //dibujo el arbol
        if (svgArbol == null)
            crearArbol();
        else {
            dibujarArbol(arbolPersonal.raiz);
            document.getElementById("arbol").style.visibility = 'visible';
            document.getElementById("arbol").style.top = '0px';
            document.getElementById("arbol").style.left = '0px';
        }

        actualizarDatosConsenso();

        //fijo intervalo de actualizacion del arbol
        timerArbol = setInterval(pedirArbol, refreshArbolInterval);

        estado = 'decidimos';
        clearInterval(timerCiclo);
    }
}

function seleccionarModelo() {
    //opciones de modelos de documentos
    if (getFloresDisponibles().length == 0)
        msg("No tienes flores disponibles");
    else {
        var list = "<table>";

        //si no hay manifiesto solo permito crear modelo Manifiesto
        for (var i in modelos) {
            var modelo = modelos[i];
            if (modelo.activo && 
                ((arbolPersonal.URLEstatuto == "" && modelo.id=='nabu.modelos.Manifiesto') || arbolPersonal.URLEstatuto != ""))
                list += "<tr><td class='btn' style='text-align: center; width: 300px;' onclick='seleccionarModelo2(\"" + modelo.id + "\");'>" + modelo.nombre + "</td></tr>";
        }
        list += "</table>";

        document.getElementById("modelosList").innerHTML = list;
        document.getElementById("modelos").style.visibility = "visible";
    }
}

function seleccionarModelo2(modeloID) {
    document.getElementById("modelos").style.visibility = "hidden";
    selectedNode.modeloID = modeloID; //se lo pongo temporalmente a la raiz
    doVerDocumento();
}

function recibirArbolPersonal(data) {
    var msgDiv = document.getElementById("msgDiv");

    if (data.substring(0, 6) == "Error=") {

        //ha habido un error
        msg(data.substring(6));
    }
    else {
        //actualizo el arbol recibido
        arbolPersonal = JSON.parse(data);

        if (arbolPersonal.URLEstatuto != "") {
            document.getElementById("ppal9").src = "res/documentos/manifiesto.png";
        }
        else {
            document.getElementById("ppal9").src = "res/noManifiesto.png";
        }

        var objetivo = document.getElementById("objetivo");
        objetivo.innerHTML = arbolPersonal.objetivo;
        
        //si se ha creado un nuevo nodo lo selecciono
        if (arbolPersonal.nuevoNodoID != 0) {
            selectedNode = getNodo(arbolPersonal.nuevoNodoID);
        }

        dibujarArbol(selectedNode);

        //condicion de consenso
        actualizarDatosConsenso();

        //timestamp
        lastArbolRecibidoTs = (new Date()).getTime();

        //flores disponibles
        document.getElementById("floresDisponibles").innerHTML = getFloresDisponibles().length;

        //reactivo evento resize por si esta descativado
        reload = true;
    }
}

function doToggleFlor() {
    var d = selectedNode;
    if (d.totalFlores == 1 && tieneFlor(d) && !d.consensoAlcanzado) {
        //quitara la ultima flor de un ultimo nodo
        //aviso que caera
        document.getElementById("ultimaFlor").style.top = (window.innerHeight / 2 - 94).toFixed(0) + 'px';
        document.getElementById("ultimaFlor").style.left = (window.innerWidth / 2 - 150).toFixed(0) + 'px';
        document.getElementById("ultimaFlor").style.visibility = "Visible";
        hidePanelIzq();
        hidePanelDer();
    }
    else
        doToggleFlor2();
}

function doToggleFlor2() {
    var d = selectedNode;
    if (!tieneFlor(d)) d.totalFlores += 1;

    if (d.totalFlores == 1 && tieneFlor(d) && !d.consensoAlcanzado) {
        //quitara la ultima flor de un ultimo nodo
        //quito seleccionado y apago menu
        doMousedown();
        hidePanelIzq();
        hidePanelDer();
    }
    else if (d.consensoAlcanzado) {
        //quitara la ultima flor de un ultimo nodo
        //quito seleccionado y apago menu
        doMousedown();
        hidePanelIzq();
        hidePanelDer();
    }

    //pido nuevo arbol
    getHttp("doDecidimos.aspx?actn=toggleflor&email=" + arbolPersonal.usuario.email + "&id=" + d.id + "&x=" + d.x0 + "&grupo=" + arbolPersonal.nombre, recibirArbolPersonal);
}

function doCerrarDocumento() {
    efectoTop(document.getElementById("documento"), 0, 20, -window.innerHeight, TWEEN.Easing.Cubic.In);
    preguntarAlSalir = false;
}

function doRevisar() {
    //leo los textos del documento y los envio al servidor
    var node = selectedNode;

    //envio
    getHttp("doDecidimos.aspx?actn=revisar&modelo=" + node.modeloID + "&email=" + arbolPersonal.usuario.email + "&id=" + node.id + "&grupo=" + arbolPersonal.nombre + "&width=" + (window.innerWidth - 80),
        function (data) {
            //muestro
            document.getElementById("documento").innerHTML = data;
        });
}

function doPrevista() {
    //leo los textos del documento y los envio al servidor
    var node = selectedNode;
    var post = getPost(document);

    //envio
    postHttp("doDecidimos.aspx?actn=prevista&modelo=" + node.modeloID + "&email=" + arbolPersonal.usuario.email + "&id=" + node.id + "&grupo=" + arbolPersonal.nombre + "&width=" + (window.innerWidth - 80),
        post,
        function (data) {
            //muestro
            document.getElementById("documento").innerHTML = data;
        });
}

function doProponer() {
    //leo los textos del documento y los envio al servidor
    var node = selectedNode;

    //quito seleccionado y apago menu
    doMousedown();

    //envio
    getHttp("doDecidimos.aspx?actn=proponer&modelo=" + node.modeloID + "&email=" + arbolPersonal.usuario.email + "&id=" + node.id + "&grupo=" + arbolPersonal.nombre + "&width=" + (window.innerWidth - 80),
        recibirArbolPersonal);

    //cierro documento
    efectoTop(document.getElementById("documento"), 0, 20, -window.innerHeight, TWEEN.Easing.Cubic.In);
    preguntarAlSalir = false;
}

function doVerDocumento() {
    var node = selectedNode;

    //pido propuestas al servidor (por si hay nuevos comentarios)
    //agrego el modelo por si de un documento nuevo y el node es la raiz, hay que decir el modeloID
    getHttp("doDecidimos.aspx?actn=HTMLDocumento&modelo=" + node.modeloID + "&id=" + node.id + "&grupo=" + arbolPersonal.nombre + "&email=" + arbolPersonal.usuario.email + "&width=" + (window.innerWidth - 80),
        function (data) {
            //puedo mostrar el documento
            hidePanelDer();
            hidePanelIzq();

            preguntarAlSalir = true;

            //muestro
            document.getElementById("documento").innerHTML = data;

            document.getElementById("documento").style.visibility = 'visible';
            document.getElementById("documento").style.left = (10) + 'px';
            document.getElementById("documento").style.width = (window.innerWidth - 80) + 'px';
            document.getElementById("documento").style.height = (window.innerHeight - 50) + 'px';
            efectoTop(document.getElementById("documento"), 0, -window.innerHeight, 20, TWEEN.Easing.Cubic.Out);
        }
    );
}

function doComentar(id) {
    var comentario = HTMLEncode(document.getElementById("comentario" + id).value);
    postHttp("doDecidimos.aspx?actn=doComentar&id=" + id + "&grupo=" + arbolPersonal.nombre + "&email=" + arbolPersonal.usuario.email,
        "comentario=" + comentario,
        function (data) {
            //muestro
            document.getElementById("comentarios" + id).innerHTML = data;
        }
    );
}

function doVariante(id) {
    var n = getNodo(id);
    selectedNode = n.parent;
    ////obtengo texto actual

    //pido propuestas al servidor sin resaltar
    getHttp("doDecidimos.aspx?actn=variante&id=" + id + "&grupo=" + arbolPersonal.nombre + "&email=" + arbolPersonal.usuario.email + "&width=" + (window.innerWidth - 80),
        function (data) {
            //muestro
            document.getElementById("documento").innerHTML = data;
        }
    );

}

function getParent(node, depth) {
    var ret = node;
    while (ret.depth > depth)
        ret = ret.parent;
    return ret;
}

function getFloresUsadas() {
    var ret = [];
    for (var i in arbolPersonal.usuario.flores) {
        var flor = arbolPersonal.usuario.flores[i];
        if (flor.id != 0)
            ret.push(flor);
    }
    return ret;
}

function tieneFlor(node) {
    for (var i in arbolPersonal.usuario.flores) {
        var flor = arbolPersonal.usuario.flores[i];
        if (flor.id == node.id)
            return true;
    }
    return false;
}

function getFloresDisponibles() {
    var ret = [];
    for (var i in arbolPersonal.usuario.flores) {
        var flor = arbolPersonal.usuario.flores[i];
        if (flor.id == 0)
            ret.push(flor);
    }
    return ret;
}

function pedirArbol() {
    if (arbolPersonal) {
        var now = (new Date()).getTime();
        if (now - lastArbolRecibidoTs > refreshArbolInterval)
            getHttp("doDecidimos.aspx?actn=getArbolPersonal&email=" + arbolPersonal.usuario.email + "&grupo=" + arbolPersonal.nombre, recibirArbolPersonal);
    }
}
