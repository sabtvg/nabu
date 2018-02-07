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
    document.getElementById("panelGrupo").style.visibility = 'hidden';

    //panel consenso
    document.getElementById("panelConsenso").style.visibility = 'visible';

    //joystick
    document.getElementById("joystickArbol").style.visibility = 'visible';
    document.getElementById("joystickArbol").style.top = (window.innerHeight - 160) + 'px';

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
    timerArbol = setInterval(pedirArbol, refreshInterval);

    estado = 'decidimos';
    clearInterval(timerCiclo);
}

function seleccionarModelo() {
    //opciones de modelos de documentos
    if (getFloresDisponibles().length == 0)
        msg("No tienes flores disponibles");
    else {
        var listE = "<table style='width:200px'>";
        var listS = "<table style='width:200px'>";
        var listI = "<table style='width:200px'>";

        //si no hay manifiesto solo permito crear modelo Manifiesto
        for (var i in modelos) {
            var modelo = modelos[i];
            if (modelo.activo && 
                ((arbolPersonal.URLEstatuto == "" && modelo.id.indexOf('Manifiesto')>=0) || arbolPersonal.URLEstatuto != ""))
            {
                if (modelo.tipo == "estructura") {
                    listE += "<tr>";
                    listE += "<td><img src='" + modelo.icono + "' style='width:32px;height:40px'></td>";
                    listE += "<td class='btn' style='text-align: center;margin:10px;' onclick='seleccionarModelo2(\"" + modelo.id + "\");'>" + modelo.nombre + "</td>";
                    listE += "</tr>";
                }
                else if (modelo.tipo == "intergrupal") {
                    listI += "<tr>";
                    listI += "<td><img src='" + modelo.icono + "' style='width:32px;height:40px'></td>";
                    listI += "<td class='btn' style='text-align: center;margin:10px;' onclick='seleccionarModelo2(\"" + modelo.id + "\");'>" + modelo.nombre + "</td>";
                    listI += "</tr>";
                }
                else {
                    listS += "<tr>";
                    listS += "<td><img src='" + modelo.icono + "' style='width:32px;height:40px'></td>";
                    listS += "<td class='btn' style='text-align: center;margin:10px;' onclick='seleccionarModelo2(\"" + modelo.id + "\");'>" + modelo.nombre + "</td>";
                    listS += "</tr>";
                }
            }
        }
        listE += "</table>";
        listS += "</table>";
        listI += "</table>";

        var list = "<table>";
        list += "<tr><td style='text-align:center' class='titulo1' colspan='3'>Modelos de debate</td></tr>";
        list += "<tr><td style='text-align:center'>Estructura</td><td style='text-align:center'>Seguimiento</td><td style='text-align:center'>Intergrupal</td></tr>";
        list += "<tr>";
        list += "<td style='vertical-align:top;'>" + listE + "</td>";
        list += "<td style='vertical-align:top;'>" + listS + "</td>";
        list += "<td style='vertical-align:top;'>" + listI + "</td>";
        list += "</tr>";
        list += "</table>";

        document.getElementById("modelosContent").innerHTML = list;
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

    //quito seleccionado y apago menu
    if (menu)
        menu.style.visibility = "hidden";
    selectedNode = null;
    dibujarArbol(selectedNode);
    hidePanelDer();
    hidePanelIzq();

    //if (d.totalFlores == 1 && tieneFlor(d) && !d.consensoAlcanzado) {
    //    //quitara la ultima flor de un ultimo nodo
    //    //quito seleccionado y apago menu
    //    doMousedown();
    //    hidePanelIzq();
    //    hidePanelDer();
    //}
    //else if (d.consensoAlcanzado) {
    //    //quitara la ultima flor de un ultimo nodo
    //    //quito seleccionado y apago menu
    //    doMousedown();
    //    hidePanelIzq();
    //    hidePanelDer();
    //}

    //pido nuevo arbol
    getHttp("doDecidimos.aspx?actn=toggleflor&email=" + arbolPersonal.usuario.email
        + "&clave=" + arbolPersonal.usuario.clave
        + "&id=" + d.id
        + "&x=" + d.x0
        + "&grupo=" + arbolPersonal.nombre,
        recibirArbolPersonal);
}

function doCerrarDocumento() {
    efectoTop(document.getElementById("documento"), 0, 20, -window.innerHeight, TWEEN.Easing.Cubic.In);
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
        + "&width=" + (window.innerWidth - 80).toFixed(0),
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
    postHttp("doDecidimos.aspx?actn=prevista&modelo=" + node.modeloID
        + "&email=" + arbolPersonal.usuario.email
        + "&clave=" + arbolPersonal.usuario.clave
        + "&id=" + node.id
        + "&grupo=" + arbolPersonal.nombre
        + "&width=" + (window.innerWidth - 80).toFixed(0),
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
        + "&width=" + (window.innerWidth - 80).toFixed(0),
        recibirArbolPersonal);

    //cierro documento
    efectoTop(document.getElementById("documento"), 0, 20, -window.innerHeight, TWEEN.Easing.Cubic.In);
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

            document.getElementById("documento").style.visibility = 'visible';
            document.getElementById("documento").style.left = (10) + 'px';
            document.getElementById("documento").style.width = (window.innerWidth - 80) + 'px';
            document.getElementById("documento").style.height = (window.innerHeight - 50) + 'px';
            efectoTop(document.getElementById("documento"), 0, -window.innerHeight, 20, TWEEN.Easing.Cubic.Out);
        }
    );
}

function doComentar(id) {
    var comentario = URIEncode(document.getElementById("comentario" + id).value);
    postHttp("doDecidimos.aspx?actn=doComentar&id=" + id
        + "&grupo=" + arbolPersonal.nombre
        + "&email=" + arbolPersonal.usuario.email
        + "&clave=" + arbolPersonal.usuario.clave,
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
    selectedNode.modeloID = n.modeloID; //se lo pongo temporalmente al padre (por si es la raiz)

    //pido propuestas al servidor sin resaltar
    getHttp("doDecidimos.aspx?actn=variante&id=" + id
        + "&modeloID=" + n.modeloID
        + "&grupo=" + arbolPersonal.nombre
        + "&email=" + arbolPersonal.usuario.email
        + "&clave=" + arbolPersonal.usuario.clave
        + "&width=" + (window.innerWidth - 80).toFixed(0),
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
    var post = getPost(document);

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
        });
}