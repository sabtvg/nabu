
var cantDebates = 8;
var cantArgumentos = 480;
var debates = [];
var argumentos = [];
var queso;
var factorh = 5;
var minh = 30;
var selColIndex;
var scaleQueso = 1;

function doAprendemos() {
    //menu ppal
    if (visual.level == 1)
        document.getElementById("menuppal").style.visibility = "hidden";
    else
        efectoOpacity(document.getElementById("menuppal"), 0, 1, 0, TWEEN.Easing.Linear.None, function () { document.getElementById("menuppal").style.visibility = "hidden"; });

    //demoMsg
    document.getElementById("demoMsg").style.visibility = "visible";
    document.getElementById("panelGrupo").style.visibility = 'hidden';
    document.getElementById("panelUsuario").style.visibility = 'hidden';

    //activo el aprendemos
    setTimeout(function () {
        if (visual.level == 1) {
            document.getElementById("aprendemos").style.visibility = "visible";
        }
        else {
            //menu ppal
            efectoOpacity(document.getElementById("aprendemos"), 0, 0, 1, TWEEN.Easing.Linear.None);
        }
        document.getElementById("aprendemos").style.display = "inline";

        estado = 'aprendemos';

        //dibujo
        ejemplo();
    }, 500);

}

function ejemplo() {
    crearEjemplo();
    dibujarQueso();
}

function crearQueso() {
    //creo la estructura del queso
    queso = { alto: 10, x: parseInt(window.innerWidth / 2), y: parseInt(150 * scaley), margen: parseInt(80 * scaley), columnas: [] };

    var apoyos = getMaxMinApoyos();
    var ancho = 15; //grados
    var maxAcumh = 0;
    var acumh = 0;
    if (ancho * debates.length > 165) ancho = parseInt(165 / debates.length);

    for (d in debates) {
        d = parseInt(d);
        var debate = debates[d];
        var columna = { debate: debates[d], celdas: [], iniDeg: 90 - debates.length / 2 * ancho + ancho * d, finDeg: 90 - debates.length / 2 * ancho + ancho * (d + 1) };

        //creo celdas de columna
        acumh = 0;
        for (i = 0; i < queso.alto; i++) {
            var step = apoyos.max / queso.alto;
            var celda = { min: (queso.alto - i - 1) * step, max: (queso.alto - i) * step, argumentos: [] };

            //le agrego sus argumentos
            for (a in argumentos) {
                var argumento = argumentos[a];
                if (argumento.debate == debate && argumento.apoyos > celda.min && argumento.apoyos <= celda.max)
                    celda.argumentos.push(argumento);
            }

            //calculo su altura
            var h = celda.argumentos.length * factorh; //altura de seguiridad
            if (h < minh) h = minh;
            celda.height = h;
            celda.top = acumh;
            acumh += h;

            columna.celdas.push(celda);
        }
        if (acumh > maxAcumh) maxAcumh = acumh;
        queso.columnas.push(columna);
    }
    //fijo escala
    scaleQueso = (window.innerHeight * scaley - queso.y) / (maxAcumh + 2 * queso.margen);
}

function doApoyos(v) {
    for (var i in argumentos) {
        argumentos[i].apoyos += parseInt(v);
    }
    dibujarQueso();
}

function dibujarQueso() {
    crearQueso();

    var quesoDiv = document.getElementById("aprendemos");
    var ret = "";
    ret += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
    ret += "<input type='button' value='menear+' onclick='doApoyos(1);'>";
    ret += "<input type='button' value='crear nuevos argumentos' onclick='ejemplo();'>";
    ret += "<input type='button' value='tema+' onclick='cantDebates++;crearEjemplo();dibujarQueso();'>";
    ret += "<input type='button' value='tema-' onclick='cantDebates--;crearEjemplo();dibujarQueso();'>";

    ret += "<svg width='" + window.innerWidth + "' height='" + window.innerHeight + "' xmlns='http://www.w3.org/2000/svg' version='1.1'>";
    //ret += "<rect x='1' y='1' width='100%' height='100%' fill='none' stroke='blue' stroke-width='2' />";
    ret += "<g transform='translate(" + queso.x + "," + queso.y + ")'>";

    //celdas
    //dinujo columnas no seleccionadas
    for (col in queso.columnas) {
        if (col != selColIndex)
            ret += dibujarColumna(col);
    }

    //dibujo columna seleccionada por encima de las otras
    for (col in queso.columnas) {
        if (col == selColIndex)
            ret += dibujarColumna(col);
    }


    //circle
    ret += "<circle cx='0' cy='0' r='5' stroke='blue' stroke-width='3' fill='white' />";

    //leyendas
    if (queso.columnas.length > 0) {
        ret += leyendas(queso.columnas[0]);
        ret += leyendas(queso.columnas[queso.columnas.length - 1]);
    }


    ret += "</g>";
    ret += "</svg>";

    quesoDiv.innerHTML = ret;
}

function dibujarColumna(colIndex) {
    //celdas
    var ret = "";
    var acumBoton = 0;  //altura del boton de agregar
    var columna = queso.columnas[colIndex];
    for (cel in columna.celdas) {
        var celda = columna.celdas[cel];
        ret += getSVGCelda(colIndex, parseInt(cel), columna.iniDeg, columna.finDeg, celda, col == selColIndex ? true : false);
        if (celda.argumentos.length != 0) acumBoton = celda.top + celda.height;
    }

    //linea
    var m = queso.margen + getTop1raCelda(columna) * scaleQueso;
    var a = columna.iniDeg + (columna.finDeg - columna.iniDeg) / 2;
    var style = col == selColIndex ? "stroke:blue;stroke-width:4" : "stroke:black;stroke-width:2";
    ret += "<line x1='" + cart(queso.margen, a - 180).x + "' y1='" + cart(queso.margen, a - 180).y + "' x2='" + cart(m, a).x + "' y2='" + cart(m, a).y + "' style='" + style + "' />";

    //circulo debate
    var stroke = col == selColIndex ? "blue" : "gray";
    ret += "<circle cx='" + cart(queso.margen, a - 180).x + "' cy='" + cart(queso.margen, a - 180).y + "' r='10' stroke='" + stroke + "' style='cursor:pointer;' stroke-width='3' fill='red' onclick='doSelectColumn(" + col + ");' />";

    //titulos
    style = col == selColIndex ? "font-weight:bold;" : "";
    ret += "<g transform='translate(" + cart(queso.margen + 20, a - 180).x + "," + cart(queso.margen + 20, a - 180).y + ")'>";
    ret += "<text x='0' y='0' fill='blue' style='" + style + "' transform='rotate(" + (a - 180) + ")translate(0,5)'>" + columna.debate.titulo + "</text>";
    ret += "</g>";

    //boton agregar
    m = queso.margen + acumBoton * scaleQueso + 25;
    ret += "<circle cx='" + cart(m, a).x + "' cy='" + cart(m, a).y + "' r='10' stroke='blue' stroke-width='3' fill='green' style='cursor:pointer;' onclick='doAgregar(" + col + ");' />";
    return ret;
}

function doSelectColumn(colIndex) {
    selColIndex = colIndex;
    dibujarQueso();
}

function doAgregar(colIndex) {
    var d = debates[colIndex];
    var argumento = { titulo: 'Titulo' + i, fecha: new Date(), descripcion: 'Descripcion ' + i, autor: 'Autor ' + i, debate: d, apoyos: 1 };
    argumentos.push(argumento);
    dibujarQueso();
}

function getTop1raCelda(columna) {
    for (c in columna.celdas) {
        var celda = columna.celdas[c];
        if (celda.argumentos.length > 0)
            return celda.top;
    }
    return 0;
}

function leyendas(columna) {
    var ret = "";
    var a;
    var m;

    for (cel in columna.celdas) {
        var celda = columna.celdas[cel];

        m = celda.top * scaleQueso + queso.margen + 8;
        if (columna.iniDeg < 90) {
            a = columna.iniDeg;
            ret += "<g transform='translate(" + cart(m, a).x + "," + cart(m, a).y + ")'>";
            ret += "<text x='0' y='0' fill='blue' transform='rotate(" + (a - 90) + ") translate(10)'>-" + celda.max.toFixed(0) + "</text>";
            ret += "</g>";
        }
        else {
            a = columna.finDeg;
            ret += "<g transform='translate(" + cart(m, a).x + "," + cart(m, a).y + ")'>";
            ret += "<text x='0' y='0' fill='blue' text-anchor='end'  transform='rotate(" + (a - 90) + ") translate(-10)'>" + celda.max.toFixed(0) + "-</text>";
            ret += "</g>";
        }
    }
    return ret;
}

function getSVGCelda(colIndex, celIndex, iniDeg, finDeg, celda, selected) {
    //poligono
    var ret = "";
    if (celda.argumentos.length != 0) {
        var color = "rgba(" + (1 - celIndex / queso.alto) * 100 + "%, " + celIndex / queso.alto * 100 + "%, " + (colIndex == selColIndex ? "50%" : "0%") + ", 0.6)";
        var stroke = selected ? "blue" : "gray";
        ret = "<polygon fill='" + color + "' stroke='" + stroke + "' stroke-width='3' style='cursor:pointer;' ";
        ret += "points='" + getSVGPoints(celIndex, iniDeg, finDeg, celda) + "' onclick='doSelectColumn(" + colIndex + ");'/>";

        //texto
        var m = celda.top * scaleQueso + celda.height * scaleQueso / 2 + queso.margen + 8;
        var a = iniDeg + (finDeg - iniDeg) / 2;
        var txt = '' + celda.argumentos.length;
        ret += "<g transform='translate(" + cart(m, a).x + "," + cart(m, a).y + ")'>";
        ret += "<text x='0' y='0' fill='white'  transform='rotate(" + (a - 90) + ") translate(" + -5 * txt.length + ")' style='cursor:pointer;' onclick='doSelectColumn(" + colIndex + ");'>" + txt + "</text>";
        ret += "</g>";
    }

    return ret;
}

function getSVGPoints(index, iniDeg, finDeg, celda) {
    var ret = "";
    var m = celda.top * scaleQueso + queso.margen + 1;
    for (var a = iniDeg; a <= finDeg; a++) {
        ret += cart(m, a).x + "," + cart(m, a).y + " ";
    }
    m = celda.top * scaleQueso + celda.height * scaleQueso - 1 + queso.margen;
    for (var a = finDeg; a >= iniDeg; a--) {
        ret += cart(m, a).x + "," + cart(m, a).y + " ";
    }
    return ret;
}

function crearEjemplo() {
    //creo debates

    debates = [];
    for (i = 0; i < cantDebates; i++) {
        var debate = { titulo: 'Titulo' + i, fecha: new Date(), descripcion: 'Descripcion ' + i };
        debates.push(debate);
    }

    //creo argumentos 
    for (i = 0; i < cantArgumentos; i++) {
        var d = debates[((debates.length - 1) * Math.random()).toFixed(0) ];
        var argumento = { titulo: 'Titulo' + i, fecha: new Date(), descripcion: 'Descripcion ' + i, autor: 'Autor ' + i, debate: d, apoyos: parseInt((10 * Math.random()).toFixed(0)) };
        argumentos.push(argumento);
    }

}

function getMaxMinApoyos() {
    var ret = { max: 0, min: 999999 };
    for (d in argumentos) {
        var argumento = argumentos[d];
        if (ret.max < argumento.apoyos) ret.max = parseInt(argumento.apoyos);
        if (ret.min > argumento.apoyos) ret.min = parseInt(argumento.apoyos);
    }
    return ret;
}

function polar(x,y){
	var r = Math.pow((Math.pow(x,2) + Math.pow(y,2)),0.5);
	var theta = Math.atan(y/x)*360/2/Math.PI;
	if (x >= 0 && y >= 0) {
		theta = theta;
	} else if (x < 0 && y >= 0) {
		theta = 180 + theta;
	} else if (x < 0 && y < 0) {
		theta = 180 + theta;
	} else if (x > 0 && y < 0) {
		theta = 360 + theta;
	} 
 
	return { m: Math.round(r), a: Math.round(theta) };
}
 
function cart(m,a){
    var x = m * Math.cos(a * 2 * Math.PI / 360);
    var y = m * Math.sin(a * 2 * Math.PI / 360);

    return { x: Math.round(x), y: Math.round(y) };
}
 

