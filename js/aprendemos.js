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




var quesoPersonal;
var quesoInicio = 60;
var selCeldaID;
var quesoScale = 2;
var quesoy = -400;
var quesox = 0;
var quesoCeldah = 10;
var minHeight = 60;
var HTMLSelectedCelda;
var scalex = window.innerWidth / 1920;
var scaley = window.innerHeight / 1080;

function doAprendemos() {
    //menu ppal
    if (visual.level == 1)
        document.getElementById("menuppal").style.visibility = "hidden";
    else
        efectoOpacity(document.getElementById("menuppal"), 0, 1, 0, TWEEN.Easing.Linear.None, function () { document.getElementById("menuppal").style.visibility = "hidden"; });

    document.getElementById("panelGrupo").style.display = 'none';

    //titulo
    document.getElementById("titulo").innerHTML = arbolPersonal.nombre + " - " + tr("Evaluamos");
    document.getElementById("titulo").style.visibility = 'visible';
    document.getElementById("pie").style.display = "none";

    //joystick
    document.getElementById("joystickQueso").style.visibility = 'visible';

    //activo el aprendemos
    setTimeout(function () {
        document.getElementById("quesoDiv").style.display = "block";
        //menu
        document.getElementById("menuEvaluacion").style.visibility = historico ? "hidden" : "visible";

        estado = 'aprendemos';

        //dibujo
        getQuesoPersonal();

    }, 500);

    timerQueso = setInterval(getQuesoPersonal, refreshInterval);
    clearInterval(timerArbol);
    clearInterval(timerGrupo);


}

function translateQueso(x, y) {
    d3.select("#svgAprendemos").select("g").attr("transform", "translate(" + (window.innerWidth / 2 + x).toFixed(0) + "," + (window.innerHeight / 2 + y).toFixed(0) + ")");
}

function getQuesoPersonal() {
    if (!preguntarAlSalir) {
        getHttp("doAprendemos.aspx?actn=getQuesoPersonal&email=" + arbolPersonal.usuario.email
        + "&grupo=" + arbolPersonal.nombre
        + "&clave=" + arbolPersonal.usuario.clave,
         function (data) {
             //atrapo el error si es que hay
             if (data.substring(0, 6) == "Error=") {
                 //ha habido un error
                 document.getElementById("msgDiv").innerHTML = '<font color=red>' + data + '</font>';
             }
             else {
                 //tengo el bosque
                 //document.getElementById("todo").innerHTML = data;
                 recibirQuesoPersonal(data);
             }
         });
    }
}

function recibirQuesoPersonal(data) {
    var primeraVez = quesoPersonal == null;
    quesoPersonal = JSON.parse(data);
    if (primeraVez)
        autoScale();
    dibujarQueso();
}

function autoScale() {
    var maxAcumh = minHeight;
    if (quesoPersonal.temas == 0)
        quesoScale = 2;
    else {
        for (d in quesoPersonal.temas) {
            d = parseInt(d);
            var tema = quesoPersonal.temas[d];
            if (!tema.celdas) tema.celdas = [];
            var acumh = 0;
            for (n = 0; n < tema.respuestas.length; n++) acumh += quesoCeldah + tema.evaluaciones.length;
            if (acumh > maxAcumh) maxAcumh = acumh;
        }
        //fijo escala
        quesoScale = (window.innerHeight * 0.5) / maxAcumh;
        quesoy = -maxAcumh * quesoScale;
    }
}

function dibujarQueso() {
    //msg
    var msg = document.getElementById("msgDiv");
    msg.innerHTML = quesoPersonal.msg;

    //preparo la estructura para dibujarla
    var maxAcumh = 0;
    var celdaid = 0;
    var totalEvaluaciones = 0;
     for (d in quesoPersonal.temas) {
        d = parseInt(d);
        var tema = quesoPersonal.temas[d];
        if (!tema.celdas) tema.celdas = [];

        var ancho = 6; //grados
        var acumh = 0;
        if (ancho * quesoPersonal.temas.length > 165) ancho = parseInt(165 / quesoPersonal.temas.length);

        tema.iniDeg = 90 - quesoPersonal.temas.length / 2 * ancho + ancho * d;
        tema.finDeg = 90 - quesoPersonal.temas.length / 2 * ancho + ancho * (d + 1);

        tema.celdas = [];
        for (n = 0; n < tema.respuestas.length; n++) {
            var celda = { top: acumh, height: quesoCeldah + tema.evaluaciones.length, respuesta: tema.respuestas[n], id: d + ":" + n };
            tema.celdas.push(celda);
            acumh += celda.height;

        }

        if (acumh > maxAcumh) maxAcumh = acumh;
        totalEvaluaciones += tema.evaluaciones.length;
    }

    //dibujo
    var quesoDiv = document.getElementById("quesoDiv");
    var ret = "";

    ret += "<svg id='svgAprendemos' width='" + window.innerWidth + "' height='" + window.innerHeight + "' xmlns='http://www.w3.org/2000/svg' version='1.1'>";

    ret += "<g transform='translate(" + (window.innerWidth / 2 + quesox).toFixed(0) + "," + (window.innerHeight / 2 + quesoy).toFixed(0) + ")'>";

    //dinujo columnas 
    HTMLSelectedCelda = "";
    for (col in quesoPersonal.temas) {
            ret += dibujarTema(col);
    }

    //ahora dibujo la celda seleccionada por encima de las demas
    if (HTMLSelectedCelda)
        ret += HTMLSelectedCelda;

    //circle
    //ret += "<circle cx='0' cy='0' r='5' stroke='blue' stroke-width='3' fill='white' />";

    ret += "</g>";
    ret += "</svg>";

    //panelqueso
    var panel = document.getElementById("panelQueso");;
    var ap = arbolPersonal;
    var born = new Date(ap.born.match(/\d+/)[0] * 1);
    var dias = Math.abs(new Date() - born) / (24 * 60 * 60 * 1000);
    var pan = "";
    var HTMLColorPromedio = quesoPersonal.colorPromedio == 0 ? "transparent" : HTMLColor(quesoPersonal.colorPromedio);

    pan = "<div class='titulo3' style='margin: 0px;padding:0px;'><nobr>" + tr("Evaluaciones") + ":" + totalEvaluaciones + "</nobr></div>";
    pan += "<div class='titulo3' style='margin: 0px;padding:0px;'><nobr>" + tr("Promedio") + ": <span style='border:1px solid gray;background-color:" + HTMLColorPromedio + "'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span></nobr></div>";
    panel.innerHTML = pan; 
    panel.style.display = 'inline';

    quesoDiv.innerHTML = ret;
}

function dibujarTema(temaIndex) {
    //celdas
    var ret = "";
    var acumBoton = minHeight;  //altura del boton de agregar
    var tema = quesoPersonal.temas[temaIndex];
    for (c in tema.celdas) {
        //dibujo celda
        var celda = tema.celdas[c];
        if (celda.id != selCeldaID)
            ret += getSVGCelda(temaIndex, c, tema.iniDeg, tema.finDeg, celda, false);
        else
            HTMLSelectedCelda = getSVGCelda(temaIndex, c, tema.iniDeg, tema.finDeg, celda, true);  //la guerpo para dinujarla despues (zindex)
        acumBoton = celda.top + celda.height;
    }
        
    var botonScale = acumBoton / minHeight * quesoScale / 5;
    if (botonScale > 2) botonScale = 2;


    ////linea
    //var m = queso.margen + getTop1raCelda(columna) * scaleQueso;
    //var a = columna.iniDeg + (columna.finDeg - columna.iniDeg) / 2;
    //var style = col == selColIndex ? "stroke:blue;stroke-width:4" : "stroke:black;stroke-width:2";
    //ret += "<line x1='" + cart(queso.margen, a - 180).x + "' y1='" + cart(queso.margen, a - 180).y + "' x2='" + cart(m, a).x + "' y2='" + cart(m, a).y + "' style='" + style + "' />";

    //cantidad de evaluaciones
    var a = tema.iniDeg + (tema.finDeg - tema.iniDeg) / 2;
    var dist = botonScale > 1 ? 15 : 15 * botonScale;
    var p = cart(quesoInicio + acumBoton * quesoScale + dist, a);
    ret += "<g transform='translate(" + p.x + "," + p.y + ")'>";
    var w = botonScale > 1 ? 15 : 15 * botonScale;
    ret += "<text x='0' y='0' font-size='" + w + "' fill='blue' transform='rotate(" + (a - 90) + ")translate(0,5)' text-anchor='middle'>(" + tema.evaluaciones.length + ")</text>";
    ret += "</g>";

    //icono documento
    dist = botonScale > 1 ? 28 : 28 * botonScale;
    p = cart(quesoInicio + acumBoton * quesoScale + dist, a);
    ret += "<g transform='translate(" + p.x + "," + p.y + ")'>";
    ret += "<image xlink:href='" + tema.icono + "' x='-" + (32 * botonScale / 2) + "' y='0' style='cursor:pointer' transform='rotate(" + (a - 90) + ")'  height='" + (40 * botonScale) + "px' width='" + (32 * botonScale) + "px' onclick='window.open(\"" + tema.URL + "\");'/>";
    ret += "</g>";

    //boton agregar
    if (!historico) {
        p = cart(quesoInicio + acumBoton * quesoScale + 70 * botonScale, a);
        ret += "<g transform='translate(" + p.x + "," + p.y + ")'>";
        w = botonScale > 1 ? 40 : 40 * botonScale;
        ret += "<image xlink:href='res/proponer.png' x='-" + (w / 2) + "' y='0' style='cursor:pointer' transform='rotate(" + (a - 90) + ")'  height='" + w + "px' width='" + w + "px' onclick='doEvaluarTema(\"" + tema.id + "\");'/>";
        ret += "</g>";
    }

    //titulo
    p = cart(quesoInicio + acumBoton * quesoScale + 110 * botonScale, a);
    ret += "<g transform='translate(" + p.x + "," + p.y + ")'>";
    ret += "<text x='0' y='0' fill='blue' transform='rotate(" + a + ")translate(0,5)' text-anchor='start'>" + tema.nombre + "</text>";
    ret += "</g>";
    return ret;
}

function doSelectCelda(celdaid) {
    selCeldaID = celdaid;
    dibujarQueso(); //resalto celda seleccionada
    var temaIndex = celdaid.substring(0, celdaid.indexOf(":"));
    var preguntaIndex = celdaid.substring(celdaid.indexOf(":") + 1);

    //pido contenido al servidor
    getHttp("doAprendemos.aspx?actn=getQuesoResultado&email=" + arbolPersonal.usuario.email
        + "&grupo=" + arbolPersonal.nombre
        + "&temaIndex=" + temaIndex
        + "&preguntaIndex=" + preguntaIndex
        + "&clave=" + arbolPersonal.usuario.clave,
         function (data) {
             //atrapo el error si es que hay
             if (data.substring(0, 6) == "Error=") {
                 //ha habido un error
                 document.getElementById("msgDiv").innerHTML = '<font color=red>' + data + '</font>';
             }
             else {
                 //enseño info
                 document.getElementById("documento").innerHTML = data;

                 //activo editores de estilo
                 activarStyleEditor();

                 document.getElementById("documento").style.display = 'block';
             }
         });
}

function getSVGCelda(temaIndex, nivel, iniDeg, finDeg, celda, selected) {
    //poligono
    var ret = "";
    var color = HTMLColor(celda.respuesta); 
    var stroke = selected ? "blue" : "gray";
    ret = "<polygon fill='" + color + "' stroke='" + stroke + "' stroke-width='1' style='cursor:pointer;' ";
    ret += "points='" + getSVGPoints(temaIndex, iniDeg, finDeg, celda) + "' ";
    if (!historico) 
        ret += "onclick='doSelectCelda(\"" + celda.id + "\");'";
    ret += "/>";

    //texto
    var m = quesoInicio + celda.top * quesoScale + celda.height * quesoScale / 2 + 10;
    var a = iniDeg + (finDeg - iniDeg) / 2;
    var txt = '' + celda.respuesta;
    ret += "<g transform='translate(" + cart(m, a).x + "," + cart(m, a).y + ")'>";
    //ret += "<text x='0' y='0' fill='white'  transform='rotate(" + (a - 90) + ") translate(" + -5 * txt.length + ")' style='cursor:pointer;' "
    //ret += "text-anchor='start' onclick='doSelectCelda(\"" + celda.id + "\");'>";
    ret += txt + "</text>";
    ret += "</g>";

    return ret;
}

function getSVGPoints(index, iniDeg, finDeg, celda) {
    var ret = "";
    var m = quesoInicio + celda.top * quesoScale;
    for (var a = iniDeg; a <= finDeg; a++) {
        ret += cart(m, a).x + "," + cart(m, a).y + " ";
    }
    m = quesoInicio + celda.top * quesoScale + celda.height * quesoScale;
    for (var a = finDeg; a >= iniDeg; a--) {
        ret += cart(m, a).x + "," + cart(m, a).y + " ";
    }
    return ret;
}

function seleccionarModeloEvaluacion() {
    //opciones de modelos de documentos
    var listE = "<table style='margin:auto;'>";

    //si no hay manifiesto solo permito crear modelo Manifiesto
    for (var i in modelosEvaluacion) {
        var modelo = modelosEvaluacion[i];
        if (modelo.activo) {
                listE += "<tr>";
                listE += "<td><img src='" + modelo.icono + "' style='width:32px;height:40px;cursor:pointer;' onclick='seleccionarModeloEvaluacionID(\"" + modelo.id + "\");'></td>";
                listE += "<td style='text-align: left;margin:5px;cursor:pointer;padding:4px;' onclick='seleccionarModeloEvaluacionID(\"" + modelo.id + "\");'>" + modelo.nombre + "</td>";
                listE += "</tr>";
        }
    }
    listE += "<tr>";
    listE += "<td style='vertical-align:top;font-size:12px;text-align:center;padding:5px;' colspan='2'>";
    listE += "<input id='btnCancelar' type='button' value='" + tr("Cancelar") + "' class='btn' onclick='document.getElementById(\"modelosEvaluacion\").style.display = \"none\";'/>";
    listE += "</td>"
    listE += "</tr>";
    listE += "</table>";

    document.getElementById("modelosEvaluacion").innerHTML = listE;
    document.getElementById("modelosEvaluacion").style.display = "block";
}

function seleccionarModeloEvaluacionID(modeloID) {
    document.getElementById("modelosEvaluacion").style.display = "none";
    doVerEvaluacion(modeloID);
}

function doEvaluarTema(idTema) {

    //pido propuestas al servidor (por si hay nuevos comentarios)
    //agrego el modelo por si de un documento nuevo y el node es la raiz, hay que decir el modeloID
    getHttp("doAprendemos.aspx?actn=EvaluarTema&idTema=" + idTema
        + "&grupo=" + arbolPersonal.nombre
        + "&email=" + arbolPersonal.usuario.email
        + "&clave=" + arbolPersonal.usuario.clave
        + "&width=" + (window.innerWidth - 200).toFixed(0),
        function (data) {
            //puedo mostrar el documento
            preguntarAlSalir = true;

            //muestro
            document.getElementById("documento").innerHTML = data;

            //activo editores de estilo
            activarStyleEditor();

            document.getElementById("documento").style.display = 'block';
        }
    );
}

function doVerEvaluacion(modeloID) {

    //pido propuestas al servidor (por si hay nuevos comentarios)
    //agrego el modelo por si de un documento nuevo y el node es la raiz, hay que decir el modeloID
    getHttp("doAprendemos.aspx?actn=HTMLEvaluacion&modeloEvaluacion=" + modeloID
        + "&grupo=" + arbolPersonal.nombre
        + "&email=" + arbolPersonal.usuario.email
        + "&clave=" + arbolPersonal.usuario.clave
        + "&width=" + (window.innerWidth - 200).toFixed(0),
        function (data) {
            //puedo mostrar el documento
            preguntarAlSalir = true;

            //muestro
            document.getElementById("documento").innerHTML = data;

            //activo editores de estilo
            activarStyleEditor();

            document.getElementById("documento").style.display = 'block';
        }
    );
}

function doRevisarEvaluacion(modeloID) {
    //envio
    getHttp("doAprendemos.aspx?actn=revisar&modelo=" + modeloID
        + "&email=" + arbolPersonal.usuario.email
        + "&clave=" + arbolPersonal.usuario.clave
        + "&grupo=" + arbolPersonal.nombre
        + "&width=" + (window.innerWidth - 200).toFixed(0),
        function (data) {
            //muestro
            document.getElementById("documento").innerHTML = data;

            //activo editores de estilo
            activarStyleEditor();
        });
}

function doPrevistaEvaluacion(modeloID) {
    //leo los textos del documento y los envio al servidor
    var post = getPost(document.getElementById("documento"));

    //envio
    postHttp("doAprendemos.aspx?actn=prevista&modelo=" + modeloID
        + "&email=" + arbolPersonal.usuario.email
        + "&clave=" + arbolPersonal.usuario.clave
        + "&grupo=" + arbolPersonal.nombre
        + "&width=" + (window.innerWidth - 200).toFixed(0),
        post,
        function (data) {
            //muestro
            document.getElementById("documento").innerHTML = data;

            //activo editores de estilo
            activarStyleEditor();
        });
}

function doCrearEvaluacion(modeloID) {
    //leo los textos del documento y los envio al servidor
    //envio
    setTimeout(function(){
        getHttp("doAprendemos.aspx?actn=crearEvaluacion&modelo=" + modeloID
            + "&email=" + arbolPersonal.usuario.email
            + "&clave=" + arbolPersonal.usuario.clave
            + "&grupo=" + arbolPersonal.nombre
            + "&width=" + (window.innerWidth - 200).toFixed(0),
            recibirQuesoPersonal
        );
    }, 1500); //doy tiempo al documento a salir de pantalla

    //cierro documento
    document.getElementById("documento").style.display = "none";
    preguntarAlSalir = false;
}

function evaluacionSubmit(accion, parametro, modeloID) {
    //envio mensaje al documento y redibujo
    var node = selectedNode;
    var post = getPost(document.getElementById("documento"));

    postHttp("doAprendemos.aspx?actn=evaluacionSubmit&modelo=" + modeloID
        + "&accion=" + accion
        + "&parametro=" + parametro
        + "&grupo=" + arbolPersonal.nombre
        + "&email=" + arbolPersonal.usuario.email
        + "&clave=" + arbolPersonal.usuario.clave
        + "&width=" + (window.innerWidth - 200).toFixed(0),
        post,
        function (data) {
            //muestro
            document.getElementById("documento").innerHTML = data;

            //activo editores de estilo
            activarStyleEditor();
        });
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
 

