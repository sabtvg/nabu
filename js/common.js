///////////////////////////////////////////////////////////////////////////
//  Copyright 2015 - 2020 Sabrina Prestigiacomo sabtvg@gmail.com
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
        getHttp("doDecidimos.aspx?actn=HTMLPropuesta&id=" + node.id
            + "&grupo=" + arbolPersonal.nombre
            + "&email=" + arbolPersonal.usuario.email
            + "&clave=" + arbolPersonal.usuario.clave
            + "&width=" + (365 * scale).toFixed(0),
            function (data) {
                var panelIzq = document.getElementById("panelIzq");
                var panelDer = document.getElementById("panelDer");

                if (node.depth > 0) {
                    //enseño panel
                    if (arbolPersonal.simulacion) {
                        //panel derecho
                        showPanelDer(data);
                    }
                    else if (node.x > 90) {
                        //panel izquierdo
                        showPanelIzq(data);
                        //desactivo el izq
                        hidePanelDer();
                    }
                    else {
                        //panel derecho
                        showPanelDer(data);
                        //desactivo el izq
                        hidePanelIzq();
                    }
                }
                else {

                }
            }
        );
    }
}

function showPanelIzq(data) {
    //activo el izq
    var panelIzq = document.getElementById("panelIzq");
    var panelIzq2 = document.getElementById("panelIzq2");
    if (panelIzq.style.visibility == "hidden") {
        panelIzq.style.visibility = "visible";
        efectoLeft(panelIzq, 0, -400 * scale, 20, TWEEN.Easing.Cubic.Out);
    }

    panelIzq.style.top = "220px";
    panelIzq.style.width = 400 * scale + "px";
    panelIzq2.style.width = 390 * scale + "px";
    panelIzq.style.height = 550 * scale + "px";

    panelIzq2.innerHTML = data;
}

function showPanelDer(data) {
    //activo el derecho
    var panelDer = document.getElementById("panelDer");
    var panelDer2 = document.getElementById("panelDer2");
    if (panelDer && panelDer.style.visibility == "hidden") {
        panelDer.style.visibility = "visible";
        efectoLeft(panelDer, 0, window.innerWidth, window.innerWidth - 400 * scale - 40, TWEEN.Easing.Cubic.Out);
    }

    panelDer.style.top = "220px";
    panelDer.style.width = 400 * scale + "px";
    panelDer2.style.width = 390 * scale + "px";
    panelDer.style.height = 550 * scale + "px";

    panelDer2.innerHTML = data;
}

function hidePanelIzq() {
    var panelIzq = document.getElementById("panelIzq");
    if (panelIzq && panelIzq.style.visibility == "visible") {
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

function HTMLFlores(node, showVariante) {
    var ret;
    ret = "<table style='width:100%;'><tr>"
    ret += "<td class='votos'  style='vertical-align:center;'>";
    ret += "<img src='res/icono.png'>";
    ret += "&nbsp;" + node.totalFlores;
    ret += "</td><td style='text-align:right;'>";
    if (arbolPersonal.raiz != node.parent && showVariante) {
        if (getFloresDisponibles().length == 0)
            ret += "<input type='button' class='btnDis' value='Crear variante' title='No tienes flores disponibles' disabled>";
        else
            ret += "<input type='button' class='btn' value='Crear variante' title='Crea otra propuesta basada en esta' onclick='doVariante(" + node.id + ");'>";
    }
    ret += "</tr></table>"
    return ret;
}

function getPost(n) {
    //alert('&_MouseX=' + mouseX() +  '&_MouseY=' + mouseY());
    //var sToSend = '&_MouseX=' + window.event.clientX +  '&_MouseY=' + window.event.clientY ;  
    var sToSend = '';
    if (n.nodeName == 'INPUT' && n.nodeType == 1 && n.id != '' && n.id.indexOf(".") >= 0) {
        //hay que enviarlo en el post
        if (n.type == 'checkbox') 
            sToSend += '&' + n.id + '=' + n.checked;
        else if (n.type == 'radio' && event.srcElement.type == 'radio' && event.srcElement.id == n.id) //tengo que poner dos condiciones para RADIO porque al hacer click aun no es checked
            sToSend += '&' + n.name + '=' + n.value;
        else if (n.type == 'radio' && n.checked) 
            sToSend += '&' + n.name + '=' + n.value;
        else
            sToSend += '&' + n.id + '=' + URIEncode(n.value);
    } else {
        if (n.nodeName == 'SELECT' && n.nodeType == 1 && n.id != '' && n.id.indexOf(".") >= 0) {
            //sToSend += '&' + n.id + '.selectedIndex=' + getSelectedIndex(n); // n.selectedIndex;
            //sToSend += '&' + n.id + '.id=' + getSelectedId(n); //n.options[n.selectedIndex].id;
            //sToSend += '&' + n.id + '.text=' + URIEncode(getSelectedText(n)); //n.options[n.selectedIndex].text;
            sToSend += '&' + n.id + '=' + getSelectedId(n); //n.options[n.selectedIndex].id;
        }
        if (n.nodeName == 'TEXTAREA' && n.nodeType == 1 && n.id != '' && n.id.indexOf(".") >= 0) {
            sToSend += '&' + n.id + '=' + URIEncode(n.value);
        }
    }

    //recurso  
    var children = n.childNodes;
    for (var i = 0; i < children.length; i++) {
        sToSend += getPost(children[i]);
    }

    return sToSend;
}

function getSelectedIndex(n) {
    var ret = '';
    for (var i = 0; i < n.options.length; i++) {
        if (n.options[i].selected) ret += i + ';';
    }
    if (ret != '') ret = ret.substr(0, ret.length - 1);
    return ret;
}

function getSelectedId(n) {
    var i;
    var ret = '';
    for (i = 0; i < n.options.length; i++)
        if (n.options[i].selected) ret = ret + n.options[i].id + ';';
    if (ret != '') ret = ret.substr(0, ret.length - 1);
    return (ret);
}

function getSelectedText(n) {
    var i;
    var ret = '';
    for (i = 0; i < n.options.length; i++)
        if (n.options[i].selected) ret = ret + n.options[i].text + ';';
    if (ret != '') ret = ret.substr(0, ret.length - 1);
    return (ret);
}

function getSelectedIndex(n) {
    var ret = '';
    for (var i = 0; i < n.options.length; i++) {
        if (n.options[i].selected) ret += i + ';';
    }
    if (ret != '') ret = ret.substr(0, ret.length - 1);
    return ret;
}

function getSelectedText(n) {
    var i;
    var ret = '';
    for (i = 0; i < n.options.length; i++)
        if (n.options[i].selected) ret = ret + n.options[i].text + ';';
    if (ret != '') ret = ret.substr(0, ret.length - 1);
    return (ret);
}

