﻿///////////////////////////////////////////////////////////////////////////
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

var duration = 750,
    d3Arbol,
    diagonal,
    svgArbol,
    treeScale = 1;

var menu;
var downEvent = null;
var translatex = 0;
var translatey = 0;
var rotFlores = 0;
var maxWidth = 50;
var useMaxWidth = false;
var historico = historico || false;
var parents = [];
var flowerOffset = 0;
var trSi = "";
var trNo = "";

function crearArbol() {
    var diameter = window.innerHeight / 2;
    d3Arbol = d3.layout.tree()
        .size([180, diameter / 2])
        .separation(function (a, b) {
            return (a.parent == b.parent ? 1 : 2) / a.depth;
        });

    diagonal = d3.svg.diagonal.radial()
        .projection(function (d) { return [d.y, d.x / 180 * Math.PI - Math.PI / 2]; });

    var i;
    if (window.innerWidth <= 400)
        i = 90;
    else if (window.innerWidth <= 800)
        i = 70;
    else
        i = 90;

    trSi = tr('Si');
    trNo = tr('No');

    svgArbol = d3.select("svg")
        .attr("id", "arbol")
        .attr("style", "width: " + window.innerWidth + "px;height:" + window.innerHeight + "px;top:0px;left:0px;position:absolute;")
        .on("mousedown", doMousedown)
        .on("mousemove", doMousemove)
        .on("mouseup", doMouseup)
        .on("mousewheel", doMousewheel1)
        //.on("DOMMouseScroll", doMousewheel2)  //firefox  //NO FUNCIONA EVENT
        .append("g")
        .attr("transform", "translate(" + (window.innerWidth / 2).toFixed(0) + "," + (window.innerHeight - i).toFixed(0) + ")");


    dibujarArbol(arbolPersonal.raiz);

    d3.select(self.frameElement).style("height", window.innerHeight + "px");
}

function getModelo(id) {
    for (var i in modelos) {
        if (modelos[i].id == id)
            return modelos[i];
    }
    return null;
}

function doMousewheel1() {
    var e = window.event || Event; // old IE support
    var delta = Math.max(-1, Math.min(1, (e.wheelDelta || -e.detail)));
    zoom(delta);
}

function zoom(delta){
    if (delta > 0)
        treeScale *= 1.1;
    else {
        treeScale *= 0.9;
        translatey *= 0.8;
    }
    dibujarArbol(arbolPersonal.raiz);
    translateArbol(translatex, translatey);
}

function doMousedown() {
    //prueba para firefox
    //var ev = document.createEvent('MouseEvents');
    //event.initMouseEvent('mousedown', true, true, window);
    //ev.initMouseEvent('mousedown', true, true, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);
    //element.dispatchEvent(event);

    var e = window.event || Event || e; // old IE support
    var element = e.toElement || e.target;
    downEvent = { "x": e.clientX, "y": e.clientY, "translatex": translatex, "translatey": translatey };
    if (element.nodeName == 'svg') {
        //ha picado en el fondo
        deselect();
    }
}

function deselect()
{ 
    if (estado == "decidimos") {
        if (menu)
            menu.style.visibility = "hidden";
        selectedNode = null;
        parents = [];
        dibujarArbol(selectedNode);
        hidePanelDer();
        hidePanelIzq();
    }
}

function doMousemove() {
    //causa confucion y falla en algunos navegadores
    //se peirde el arbol cuando se presiones la tecla CTRL
    //if (downEvent) {
    //    var e = window.event || Event || e; // old IE support
    //    translatex = downEvent.translatex - (downEvent.x - e.clientX);
    //    translatey = downEvent.translatey - (downEvent.y - e.clientY);
    //    if (translatey < 0) translatey = 0;
    //    translateArbol(translatex, translatey);        
    //}
}

function doMouseup() {
    downEvent = null;
}

function translateArbol(x, y) {
    d3.select("#arbol").select("g").attr("transform", "translate(" + (window.innerWidth / 2 + x).toFixed(0) + "," + (window.innerHeight * 0.90 + y).toFixed(0) + ")");
}

function rotarFlores() {
    rotFlores++;
    var offFlor;
    if (window.innerWidth <= 400) {
        //movil vertical
        offFlor = -75
    }
    else if (window.innerWidth <= 800) {
        //movil horizontal
        offFlor = -39
    }
    else {
        //ordenador
        offFlor = -19
    }
    //flores
    svgArbol.selectAll(".florImage")
        .attr("transform", "rotate(" + rotFlores.toFixed(0) + ")translate(" + (offFlor * scale).toFixed(0) + "," + (offFlor * scale).toFixed(0) + ")");
    //objeciones
    svgArbol.selectAll("._nodo")
        .attr("r", function (d) {
            if (!d.consensoAlcanzado && !historico && (!selectedNode || (d.id != selectedNode.id))) {
                    var i = getNodeR(d);
                    if (d.objecion)
                        i += Math.sin(rotFlores / 2) * 3 + 1.5;
                    else if (d.comentario)
                        i += Math.sin(rotFlores / 10) * 3 + 1.5;
                    return i;
            }
            else
                return getNodeR(d);
        })
}

function getNodeR(d) {
    //get radio
    var i;
    if (window.innerWidth <= 400)
        i = 10;
    else if (window.innerWidth <= 800)
        i = 10;
    else
        i = 8;

    if (d.nivel == 0) //root
        i = i * 1.5;

    if (selectedNode && d.id == selectedNode.id)
        i = 15;
    if (selectedNode && isNodeFlor(d.id) && d.id == selectedNode.id)
        i = 24;
    return i;
}

function dibujarArbol(referencia) {
    d3.select("svg")
        .attr("style", "width: " + window.innerWidth + "px;height:" + window.innerHeight + "px;top:0px;left:0px;position:absolute;")

    //d3.select("svg")
    //    .select("g")
    //    .attr("transform", "translate(" + (window.innerWidth / 2).toFixed(0) + "," + (window.innerHeight * 0.90).toFixed(0) + ")");

    flowerOffset = 0;

    //actualizo las flores de los padres
    useMaxWidth = true; //always true right now 3/10/2019
    setParents(arbolPersonal.raiz);
    updateFloresTotales(arbolPersonal.raiz);

    // Compute the new tree layout.
    var nodes = d3Arbol.nodes(arbolPersonal.raiz).reverse(),
        links = d3Arbol.links(nodes);

    // Normalize for fixed-depth.
    nodes.forEach(function (d) {
        if (isNaN(d.x)) d.x = 90;
        d.y = d.depth * 140 * treeScale;
    });

    //node -----------------------------------------------------------------------------------------------------------------------
    var node = svgArbol.selectAll("g.node")
        .data(nodes, function (d) { return 'g' + d.id; });

    // Enter any new nodes at the parent's previous position.
    var nodeEnter = node.enter().append("g")
        .attr("id", function (d) {
                return 'g' + d.id;
        })
        .attr("class", "node")
        .attr("transform", function (d) {
            if (referencia)
                return "rotate(" + (referencia.x - 180).toFixed(0) + ")translate(" + (referencia.y).toFixed(0) + ")";
            else if (d.parent)
                return "rotate(" + (d.parent.x - 180).toFixed(0) + ")translate(" + (d.parent.y).toFixed(0) + ")";
            else
                return "rotate(0)translate(0)";
        })
        .on("click", nodeClick);

    nodeEnter.append("circle")
        .style("cursor", "pointer")
        .attr("id", function (d) {
            return 'c' + d.id;
        });

    //1ra linea de texto ETIQUETA
    nodeEnter.append("text")
        .attr("id", function (d) {
            return 't1.' + d.id;
        })
        .attr("x", function (d) {
            return 0;
        })
        .attr("dy", function (d) {
            var i;
            if (window.innerWidth <= 400)
                i = 15;
            else if (window.innerWidth <= 800)
                i = 15;
            else
                i = 15;
            return -i;
        })
        .attr("transform", function (d) {
                return "rotate(90)";
        })
        .attr("fill", function (d) {
                return "black";
        })
        .attr("text-anchor", function (d) {
            return "middle";
        })
        .style("font-size", function (d) {
            var i;
            if (window.innerWidth <= 400)
                i = 50
            else if (window.innerWidth <= 800)
                i = 25;
            else
                i = 15;
            var size = i * scale > 25 ? 25 : i * scale;
            if (size < 10) size = 10;
            return size.toFixed(0) + 'px';
        });

    //2da linea de texto NO
    nodeEnter.append("text")
        .attr("id", function (d) {
            return 't2.' + d.id;
        })
        .attr("x", function (d) {
            return 0;
        })
        .attr("dy", function (d) {
            var i;
            if (window.innerWidth <= 400)
                i = 25;
            else if (window.innerWidth <= 800)
                i = 25;
            else
                i = 30;
            return -i;
        })
        .attr("transform", function (d) {
            return "rotate(90)";
        })
        .attr("text-anchor", function (d) {
            return "middle";
        })
        .style("font-size", function (d) {
            var i;
            if (window.innerWidth <= 400)
                i = 45;
            else if (window.innerWidth <= 800)
                i = 25;
            else
                i = 15;
            var size = i * scale > 25 ? 25 : i * scale;
            if (size < 10) size = 10;
            return size.toFixed(0) + 'px';
        });

    //3da linea de texto SI
    nodeEnter.append("text")
        .attr("id", function (d) {
            return 't3.' + d.id;
        })
        .attr("x", function (d) {
            return 0;
        })
        .attr("dy", function (d) {
            var i;
            if (window.innerWidth <= 400)
                i = 35;
            else if (window.innerWidth <= 800)
                i = 35;
            else
                i = 45;
            return -i;
        })
        .attr("transform", function (d) {
            return "rotate(90)";
        })
        .attr("text-anchor", function (d) {
            return "middle";
        })
        .style("font-size", function (d) {
            var i;
            if (window.innerWidth <= 400)
                i = 45;
            else if (window.innerWidth <= 800)
                i = 25;
            else
                i = 15;
            var size = i * scale > 25 ? 25 : i * scale;
            if (size < 10) size = 10;
            return size.toFixed(0) + 'px';
        });

    // Transition nodes to their new position.
    var nodeUpdate = node.transition()
        .duration(duration)
        .attr("transform", function (d) {
            return "rotate(" + (d.x - 180).toFixed(0) + ")translate(" + d.y.toFixed(0) + ")";
        })

    nodeUpdate.select("circle")
        .attr("class", "_nodo")
        .attr("r", function (d) {
            return getNodeR(d);
        })
        .style("stroke", function (d) {
            if (selectedNode)
                return d.id == selectedNode.id ? "blue" : "gray";
            else if (d.nivel == 0) //root
                return "blue";
            else
                return "gray";
        })
        .style("stroke-width", function (d) {
            if (selectedNode)
                return d.id == selectedNode.id ? 8 : 2;
            else if (d.nivel == 0) //root
                return 8;
            else
                return 2;
        })
        .style("fill", function (d) {
            var color = "";
            if (d.consensoAlcanzado || historico)
                color = "gray";
            else if (d.objecion)
                color = "#ffa6c8";
            else if (d.comentario)
                color = "#b0ffbd";
            else
                color = "yellow";

            if (isNodeFlor(d.id))
                color = "none";

            return color;
        });   

    nodeUpdate.select("text")  //etiqueta
        .text(function (d) {
            if (d.nivel == 0)
                return "";
            else
                return txtCut(d.nombre);
        })
        .style("fill-opacity", 1);

    // Transition exiting nodes to the parent's new position.
    var nodeExit = node.exit().transition()
        .duration(duration)
        .attr("transform", function (d) {
            if (referencia)
                return "rotate(" + (referencia.x - 180).toFixed(0) + ")translate(" + referencia.y.toFixed(0) + ")";
            else if (d.parent)
                return "rotate(" + (d.parent.x - 180).toFixed(0) + ")translate(" + d.parent.y.toFixed(0) + ")";
            else
                return "rotate(0)translate(0)";
        })
        .remove();

    nodeExit.select("circle")
        .attr("r", function (d) {
            var r = d.totalFlores / 4 + 2;
            var maxWidth = arbolPersonal.usuarios < 20 ? 20 : arbolPersonal.usuarios; //me aseguro que sea un poco ancho cuando son muy pocos usuarios
            if (useMaxWidth)
                r = Math.ceil(r / arbolPersonal.usuarios * maxWidth);  //grosor proporcional solo cuando hay muchos votos
            if (r < 10) r = 10;
            return r * scale;
        })

    nodeExit.select("text")
        .style("fill-opacity", 1e-6)
        .attr("text-anchor", function (d) { return d.x < 180 ? "start" : "end"; })
        .attr("transform", function (d) { return d.x < 180 ? "translate(10)" : "rotate(180)translate(-8)"; });

    // textos si y no
    nodes.forEach(function (d) {
        var t2 = document.getElementById('t2.' + d.id);
        if (t2 && d.no && !d.consensoAlcanzado) {
            if (d.nivel != 0) //root
            {
                t2.style.fill = d.noColor; //actualizo el no texto
                t2.innerHTML = d.no; //actualizo el no texto
            }
        }

        var t3 = document.getElementById('t3.' + d.id);
        if (t3 && d.si && !d.consensoAlcanzado) {
            if (d.nivel != 0) //root
            {
                t3.style.fill = d.siColor; //actualizo el si texto
                t3.innerHTML = d.si; //actualizo el si texto
            }
        }
    });

    //flores ------------------------------------------------------------------------------------------------------------------
    var flor = svgArbol.selectAll("g.flor")
        .data(arbolPersonal.usuario.flores);

    var florEnter = flor.enter().append("g")
        .attr("class", "flor")
        .attr("transform", "translate(0)")
        .on("click", florClick);

    florEnter.append("image")
        .style("cursor", "pointer")
        .attr("class", "florImage")
        .attr("transform", "translate(0)")
        .attr("xlink:href", "res/icono2.png");

    var florUpdate = flor.transition()
        .duration(duration)
        .attr("transform", function (d) {
            if (d.id == 0) {
                //flor disponible
                var libres = getFloresDisponibles().length;
                var i;
                if (window.innerWidth <= 400)
                    i = 60;
                else if (window.innerWidth <= 800)
                    i = 90;
                else
                    i = 50;
                var init = i * treeScale * (libres - 1);
                return "rotate(0)translate(" + (init / 2 - i * treeScale * flowerOffset++) + "," + (i * treeScale) + ")";
            }
            else {
                var n = getNodo(d.id);

                if (n)
                    return "rotate(" + (n.x - 180).toFixed(0) + ")translate(" + n.y.toFixed(0) + ")";
                else
                    //error de consistencia
                    return "rotate(0)translate(0)";
            }
        });

    florUpdate.select("image")
        .attr("width", function (d) {
            if (window.innerWidth <= 400)
                return 152 * scale;
            else if (window.innerWidth <= 800)
                return 74 * scale;
            else
                return 37 * scale;
        }
        )
        .attr("height", function (d) {
            if (window.innerWidth <= 400)
                return 152 * scale;
            else if (window.innerWidth <= 800)
                return 72 * scale;
            else
                return 36 * scale;
        })
        .attr("xlink:href", "res/icono2.png")
        .attr("visibility", !historico);

    var florExit = flor.exit().transition()
        .duration(duration)
        .attr("transform", "rotate(0)translate(0)")
        .remove();

    //documentos ------------------------------------------------------------------------------------------------------------------
    //filtro documentos viejos
    var logNew = [];
    var limit = new Date();
    limit.setDate(limit.getDate() - 30);
    for (var i in arbolPersonal.logDecisiones) {
        var doc = arbolPersonal.logDecisiones[i];
        if (jsonToDate(doc.fecha) >= limit)
            logNew.push(doc);
    }
    var doc = svgArbol.selectAll("g.doc")
        .data(logNew, function (d) { return 'd' + d.docID; });

    var docEnter = doc.enter().append("g")
        .attr("class", "doc")
        .attr("transform", "translate(0,0)")
        .on("click", docClick);

    docEnter.append("image")
        .attr("class", "docImage")
        .style("cursor", "pointer")
        .attr("xlink:href", function (d) { return d.icono; });

    docEnter.append("text")
        .attr("x", function (d) {
            return 0;
        })
        .attr("transform", "rotate(90)translate(-5, " + (window.innerWidth <= 800 ? "10" : "20") + ")")
        .style("font-size", function (d) {
            var i = window.innerWidth <= 800 ? 25 : 15;
            var size = i * scale > 30 ? 30 : i * scale;
            return size + 'px';
        })
        .attr("text-anchor", "middle ")
        .text(function (d) { return d.fname; });

    docEnter.append("text")
        .attr("x", function (d) {
            return 0;
        })
        .attr("transform", "rotate(90)translate(-5, " + (window.innerWidth <= 800 ? "20" : "35") + ")")
        .style("font-size", function (d) {
            var i = window.innerWidth <= 800 ? 25 : 15;
            var size = i * scale > 30 ? 30 : i * scale;
            return size + 'px';
        })
        .style("cursor", "pointer")
        .attr("text-anchor", "middle ")
        .text(function (d) { return txtCut(d.titulo); });

    docEnter.append("text")
        .attr("x", function (d) {
            return 0;
        })
        .attr("transform", "rotate(90)translate(-5, " + (window.innerWidth <= 800 ? "30" : "50") + ")")
        .style("font-size", function (d) {
            var i = window.innerWidth <= 800 ? 25 : 15;
            var size = i * scale > 30 ? 30 : i * scale;
            return size + 'px';
        })
        .style("cursor", "pointer")
        .attr("text-anchor", "middle ")
        .text(function (d) { return d.sFecha; });

    var docUpdate = doc.transition()
        .duration(duration)
        .attr("transform", function (d) {
            if (arbolPersonal.simulacion) {
                var distancia = 5 * 140 * treeScale + d.minutos * 5 + 120;
                if (distancia > 1400) distancia = 1400;
                return "rotate(" + (d.x - 180).toFixed(0) + ")translate(" + (distancia).toFixed(0) + ")";
            }
            else {
                var distancia = 5 * 140 * treeScale + d.dias * 5 + 120;
                if (distancia > 1400) distancia = 1400;
                return "rotate(" + (d.x - 180).toFixed(0) + ")translate(" + (distancia).toFixed(0) + ")";
            }
        });

    docUpdate.select("image")
        .attr("width", (64 * 0.6).toFixed(0) + "px")
        .attr("height", (79 * 0.6).toFixed(0) + "px")
        .attr("transform", "rotate(90) translate(" + (-32 * 0.6).toFixed(0) + "," + (-70 * 0.6).toFixed(0) + ")")

    var docExit = doc.exit().transition()
        .duration(duration)
        .attr("transform", "rotate(0)translate(0)")
        .remove();

    // link ------------------------------------------------------------------------------------------------------------------------
    var link = svgArbol.selectAll("path")
        .data(links, function (d) {
            return d.target.id;
        });

    // Enter any new links at the parent's previous position.
    link.enter().insert("path", "g")
        .attr("d", function (d) {
            var o;
            if (referencia)
                o = { x: referencia.x, y: referencia.y };
            else
                o = { x: d.source.x, y: d.source.y };
            return diagonal({ source: o, target: o });
        });

    // Transition links to their new position.
    link.transition()
        .duration(duration)
        .attr("style", function (d) {
            var w = d.target.totalFlores;
            var maxWidth = arbolPersonal.usuarios < 20 ? 20 : arbolPersonal.usuarios; //me aseguro que sea un poco ancho cuando son muy pocos usuarios
            if (maxWidth > 50) maxWidth = 50;
            if (useMaxWidth)
                w = Math.ceil(w / arbolPersonal.usuarios * maxWidth);  //grosor proporcional solo cuando hay muchos votos
            var r = Math.round(255 / d.target.depth);
            var stroke = historico ? "gray" : "rgb(100," + (255 - r) + ",0)";
            if (d.target.consensoAlcanzado) stroke = "gray";
            else if (d.target.email == arbolPersonal.usuario.email) stroke = "Crimson";

            //selected
            if (isNodeSelectedPath(d.target)) {
                w = 18;
                stroke = 'blue';
                //stroke = "blue";
            }

            return "fill: none; stroke: " + stroke + ";stroke-width: " + (w * scale + 1).toFixed(0) + "px;";
        })
        .attr("d", diagonal);

    // Transition exiting nodes to the parent's new position.
    link.exit().transition()
        .duration(duration)
        .attr("d", function (d) {
            var o = { x: d.source.x, y: d.source.y };
            return diagonal({ source: o, target: o });
        })
        .remove();


    // Stash the old positions for transition.
    nodes.forEach(function (d) {
        d.x0 = d.x;
        d.y0 = d.y;
    });
}

function isNodeFlor(id) {
    for (var fi in arbolPersonal.usuario.flores) {
        var flor = arbolPersonal.usuario.flores[fi];
        if (flor.id == id)
            return true;
    }
    return false;
}

function isNodeSelectedPath(n) {
    for (var i in parents)
        if (parents[i] == n)
            return true;
    return false;
}

function arrayRemove(parent, node) {
    for (var i in parent.children)
        if (parent.children[i] == node) {
            parent.children.splice(i, 1);
            break;
        }

}

function txtCut(s) {
    var l = s.length * (scale * 0.6 - 1);
    l = Math.ceil(l);
    //quito acentos, no van en las etiquetas de los nodos
    s = s.replace('&#225;', 'a');
    s = s.replace('&#193;', 'A');
    s = s.replace('&#233;', 'e');
    s = s.replace('&#201;', 'E');
    s = s.replace('&#237;', 'i');
    s = s.replace('&#205;', 'I');
    s = s.replace('&#243;', 'o');
    s = s.replace('&#211;', 'O');
    s = s.replace('&#250;', 'u');
    s = s.replace('&#218;', 'U');

    s = s.replace('&#224;', 'a');
    s = s.replace('&#192;', 'A');
    s = s.replace('&#232;', 'e');
    s = s.replace('&#200;', 'E');
    s = s.replace('&#236;', 'i');
    s = s.replace('&#204;', 'I');
    s = s.replace('&#242;', 'o');
    s = s.replace('&#210;', 'O');
    s = s.replace('&#249;', 'u');
    s = s.replace('&#217;', 'U');
    //corto
    if (l < 20) l = 20;
    if (l < s.length)
        return s.substring(0, l) + '...';
    else
        return s;
}

function setParents(node) {
    var hijos = node.children;
    for (var i in hijos) {
        var hijo = hijos[i];
        hijo.parent = node;                 //aprovecho la recorrida y fijo al padre de cada nodo
        setParents(hijo);
    }
}

function updateFloresTotales(node) {
    if (node.flores == undefined)
        if (node.children)
            node.flores = 0;
        else
            node.flores = 1;

    node.totalFlores = node.flores;

    //obtengo los hijos 
    var hijos = node.children;
    var totalFlores = 0;
    for (var i in hijos) {
        var hijo = hijos[i];
        //hijo.nivel = node.nivel + 1;
        updateFloresTotales(hijo);                          //<<------recursion
        totalFlores += hijo.totalFlores;
    }
    node.totalFlores = totalFlores + node.flores;

    if (node.totalFlores > maxWidth)
        useMaxWidth = true; //esto es para que se dibuje proporcional

    //defino texto de parametros de consenso si es una hoja
    //node.negados = getNegados(node);
    var modelo = getModelo(node.modeloID);
    var ap = arbolPersonal;
    if (modelo && node.nivel == node.niveles) {
        //es una hoja de un debate completo, mido condicion de consenso

        node.si = trSi + ':' + node.flores; // + "≥" + ap.minSiValue;
        if (node.flores >= ap.minSiValue)
            node.siColor = 'green';
        else
            node.siColor = 'red';

        node.no = trNo + ':' + node.negados; // + "≤" + ap.maxNoValue;
        if (node.negados <= ap.maxNoValue)
            node.noColor = 'green';
        else
            node.noColor = 'red';
    }
    else {
        node.si = "";
        node.no = "";
    }
}

function getNodo(id) {
    return getNodo2(arbolPersonal.raiz, id);
}

function getNodo2(padre, id) {
    var encontrado;

    if (padre.id == id)
        return padre;
    else
    {
        for (var i in padre.children)
        {
            var hijo = padre.children[i];
            encontrado = getNodo2(hijo, id);
            if (encontrado != null)
                return encontrado;
        }
    }
    return null;
}

function docClick(d) {
    window.open(d.URL);
}

function florClick(d) {
    if (!historico) {
        if (d.id != 0) {
            var n = getNodo(d.id);
            nodeClick(n);
        }
        else
            nodeClick(arbolPersonal.raiz);
    }
}

function selectNode(d) {
    selectedNode = d;

    //remember parents
    var n = d;
    parents = [];
    while (n.parent) {
        parents.push(n);
        n = n.parent;
    }
}

function nodeClick(d) {
    //guardo nodo seleccionado
    if (menu != null)
        menu.style.visibility = "hidden";

    selectNode(d);

    //cambio imagen al nodo seleccionado
    dibujarArbol(selectedNode);

    //activo panel
    showPanel();

    //muevo menuNode
    var menuNode = document.getElementById("menuNode");

    var transform = "translate(" + translatex.toFixed(0) + "px," + translatey.toFixed(0) + "px)rotate(" + (d.x - 180).toFixed(0) + "deg)translate(" + (d.y).toFixed(0) + "px)rotate(" + ((d.x - 180) * -1).toFixed(0) + "deg)";
    menuNode.style.transform = transform;

    //activo menu contextual 
    if (!historico) {
        if (arbolPersonal.usuario.habilitado && !arbolPersonal.usuario.readOnly) {
            if (selectedNode == arbolPersonal.raiz) {
                seleccionarModelo();
            }
            else {
                menu = document.getElementById("menuNode");
                menu.style.visibility = "visible";
            }
        }
    }
}

function pany(y) {
    //if (visual.level == 1)
        translateArbol(translatex, translatey += y * 3)  //paso a paso
    //else {
    //    clearInterval(joyInterval);
    //    joyInterval = setInterval(function ()
    //    {
    //        translateArbol(translatex, translatey += y)
    //    }, 20); //continuo
    //}
}

function panx(x) {
    //if (visual.level == 1)
        translateArbol(translatex += x * 3, translatey)  //paso a paso
    //else {
    //    clearInterval(joyInterval);
    //    joyInterval = setInterval(function ()
    //    {
    //        translateArbol(translatex += x, translatey)
    //    }, 20); //continuo
    //}
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
