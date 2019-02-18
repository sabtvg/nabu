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

var frasesDelDia = 16;
var frasesVistas = [];

var selectedNode;
var scale = window.innerWidth / 1920;
var estado = '';
var rotacionCiclo = 0;
var timerCiclo, timerFlores, timerArbol, timerGrupo, timerQueso;
var rotFlores = 0;
var refreshInterval = 10000; //10seg
var lastArbolRecibidoTs = (new Date()).getTime();
var joyInterval;
var textAreas;
var preguntarAlSalir = false;
var propuestaTemp;
var scalex = window.innerWidth / 1920;
var scaley = window.innerHeight / 955; //1080-bordes de pantalla
var docsTimeScale = 15;
var usuario;  ////creo que no se usa
var grupoParam;
var idiomaParam;
var emailParam;
var historico = false;
var historicoFecha;
var grupoPersonal;
var lastMouse = null;

//parametros para consenso
var vUsuarios, vActivos, vminSi, vmaxNo;

//tipo de visualizacion
var visual;  //Chrome, Zafari, InternetExplorer

//config general del sistema
var config;

//grupo
var grupo;

//arbol y usuario con sus flores
var arbolPersonal;

//modelos de documentos
var modelos;

//modelos de documentos de evaluacion
var modelosEvaluacion;

$(document).mousemove(function (event) {
    move(event);
    //event.preventDefault();
});

function move(event) {
    if (!preguntarAlSalir && event.target && (event.target.id == 'arbol' || event.target.id == 'svgAprendemos')) {
        if (event.which == 1 && !event.ctrlKey) {
            //scroll
            if (lastMouse) {
                if (estado == "aprendemos") {
                    quesox -= lastMouse.clientX - event.clientX;
                    quesoy -= lastMouse.clientY - event.clientY;
                    translateQueso(quesox, quesoy);
                }
                else if (estado == "decidimos") {
                    translatex -= lastMouse.clientX - event.clientX;
                    translatey -= lastMouse.clientY - event.clientY;
                    //document.body.style.cursor = "all-scroll";
                    translateArbol(translatex, translatey);
                }
            }
            lastMouse = { clientX: event.clientX, clientY: event.clientY };
            event.cancelBubble = true;
        }
        else if (event.which == 1 && event.ctrlKey) {
            //zoom
            if (lastMouse) {
                if (estado == "aprendemos") {
                    quesoScale += (lastMouse.clientX - event.clientX) * 2 / window.innerWidth;
                    dibujarQueso();
                }
                else if (estado == "decidimos") {
                    treeScale += (lastMouse.clientX - event.clientX) * 2 / window.innerWidth;
                    //document.body.style.cursor = "w-resize";
                    dibujarArbol(arbolPersonal.raiz);
                }
            }
            lastMouse = { clientX: event.clientX, clientY: event.clientY };
            event.cancelBubble = true;
        }
        else {
            lastMouse = null;
            //document.getElementById("todo").style.cursor = "default";
        }
        //$("div").text(event.which + ", " + event.ctrlKey);
    }
}

function doLoad() {
    //settings
    window.onbeforeunload = preguntar;
    //background
    setBackgroundImage();

    //eventos tactiles
    document.body.addEventListener('touchmove', function (e) {
        var touch = e.changedTouches[0] // reference first touch point for this event
        touch.which = 1;
        touch.ctrlKey = false;
        move(touch);
    }, false)

    document.body.addEventListener('touchend', function (e) {
        lastMouse = null;
    }, false)

    //busco arboles
    getHttp("doMain.aspx?actn=getConfig&width=" + window.innerWidth + "&height=" + window.innerHeight,
        function (data) {
            getConfig(data);

            if (visual.level == 0) {
                //navegador no soporta nabu
                document.getElementById("noSoportado").style.visibility = "visible";
                document.getElementById("noSoportadoMsg").innerHTML = "Nab&uacute; no puede mostrarse<br /> en esta versi&oacute;n de navegador";
            }
            else if (!visual.screen){
                document.getElementById("noSoportado").style.visibility = "visible";
                document.getElementById("noSoportadoMsg").innerHTML = "Nab&uacute; no puede mostrarse<br /> en esta resoluci&oacute;n de pantalla.<br> M&iacute;nimo 800x600.";
            }
            else
                //continuo con la carga
                doLoad2();
        });
}

function doLoad2() {
    try {
        //segunda parte del load
        //sinconizo animaciones con el navegador
        try {
            var requestAnimationFrame = window.requestAnimationFrame || window.mozRequestAnimationFrame || window.webkitRequestAnimationFrame || window.msRequestAnimationFrame;
            window.requestAnimationFrame = requestAnimationFrame;
            window.requestAnimationFrame(animate);
        }
        catch (ex) {
            //el navegador no soporta requestAnimationFrame
            //no puedo usar tweens
        }

        treeScale = scale; //valor default

        //idioma
        idiomaParam = getParameterByName('idioma');
        if (idiomaParam == null || idiomaParam == '') idiomaParam = 'es';

        //obtengo datos de los parametros
        grupoParam = getParameterByName('grupo');
        emailParam = getParameterByName('email');
        idioma = idiomaParam;  //para el dicionario

        //si hay cookie, login automatico si no login normal
        var cookie = "";
        if (grupoParam && grupoParam != "" && grupoParam != "null") {
            cookie = getCookie("nabu-" + grupoParam.replace(' ', ''));

            document.getElementById("pie").style.display = "none";

            if (cookie == "") {
                //login normal con el grupo parametro
                doResize();
                loginEffectIn();
            }
            else {
                //login automatico to server
                //obtengo datos de cookie
                var vals = cookie.split("|");
                var usuario = { nombre: vals[0], email: vals[1], clave: vals[2], grupo: vals[3], isAdmin: vals[4], idioma: vals[5] };

                idiomaParam = vals[5]; //para el tradcutor
                idioma = idiomaParam;  //para el dicionario

                doResize();

                if (emailParam && emailParam == usuario.email) {
                    //la cookie es de este usuario
                    //intento login Automatico
                    loginAutomatico(emailParam, usuario.clave, grupoParam);
                }
                else if (emailParam && emailParam != usuario.email) {
                    //no tengo la clave de este usuario
                    //login normal con el grupo parametro
                    loginEffectIn();
                }
                else {
                    //login con los datos de la cookie
                    loginAutomatico(usuario.email, usuario.clave, usuario.grupo);
                }
            }
        }
        else {
            //no hay parametro de grupo, enseño lista de grupos
            gruposEffectIn();
            doResize();
            //document.location = "bosques.html";
        }
    }
    catch (ex) {
        //envio al server
        sendException(ex, "doLoad2");
    }
}

function doKeypress() {
    if (estado == 'login' && event.keyCode == 13) {
        doLogin();
    }
}

function gruposEffectIn() {
    var s = "";
    s += "<div style='font-size:10vh;margin:top;'>";
    s += "<img src='res/logo2.png' class='logo' style='position:relative;left:3px;top:0px;padding:4px;'>";
    if (window.innerWidth <= 400)
        s += "<span class='titulo0' style='position:relative;top:-10px;left:2px;font-size:30px;'>Nab&uacute;</span>";
    else if (window.innerWidth <= 800)
        s += "<span class='titulo0' style='position:relative;top:-10px;left:2px;font-size:30px;'>Nab&uacute;</span>";
    else
        s += "<span class='titulo0' style='position:relative;top:-27px;left:2px;font-size:44px;'>Nab&uacute;</span>";

    s += "</div>"
    s += "<div class='titulo1' style='clear:left;'><b>" + tr("Grupos") + "</b></div>";
    for (i in config.grupos) {
        var grupo = config.grupos[i];
        s += " <a href='default.html?grupo=" + grupo;
        if (idioma)
            s += "&idioma=" + idioma;
        s += "' class='grupo'><nobr>" + grupo + "</nobr></a>"
    }
    s += "<br><br><input type='button' class='btn' value='" + tr("Crear nuevo grupo") + "' onclick=\"document.location='creargrupo.html?idioma=" + idioma + "'\">";

    document.getElementById("grupos").style.display = "block";
    document.getElementById("grupos").innerHTML = s;

    //pie
    document.getElementById("pie").style.display = "block";

    estado = 'grupos';
}

function loginAutomatico(email, clave, grupo) {
    getHttp("doMain.aspx?actn=login&email=" + email
        + "&clave=" + clave
        + "&grupo=" + grupo,
        function (data) {
            try {
                //atrapo el error si es que hay
                if (data.substring(0, 6) == "Error=") {
                    //ha habido un error
                    //login normal
                    doResize();
                    loginEffectIn();
                }
                else {
                    //login ok, he recibido el arbol
                    var loginData = JSON.parse(data);

                    //guardo el grupo
                    grupo = loginData.grupo;

                    //guardo el arbol
                    arbolPersonal = loginData.arbolPersonal;

                    //para el traductor
                    idioma = arbolPersonal.idioma;

                    //guardo los modelos
                    modelos = loginData.modelos;
                    modelosEvaluacion = loginData.modelosEvaluacion;

                    //guardo cookie
                    setCookie("nabu-" + arbolPersonal.nombre,
                        arbolPersonal.usuario.nombre + "|" + arbolPersonal.usuario.email + "|" + arbolPersonal.usuario.clave + "|" + arbolPersonal.nombre + "|" + arbolPersonal.usuario.isAdmin + "|" + grupo.idioma,
                        7);

                    //activo menuppal
                    doMenuppal();

                    doResize();
                }
            }
            catch (ex) {
                //envio al server
                sendException(ex, "doLoad2.2");
            }
        });
}

function preguntar() {
    if (preguntarAlSalir)
        return tr("Se perderan los datos no guardados");
}

function doTimeBack() {
    historico = true;
    historicoFecha = new Date();
    historicoFecha.setDate(historicoFecha.getDate() - 1);
    showTimePanel();
    document.body.style.backgroundImage = "url('res/night.jpg')";
    document.body.style.backgroundSize = 'cover';
    menuOptions();
}

function doTimePresent() {
    historico = false;
    showTimePanel();
    document.body.style.backgroundImage = "url('res/background.jpg')";
    document.body.style.backgroundSize = 'cover';
    menuOptions();    
}

function showTimePanel() {
    //no ready to use
    document.getElementById("timeBack").style.visibility = "hidden";

    ////timePanel
    //if (historico) {
    //    var now = new Date();
    //    now.setDate(now.getDate() - 1);
    //    if (historicoFecha > now) 
    //        historicoFecha = now;
    //    document.getElementById("timePanel").style.visibility = "visible";
    //    document.getElementById("timeDia").innerHTML = historicoFecha.getDate() < 10 ? '0' + historicoFecha.getDate() : historicoFecha.getDate();
    //    document.getElementById("timeMes").innerHTML = historicoFecha.getMonth() + 1 < 10 ? '0' + (historicoFecha.getMonth() + 1) : historicoFecha.getMonth() + 1;
    //    document.getElementById("timeYear").innerHTML = historicoFecha.getFullYear();
    //    document.getElementById("timeBack").style.visibility = "hidden";
    //}
    //else {
    //    document.getElementById("timeBack").style.visibility = "visible";
    //    document.getElementById("timePanel").style.visibility = "hidden";
    //}
}

function doResize() {
    scalex = window.innerWidth / 1920;
    scaley = window.innerHeight / 955; //1080-bordes de pantalla

    //pie
    document.getElementById("lnkSim").href = 'simulacion.html?grupo=' + grupoParam + '&idioma=' + idiomaParam;

    //frase del dia
    //busco una no vista
    var cant = 0;
    var index = Math.round(Math.random() * (frasesDelDia - 1));
    while (frasesVistas.indexOf(index) >= 0 && cant < frasesDelDia) {
        cant++;
        index = Math.round(Math.random() * (frasesDelDia - 1));
    }
    if (cant >= frasesDelDia)
        frasesVistas = [];
    document.getElementById("tipContent").innerHTML = tr("Frase" + index);
    frasesVistas.push(index);

    //background
    setBackgroundImage();

    //timePanel
    showTimePanel();

    //resize del menuppal segun pantalla
    var scale, menuscale;
    if (window.innerWidth <= 400) {
        //movil vertical
        scale = scalex;
        menuscale = scalex * 2.1;
        //if (menuscale > 2) menuscale = 2;
    }
    else if (window.innerWidth <= 800) {
        //movil horizontal
        scale = scalex;
        menuscale = scalex * 1.4;
        if (menuscale > 0.8) menuscale = 0.8;
    }
    else {
        //ordenador
        scale = scalex;
        menuscale = scalex * 1.4;
        if (menuscale > 0.8) menuscale = 0.8;
    }

    document.getElementById("padrenombre").style.width = 800 * menuscale + 'px';
    document.getElementById("padrenombre").style.top = -15 - 5 * menuscale + 'px';

    document.getElementById("hijos").style.width = 1600 * menuscale + 'px';
    document.getElementById("hijos").style.left = -400 * menuscale + 'px';
    document.getElementById("hijos").style.top = (590 * menuscale) + "px";

    document.getElementById("tituloppal").style.top = 278 * menuscale + 'px';
    document.getElementById("tituloppal").style.left = 250 * menuscale + 'px';
    document.getElementById("tituloppal").style.width = 285 * menuscale + 'px';
    if (arbolPersonal && arbolPersonal.nombre.length < 10)
        document.getElementById("tituloppal").style.fontSize = 30 * menuscale + 'px';
    else
        document.getElementById("tituloppal").style.fontSize = 16 * menuscale + 'px';

    var top = (window.innerHeight / 2 - 600 * menuscale / 2 - 10 * menuscale).toFixed(0);
    if (top < 0) top = 5;
    document.getElementById("menuppal").style.top = top + 'px';
    document.getElementById("menuppal").style.left = (window.innerWidth / 2 - 800 * menuscale / 2).toFixed(0) + 'px';
    document.getElementById("menuppal").style.width = 800 * menuscale + 'px';
    document.getElementById("menuppal").style.height = 600 * menuscale + 'px';

    document.getElementById("ciclo").style.width = 508 * menuscale + 'px';
    document.getElementById("ciclo").style.height = 526 * menuscale + 'px';
    document.getElementById("ciclo").style.left = 159 * menuscale + 'px';
    document.getElementById("ciclo").style.top = 47 * menuscale + 'px';

    document.getElementById("ppal1").src = "res/" + idioma + "/debates.png";
    document.getElementById("ppal1").onmouseover = "this.src='res/" + idioma + "/debates2.png';";
    document.getElementById("ppal1").onmouseout = "this.src='res/" + idioma + "/debates.png';";
    document.getElementById("ppal1").title = tr("Arbol de decisiones");
    document.getElementById("ppal1").style.width = 269 * menuscale + 'px';
    document.getElementById("ppal1").style.height = 200 * menuscale + 'px';
    document.getElementById("ppal1").style.left = 35 * menuscale + 'px';
    document.getElementById("ppal1").style.top = 105 * menuscale + 'px';

    document.getElementById("ppal2").src = "res/" + idioma + "/noticias.png";
    document.getElementById("ppal2").onmouseover = "this.src='res/" + idioma + "/noticias2.png';";
    document.getElementById("ppal2").onmouseout = "this.src='res/" + idioma + "/noticias.png';";
    document.getElementById("ppal2").title = tr("Publicacion de argumentos");
    document.getElementById("ppal2").style.width = 269 * menuscale + 'px';
    document.getElementById("ppal2").style.height = 199 * menuscale + 'px';
    document.getElementById("ppal2").style.left = 35 * menuscale + 'px';
    document.getElementById("ppal2").style.top = 282 * menuscale + 'px';

    document.getElementById("ppal3").src = "res/" + idioma + "/estructuras.png";
    document.getElementById("ppal3").title = tr("Estructuras");
    document.getElementById("ppal3").style.width = 240 * menuscale + 'px';
    document.getElementById("ppal3").style.height = 90 * menuscale + 'px';
    document.getElementById("ppal3").style.left = 530 * menuscale + 'px';
    document.getElementById("ppal3").style.top = 200 * menuscale + 'px';

    document.getElementById("ppal4").src = "res/" + idioma + "/seguimiento.png";
    document.getElementById("ppal4").title = tr("Seguimiento");  
    document.getElementById("ppal4").style.width = 240 * menuscale + 'px';
    document.getElementById("ppal4").style.height = 84 * menuscale + 'px';
    document.getElementById("ppal4").style.left = 530 * menuscale + 'px';
    document.getElementById("ppal4").style.top = 287 * menuscale + 'px';

    document.getElementById("ppal5").src = "res/" + idioma + "/docConsensos.png";
    document.getElementById("ppal5").onmouseover = "this.src='res/" + idioma + "/docConsensos2.png';";
    document.getElementById("ppal5").onmouseout = "this.src='res/" + idioma + "/docConsensos.png';";
    document.getElementById("ppal5").title = tr("Documentos de decision");
    document.getElementById("ppal5").style.width = 124 * menuscale + 'px';
    document.getElementById("ppal5").style.height = 143 * menuscale + 'px';
    document.getElementById("ppal5").style.left = 338 * menuscale + 'px';
    document.getElementById("ppal5").style.top = 31 * menuscale + 'px';
    

    document.getElementById("ppal7").src = "res/" + idioma + "/docRealizacion.png";
    document.getElementById("ppal7").onmouseover = "this.src='res/" + idioma + "/docRealizacion2.png';";
    document.getElementById("ppal7").onmouseout = "this.src='res/" + idioma + "/docRealizacion.png';";
    document.getElementById("ppal7").title = tr("Documentos de resultado");
    document.getElementById("ppal7").style.width = 221 * menuscale + 'px';
    document.getElementById("ppal7").style.height = 145 * menuscale + 'px';
    document.getElementById("ppal7").style.left = 285 * menuscale + 'px';
    document.getElementById("ppal7").style.top = 439 * menuscale + 'px';

    document.getElementById("ppal9").style.width = 64 * 1.1 * menuscale + 'px';
    document.getElementById("ppal9").style.height = 79 * 1.1 * menuscale + 'px';
    document.getElementById("ppal9").style.left = 130 * menuscale + 'px';
    document.getElementById("ppal9").style.top = 248 * menuscale + 'px';

    document.getElementById("ppal10").src = "res/" + idioma + "/politico.png";
    document.getElementById("ppal10").style.width = 35 * menuscale + 'px';
    document.getElementById("ppal10").style.height = 230 * menuscale + 'px';
    document.getElementById("ppal10").style.left = 0 * menuscale + 'px';
    document.getElementById("ppal10").style.top = 170 * menuscale + 'px';

    document.getElementById("ppal11").src = "res/" + idioma + "/operativo.png";
    document.getElementById("ppal11").style.width = 35 * menuscale + 'px';
    document.getElementById("ppal11").style.height = 230 * menuscale + 'px';
    document.getElementById("ppal11").style.left = 775 * menuscale + 'px';
    document.getElementById("ppal11").style.top = 150 * menuscale + 'px';

    //traducir
    document.getElementById("loginAltaUsuario").innerHTML = tr("Alta de usuario");
    document.getElementById("btnEntrar").value = tr("Entrar");
    document.getElementById("email").placeholder = tr("email");
    document.getElementById("clave").placeholder = tr("clave");
    //document.getElementById("tit1").innerHTML = tr("Sociocracia");
    document.getElementById("lnkSim").innerHTML = tr("simulacion");
    document.getElementById("mejorVisto").innerHTML = tr("Mejor visto en");
    document.getElementById("version").innerHTML = tr("version beta");
    document.getElementById("lnkAyuda").innerHTML = tr("Ayuda");
    document.getElementById("titulo1").innerHTML = tr("Sociocracia");
    //document.getElementById("titulo2").innerHTML = tr("Democracia interactiva");
    document.getElementById("oldPass").placeholder = tr("Clave actual");
    document.getElementById("newPass").placeholder = tr("Nueva clave");
    document.getElementById("repeat").placeholder = tr("Repitela");
    document.getElementById("btnCCCambiar").value = tr("Cambiar");
    document.getElementById("btnCCCancelar").value = tr("Cancelar");
    document.getElementById("auTit1C").innerHTML = tr("Alta de usuario");
    document.getElementById("auTit2C").innerHTML = tr("auTit2");
    document.getElementById("altaUsuarioNombreC").placeholder = tr("Nombre completo");
    document.getElementById("altaUsuarioEmailC").placeholder = tr("Email");
    document.getElementById("auEnviarC").value = tr("Enviar solicitud");
    document.getElementById("auCerrarC").innerHTML = tr("Cerrar");
    document.getElementById("ufTit1").innerHTML = tr("ufTit1");
    document.getElementById("ufAceptar").value = tr("Aceptar");
    document.getElementById("ufCancelar").value = tr("Cancelar");

    //arbol
    translateArbol(translatex = 0, translatey = 0);
    if (arbolPersonal) dibujarArbol(arbolPersonal.raiz);

}

function sendException(ex, flag) {
    if (arbolPersonal)
        getHttp("doMain.aspx?actn=exception&flag=" + flag + "&message=" + ex.message + "&stack=" + ex.stack + "&email=" + arbolPersonal.usuario.email + "&grupo=" + arbolPersonal.nombre, null);
    else
        getHttp("doMain.aspx?actn=exception&flag=" + flag + "&message=" + ex.message + "&stack=" + ex.stack, null);
}

function doManifiesto() {
    if (arbolPersonal.URLEstatuto != "")
        window.open(arbolPersonal.URLEstatuto);
}

function loginEffectIn(){
    //login effect
    try {

        document.getElementById("tip").style.visibility = "hidden";
        document.getElementById("loginIn").style.visibility = "hidden";
        document.getElementById("loginFlor").style.visibility = "hidden";
        document.getElementById("timeBack").style.visibility = "hidden";

        var florTop = window.innerHeight / 6;
        var loginTop = window.innerHeight / 6;
        var florXfin = window.innerWidth / 2 - 170;
        var loginXfin = window.innerWidth / 2;
        var tipYfin = window.innerHeight / 4 + 200;
        if (window.innerWidth <= 400) {
            //movil vertical
            florTop = 260;
            loginTop = 80;
            florXfin = window.innerWidth / 2 - 65;
            loginXfin = window.innerWidth / 2 - 140;
            tipYfin = 350;
        }
        else if (window.innerWidth <= 800) {
            //movil horizonta
        }
        document.getElementById("loginIn").style.top = loginTop + 'px';
        document.getElementById("loginFlor").style.top = florTop + 'px';
        efectoLeft(document.getElementById("loginIn"), 0, 750, window.innerWidth, loginXfin, TWEEN.Easing.Cubic.Out);
        efectoLeft(document.getElementById("loginFlor"), 0, 750, -200, florXfin, TWEEN.Easing.Exponential.Out);

        efectoTop(document.getElementById("tip"), 800, window.innerHeight, tipYfin, TWEEN.Easing.Elastic.Out);
        timerFlores = setInterval(function () {
            rotFlores += 0.3;
            document.getElementById("loginFlor").style.transform = "rotate(" + rotFlores + "deg)";
        }, 100);

        //atras
        document.getElementById("atras").style.visibility = "visible";

        //nombre de grupo
        document.getElementById("loginGrupo").style.left = (window.innerWidth / 2 - 250) + 'px';
        document.getElementById("loginGrupo").style.top = '3vh';
        document.getElementById("loginGrupo").style.visibility = "visible";
        document.getElementById("loginGrupo").innerHTML = grupoParam;

        estado = 'login';
    }
    catch (ex) {
        //envio al server
        sendException(ex, "loginEffectIn");
    }
}

function loginEffectOut() {
    //login effect
    efectoLeft(document.getElementById("loginIn"), 0, 750, window.innerWidth / 2 - 50, window.innerWidth, TWEEN.Easing.Cubic.Out);
    efectoLeft(document.getElementById("loginFlor"), 0, 750, window.innerWidth / 2 - 280, -250, TWEEN.Easing.Linear.None);
    efectoTop(document.getElementById("tip"), 0, window.innerHeight / 2 + 130, window.innerHeight, TWEEN.Easing.Exponential.Out);
    clearInterval(timerFlores);
    timerFlores = setInterval(function () {
        rotFlores -= 8;
        document.getElementById("loginFlor").style.transform = "rotate(" + rotFlores + "deg)";
    }, 50);
    setTimeout(function () {
        //fin efecto, limpio cosas
        clearInterval(timerFlores);
        document.getElementById("loginIn").style.visibility = "hidden";
        document.getElementById("loginFlor").style.visibility = "hidden";
        document.getElementById("tip").style.visibility = "hidden";
    }, 1000);
    document.getElementById("loginGrupo").style.visibility = "hidden";
}

function loginEffectNo() {
    //login effect
    var l = document.getElementById("loginIn");
    var x = window.innerWidth / 2;
    efectoLeft(l, 0, 100, x - 50, x - 10, TWEEN.Easing.Cubic.InOut);
    efectoLeft(l, 100, 200, x - 10, x - 90, TWEEN.Easing.Cubic.InOut);
    efectoLeft(l, 300, 100, x - 90, x - 50, TWEEN.Easing.Cubic.InOut);
}

function animate(time) {
    requestAnimationFrame(animate);
    TWEEN.update(time);
}

function getConfig(data) {
    config = JSON.parse(data);
    visual = getVisualizacion(config);
}

function doLogin() {
    email = document.getElementById("email").value;
    var clave = document.getElementById("clave").value;
    var loginResponse = document.getElementById("loginResponse");
    loginResponse.innerHTML = '';

    email = email.trim();
    clave = clave.trim();

    //login to server
    getHttp("doMain.aspx?actn=login&email=" + email
        + "&clave=" + clave
        + "&grupo=" + grupoParam,
        function (data) {
            if (data.substring(0, 6) == "Error=") {
                //ha habido un error
                var loginResponse = document.getElementById("loginResponse");
                loginResponse.innerHTML = '<font color=red>' + data.substring(6) + '</font>';
                loginEffectNo();
            }
            else {
                //login effect
                //login ok, he recibido el arbol
                var loginData = JSON.parse(data);

                //guardo el grupo
                grupo = loginData.grupo;

                //guardo el arbol
                arbolPersonal = loginData.arbolPersonal;

                //guardo los modelos
                modelos = loginData.modelos;

                //guardo cookie
                setCookie("nabu-" + grupo.nombre
                    , arbolPersonal.usuario.nombre + "|" + arbolPersonal.usuario.email + "|" + arbolPersonal.usuario.clave + "|" + grupo.nombre + "|" + arbolPersonal.usuario.isAdmin + "|" + grupo.idioma, 7);

                //efecto login out
                loginEffectOut();

                //activo menuppal
                doMenuppal();
            }
        });
}

function showAlertasIcon() {
    var icon = document.getElementById("alertaIcon");
    icon.style.display = grupoPersonal.alertas.length > 0 ? "block" : "none";
}

function showAlertas() {
    if (grupoPersonal.alertas.length > 0) {
        var s = "<div class='titulo3'>";
        s += "<span style='float:left'><b>" + tr("Alertas") + "</b></span>";
        s += "<img src='res/rojo.gif' style='cursor:pointer;float:right' onclick='doBorrarAlertas();'>";
        s += "</div>";

        s += "<div style='clear:left;float:left;overflow:auto;max-height:60%'>";
        for (var i in grupoPersonal.alertas) {
            var a = grupoPersonal.alertas[i];
            s += "<span class='titulo4' style='color:#bbbbbb'>" + formatDate(jsonToDate(a.ts)) + "</span><br>";
            s += "<span class='titulo4'>" + a.msg + "</span><br><br>";
        }
        s += "</div>";
        var div = document.getElementById("alertas");
        div.innerHTML = s;
        div.style.display = "block";
    }
}

function doBorrarAlertas() {
    var div = document.getElementById("alertas");
    //login to server
    getHttp("doMain.aspx?actn=borrarAlertas&email=" + arbolPersonal.usuario.email
        + "&clave=" + arbolPersonal.usuario.clave
        + "&grupo=" + arbolPersonal.nombre,
        function (data) {}); //no proceso respuesta
    div.style.display = "none";
    var icon = document.getElementById("alertaIcon");
    icon.style.display = "none";
}

function doMenuppal() {
    //paso a menuppal       

    //menu ppal
    setTimeout(function () {
        //link al padre
        var padreURL = arbolPersonal.padreURL + "/default.html?grupo=" + arbolPersonal.padreNombre + "&email=" + arbolPersonal.usuario.email + "&idioma=" + idiomaParam;
        document.getElementById("padrenombre").innerHTML = "<a href='" + padreURL + "'>" + arbolPersonal.padreNombre + "</a>";

        //nombre del arbol
        document.getElementById("tituloppal").innerHTML = arbolPersonal.nombre;
        document.getElementById("tituloNabu").style.visibility = "visible";

        //timeback
        showTimePanel();

        //hijos
        var hijos = "<nobr>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
        for (q in arbolPersonal.hijos) {
            var thijo = arbolPersonal.hijos[q];
            var hijoURL = thijo.URL + "/default.html?grupo=" + thijo.nombre + "&email=" + arbolPersonal.usuario.email;
            hijos += "<td><a href='" + hijoURL + "'>" + thijo.nombre + "</a></td>";
            hijos += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"
        }
        hijos += "</nobr>";
        document.getElementById("hijos").innerHTML = hijos;

        //atras
        document.getElementById("atras").style.visibility = "visible";
        document.getElementById("pie").style.display = "block";

        //panel de usuario
        document.getElementById("usuario").innerHTML = "<div id='usuarioNombre' style='color:blue;cursor: pointer;' onclick='showPerfil();'>" + arbolPersonal.usuario.nombre + "</div>";
        document.getElementById("floresDisponibles").innerHTML = getFloresDisponibles().length;

        //panel consenso
        document.getElementById("panelConsenso").style.display = 'none';

        //panel grupo
        actualizarDatosGrupo();

        //alertas
        timerGrupo = setInterval(getGrupoPersonal, refreshInterval);
        clearInterval(timerArbol);
        clearInterval(timerQueso);

        //panel usuario        
        document.getElementById("panelUsuario").style.display = 'block';

        menuOptions();

        //pongo icono ce manifiesto
        if (arbolPersonal && arbolPersonal.URLEstatuto != "")
            document.getElementById("ppal9").src = "res/documentos/manifiesto.png";
        else
            document.getElementById("ppal9").src = "res/noManifiesto.png";

        //activo la rueda
        if (arbolPersonal && arbolPersonal.URLEstatuto != "" && timerCiclo == null)
            timerCiclo = setInterval(function () {
                document.getElementById("ciclo").style.transform = 'rotate(' + rotacionCiclo++ + 'deg)';
            }, 100);
        else
            timerCiclo == null;

        //menuppal
        //var menuscale = scale * 1.1;
        //document.getElementById("menuppal").style.top = (window.innerHeight / 2 - 300 * menuscale).toFixed(0) + 'px';
        //document.getElementById("menuppal").style.left = (window.innerWidth / 2 - 400 * menuscale).toFixed(0) + 'px';
        if (visual.level == 1) {
            document.getElementById("menuppal").style.visibility = 'visible';
        }
        else {
            efectoOpacity(document.getElementById("menuppal"), 0, 0, 1, TWEEN.Easing.Cubic.Out);
        }
    }, 1000); //doy tiempo a que salga el logInEffectOut()

    
    estado = 'menuppal';
}

function getGrupoPersonal() {
    if (arbolPersonal)
        getHttp("doMain.aspx?actn=getGrupoPersonal&grupo=" + arbolPersonal.nombre
            + "&email=" + arbolPersonal.usuario.email
            + "&clave=" + arbolPersonal.usuario.clave,
            function (data) {
                //atrapo el error si es que hay
                if (data.substring(0, 6) == "Error=") {
                }
                else {
                    grupoPersonal = JSON.parse(data);
                    showAlertasIcon();
                }
            }); 
}

function menuOptions() {
    //opciiones de menu
    var mnu = "";
    mnu += "<div class='menuItem'><a class='menuItem' href='comunidadGrupos.html?grupo=" + grupoParam + "&idioma=" + idiomaParam + "'>" + tr("El bosque") + "</a></div>";
    if (arbolPersonal.usuario.isAdmin) {
        //adminOptions
        mnu += "<div class='menuItem'><a class='menuItem' href='modificargrupo.html?grupo=" + grupoParam + "&idioma=" + idiomaParam + "'>" + tr("El arbol") + "</a></div>";
        mnu += "<div class='menuItem'><a class='menuItem' href='usuarios.html?grupo=" + grupoParam + "&idioma=" + idiomaParam + "'>" + tr("Usuarios") + "</a></div>";
    }
    else {
        //user options
        mnu += "<div class='menuItem'><a class='menuItem' href='verusuarios.html?grupo=" + grupoParam + "&idioma=" + idiomaParam + "'>" + tr("Usuarios") + "</a></div>";
    }
    if (arbolPersonal.usuario.isFacilitador || arbolPersonal.usuario.isAdmin) {
        mnu += "<div class='menuItem'><a class='menuItem' href='mailer.html?grupo=" + grupoParam + "&idioma=" + idiomaParam + "'>" + tr("Mailer") + "</a></div>";
    }  

    //movile page footer
    mnu += "<div class='menuItem'><a class='menuItem movilePie' href='javascript:' onclick='showMovileFooter();'>?</a></div>";
    if (historico) mnu = "";
    document.getElementById("mnuOptions").innerHTML = mnu;
}

function showMovileFooter() {
    document.getElementById("movilePie").style.display = "block";
}

function hideMovileFooter() {
    document.getElementById("movilePie").style.display = "none";
}

function doCloseHelp() {
    document.getElementById("help").style.visibility = "hidden";
}

function showPerfil() {
    document.getElementById("perfilEmail").value = arbolPersonal.usuario.email;
    document.getElementById("perfilNombre").value = arbolPersonal.usuario.nombre;
    document.getElementById("perfilFuncion").value = arbolPersonal.usuario.funcion;
    document.getElementById("perfilValidacion").value = "";
    document.getElementById("googleLocation").value = "";

    document.getElementById("mision").value = arbolPersonal.usuario.mision;
    document.getElementById("capacidades").value = arbolPersonal.usuario.capacidades;
    document.getElementById("expectativas").value = arbolPersonal.usuario.expectativas;
    document.getElementById("participacion").value = arbolPersonal.usuario.participacion;
    document.getElementById("address").value = arbolPersonal.usuario.address;

    document.getElementById("perfil").style.display = 'block';    
    document.getElementById("cambiarClaveBtn").value = tr("Cambiar clave");
    var img = document.getElementById("perfilImg");
    var d = new Date();
    img.src = "grupos/" + grupoParam + "/usuarios/" + arbolPersonal.usuario.email + "/" + arbolPersonal.usuario.email + ".png?now=" + d.getTime();
}

function showCambiarClave() {
    document.getElementById("oldPass").value = "";
    document.getElementById("newPass").value = "";
    document.getElementById("repeat").value = "";
    document.getElementById("cambiarClaveMsg").innerHTML = "";
    document.getElementById("cambiarClave").style.display = 'block';
}

function doAltaUsuario() {
    var list = config.grupos;
    var grupo = getParameterByName('grupo') + "";

    getHttp("doMain.aspx?actn=gettipogrupo&grupo=" + grupo,
    function (data) {
        //atrapo el error si es que hay
        if (data.substring(0, 6) == "Error=") {
        }
        else {
            var tipoGrupo = data.split(';')[0];
            var URLEstatuto = data.split(';')[1];
            if (tipoGrupo == 'cerrado') {
                document.getElementById("altaUsuarioMsgC").innerHTML = "";
                document.getElementById("altaUsuarioC").style.display = 'block';
            }
            else {
                if (URLEstatuto != "")
                    document.getElementById("altaUsuarioURLEstatuto").innerHTML = "<a href='" + URLEstatuto + "' target='_blank'>" + tr("Ver manifiesto") + "</a>";
                else
                    document.getElementById("altaUsuarioURLEstatuto").innerHTML = "";

                document.getElementById("altaUsuarioLeido").innerHTML = tr("altaUsuarioLeido");
                document.getElementById("altaUsuarioMsgA").innerHTML = "";
                document.getElementById("altaUsuarioA").style.display = 'block';
            }
        }
    });
}

function doAltaUsuarioCrear() {
    var grupo = getParameterByName('grupo') + "";
    var nombre = document.getElementById("altaUsuarioNombreA");
    var email = document.getElementById("altaUsuarioEmailA");
    var msg = document.getElementById("altaUsuarioMsgA");
    var clave1 = document.getElementById("altaUsuarioClave1A");
    var clave2 = document.getElementById("altaUsuarioClave2A");
    var altaUsuarioCheck = document.getElementById("altaUsuarioCheck");

    if (nombre.value == "")
        msg.innerHTML = "<font color=red>" + tr("Nombre no puede ser vacio") + "</font>";
    else if (email.value == "")
        msg.innerHTML = "<font color=red>" + tr("Email no puede ser vacio") + "</font>";
    else if (clave1.value != clave2.value)
        msg.innerHTML = "<font color=red>" + tr("Las claves no coinciden") + "</font>";
    else if (!altaUsuarioCheck.checked)
        msg.innerHTML = "<font color=red>" + tr("Debes aceptar el manifiesto actual para ingresar al grupo") + "</font>";
    else {
        getHttp("doMain.aspx?actn=crearusuarioabierto&grupo=" + grupo
            + "&nombre=" + nombre.value
            + "&clave=" + clave1.value
            + "&email=" + email.value,
            function (data) {
                //atrapo el error si es que hay
                if (data.substring(0, 6) == "Error=") {
                    //ha habido un error
                    msg.innerHTML = "<font color=red>" + data + "</font>";
                }
                else {
                    loginEffectOut();
                    efectoTop(document.getElementById('altaUsuarioA'), 0, 80 * scale, -380, TWEEN.Easing.Cubic.Out);
                    loginAutomatico(email.value, clave1.value, grupo);
                }
            });
    }
}

function previewFile(perfilFile) {
    var perfilImg = document.getElementById('perfilImg'); //selects the query named img
    var reader = new FileReader();

    reader.onloadend = function () {
        uploadToServer(reader.result, perfilFile.type);
    }

    if (perfilFile) {
        reader.readAsDataURL(perfilFile); //reads the data as a URL
    } else {
        preview.src = "";
    }
}

function uploadToServer(base64) {
    //upload to server
    var grupo = getParameterByName('grupo') + "";
    postHttp("doMain.aspx?actn=upload&grupo=" + grupo
        + "&email=" + arbolPersonal.usuario.email,
        "base64=" + URIEncode(base64),
        function (data) {
            //muestro
            var d = new Date();
            var img = document.getElementById("perfilImg");
            img.src = "grupos/" + grupoParam + "/usuarios/" + arbolPersonal.usuario.email + "/" + arbolPersonal.usuario.email + ".png?now=" + d.getTime();
        });
}

function doAltaUsuarioEnviar() {
    var nombre = document.getElementById("altaUsuarioNombreC");
    var email = document.getElementById("altaUsuarioEmailC");
    var msg = document.getElementById("altaUsuarioMsgC");

    if (nombre == "")
        msg.innerHTML = "<font color=green>" + tr("Nombre no puede ser vacio") + "</font>";
    else if (email == "")
        msg.innerHTML = "<font color=green>" + tr("Email no puede ser vacio") + "</font>";
    else {
        getHttp("doMain.aspx?actn=sendMailAlta&grupo=" + grupoParam
            + "&nombre=" + nombre.value
            + "&usuarioemail=" + email.value,
            function (data) {
                //atrapo el error si es que hay
                if (data.substring(0, 6) == "Error=") {
                    //ha habido un error
                    msg.innerHTML = "<font color=red>" + data + "</font>";
                }
                else {
                    nombre.value = "";
                    email.value = "";
                    msg.innerHTML = "<font color=green>" + tr("Mensaje enviado al coordinador") + "</font>";
                }
            });
    }
}

function doCambiarClave() {
    var oldPass = document.getElementById("oldPass").value;
    var newPass = document.getElementById("newPass").value;
    var repeat = document.getElementById("repeat").value;

    if (newPass != repeat)
        document.getElementById("cambiarClaveMsg").innerHTML = "<font color='red'>" + tr("Las nuevas claves no coinciden") + "</font>";
    else {
        getHttp("doMain.aspx?actn=cambiarClave&grupo=" + arbolPersonal.nombre
            + "&email=" + arbolPersonal.usuario.email
            + "&claveActual=" + oldPass
            + "&nuevaClave=" + newPass,
            function (data) {
                if (data != '')
                    document.getElementById("cambiarClaveMsg").innerHTML = "<font color='red'>" + data.substring(6) + "</font>";
                else 
                    document.getElementById("cambiarClave").style.display = 'none';
            });
    }
}

function geocodeAddress() {
    var geocoder = new google.maps.Geocoder();
    var address = document.getElementById("address").value;
    geocoder.geocode({ 'address': address }, function (results, status) {
        if (status === 'OK') {
            document.getElementById("googleLocation").innerHTML = results[0].formatted_address;
            arbolPersonal.usuario.lat = results[0].geometry.location.lat();
            arbolPersonal.usuario.lng = results[0].geometry.location.lng();
        }
        else
            document.getElementById("googleLocation").innerHTML = tr("Ubicacion no encontrada");
    });
}

function doCerrarPerfil() {
    if (arbolPersonal.usuario.lat == 0 || arbolPersonal.usuario.lng == 0)
        document.getElementById("perfilValidacion").innerHTML = tr("Ubicacion no validada");
    else {
        var perfilNombre = document.getElementById("perfilNombre").value;
        var perfilFuncion = document.getElementById("perfilFuncion").value;

        var mision = document.getElementById("mision").value;
        var capacidades = document.getElementById("capacidades").value;
        var expectativas = document.getElementById("expectativas").value;
        var participacion = document.getElementById("participacion").value;
        var address = document.getElementById("address").value;

        var post = "&nombre=" + perfilNombre
            + "&funcion=" + perfilFuncion
            + "&mision=" + URIEncode(mision)
            + "&capacidades=" + URIEncode(capacidades)
            + "&expectativas=" + URIEncode(expectativas)
            + "&participacion=" + URIEncode(participacion)
            + "&address=" + URIEncode(address)
            + "&lat=" + arbolPersonal.usuario.lat
            + "&lng=" + arbolPersonal.usuario.lng;
        postHttp("doMain.aspx?actn=actualizarperfilusuario&grupo=" + arbolPersonal.nombre
            + "&email=" + arbolPersonal.usuario.email
            + "&clave=" + arbolPersonal.usuario.clave,
            post,
            function (data) {
                if (data.substring(0, 6) == "Error=")
                    document.getElementById("perfilMsg").innerHTML = "<font color='red'>" + data.substring(6) + "</font>";
                else {
                    document.getElementById("perfil").style.display = 'none';

                    //local update
                    arbolPersonal.usuario.nombre = perfilNombre;
                    arbolPersonal.usuario.funcion = perfilFuncion;
                    arbolPersonal.usuario.mision = mision;
                    arbolPersonal.usuario.capacidades = capacidades;
                    arbolPersonal.usuario.expectativas = expectativas;
                    arbolPersonal.usuario.participacion = participacion;
                    arbolPersonal.usuario.address = address;
                    document.getElementById("usuarioNombre").innerHTML = perfilNombre;
                }
            });
    }
}

function doHideCambiarClave() {
    document.getElementById("cambiarClave").style.display = 'none';
}

function doAtras() {
    document.getElementById("cambiarClave").style.display = 'none';

    if (estado == 'login') {
        //login voy a grupos
        loginEffectOut();
        setTimeout(gruposEffectIn, 600); //doy tiempo al login a irse
        doResize();
        document.getElementById("atras").style.visibility = "hidden";
        document.getElementById("timeBack").style.visibility = "hidden";
    }
    else if (estado == 'aprendemos') {
        //aprendemos voy a menuppal
        document.getElementById("panelQueso").style.display = 'none';

        document.getElementById("quesoDiv").style.display = "none";

        document.getElementById("titulo").style.visibility = 'hidden';
        document.getElementById("panelUsuario").style.display = 'block';
        document.getElementById("menuEvaluacion").style.visibility = 'hidden';
        document.getElementById("modelosDebate").style.display = "none";
        document.getElementById("modelosEvaluacion").style.display = "none";
        document.getElementById("joystickQueso").style.visibility = "hidden";
        document.getElementById("pie").style.display = "block";

        quesoPersonal = null;
        estado = 'menuppal';
        actualizarDatosGrupo();
        clearInterval(timerQueso);

        //activo el menuppal
        if (visual.level == 1) {
            document.getElementById("menuppal").style.visibility = "visible";
        }
        else {
            //menu ppal
            efectoOpacity(document.getElementById("menuppal"), 0, 0, 1, TWEEN.Easing.Cubic.Out);
        }
    }
    else if (estado == 'menuppal') {
        //menu ppal voy a login
        efectoOpacity(document.getElementById("menuppal"), 0, 1, 0, TWEEN.Easing.Cubic.Out, function () { document.getElementById("menuppal").style.visibility = "hidden"; });
        document.getElementById("panelGrupo").style.display = 'none';

        arbolPersonal = null;
        propuestas = [];
        setCookie("nabu-" + grupoParam, "", 1);

        document.getElementById("tituloNabu").style.visibility = "hidden";
        document.getElementById("email").value = '';
        document.getElementById("clave").value = '';
        setTimeout(loginEffectIn, 600); //doy tiempo al menu a irse

        document.getElementById("mnuOptions").innerHTML = "";
        document.getElementById("panelUsuario").style.display = 'none';

        document.getElementById("alertas").style.display = 'none';
        document.getElementById("pie").style.display = 'none';

        document.getElementById("atras").style.visibility = "hidden";
        document.getElementById("timeBack").style.visibility = "hidden";
        root = { "name": "?" };
        clearInterval(timerFlores);

        estado = 'login';
    }
    else if (estado == 'decidimos') {
        //decidimos voy a menuppal
        var objetivo = document.getElementById("objetivo");
        objetivo.innerHTML = arbolPersonal.objetivo;
        objetivo.style.visibility = 'hidden';

        document.getElementById("panelUsuario").style.display = 'block';

        //efecto de salida: podo el arbol
        var children = arbolPersonal.raiz.children;
        var logDecisiones = arbolPersonal.logDecisiones;
        arbolPersonal.raiz.children = [];
        arbolPersonal.logDecisiones = [];
        dibujarArbol(arbolPersonal.raiz);
        arbolPersonal.raiz.children = children;
        arbolPersonal.logDecisiones = logDecisiones;

        document.getElementById("titulo").style.visibility = 'hidden';
        document.getElementById("joystickArbol").style.visibility = 'hidden';
        document.getElementById("modelosDebate").style.display = "none";
        document.getElementById("panelConsenso").style.display = 'none';
        document.getElementById("documento").style.display = 'none';
        document.getElementById("pie").style.display = "block";
        if (menu) menu.style.visibility = "hidden";
        clearInterval(timerFlores);
        clearInterval(timerArbol);
        hidePanelDer();
        hidePanelIzq();

        //doy tiempo a salir al arbol
        setTimeout(function () {
            document.getElementById("arbol").style.visibility = 'hidden';

            //activo el menuppal
            if (visual.level == 1) {
                document.getElementById("menuppal").style.visibility = "visible";
            }
            else {
                //menu ppal
                efectoOpacity(document.getElementById("menuppal"), 0, 0, 1, TWEEN.Easing.Linear.None);
            }

            timerCiclo = setInterval(function () {
                document.getElementById("ciclo").style.transform = 'rotate(' + rotacionCiclo++ + 'deg)';
            }, 100);

        }, 800); //doy tiempo a salir al arbol

        estado = 'menuppal';
        actualizarDatosGrupo();
    }
}

function doDecisiones() {
    document.location = "decisiones.html?grupo=" + grupoParam + "&idioma=" + idiomaParam;
}

function doResultados() {
    document.location = "resultados.html?grupo=" + grupoParam + "&idioma=" + idiomaParam;
}

function doEstructuras() {
    document.location = "estructuras.html?grupo=" + grupoParam + "&idioma=" + idiomaParam;
}

function doSeguimiento() {
    document.location = "seguimiento.html?grupo=" + grupoParam + "&idioma=" + idiomaParam;
}

function startRecognition(id) {

    if (window.hasOwnProperty('webkitSpeechRecognition')) {

        var recognition = new webkitSpeechRecognition();

        recognition.continuous = true;
        recognition.interimResults = true;

        recognition.lang = "es-ES";
        recognition.start();
        acum = "";

        recognition.onresult = function (e) {
            document.getElementById(id).innerHTML = e.results[0][0].transcript;

            if (e.results[0].isFinal)
                recognition.stop();
        };

        recognition.onerror = function (e) {
            recognition.stop();
        }

    }
    else
        alert(tr("norecognition"));
}