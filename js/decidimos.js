///////////////////////////////////////////////////////////////////////////
//  Copyright 2015 - 2020 Sabrina Prestigiacomo nabu@nabu.pt
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  any later version.
//  
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//  
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//  
///////////////////////////////////////////////////////////////////////////



function doDecidimos() {
    //menu ppal
    if (visual.level == 1)
        document.getElementById("menuppal").style.visibility = "hidden";
    else
        efectoOpacity(document.getElementById("menuppal"), 0, 1, 0, TWEEN.Easing.Cubic.Out, function () { document.getElementById("menuppal").style.visibility = "hidden"; });

    document.getElementById("menuppal").style.visibility = "hidden";
    document.getElementById("panelGrupo").style.display = 'none';

    //panel consenso
    document.getElementById("panelConsenso").style.display = 'inline';

    //titulo
    document.getElementById("titulo").innerHTML = arbolPersonal.nombre + " - " + tr("Decisiones");
    document.getElementById("titulo").style.visibility = 'visible';
    document.getElementById("pie").style.display = "none";

    //joystick
    document.getElementById("joystickArbol").style.visibility = 'visible';

    //objetivo
    var objetivo = document.getElementById("objetivo");
    objetivo.innerHTML = arbolPersonal.objetivo;
    objetivo.style.visibility = 'visible';

    //flores
    timerFlores = setInterval(rotarFlores, 100);

    if (window.innerWidth <= 400)
        treeScale = scale * 3;

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
    timerArbol = setInterval(pedirArbol, refreshInterval);
    clearInterval(timerQueso);
    clearInterval(timerGrupo);

    estado = 'decidimos';
    clearInterval(timerCiclo);

    //activo cartel ayuda
    if (arbolPersonal.usuario && arbolPersonal.usuario.reactivado && cartel1) {
        showAyuda();
        cartel1 = false; //no volver a enseñar
    }
}

function showAyuda() {
    disableBackground();
    document.getElementById("ayudaNuevoDebate").innerHTML = tr("ayudaNuevoDebate");
    document.getElementById("ayudaApoyar").innerHTML = tr("ayudaApoyar");
    document.getElementById("ayudaVariante").innerHTML = tr("ayudaVariante");
    document.getElementById("instrucciones").style.display = "block";
}

function hideAyuda() {
    enableBackground();
    document.getElementById("instrucciones").style.display = "none";
}

function cumplePermisos(modelo) {
    if (modelo.permisos == "")
        return true;
    else if (modelo.permisos == "isSecretaria" && arbolPersonal.usuario.isSecretaria)
        return true;
    else
        return false;
}

function getModeloIcon(modelo) {
    var ret = "<div class='modeloIcon'>";
    ret += "<img src='" + modelo.icono + "' style='float:left;clear:left;width:32px;height:40px;cursor:pointer;' onclick='seleccionarModelo2(\"" + modelo.id + "\");'>";
    ret += "<div style='float:left;text-align:left;margin:5px;cursor:pointer;padding:4px;' onclick='seleccionarModelo2(\"" + modelo.id + "\");'>"
        + modelo.trNombre
        + "</div>";
    ret += "</div>";
    return ret;
}

function seleccionarModelo() {
    //opciones de modelos de documentos
    if (getFloresDisponibles().length == 0)
        msg("No tienes flores disponibles");
    else {
        var listE = "<div class='modeloBox'><div><b>" + tr("Estructura") + "</b></div>";
        var listS = "<div class='modeloBox'><div><b>" + tr("Seguimiento") + "</b></div>";
        var listI = "<div class='modeloBox'><div><b>" + tr("Intergrupal") + "</b></div>";
        var listO = "<div class='modeloBox'><div><b>" + tr("Otros") + "</b></div>";

        //si no hay manifiesto solo permito crear modelo Manifiesto
        for (var i in modelos) {
            var modelo = modelos[i];
            if (modelo.activo && 
                ((arbolPersonal.URLEstatuto == "" && modelo.id.indexOf('Manifiesto')>=0) || arbolPersonal.URLEstatuto != ""))
            {
                if (modelo.tipo == "estructura" && cumplePermisos(modelo)) {
                    listE += getModeloIcon(modelo);
                }
                else if (modelo.tipo == "padre" && arbolPersonal.padreNombre != '' && cumplePermisos(modelo)) {
                    listI += getModeloIcon(modelo);
                }
                else if (modelo.tipo == "hijo" && arbolPersonal.hijos.length > 0 && cumplePermisos(modelo)) {
                    listI += getModeloIcon(modelo);
                }
                else if (modelo.tipo == "seguimiento" && cumplePermisos(modelo)) {
                    listS += getModeloIcon(modelo);
                }
                else if (modelo.tipo == "otro" && cumplePermisos(modelo)) {
                    listO += getModeloIcon(modelo);
                }
            }
        }
        listE += "</div>";
        listS += "</div>";
        listI += "</div>";
        listO += "</div>";

        var list = "<div style='clear:left;float:left;text-align:center;padding:10px;width:100%;' class='titulo1' colspan='4'><b>" + tr("Modelos de debate") + "</b></div>";
        list += listE;
        list += listS;
        list += listI;
        list += listO;

        //cartel sociocracia.net
        list += "<div style='clear:left;float:left;vertical-align:top;font-size:12px;text-align:center;padding:5px;width:100%;' colspan=4>" + tr("nuevos modelos") + "</div>";
        list += "<div style='clear:left;float:left;width:100%;text-align:center;'><input id='btnCancelar' type='button' value='" + tr("Cancelar") + "' class='btn' style='float:unset;' onclick='document.getElementById(\"modelosDebate\").style.display = \"none\";' /></div>";
        list += "</div>";


        document.getElementById("modelosDebate").innerHTML = list;
        document.getElementById("modelosDebate").style.display = "block";
    }
}

function seleccionarModelo2(modeloID) {
    document.getElementById("modelosDebate").style.display = "none";
    if (selectedNode)
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
            setParents(arbolPersonal.raiz);
            selectNode(getNodo(arbolPersonal.nuevoNodoID));
        }
        else if (selectedNode){
            //actualizo el selectedNode en la nueva estructura
            var n = getNodo(selectedNode.id);
            setParents(arbolPersonal.raiz);
            selectNode(n);
        }

        dibujarArbol(selectedNode);

        //condicion de consenso
        actualizarDatosConsenso();

        //timestamp
        lastArbolRecibidoTs = (new Date()).getTime();

        //msgBox
        if (arbolPersonal.msgBox != "")
            popupMsgOn("<img src='res/consenso.png'><br>" + tr("consenso alcanzado") + "<br>" + arbolPersonal.msgBox);

        //reactivo evento resize por si esta descativado
        reload = true;
    }
}

function doToggleFlor() {
    var d = selectedNode;
    if (d.totalFlores == 1 && tieneFlor(d) && !d.consensoAlcanzado) {
        //quitara la ultima flor de un ultimo nodo
        //aviso que caera
        disableBackground();
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

    //quito seleccionado y apago menu
    if (menu)
        menu.style.visibility = "hidden";
    selectedNode = null;
    dibujarArbol(selectedNode);
    hidePanelDer();
    hidePanelIzq();

    //pido nuevo arbol
    getHttp("doDecidimos.aspx?actn=toggleflor&email=" + arbolPersonal.usuario.email
        + "&clave=" + arbolPersonal.usuario.clave
        + "&id=" + d.id
        + "&x=" + d.x0
        + "&grupo=" + arbolPersonal.nombre,
        recibirArbolPersonal);
}

function doCerrarDocumento() {
    enableBackground();
    document.getElementById("documento").style.display = "none";
    preguntarAlSalir = false;
}

function doRevisar() {
    //leo los textos del documento y los envio al servidor
    var node = selectedNode;

    //envio
    getHttp("doDecidimos.aspx?actn=revisar&modelo=" + node.modeloID
        + "&email=" + arbolPersonal.usuario.email
        + "&clave=" + arbolPersonal.usuario.clave
        + "&id=" + node.id
        + "&grupo=" + arbolPersonal.nombre
        + "&width=" + (window.innerWidth - 10).toFixed(0),
        function (data) {
            //muestro
            document.getElementById("documento").innerHTML = data;

            //activo editores de estilo
            activarStyleEditor();
        });
}

function doPrevista() {
    //leo los textos del documento y los envio al servidor
    var node = selectedNode;
    var post = getPost(document.getElementById("documento"));

    //envio
    postHttp("doDecidimos.aspx?actn=prevista&modelo=" + node.modeloID
        + "&email=" + arbolPersonal.usuario.email
        + "&clave=" + arbolPersonal.usuario.clave
        + "&id=" + node.id
        + "&grupo=" + arbolPersonal.nombre
        + "&width=" + (window.innerWidth - 10).toFixed(0),
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
    //ha picado en el fondo
    if (menu)
        menu.style.visibility = "hidden";
    selectedNode = null;
    dibujarArbol(selectedNode);
    hidePanelDer();
    hidePanelIzq();

    //envio
    getHttp("doDecidimos.aspx?actn=proponer&modelo=" + node.modeloID
        + "&email=" + arbolPersonal.usuario.email
        + "&clave=" + arbolPersonal.usuario.clave
        + "&id=" + node.id
        + "&grupo=" + arbolPersonal.nombre
        + "&width=" + (window.innerWidth - 10).toFixed(0),
        recibirArbolPersonal);

    //cierro documento
    enableBackground();
    document.getElementById("documento").style.display = "none";
    preguntarAlSalir = false;
}

function doVerDocumento() {
    var node = selectedNode;

    //pido propuestas al servidor (por si hay nuevos comentarios)
    //agrego el modelo por si de un documento nuevo y el node es la raiz, hay que decir el modeloID
    getHttp("doDecidimos.aspx?actn=HTMLDocumento&modelo=" + node.modeloID
        + "&id=" + node.id
        + "&grupo=" + arbolPersonal.nombre
        + "&email=" + arbolPersonal.usuario.email
        + "&clave=" + arbolPersonal.usuario.clave
        + "&width=" + (window.innerWidth - 80).toFixed(0),
        function (data) {
            //puedo mostrar el documento
            hidePanelDer();
            hidePanelIzq();

            preguntarAlSalir = true;

            //muestro
            document.getElementById("documento").innerHTML = data;

            //activo editores de estilo
            activarStyleEditor();

            //show
            disableBackground();
            document.getElementById("documento").style.display = 'block';
        }
    );
}

function doComentar(id, objecion) {
    var comentario = URIEncode(document.getElementById("comentario" + id).value);
    postHttp("doDecidimos.aspx?actn=doComentar&id=" + id
        + "&grupo=" + arbolPersonal.nombre
        + "&email=" + arbolPersonal.usuario.email
        + "&clave=" + arbolPersonal.usuario.clave
        + "&objecion=" + objecion,
        "comentario=" + comentario,
        function (data) {
            //muestro
            document.getElementById("comentarios" + id).innerHTML = data;
        }
    );
}

function doVariante(id, finalID) {
    var n = getNodo(id);
    selectedNode = n.parent;
    selectedNode.modeloID = n.modeloID; //se lo pongo temporalmente al padre (por si es la raiz)

    //pido propuestas al servidor sin resaltar
    getHttp("doDecidimos.aspx?actn=variante&id=" + id
        + "&finalID=" + finalID
        + "&modeloID=" + n.modeloID
        + "&grupo=" + arbolPersonal.nombre
        + "&email=" + arbolPersonal.usuario.email
        + "&clave=" + arbolPersonal.usuario.clave
        + "&width=" + (window.innerWidth - 80).toFixed(0),
        function (data) {
            //muestro
            document.getElementById("documento").innerHTML = data;

            //activo editores de estilo
            activarStyleEditor();
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

function pedirArbol() {
    if (arbolPersonal && !preguntarAlSalir) {
        var now = (new Date()).getTime();
        if (now - lastArbolRecibidoTs > refreshInterval)
            getHttp("doDecidimos.aspx?actn=getArbolPersonal&email=" + arbolPersonal.usuario.email
                + "&clave=" + arbolPersonal.usuario.clave
                + "&grupo=" + arbolPersonal.nombre,
                recibirArbolPersonal);
    }
}

function setNotEmpyList(id) {
    //caso especial lkista vacia debe ser <>'' para que se envie en el submit
    var list = document.getElementById(id);
    if (list.value == '')
        list.value = "*";
}

function documentSubmit(accion, parametro) {
    //envio mensaje al documento y redibujo
    var node = selectedNode;
    var post = getPost(document.getElementById("documento"));

    postHttp("doDecidimos.aspx?actn=documentSubmit&modelo=" + node.modeloID
        + "&accion=" + accion
        + "&parametro=" + parametro
        + "&id=" + node.id
        + "&grupo=" + arbolPersonal.nombre
        + "&email=" + arbolPersonal.usuario.email
        + "&clave=" + arbolPersonal.usuario.clave
        + "&width=" + (window.innerWidth - 80).toFixed(0),
        post,
        function (data) {
            //muestro
            document.getElementById("documento").innerHTML = data;

            //activo editores de estilo
            activarStyleEditor();
        });
}