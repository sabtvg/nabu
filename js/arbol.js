var duration = 750,
    tree,
    diagonal,
    svg,
    treeScale = 1;

var menu;
var downEvent = null;
var translatex = 0;
var translatey = 0;
var rotFlores;

function crearArbol() {
    var diameter = window.innerHeight / 2;
    tree = d3.layout.tree()
        .size([180, diameter / 2])
        .separation(function (a, b) {
            return (a.parent == b.parent ? 1 : 2) / a.depth;
        });

    diagonal = d3.svg.diagonal.radial()
        .projection(function (d) { return [d.y, d.x / 180 * Math.PI - Math.PI / 2]; });

    svg = d3.select("body").append("svg")
        .attr("id", "svg")
        .attr("width", window.innerWidth - 40)
        .attr("height", window.innerHeight - 50)
        .on("mousedown", doMousedown)
        .on("mousemove", doMousemove)
        .on("mouseup", doMouseup)
        .on("mousewheel", doMousewheel)
        .on("DOMMouseScroll", doMousewheel)  //firefox
        .append("g")
        .on("mousedown", doMousedown)
        .on("mousemove", doMousemove)
        .on("mouseup", doMouseup)
        .on("mousewheel", doMousewheel)  //chrome
        .attr("transform", "translate(" + window.innerWidth / 2 + "," + (window.innerHeight * 0.90) + ")");

    dibujarArbol(arbolPersonal.raiz);

    d3.select(self.frameElement).style("height", window.innerHeight + "px");
}

function getModelo(id) {
    for (var i in modelosDocumento) {
        if (modelosDocumento[i].id == id)
            return modelosDocumento[i];
    }
    return null;
}

function doMousewheel() {
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
    var e = window.event || Event || e; // old IE support
    downEvent = { "x": e.clientX, "y": e.clientY, "translatex": translatex, "translatey": translatey };
    if (menu)
        menu.style.visibility = "hidden";
}

function doMousemove() {
    if (downEvent) {
        var e = window.event || Event || e; // old IE support
        translatex = downEvent.translatex - (downEvent.x - e.clientX);
        translatey = downEvent.translatey - (downEvent.y - e.clientY);
        if (translatey < 0) translatey = 0;
        translateArbol(translatex, translatey);        
    }
}

function doMouseup() {
    downEvent = null;
}

function translateArbol(x, y) {
    d3.select("svg").select("g").attr("transform", "translate(" + (window.innerWidth / 2 + x) + "," + (window.innerHeight * 0.90 + y) + ")");
}

function rotarFlores() {
    rotFlores++;

    d3.select("svg").selectAll(".florImage")
        .attr("transform", "rotate(" + rotFlores + ")translate(-19, -19)");
}

function dibujarArbol(referencia) {

    //actualizo las flores de los padres
    updateFloresTotales(arbolPersonal.raiz);

    // Compute the new tree layout.
    var nodes = tree.nodes(arbolPersonal.raiz).reverse(),
        links = tree.links(nodes);

    // Normalize for fixed-depth.
    nodes.forEach(function (d) {
        if (isNaN(d.x)) d.x = 90;
        d.y = d.depth * 140 * treeScale;
    });

    //node -----------------------------------------------------------------------------------------------------------------------
    var node = svg.selectAll("g.node")
        .data(nodes, function (d) { return 'g' + d.id; });

    // Enter any new nodes at the parent's previous position.
    var nodeEnter = node.enter().append("g")
        .attr("id", function (d) {
                return 'g' + d.id;
        })
        .attr("class", "node")
        .attr("transform", function (d) {
            if (referencia)
                return "rotate(" + (referencia.x - 180) + ")translate(" + referencia.y + ")";
            else if (d.parent)
                return "rotate(" + (d.parent.x - 180) + ")translate(" + d.parent.y + ")";
            else
                return "rotate(0)translate(0)";
        })
        .on("click", nodeClick);

    nodeEnter.append("circle")
        .attr("r", function (d) {
            var r = d.totalFlores / 4 + 2;
            if (r < 10) r = 10;
            if (visual.level == 1)
                r = r * 2;
            return r * scale;
        })
        .style("fill", function (d) {
            return d.consensoAlcanzado ? "gray" : "yellow";
        });

    //1ra linea de texto
    nodeEnter.append("text")
        .attr("id", function (d) {
            return 't1.' + d.id;
        })
        .attr("x", function (d) {
            return 0;
        })
        .attr("dy", function (d) {
            if (d.si && !d.consensoAlcanzado)
                return -40;
            else
                return 0;
        })
        .attr("transform", function (d) {
            if (d.si && !d.consensoAlcanzado)
                return "rotate(90)";
            else
                return "rotate(10)";
        })
        .attr("fill", function (d) {
            if (d.si && !d.consensoAlcanzado)
                return d.siColor;
            else
                return "black";
        })
        .attr("text-anchor", function (d) { return "middle"; });

    //2da linea de texto
    nodeEnter.append("text")
        .attr("id", function (d) {
            return 't2.' + d.id;
        })
        .attr("x", function (d) {
            return 0;
        })
        .attr("dy", function (d) {
            if (d.si && !d.consensoAlcanzado)
                return -40 + 16 * scale;
            else
                return 0;
        })
        .attr("transform", function (d) {
            if (d.si && !d.consensoAlcanzado)
                return "rotate(90)";
            else
                return "rotate(10)";
        })
        .attr("fill", function (d) {
            if (d.no && !d.consensoAlcanzado)
                return d.noColor;
            else
                return "black";
        })
        .attr("text-anchor", function (d) { return "middle"; });

    // Transition nodes to their new position.
    var nodeUpdate = node.transition()
        .duration(duration)
        .attr("transform", function (d) {
            return "rotate(" + (d.x - 180) + ")translate(" + d.y + ")";
        })

    nodeUpdate.select("circle")
        .attr("r", function (d) {
            var r = d.totalFlores / 4 + 2;
            if (r < 10) r = 10;
            return r * scale;
        })
        .style("stroke", function (d) {
            return d == selectedNode ? "black" : "steelblue";
        })
        .style("fill", function (d) {
            return d.consensoAlcanzado ? "gray" : (d == selectedNode ? "blue" : "yellow");
        });

    nodeUpdate.select("text")
        .style("font-size", function (d) {
            var size = 20 * scale > 30 ? 30 : 15 * scale;
            document.getElementById('t2.' + d.id).style.fontSize = size + 'px'; //actualizo el 2do texto
            return size + 'px';
        })
        .text(function (d) {
            if (d.si && !d.consensoAlcanzado) {
                document.getElementById('t2.' + d.id).innerHTML = d.no; //actualizo el 2do texto
                return d.si;
            }
            else {
                document.getElementById('t2.' + d.id).innerHTML = ""; //actualizo el 2do texto
                return txtCut(d.nombre);
            }
        })
        .style("fill", function (d) {
            document.getElementById('t2.' + d.id).style.fill = d.noColor;
            return d.si ? d.siColor : (d == selectedNode ? "blue" : "black");
        })
        .style("fill-opacity", 1);

    // Transition exiting nodes to the parent's new position.
    var nodeExit = node.exit().transition()
        .duration(duration)
        .attr("transform", function (d) {
            if (referencia)
                return "rotate(" + (referencia.x - 180) + ")translate(" + referencia.y + ")";
            else if (d.parent)
                return "rotate(" + (d.parent.x - 180) + ")translate(" + d.parent.y + ")";
            else
                return "rotate(0)translate(0)";
        })
        .remove();

    nodeExit.select("circle")
        .attr("r", function (d) {
            var r = d.totalFlores / 2 + 2;
            return r * scale;
        })

    nodeExit.select("text")
        .style("fill-opacity", 1e-6)
        .attr("text-anchor", function (d) { return d.x < 180 ? "start" : "end"; })
        .attr("transform", function (d) { return d.x < 180 ? "translate(10)" : "rotate(180)translate(-8)"; });

    //flores ------------------------------------------------------------------------------------------------------------------
    var flor = svg.selectAll("g.flor")
        .data(arbolPersonal.usuario.flores);

    var florEnter = flor.enter().append("g")
        .attr("class", "flor")
        .attr("transform", "translate(0,0)")
        .on("click", florClick);

    florEnter.append("image")
        .attr("class", "florImage")
        .attr("width", "37px")
        .attr("height", "36px")
        .attr("transform", "translate(-19, -19)")
        .attr("xlink:href", "res/icono2.png");

    var florUpdate = flor.transition()
        .duration(duration)
        .attr("transform", function (d) {
            if (d.id == 0)
                //flor disponible
                return "rotate(0)translate(0)";
            else {
                var n = getNodo(d.id);
                if (n)
                    return "rotate(" + (n.x - 180) + ")translate(" + n.y + ")";
                else
                    //error de consistencia
                    return "rotate(0)translate(0)";
            }
        });

    florUpdate.select("image")
        .attr("width", "37px")
        .attr("height", "36px")
        .attr("xlink:href", "res/icono2.png");

    var florExit = flor.exit().transition()
        .duration(duration)
        .attr("transform", "rotate(0)translate(0)")
        .remove();

    //documentos ------------------------------------------------------------------------------------------------------------------
    var doc = svg.selectAll("g.doc")
        .data(arbolPersonal.logDocumentos, function (d) { return 'd' + d.docID; });

    var docEnter = doc.enter().append("g")
        .attr("class", "doc")
        .attr("transform", "translate(0,0)")
        .on("click", docClick);

    docEnter.append("image")
        .attr("class", "docImage")
        .attr("width", "32px")
        .attr("height", "40px")
        .attr("transform", "translate(-16, -20)")
        .attr("xlink:href", "res/doc.png");

    docEnter.append("text")
        .attr("x", function (d) {
            return 0;
        })
        .attr("dy", ".35em")
        .attr("transform", "rotate(90)translate(-5, 30)")
        .style("font-size", function (d) { return '12px'; })
        .attr("text-anchor", "middle ")
        .text(function (d) { return d.fname; });

    docEnter.append("text")
        .attr("x", function (d) {
            return 0;
        })
        .attr("dy", ".35em")
        .attr("transform", "rotate(90)translate(-5, 45)")
        .style("font-size", function (d) { return '12px'; })
        .attr("text-anchor", "middle ")
        .text(function (d) { return d.titulo; });

    docEnter.append("text")
        .attr("x", function (d) {
            return 0;
        })
        .attr("dy", ".35em")
        .attr("transform", "rotate(90)translate(-5, 60)")
        .style("font-size", function (d) { return '12px'; })
        .attr("text-anchor", "middle ")
        .text(function (d) { return d.sFecha; });

    var docUpdate = doc.transition()
        .duration(duration)
        .attr("transform", function (d) {
            if (arbolPersonal.simulacion)
                return "rotate(" + (d.x - 180) + ")translate(" + (5 * 140 * treeScale + d.minutos * 50 + 120) + ")";
            else
                return "rotate(" + (d.x - 180) + ")translate(" + (5 * 140 * treeScale + d.dias * 50 + 120) + ")";
        });

    docUpdate.select("image")
        .attr("width", "32px")
        .attr("height", "40px")
        .attr("transform", "rotate(90)translate(-16, -20)")
        .attr("xlink:href", "res/doc.png");

    var docExit = doc.exit().transition()
        .duration(duration)
        .attr("transform", "rotate(0)translate(0)")
        .remove();

    // link ------------------------------------------------------------------------------------------------------------------------
    var link = svg.selectAll("path")
        .data(links, function (d) {
            return d.target.id;
        });

    // Enter any new links at the parent's previous position.
    link.enter().insert("path", "g")
        .attr("style", function (d) {
            var w = d.target.totalFlores + 1;
            var r = Math.round(200 / d.target.depth);
            return "fill: none; stroke: " + (d.target.consensoAlcanzado ? "gray" : "rgb(100," + (255 - r) + ",0)") + ";stroke-width: " + w * scale + "px;";
        })
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
            var w = d.target.totalFlores + 1;
            var r = Math.round(200 / d.target.depth);
            return "fill: none; stroke: " + (d.source.consensoAlcanzado ? "gray" : "rgb(100," + (255 - r) + ",0)") + ";stroke-width: " + w * scale + "px;";
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
    if (l < 20) l = 20;
    if (l < s.length)
        return s.substring(0, l) + '...';
    else
        return s;
}

function updateFloresTotales(node) {
    if (node.flores == undefined)
        if (node.children)
            node.flores = 0;
        else
            node.flores = 1;

    //if (node.nivel == undefined)
    //    node.nivel = 0;

    node.totalFlores = node.flores;

    //obtengo los hijos segun si se visualiza o no
    var hijos = node.children;
    var totalFlores = 0;
    for (var i in hijos) {
        var hijo = hijos[i];
        hijo.parent = node;                 //aprovecho la recorrida y fijo al padre de cada nodo
        //hijo.nivel = node.nivel + 1;
        updateFloresTotales(hijo);
        totalFlores += hijo.totalFlores;
    }
    node.totalFlores = totalFlores + node.flores;

    //defino texto de parametros de consenso si es una hoja
    //node.negados = getNegados(node);
    var modelo = getModelo(node.modeloID);
    var ap = arbolPersonal;
    if (modelo && node.nivel == modelo.secciones.length) {
        //es una hoja de un debate completo, mido condicion de consenso

        node.si = 'Si:' + node.flores + "≥" + ap.minSiValue;
        if (node.flores >= ap.minSiValue)
            node.siColor = 'green';
        else
            node.siColor = 'red';

        node.no = 'No:' + node.negados + "≤" + ap.maxNoValue;
        if (node.negados <= ap.maxNoValue)
            node.noColor = 'green';
        else
            node.noColor = 'red';
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
    if (!arbolPersonal.simulacion)
        doOpen('cooperativas/' + d.arbol + '/documentos/' + d.fname + '.html');
}

function florClick(d) {
    if (d.id != 0) {
        var n = getNodo(d.id);
        nodeClick(n);
    }
    else
        nodeClick(arbolPersonal.raiz);
}

function nodeClick(d) {
    //guardo nodo seleccionado
    if (menu != null)
        menu.style.visibility = "hidden";

    selectedNode = d;

    //activo menu contextual 
    if (selectedNode == arbolPersonal.raiz) {
        menu = document.getElementById("menuRoot");
        menu.style.left = (window.innerWidth / 2 - 26).toFixed(0) + 'px';
        menu.style.visibility = "visible";
    }
    else {
        menu = document.getElementById("menuNode");  
        menu.style.left = (window.innerWidth / 2 - 122).toFixed(0) + 'px';
        menu.style.visibility = "visible";

        //enseño los datos de consenso para este nodo
        actualizarDatosConsenso(selectedNode);

        //activo panel
        showPanel();

        //actualizo link a download
        var btnDownload = document.getElementById("btnDownload");
        btnDownload.href = 'doArbol.aspx?actn=download&id=' + selectedNode.id + '&arbol=' + arbolPersonal.nombre;
    }

}

function pany(y) {
    if (visual.level == 1)
        translateArbol(translatex, translatey += y * 3)  //paso a paso
    else
        clearInterval(joyInterval); joyInterval = setInterval(function () { translateArbol(translatex, translatey += y) }, 20); //continuo
}

function panx(x) {
    if (visual.level == 1)
        translateArbol(translatex += x * 3, translatey)  //paso a paso
    else
        clearInterval(joyInterval); joyInterval = setInterval(function () { translateArbol(translatex += x, translatey) }, 20); //continuo
}
