var frasesDelDia = [
    "Estamos tod@s juntos en esto",
    "Tod@s somos distint@s, pero tod@s tenemos algo en com&uacute;n.<br>Aceptamos la diversidad como parte de la vida, no la juzgamos.<br>Buscamos el lado com&uacute;n<br>y construimos sobre &eacute;l.<br>No competimos, cooperamos.",
    "No competimos, cooperamos.",
    "No se puede resolver los problemas<br>con la misma mentalidad con que fueron creados<br>hay que cambiar la manera de pensar primero",
    "Construyamos un bonito &aacute;rbol, <br>un &aacute;rbol colectivo.",
    "Tod@s triunfamos o tod@s fracasamos,<br>tod@s damos y tod@s recibimos.",
    "La meta es la unidad, <br>no la unanimidad.",
    "Agr&uacute;pate y conquistar&aacute;s.",
    "Sincronizamos intereses, <br>tomamos decisiones, <br>generamos acciones.",
    "Buscamos eficiencia, <br>no beneficio.",
    "No necesitamos representantes.",
    "No te preguntes qu&eacute; puede hacer la cooperativa por ti, <br>preg&uacute;ntate qu&eacute; puedes hacer t&uacute; por ella.",
    "Bienvenid@ al juego de la cooperaci&oacute;n.",
    "Un consenso no se elige, se construye.",
    "Si educaramos para la diversidad,<br>toda rareza ser&iacute;a bienvenida,<br>no habr&iacute;a nadie a quien juzgar,<br>y serias tan libre como puedas so&ntilde;ar.",
    "El consenso es un proceso cooperativo"];
var frasesVistas = [];

var selectedNode;
var scale = window.innerWidth / 1920;
var estado = '';
var rotacionCiclo = 0;
var timerCiclo, timerFlores, timerArbol;
var rotFlores = 0;
var refreshArbolInterval = 10000; //10seg
var lastArbolRecibidoTs = (new Date()).getTime();
var joyInterval;
var textAreas;
var preguntarAlSalir = false;
var propuestaTemp;
var scalex = window.innerWidth / 1920;
var scaley = window.innerHeight / 955; //1080-bordes de pantalla

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

function doLoad() {
    //settings
    window.onbeforeunload = preguntar;
    //background
    document.body.style.backgroundSize = window.innerWidth + 'px ' + window.innerHeight + 'px';
    //wait
    document.getElementById("florWait").style.top = (window.innerHeight / 2 - 100).toFixed(0) + 'px';
    document.getElementById("florWait").style.left = (window.innerWidth / 2 - 98).toFixed(0) + 'px';
    document.getElementById("florWait").style.visibility = "visible";

    //busco arboles
    getHttp("doMain.aspx?actn=getConfig&width=" + window.innerWidth + "&height=" + window.innerHeight,
        function (data) {
            getConfig(data);

            if (visual.level == 0) {
                //navegador no soporta nabu
                document.getElementById("florWait").style.visibility = "hidden";
                document.getElementById("noSoportado").style.visibility = "visible";
                document.getElementById("noSoportadoMsg").innerHTML = "Nab&uacute; no puede mostrarse<br /> en esta versi&oacute;n de navegador";
            }
            else if (!visual.screen){
                document.getElementById("florWait").style.visibility = "hidden";
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

        //si hay cookie, login automatico si no login normal
        var cookie = getCookie("nabu");
        if (cookie == "") {
            //login normal
            loginEffectIn();
            calcularResize();
        }
        else {
            //login automatico to server
            var vals = cookie.split("|");
            var usuario = { nombre: vals[0], email: vals[1], clave: vals[2], grupo: vals[3], isAdmin: vals[4] };

            getHttp("doMain.aspx?actn=login&email=" + usuario.email + "&clave=" + usuario.clave + "&grupo=" + usuario.grupo,
                function (data) {
                    try {
                        //atrapo el error si es que hay
                        if (data.substring(0, 6) == "Error=") {
                            //ha habido un error
                            //login normal
                            calcularResize();
                            loginEffectIn();
                        }
                        else {
                            //login ok, he recibido el arbol
                            var loginData = JSON.parse(data);

                            //guardo el grupo
                            grupo = loginData.grupo;

                            //guardo el arbol
                            arbolPersonal = loginData.arbolPersonal;

                            //guardo los modelos
                            modelos = loginData.modelos;

                            //guardo cookie
                            setCookie("nabu", arbolPersonal.usuario.nombre + "|" + arbolPersonal.usuario.email + "|" + arbolPersonal.usuario.clave + "|" + arbolPersonal.nombre + "|" + arbolPersonal.usuario.isAdmin, 7);

                            //activo menuppal
                            doMenuppal();

                            calcularResize();
                        }
                    }
                    catch (ex) {
                        //envio al server
                        sendException(ex, "doLoad2.2");
                    }
                });
        }
    }
    catch (ex) {
        //envio al server
        sendException(ex, "doLoad2");
    }
}

function preguntar() {
    if (preguntarAlSalir)
        return "Se perderan los datos no guardados";
}

function calcularResize() {
    scalex = window.innerWidth / 1920;
    scaley = window.innerHeight / 955; //1080-bordes de pantalla

    //login
    document.getElementById("tip").style.top = (window.innerHeight / 2 + 130) + 'px';
    document.getElementById("tip").style.left = (window.innerWidth / 2 - 200) + 'px';
    document.getElementById("loginIn").style.top = window.innerHeight / 6 + 'px';
    document.getElementById("loginFlor").style.top = (window.innerHeight / 6 + 50 * scaley) + 'px';
    document.getElementById("loginIn").style.left = (window.innerWidth / 2 - 50) + 'px';
    document.getElementById("loginFlor").style.left = (window.innerWidth / 2 - 280) + 'px';

    //titulo
    document.getElementById("titulo").style.left = (window.innerWidth - 280) + 'px';
    document.getElementById("titulo0").style.fontSize = (50 * scaley).toFixed(0) + 'px';
    document.getElementById("titulo1").style.fontSize = (30 * scaley).toFixed(0) + 'px';
    document.getElementById("titulo2").style.fontSize = (25 * scaley).toFixed(0) + 'px';

    //atras
    document.getElementById("atras").style.width = (100 * scalex) + 'px';
    document.getElementById("atras").style.height = (100 * scaley).toFixed(0) + 'px';

    //pie
    document.getElementById("pie").style.top = (window.innerHeight - 25).toFixed(0) + 'px';
    document.getElementById("pie").style.left = (window.innerWidth / 2 - 300).toFixed(0) + 'px';
    document.getElementById("pie").style.visibility = 'visible';

    //frase del dia
    //busco una no vista
    var cant = 0;
    var index = Math.round(Math.random() * (frasesDelDia.length - 1));
    while (frasesVistas.indexOf(index) >= 0 && cant < frasesDelDia.length) {
        cant++;
        index = Math.round(Math.random() * (frasesDelDia.length - 1));
    }
    if (cant >= frasesDelDia.length)
        frasesVistas = [];
    document.getElementById("tip").innerHTML = frasesDelDia[index];
    frasesVistas.push(index);

    //background
    document.body.style.backgroundSize = window.innerWidth + 'px ' + window.innerHeight + 'px';

    //joystick
    document.getElementById("joystick").style.top = (window.innerHeight - 190) + 'px';

    //resize del menuppal segun pantalla
    var idioma = grupo ? grupo.idioma : "ES"; //por si aun no se ha cargado el grupo

    var menuscale = scaley * 1.1;
    document.getElementById("tituloppal").style.width = 800 * menuscale + 'px';
    document.getElementById("tituloppal").fontSize = (60 * scale).toFixed(0) + 'px';

    document.getElementById("menuppal").style.top = (window.innerHeight / 2 - 600 * menuscale / 2).toFixed(0) + 'px';
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
    document.getElementById("ppal2").title = tr("Publicación de argumentos");
    document.getElementById("ppal2").style.width = 269 * menuscale + 'px';
    document.getElementById("ppal2").style.height = 199 * menuscale + 'px';
    document.getElementById("ppal2").style.left = 35 * menuscale + 'px';
    document.getElementById("ppal2").style.top = 282 * menuscale + 'px';

    document.getElementById("ppal3").src = "res/" + idioma + "/vert.png";
    document.getElementById("ppal3").onmouseover = "this.src='res/" + idioma + "/vert2.png';";
    document.getElementById("ppal3").onmouseout = "this.src='res/" + idioma + "/vert.png';";
    document.getElementById("ppal3").title = tr("Ambito operativo");
    document.getElementById("ppal3").style.width = 336 * menuscale + 'px';
    document.getElementById("ppal3").style.height = 213 * menuscale + 'px';
    document.getElementById("ppal3").style.left = 488 * menuscale + 'px';
    document.getElementById("ppal3").style.top = 179 * menuscale + 'px';

    document.getElementById("ppal5").src = "res/" + idioma + "/docConsensos.png";
    document.getElementById("ppal5").onmouseover = "this.src='res/" + idioma + "/docConsensos2.png';";
    document.getElementById("ppal5").onmouseout = "this.src='res/" + idioma + "/docConsensos.png';";
    document.getElementById("ppal5").title = tr("Documento de consenso alcanzados");
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

    document.getElementById("ppal8").src = "res/" + idioma + "/manifiesto.png";
    document.getElementById("ppal8").title = tr("Manifiesto del grupo");
    document.getElementById("ppal8").style.width = 130 * menuscale + 'px';
    document.getElementById("ppal8").style.height = 50 * menuscale + 'px';
    document.getElementById("ppal8").style.left = 210 * menuscale + 'px';
    document.getElementById("ppal8").style.top = 268 * menuscale + 'px';

    if (arbolPersonal && arbolPersonal.URLEstatuto != "")
        document.getElementById("ppal9").src = "res/documentos/manifiesto.png";
    else
        document.getElementById("ppal9").src = "res/noManifiesto.png";

    document.getElementById("ppal9").style.width = 64 * menuscale + 'px';
    document.getElementById("ppal9").style.height = 79 * menuscale + 'px';
    document.getElementById("ppal9").style.left = 140 * menuscale + 'px';
    document.getElementById("ppal9").style.top = 258 * menuscale + 'px';

    document.getElementById("ppal10").style.width = 35 * menuscale + 'px';
    document.getElementById("ppal10").style.height = 230 * menuscale + 'px';
    document.getElementById("ppal10").style.left = 0 * menuscale + 'px';
    document.getElementById("ppal10").style.top = 170 * menuscale + 'px';

    document.getElementById("ppal11").style.width = 35 * menuscale + 'px';
    document.getElementById("ppal11").style.height = 230 * menuscale + 'px';
    document.getElementById("ppal11").style.left = 839 * menuscale + 'px';
    document.getElementById("ppal11").style.top = 150 * menuscale + 'px';

    //arbol
    translateArbol(translatex = 0, translatey = 0);
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
        if (visual.level == 1) {
            //sin efectos
            document.getElementById("tip").style.visibility = "visible";
            document.getElementById("loginIn").style.visibility = "visible";
            document.getElementById("loginFlor").style.visibility = "visible";

            document.getElementById("tip").style.top = (window.innerHeight / 2 + 130) + 'px';
            document.getElementById("tip").style.left = (window.innerWidth / 2 - 200) + 'px';
            document.getElementById("loginIn").style.top = window.innerHeight / 6 + 'px';
            document.getElementById("loginFlor").style.top = (window.innerHeight / 6 + 50 * scale) + 'px';
            document.getElementById("loginIn").style.left = (window.innerWidth / 2 - 50) + 'px';
            document.getElementById("loginFlor").style.left = (window.innerWidth / 2 - 280) + 'px';
            document.getElementById("pie").style.left = (window.innerWidth / 2 - 300) + 'px';
        }
        else {
            document.getElementById("tip").style.visibility = "hidden";
            document.getElementById("loginIn").style.visibility = "hidden";
            document.getElementById("loginFlor").style.visibility = "hidden";

            document.getElementById("tip").style.left = (window.innerWidth / 2 - 200) + 'px';
            document.getElementById("loginIn").style.top = window.innerHeight / 4 + 'px';
            document.getElementById("loginFlor").style.top = (window.innerHeight / 4 + 50 * scale) + 'px';

            efectoLeft(document.getElementById("loginIn"), 0, window.innerWidth, window.innerWidth / 2 - 50, TWEEN.Easing.Cubic.Out);
            efectoLeft(document.getElementById("loginFlor"), 200, -200, window.innerWidth / 2 - 280, TWEEN.Easing.Exponential.Out);
            efectoTop(document.getElementById("tip"), 800, window.innerHeight, window.innerHeight / 2 + 130, TWEEN.Easing.Elastic.Out);
            timerFlores = setInterval(function () {
                rotFlores += 0.3;
                document.getElementById("loginFlor").style.transform = "rotate(" + rotFlores + "deg)";
            }, 100);
        }

        //wait
        document.getElementById("florWait").style.visibility = "hidden";
    }
    catch (ex) {
        //envio al server
        sendException(ex, "loginEffectIn");
    }
}

function loginEffectOut() {
    //login effect
    efectoLeft(document.getElementById("loginIn"), 0, window.innerWidth / 2 - 50, window.innerWidth, TWEEN.Easing.Cubic.Out);
    efectoLeft(document.getElementById("loginFlor"), 200, window.innerWidth / 2 - 280, -250, TWEEN.Easing.Linear.None);
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
}

function animate(time) {
    requestAnimationFrame(animate);
    TWEEN.update(time);
}

function getConfig(data) {
    config = JSON.parse(data);

    //fijo arbol segun querystring
    var grupo = getParameterByName('grupo') + "";
    var selected = false;

    //cargo la lista de arboles
    var list = config.grupos;
    var grupos = document.getElementById("grupos");
    for (var q in list) {
        var option = document.createElement("option");
        option.text = list[q];
        if (list[q].toLowerCase() == grupo.toLowerCase()) {
            option.selected = true;
            selected = true;
        }
        grupos.add(option);
    }

    if (selected)
        grupos.disabled = true;

    visual = getVisualizacion(config);
}

function doLogin() {
    email = document.getElementById("email").value;
    var clave = document.getElementById("clave").value;
    var loginResponse = document.getElementById("loginResponse");
    var grupos = document.getElementById("grupos");
    loginResponse.innerHTML = '';

    //login to server
    getHttp("doMain.aspx?actn=login&email=" + email + "&clave=" + clave + "&grupo=" + grupos.value,
        function (data) {
            if (data.substring(0, 6) == "Error=") {
                //ha habido un error
                var loginResponse = document.getElementById("loginResponse");
                loginResponse.innerHTML = '<font color=red>' + data.substring(6) + '</font>';
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
                setCookie("nabu", arbolPersonal.usuario.nombre + "|" + arbolPersonal.usuario.email + "|" + arbolPersonal.usuario.clave + "|" + grupo.nombre + "|" + arbolPersonal.usuario.isAdmin, 7);

                //efecto login out
                loginEffectOut();

                //activo menuppal
                doMenuppal(data);
            }
        });
}

function doMenuppal() {
    //paso a menuppal       

    //menu ppal
    setTimeout(function () {
        document.getElementById("tituloppal").innerHTML = arbolPersonal.nombre;
        document.getElementById("titulo").style.visibility = "visible";

        //atras
        document.getElementById("atras").style.visibility = "visible";

        //panel de usuario
        document.getElementById("usuario").innerHTML = "<div style='color:blue;cursor: pointer; font-size:14px; margin: 10px;' onclick='showCambiarClave();'>" + arbolPersonal.usuario.nombre + "</div>";
        document.getElementById("panelUsuario").style.visibility = 'hidden';
        document.getElementById("floresDisponibles").innerHTML = getFloresDisponibles().length;

        //panel consenso
        document.getElementById("panelConsenso").style.visibility = 'hidden';

        //panel grupo
        actualizarDatosGrupo();

        //panel usuario        
        document.getElementById("panelUsuario").style.visibility = 'visible';

        //adminOptions
        document.getElementById("adminOptions").style.visibility = arbolPersonal.usuario.isAdmin ? "visible" : "hidden";
    
        //user options
        document.getElementById("userOptions").style.visibility = arbolPersonal.usuario.isAdmin ? "hidden" : "visible";

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

    timerCiclo = setInterval(function () {
        document.getElementById("ciclo").style.transform = 'rotate(' + rotacionCiclo++ + 'deg)';
    }, 100);  

    estado = 'menuppal';

    //wait
    document.getElementById("florWait").style.visibility = "hidden";
}

function doCloseHelp() {
    document.getElementById("help").style.visibility = "hidden";
}

function showCambiarClave() {
    document.getElementById("altaUsuarioNombre").value = "";
    document.getElementById("altaUsuarioEmail").value = "";
    document.getElementById("cambiarClaveMsg").innerHTML = "";
    document.getElementById("cambiarClave").style.top = '240px';
    document.getElementById("cambiarClave").style.visibility = 'visible';
    efectoLeft(document.getElementById("cambiarClave"), 0, -230, 20, TWEEN.Easing.Cubic.Out);
}

function doAltaUsuario() {
    var list = config.grupos;
    var grupo = getParameterByName('grupo') + "";
    var selected = false;
    var grupos = document.getElementById("altaUsuarioGrupos");
    for (var q in list) {
        var option = document.createElement("option");
        option.text = list[q];
        if (list[q].toLowerCase() == grupo.toLowerCase()) {
            option.selected = true;
            selected = true;
        }
        grupos.add(option);
    }
    if (selected)
        grupos.disabled = true;

    document.getElementById("altaUsuarioMsg").innerHTML = "";
    document.getElementById("altaUsuario").style.left = (window.innerWidth / 2 - 330 / 2).toFixed(0) + 'px';
    document.getElementById("altaUsuario").style.visibility = 'visible';
    efectoTop(document.getElementById("altaUsuario"), 0, -330, window.innerHeight / 2 - 330 / 2, TWEEN.Easing.Cubic.Out);
}

function doAltaUsuarioEnviar() {
    var nombre = document.getElementById("altaUsuarioNombre");
    var email = document.getElementById("altaUsuarioEmail");
    var grupos = document.getElementById("altaUsuarioGrupos");
    var msg = document.getElementById("altaUsuarioMsg");

    if (nombre == "")
        msg.innerHTML = "<font color=green>Nombre no puede ser vac&iacute;o</font>";
    else if (email == "")
        msg.innerHTML = "<font color=green>Email no puede ser vac&iacute;o</font>";
    else {
        getHttp("doMain.aspx?actn=sendMailAlta&grupo=" + grupos.value + "&nombre=" + nombre.value + "&email=" + email.value,
            function (data) {
                //atrapo el error si es que hay
                if (data.substring(0, 6) == "Error=") {
                    //ha habido un error
                    msg.innerHTML = "<font color=red>" + data + "</font>";
                }
                else {
                    nombre.value = "";
                    email.value = "";
                    msg.innerHTML = "<font color=green>Mensaje enviado al administrador</font>";
                }
            });
    }
}

function doCambiarClave() {
    var oldPass = document.getElementById("oldPass").value;
    var newPass = document.getElementById("newPass").value;
    var repeat = document.getElementById("repeat").value;

    if (newPass != repeat)
        document.getElementById("cambiarClaveMsg").innerHTML = "<font color='red'>Las nuevas claves no coinciden</font>";
    else {
        getHttp("doMain.aspx?actn=cambiarClave&grupo=" + arbolPersonal.nombre + "&email=" + arbolPersonal.usuario.email + "&claveActual=" + oldPass + "&nuevaClave=" + newPass,
            function (data) {
                if (data != '')
                    document.getElementById("cambiarClaveMsg").innerHTML = "<font color='red'>" + data.substring(6) + "</font>";
                else 
                    efectoLeft(document.getElementById("cambiarClave"), 0, 20, -260, TWEEN.Easing.Cubic.Out, function () {
                        document.getElementById("cambiarClave").style.visibility = 'hidden';
                    });
            });
    }
}

function doHideCambiarClave() {
    efectoLeft(document.getElementById("cambiarClave"), 0, 20, -260, TWEEN.Easing.Cubic.In, function () {
            document.getElementById("cambiarClave").style.visibility = 'hidden';
        });
}

function doAtras() {
    if (estado == 'listaConsensos') {
        //listaConsensos voy a menuppal

        document.getElementById("panelListaConsensos").style.visibility = "hidden";

        estado = 'menuppal';
        actualizarDatosGrupo();

        //activo el menuppal
        if (visual.level == 1) {
            document.getElementById("menuppal").style.visibility = "visible";
        }
        else {
            //menu ppal
            efectoOpacity(document.getElementById("menuppal"), 0, 0, 1, TWEEN.Easing.Cubic.Out);
        }
    }
    else if (estado == 'aprendemos') {
        //aprendemos voy a menuppal

        //document.getElementById("joystick").style.visibility = 'hidden';
        document.getElementById("aprendemos").style.visibility = "hidden";
        document.getElementById("aprendemos").style.display = "none";

        //demoMsg
        document.getElementById("demoMsg").style.visibility = "hidden";
        document.getElementById("panelUsuario").style.visibility = 'visible';

        estado = 'menuppal';
        actualizarDatosGrupo();

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
        document.getElementById("panelGrupo").style.visibility = 'hidden';

        arbolPersonal = null;
        propuestas = [];
        setCookie("nabu", "", 1);

        document.getElementById("panelUsuario").style.visibility = 'hidden';
        document.getElementById("titulo").style.visibility = "hidden";
        document.getElementById("email").value = '';
        document.getElementById("clave").value = '';
        setTimeout(loginEffectIn, 600); //doy tiempo al menu a irse

        document.getElementById("adminOptions").style.visibility = "hidden";
        document.getElementById("userOptions").style.visibility = "hidden";
        document.getElementById("panelUsuario").style.visibility = 'hidden';

        document.getElementById("atras").style.visibility = "hidden";
        root = { "name": "?" };
        clearInterval(timerFlores);

        estado = '';
    }
    else if (estado == 'decidimos') {
        //decidimos voy a menuppal
        var objetivo = document.getElementById("objetivo");
        objetivo.innerHTML = arbolPersonal.objetivo;
        objetivo.style.visibility = 'hidden';

        document.getElementById("panelUsuario").style.visibility = 'visible';

        //efecto de salida: podo el arbol
        arbolPersonal.podado = true;
        arbolPersonal.raiz.children = [];
        arbolPersonal.logDocumentos = [];
        dibujarArbol(arbolPersonal.raiz);

        document.getElementById("joystick").style.visibility = 'hidden';
        document.getElementById("modelos").style.visibility = "hidden";
        document.getElementById("panelConsenso").style.visibility = 'hidden';
        document.getElementById("documento").style.visibility = 'hidden';
        if (menu) menu.style.visibility = "hidden";
        clearInterval(timerFlores);
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

function resize() {
    calcularResize();
}

function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    url = url.toLowerCase();
    name = name.toLowerCase();
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

function doListaConsensos() {
    //desde menu ppal
    if (visual.level == 1)
        document.getElementById("menuppal").style.visibility = "hidden";
    else
        efectoOpacity(document.getElementById("menuppal"), 0, 1, 0, TWEEN.Easing.Cubic.Out, function () { document.getElementById("menuppal").style.visibility = "hidden"; });

    document.getElementById("menuppal").style.visibility = "hidden";
    document.getElementById("panelGrupo").style.visibility = 'hidden';

    //creo listado
    var ret = "";
    ret += "<table>"   
    ret += "<tr><td colspan=2><h1>" + tr("Consensos") + "</h1></td></tr>"
    for (var i in modelos) {
        var m = modelos[i];
        ret += "<tr>";
        ret += "<td style='height: 60px;text-align:center;'><b>" + m.nombre + "</b></td>";
        ret += "<td>" + doListaConsensosModelo(m.id) + "</td>";
        ret += "</tr>"
    }
    ret += "</table>"

    //panel lista consensos
    document.getElementById("panelListaConsensos").style.height = (900 * scaley).toFixed(0) + 'px'; 
    document.getElementById("panelListaConsensos").style.width = (1700 * scalex).toFixed(0) + 'px';
    document.getElementById("panelListaConsensos").style.top = (window.innerHeight / 2 - 900 * scaley / 2).toFixed(0) + 'px';
    document.getElementById("panelListaConsensos").style.left = '120px';
    document.getElementById("panelListaConsensos").innerHTML = ret;

    if (visual.level == 1)
        document.getElementById("panelListaConsensos").style.visibility = 'visible';
    else 
        efectoOpacity(document.getElementById("panelListaConsensos"), 0, 0, 1, TWEEN.Easing.Linear.None);
    
    estado = 'listaConsensos';
}

function doListaConsensosModelo(modeloID) {
    var ret = "";
    for (var i in arbolPersonal.logDocumentos) {
        var ld = arbolPersonal.logDocumentos[i];
        if (ld.modeloID == modeloID) {
            ret += "<div style='float:left;'>";
            ret += "<table>";
            ret += "<tr>";
            ret += "<td><img src='" + ld.icono + "' style='height: 40px; width:32px;'></td>";
            ret += "<td style='font-size:12px;curso:pointer;'><a href='" + ld.URL + "' target='_blank'>" + ld.fname + "<br>" + ld.titulo + "<br>" + ld.sFecha + "</a></td>"
            ret += "</tr>";
            ret += "<tr>";
            ret += "<td colspan=2 style='font-size:12px;cursor:pointer;'><font color='blue' onclick='doSeguimiento(" + ld.docID + ");'><u>(ver seguimiento)</u></font></td>";
            ret += "</tr>";
            ret += "</table>";
            ret += "</div>";
        }
    }
    return ret;
}

function getLogDocumento(docID) {
    for (var i in arbolPersonal.logDocumentos) {
        var ld = arbolPersonal.logDocumentos[i];
        if (ld.docID == docID)
            return ld;
    }
}

function doSeguimiento(docID) {
    //envio
    getHttp("doDecidimos.aspx?actn=seguimiento&docID=" + docID + "&grupo=" + arbolPersonal.nombre + "&width=" + (window.innerWidth - 80),
        function (data) {
            //muestro
            document.getElementById("documento").innerHTML = data;

            document.getElementById("documento").style.visibility = 'visible';
            document.getElementById("documento").style.left = (10) + 'px';
            document.getElementById("documento").style.width = (window.innerWidth - 80) + 'px';
            document.getElementById("documento").style.height = (window.innerHeight - 50) + 'px';
            efectoTop(document.getElementById("documento"), 0, -window.innerHeight, 20, TWEEN.Easing.Cubic.Out);
        });
}
