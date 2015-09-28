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
    "El consenso es un proceso cooperativo"];

var selectedNode;
var scale = window.innerWidth / 1920;
var estado = '';
var rotacionCiclo = 0;
var timerCiclo, timerFlores, timerArbol;
var rotFlores = 0;
var documentoModeOn = false;
var refreshArbolInterval = 10000; //10seg
var lastArbolRecibidoTs = (new Date()).getTime();
var joyInterval;

//parametros para consenso
var vUsuarios, vActivos, vminSi, vmaxNo;

//tipo de visualizacion
var visual;  //Chrome, Zafari, InternetExplorer

//arbol y usuario con sus flores
var arbolPersonal;

//modelos de documentos
var modelosDocumento;

//propuestas
var propuestas = [];

function doLoad() {
    //wait
    document.getElementById("florWait").style.top = (window.innerHeight / 2 - 100).toFixed(0) + 'px';
    document.getElementById("florWait").style.left = (window.innerWidth / 2 - 98).toFixed(0) + 'px';
    document.getElementById("florWait").style.visibility = "visible";

    //busco arboles
    getHttp("doArbol.aspx?actn=getConfig&width=" + window.innerWidth + "&height=" + window.innerHeight,
        function (data) {
            getConfig(data);

            if (visual.level == 0) {
                //navegador no soporta nabu
                document.getElementById("florWait").style.visibility = "hidden";
                document.getElementById("noSoportado").style.visibility = "visible";
            }
            else
                //continuo con la carga
                doLoad2();
        });
}

function doLoad2() {
    try {
        //segunda parte del load
        //titulo
        document.getElementById("titulo").style.left = (window.innerWidth - 280) + 'px';

        //pie
        document.getElementById("pie").style.top = (window.innerHeight - 25) + 'px';
        document.getElementById("pie").style.left = (window.innerWidth / 2 - 300).toFixed(0) + 'px';
        document.getElementById("pie").style.visibility = 'visible';

        //frase del dia
        document.getElementById("tip").innerHTML = frasesDelDia[Math.round(Math.random() * (frasesDelDia.length - 1))];

        //forkme
        document.getElementById("forkme").style.left = (window.innerWidth - 87) + 'px';
        document.getElementById("forkme").style.top = '0px';
        document.getElementById("forkme").style.visibility = 'visible';
    
        //resize del menuppal segun pantalla
        var menuscale = scale * 1.1;
        document.getElementById("tituloppal").style.width = 800 * menuscale + 'px';

        document.getElementById("menuppal").style.width = 800 * menuscale + 'px';
        document.getElementById("menuppal").style.height = 600 * menuscale + 'px';

        document.getElementById("ciclo").style.width = 508 * menuscale + 'px';
        document.getElementById("ciclo").style.height = 526 * menuscale + 'px';
        document.getElementById("ciclo").style.left = 159 * menuscale + 'px';
        document.getElementById("ciclo").style.top = 47 * menuscale + 'px';

        document.getElementById("ppal1").style.width = 269 * menuscale + 'px';
        document.getElementById("ppal1").style.height = 150 * menuscale + 'px';
        document.getElementById("ppal1").style.left = 15 * menuscale + 'px';
        document.getElementById("ppal1").style.top = 145 * menuscale + 'px';

        document.getElementById("ppal2").style.width = 269 * menuscale + 'px';
        document.getElementById("ppal2").style.height = 149 * menuscale + 'px';
        document.getElementById("ppal2").style.left = 15 * menuscale + 'px';
        document.getElementById("ppal2").style.top = 292 * menuscale + 'px';

        document.getElementById("ppal3").style.width = 259 * menuscale + 'px';
        document.getElementById("ppal3").style.height = 84 * menuscale + 'px';
        document.getElementById("ppal3").style.left = 488 * menuscale + 'px';
        document.getElementById("ppal3").style.top = 129 * menuscale + 'px';

        document.getElementById("ppal4").style.width = 258 * menuscale + 'px';
        document.getElementById("ppal4").style.height = 85 * menuscale + 'px';
        document.getElementById("ppal4").style.left = 487 * menuscale + 'px';
        document.getElementById("ppal4").style.top = 389 * menuscale + 'px';

        document.getElementById("ppal5").style.width = 124 * menuscale + 'px';
        document.getElementById("ppal5").style.height = 143 * menuscale + 'px';
        document.getElementById("ppal5").style.left = 338 * menuscale + 'px';
        document.getElementById("ppal5").style.top = 31 * menuscale + 'px';

        document.getElementById("ppal6").style.width = 195 * menuscale + 'px';
        document.getElementById("ppal6").style.height = 79 * menuscale + 'px';
        document.getElementById("ppal6").style.left = 485 * menuscale + 'px';
        document.getElementById("ppal6").style.top = 267 * menuscale + 'px';

        document.getElementById("ppal7").style.width = 221 * menuscale + 'px';
        document.getElementById("ppal7").style.height = 145 * menuscale + 'px';
        document.getElementById("ppal7").style.left = 285 * menuscale + 'px';
        document.getElementById("ppal7").style.top = 439 * menuscale + 'px';

        document.getElementById("ppal8").style.width = 159 * menuscale + 'px';
        document.getElementById("ppal8").style.height = 79 * menuscale + 'px';
        document.getElementById("ppal8").style.left = 120 * menuscale + 'px';
        document.getElementById("ppal8").style.top = 258 * menuscale + 'px';

        //sinconizo animaciones con el navegador
        try{
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
        if (cookie == "")
            //login normal
            loginEffectIn();
        else {
            //login automatico to server
            var vals = cookie.split("|");
            var usuario = { nombre: vals[0], email: vals[1], clave: vals[2], arbol: vals[3] };

            getHttp("doArbol.aspx?actn=login&email=" + usuario.email + "&clave=" + usuario.clave + "&arbol=" + usuario.arbol,
                function (data) {
                    try{
                        //atrapo el error si es que hay
                        if (data.substring(0, 6) == "Error=") {
                            //ha habido un error
                            //login normal
                            loginEffectIn();
                        }
                        else {
                            //login ok, he recibido el arbol
                            arbolPersonal = JSON.parse(data);

                            //guardo cookie
                            setCookie("nabu", arbolPersonal.usuario.nombre + "|" + arbolPersonal.usuario.email + "|" + arbolPersonal.usuario.clave + "|" + arbolPersonal.nombre, 7);

                            //pido modelos de documentos
                            getHttp("doArbol.aspx?actn=getModelosDocumento&arbol=" + usuario.arbol,
                                function (data) {
                                    //guardo los modelos de oducmentos
                                    var ret = JSON.parse(data);
                                    modelosDocumento = ret.modelos;

                                    //activo menuppal
                                    doMenuppal();
                                });
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

function sendException(ex, flag) {
    if (arbolPersonal)
        getHttp("doArbol.aspx?actn=exception&flag=" + flag + "&message=" + ex.message + "&stack=" + ex.stack + "&email=" + arbolPersonal.usuario.email + "&arbol=" + arbolPersonal.nombre, null);
    else
        getHttp("doArbol.aspx?actn=exception&flag=" + flag + "&message=" + ex.message + "&stack=" + ex.stack, null);
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
    var config = JSON.parse(data);

    //cargo la lista de arboles
    var list = config.arbolList;
    var arbolList = document.getElementById("arbolList");
    for (var q in list) {
        var option = document.createElement("option");
        option.text = list[q];
        arbolList.add(option);
    }

    visual = getVisualizacion(config);
}

function doLogin() {
    email = document.getElementById("email").value;
    var clave = document.getElementById("clave").value;
    var loginResponse = document.getElementById("loginResponse");
    var arbolList = document.getElementById("arbolList");
    loginResponse.innerHTML = '';

    //login to server
    getHttp("doArbol.aspx?actn=login&email=" + email + "&clave=" + clave + "&arbol=" + arbolList.value,
        function (data) {
            if (data.substring(0, 6) == "Error=") {
                //ha habido un error
                var loginResponse = document.getElementById("loginResponse");
                loginResponse.innerHTML = '<font color=red>' + data.substring(6) + '</font>';
            }
            else {
                //login effect
                //login ok, he recibido el arbol
                arbolPersonal = JSON.parse(data);
                //guardo cookie
                setCookie("nabu", arbolPersonal.usuario.nombre + "|" + arbolPersonal.usuario.email + "|" + arbolPersonal.usuario.clave + "|" + arbolPersonal.nombre, 7);
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
        document.getElementById("usuario").innerHTML = "<div style='color:blue;cursor: pointer; font-size:14px; margin: 10px;' onclick='showCambiarClave();'>" + arbolPersonal.usuario.email + "</div>";
        document.getElementById("panelUsuario").style.visibility = 'visible';
        document.getElementById("floresDisponibles").innerHTML = getFloresDisponibles().length;

        //panel consenso
        document.getElementById("panelConsenso").style.visibility = 'hidden';

        //adminOptions
        document.getElementById("adminOptions").style.visibility = arbolPersonal.usuario.isAdmin ? "visible" : "hidden";

        //menuppal
        var menuscale = scale * 1.1;
        document.getElementById("menuppal").style.top = (window.innerHeight / 2 - 300 * menuscale).toFixed(0) + 'px';
        document.getElementById("menuppal").style.left = (window.innerWidth / 2 - 400 * menuscale).toFixed(0) + 'px';
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

function doProponer() {
    var d = selectedNode;
    var modelo = getModelo(d.modeloID);

    if (getFloresDisponibles().length == 0)
        msg("No tienes flores disponibles");
    else if (d.consensoAlcanzado) 
        msg("Este debate ya ha alcanzado el consenso");
    else if (d.depth < modelo.secciones.length)
        doEditarDocumento();
    else
        msg("El modelo de documento no permite mas niveles de detalle");
}

function doProponerRoot() {
    //opciones de modelos de documentos
    if (getFloresDisponibles().length == 0)
        msg("No tienes flores disponibles");
    else {
        var list = "<table>";

        for (var i in modelosDocumento) {
            var modelo = modelosDocumento[i];
            if (modelo.activo)
                list += "<tr><td class='btn' style='text-align: center; width: 300px;' onclick='doProponerRoot2(" + modelo.id + ");'>" + modelo.nombre + "</td></tr>";
        }
        list += "</table>";

        document.getElementById("modelosList").innerHTML = list;
        document.getElementById("modelos").style.visibility = "visible";
    }
}

function doProponerRoot2(modeloID) {
    document.getElementById("modelos").style.visibility = "hidden";
    selectedNode.modeloID = modeloID; //se lo pongo temporalmente a la raiz
    doEditarDocumento();
}

function doCloseHelp() {
    document.getElementById("help").style.visibility = "hidden";
}

function showCambiarClave() {
    document.getElementById("cambiarClaveMsg").innerHTML = "";
    document.getElementById("cambiarClave").style.top = '240px';
    document.getElementById("cambiarClave").style.visibility = 'visible';
    efectoLeft(document.getElementById("cambiarClave"), 0, -230, 20, TWEEN.Easing.Cubic.Out);
}

function doCambiarClave() {
    var oldPass = document.getElementById("oldPass").value;
    var newPass = document.getElementById("newPass").value;
    var repeat = document.getElementById("repeat").value;

    if (newPass != repeat)
        document.getElementById("cambiarClaveMsg").innerHTML = "<font color='red'>Las nuevas claves no coinciden</font>";
    else {
        getHttp("doArbol.aspx?actn=cambiarClave&arbol=" + arbolPersonal.nombre + "&email=" + arbolPersonal.usuario.email + "&claveActual=" + oldPass + "&nuevaClave=" + newPass,
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

function recibirArbolPersonal(data) {
    var msgDiv = document.getElementById("msgDiv");

    if (data.substring(0, 6) == "Error=") {

        //ha habido un error
        msg(data.substring(6));
    }
    else {
        //actualizo el arbol recibido
        arbolPersonal = JSON.parse(data);

        dibujarArbol();

        //condicion de consenso
        actualizarDatosConsenso();

        //timestamp
        lastArbolRecibidoTs = (new Date()).getTime();

        //flores disponibles
        document.getElementById("floresDisponibles").innerHTML = getFloresDisponibles().length;
    }
}

function doToggleFlor() {
    var d = selectedNode;
    getHttp("doArbol.aspx?actn=toggleflor&email=" + arbolPersonal.usuario.email + "&id=" + d.id + "&x=" + d.x0 + "&arbol=" + arbolPersonal.nombre, recibirArbolPersonal);
}

function doCerrarDocumento() {
    document.getElementById("btnProponer").style.visibility = 'hidden';
    efectoTop(document.getElementById("documento"), 0, 50, -window.innerHeight, TWEEN.Easing.Cubic.In);
    showPanel();
}

function doPreguntarNombre() {
    //cierro documento
    document.getElementById("btnProponer").style.visibility = 'hidden';
    efectoTop(document.getElementById("documento"), 0, 50, -window.innerHeight, TWEEN.Easing.Cubic.In);

    //pregunto el nombre
    var node = selectedNode;
    if (node.nivel == 0)
        document.getElementById("tituloDebate").innerHTML = "Define un titulo para este debate";
    else
        document.getElementById("tituloDebate").innerHTML = "Define un nombre para la propuesta";

    document.getElementById("nombrePropuestaValor").value = "";
    document.getElementById("nombrePropuesta").style.top = (window.innerHeight / 2 - 94).toFixed(0) + 'px';
    document.getElementById("nombrePropuesta").style.left = (window.innerWidth / 2 - 275).toFixed(0) + 'px';
    efectoOpacity(document.getElementById("nombrePropuesta"), 250, 0, 1, TWEEN.Easing.Cubic.In);
}

function doEnviarPropuesta() {
    //leo los textos del documento y los envio al servidor
    var node = selectedNode;
    var nombre = document.getElementById("nombrePropuestaValor").value;

    //ejemplo json a enviar
    //[{"modeloID":0,"seccion":0,"textos":[{"maxLen":0,"texto":"2222222222222","titulo":""}]}]

    if (nombre != '') {
        //armo textos para proponer
        var post = '[';
        var modelo = getModelo(node.modeloID);

        for (var s = node.depth; s < modelo.secciones.length; s++) {
            var seccion = modelo.secciones[s]; //siempre se propone la seccion posterior al nodo actual (nuevo nodo)

            post += "{\"modeloID\":" + modelo.id + ",\"seccion\":" + s + ",\"textos\":["; //inicio seccion

            for (var t in seccion.temas) {
                var tema = seccion.temas[t];
                var area = document.getElementById("area_" + s + "_" + t);
                if (area) {
                    //estaba editando, envio al servidor
                    post += "{\"maxLen\":" + tema.maxLen + ", \"titulo\":\"" + tema.titulo + "\", \"texto\":\"" + area.value.replace("\"", "") + "\"},";
                }
            }
            post = post.substring(0, post.length - 1);

            post += "]},"; //fin seccion
        }
        post = post.substring(0, post.length - 1);
        post += "]";

        //envio
        postHttp("doArbol.aspx?actn=proponer&email=" + arbolPersonal.usuario.email + "&nombre=" + nombre + "&id=" + node.id + "&arbol=" + arbolPersonal.nombre,
            "propuestas=" + post,
            recibirArbolPersonal);

        //cierro pregunta de nombre
        document.getElementById("nombrePropuesta").style.visibility = "hidden";

        documentoModeOn = false; //si lo dejo activado se me cruzan las peticiones getHttp de showPanel con recibirArbolPersonal()
    }
}

function doVerDocumento() {
    var node = selectedNode;
    var modelo = getModelo(node.modeloID);

    //pido propuestas al servidor (por si hay nuevos comentarios)
    getHttp("doArbol.aspx?actn=getPropuestas&id=" + node.id + "&arbol=" + arbolPersonal.nombre,
        function (data) {
            recibirPropuestas(data); 
            doVerDocumento2();
        }
    );
}

function doVerDocumento2() {
    var node = selectedNode;
    var modelo = getModelo(node.modeloID);
    var path = getPath(node);
    var propuesta = propuestas[path[0].id];

    //puedo mostrar el documento
    hidePanelDer();
    hidePanelIzq();

    document.getElementById("tituloDocumento").innerHTML = modelo.nombre + ": " + propuesta.titulo;
    document.getElementById("anterior").innerHTML = getDocumento(node);
    document.getElementById("actual").innerHTML = "";
    document.getElementById("actual").style.visibility = "hidden";

    document.getElementById("documento").style.visibility = 'visible';
    document.getElementById("documento").style.left = (window.innerWidth * 0.05) + 'px';
    document.getElementById("documento").style.width = (window.innerWidth * 0.8) + 'px';
    document.getElementById("documento").style.height = (window.innerHeight - 100) + 'px';
    document.getElementById("docClose").style.top = (60) + 'px';
    document.getElementById("docClose").style.left = (window.innerWidth * 0.05 + window.innerWidth * 0.8 - 50) + 'px';
    document.getElementById("btnProponer").style.visibility = 'hidden';
    efectoTop(document.getElementById("documento"), 0, -window.innerHeight, 50, TWEEN.Easing.Cubic.Out);
}

function doEditarDocumento() {
    var node = selectedNode;
    var modelo = getModelo(node.modeloID);

    if (tengoPropuestas(node)) {
        //puedo mostrar el documento
        hidePanelDer();
        hidePanelIzq();

        //resuelvo el titulo del documento si se puede
        var path = getPath(node);
        var propuesta;
        if (path.length > 0) {
            propuesta = propuestas[path[0].id];
            document.getElementById("tituloDocumento").innerHTML = propuesta.titulo;
        }
        else
            document.getElementById("tituloDocumento").innerHTML = "";

        document.getElementById("anterior").innerHTML = getAnterior(node);
        document.getElementById("actual").innerHTML = getActual(node);
        document.getElementById("actual").style.visibility = "visible";

        document.getElementById("documento").style.visibility = 'visible';
        document.getElementById("documento").style.left = (window.innerWidth * 0.05) + 'px';
        document.getElementById("documento").style.width = (window.innerWidth * 0.8) + 'px';
        document.getElementById("documento").style.height = (window.innerHeight - 100) + 'px';
        document.getElementById("btnProponer").style.visibility = 'visible';
        efectoTop(document.getElementById("documento"), 0, -window.innerHeight, 50, TWEEN.Easing.Cubic.Out);
    }
    else {
        //pido propuestas al servidor
        getHttp("doArbol.aspx?actn=getPropuestas&id=" + node.id + "&arbol=" + arbolPersonal.nombre,
            function (data) {
                recibirPropuestas(data);
                doEditarDocumento();
            }
        );
    }
    //activo editor RTF
    var opts = { iconsPath: 'res/iconsPath.gif', buttonList: ['bold', 'italic', 'underline', 'left', 'center', 'right', 'justify', 'ol', 'ul', 'subscript', 'superscript', 'removeformat'] }
    var modelo = getModelo(node.modeloID);
    var seccion = modelo.secciones[node.depth]; 

}

function tengoPropuestas(node) {
    //veo si tengo las propuestas de este nodo y sus padres
    var parent = node;
    while (parent.id != arbolPersonal.raiz.id) {
        if (!propuestas[parent.id])
            return false; //me falta esta
        parent = parent.parent;
    }
    return true; //tengo todas
}

function recibirPropuestas(data) {
    var nuevasPropuestas = JSON.parse(data);
    for (var i in nuevasPropuestas) {
        var nop = nuevasPropuestas[i];
        propuestas[nop.nodoID] = nop;
    }
}

function getPath(node) {
    var ret = [];

    while (node.id != arbolPersonal.raiz.id) {
        ret.push(node);
        node = node.parent;
    }
    return ret;
}

function getDocumento(node) {
    //devuelvo documento con textos
    var ret = '';
    var modelo = getModelo(node.modeloID);
    var path = getPath(node);

    ret += "<table>";
    for (var i = 0; i < modelo.secciones.length; i++) {
        //escribo secciones
        ret += "<tr>";
        ret += "<td style='vertical-align: text-top;width:" + (window.innerWidth * 0.6) + "px;'>";
        ret += "<table style='width:100%;'><tr><td style='width:150px;'><font size=2><b>Nivel en el arbol " + (i + 1) + "</b></font></td><td><hr></td></tr></table>";
        var seccion = modelo.secciones[i];
        var nodoActual = path[path.length - i - 1];
        for (var t in seccion.temas) {
            //escribo temas
            var tema = seccion.temas[t];
            ret += "<br><font class='tema'>" + tema.titulo + ":</font><br>";

            if (path.length - i > 0) {
                var propuesta = propuestas[nodoActual.id];
                ret += "<div style='width:" + (window.innerWidth * 0.6 - 15) + "px;' class='texto'><pre>" + propuesta.textos[t].texto + "</pre></div><br><br>";
            }
        }
        if (i < path.length) ret += HTMLFlores(nodoActual);
        ret += "</td>";

        //escribo comentarios de seccion
        ret += "<td class='comentarios' style='vertical-align: text-top;width:" + (window.innerWidth * 0.2) + "px;'>";
        if (path.length - i > 0) {
            var propuesta = propuestas[path[path.length - i - 1].id];
            for (var t in propuesta.comentarios) {
                //escribo comentario de propuesta
                ret += "<div class='comentario' style='overflow: auto;width:" + (window.innerWidth * 0.2 - 40) + "px'><pre>" + propuesta.comentarios[t] + "</pre></div>";
            }
        }

        //escribo alta nuevo comentario
        if (path.length - i > 0 && path[path.length - i - 1].id == node.id) {
            var propuesta = propuestas[path[path.length - i - 1].id];
            ret += "<div id='replaceComentario" + propuesta.nodoID + "'>";
            ret += "<div class='smalltip'>Comenta la propuesta de manera constructiva, aporta referencias</div>";
            ret += "<textarea id='comentario" + propuesta.nodoID + "' maxlength='200'  class='actual' id='nuevoComentario' style='width: 90%; height: 70px;'></textarea><br>";
            ret += "<input type='button' class='btn' value='Enviar' onClick='doEnviarComentario(" + propuesta.nodoID + ");'>&nbsp;<font size=2>(max: 200)</font>";
            ret += "</div>";
        }
        ret += "</td>";
        //ret += "<tr><td colspan=2><hr></td></tr>";
        ret += "</tr>";

    }
    ret += '</table>';
    return ret;
}

function doEnviarComentario(id) {
    var comentario = document.getElementById("comentario" + id);
    if (comentario.value != "") {
        postHttp("doArbol.aspx?actn=doComentar&id=" + id + "&arbol=" + arbolPersonal.nombre,
            "comentario=" + comentario.value,
            function (data) {
                var replaceComentario = document.getElementById("replaceComentario" + id);
                replaceComentario.innerHTML = "<div class='comentario' style='width:" + (window.innerWidth * 0.2 - 10) + "px'><pre>" + comentario.value + "</pre></div><br>";
            }
        );
    }
}

function getAnterior(node) {
    //devuelvo documento con textos anteriores hasta node
    var ret = '';
    var modelo = getModelo(node.modeloID);

    ret += "<table style='width: 100%;'>";
    for (var i = 0; i < node.depth; i++) {
        //escribo seccion 
        var padre = getParent(node, i + 1);
        var propuesta = propuestas[padre.id]; //accedo al texto segun el indice de temas
        var seccion = modelo.secciones[i];

        ret += "<tr><td>";
        for (var t in seccion.temas) {
            //escribo tema de seccion
            var tema = seccion.temas[t];
            ret += "<br><font class='tema'>" + tema.titulo + ":</font><br>";

            //texto de tema
            ret += "<div class='texto' style='width:" + (window.innerWidth * 0.6 - 15) + "px;'><pre>" + propuesta.textos[t].texto + "</pre></div><br><br>"; //accedo al texto segun el indice de temas
            ret += HTMLFlores(padre);
        }
        ret += "</td>";

        //escribo comentarios de seccion
        ret += "<td class='comentarios' style='vertical-align: text-top;width: 250px;'>";
        for (var t in propuesta.comentarios) {
            //escribo comentario de propuesta
            ret += "<div class='comentario' style='overflow: auto;width:" + (window.innerWidth * 0.2 - 40) + "px'><pre>" + propuesta.comentarios[t] + "</pre></div>";
        }
        ret += "</td>";
        ret += "<tr><td colspan=2><hr></td></tr>";
        ret += "</tr>";
    }
    ret += "</table>";
    return ret;
}

function getActual(node) {
    //enseño interfaz para cargar textos de todas las secciones y temas >= al nivel actual
    var ret = '';
    var modelo = getModelo(node.modeloID);
   
    for (var s = node.depth; s < modelo.secciones.length; s++) {
        var seccion = modelo.secciones[s]; //edito el siguiente al nodo actual

        ret += "<table style='width:100%;'><tr><td style='width:150px;'><font size=2><b>Nivel en el arbol " + (s + 1) + "</b></font></td><td><hr></td></tr></table>";
        ret += "<div class='actual'>"
        for (var i in seccion.temas) {
            var tema = seccion.temas[i];
            ret += "<br><font class='tema'>" + tema.titulo + ":</font><br>";
            if (tema.tip != "") ret += "<div class='smalltip'>" + tema.tip + "</div>";
            ret += "<textarea id='area_" + s + "_" + i + "' maxlength='" + tema.maxLen + "' class='actual' style='width:97%; height: 200px;'  placeholder='[Dejalo vac&iacute;o si no quieres definirlo]'></textarea>";
            ret += "<div style='font-size: 12px; text-align: right;'>(max: " + tema.maxLen + ")</div>"
        }
        ret += "</div>";
        ret += "<br><br>";
    }
    return ret;
}

function HTMLFlores(node) {
    var ret;
    ret = "<div class='votos'>";
    ret += "<img src='res/icono.png'>";
    ret += "&nbsp;" + node.totalFlores;
    ret += "</div>";
    return ret;
}

function getParent(node, depth) {
    var ret = node;
    while (ret.depth > depth)
        ret = ret.parent;
    return ret;
}

function doVerDocumentoMode() {

    if (documentoModeOn)
        document.getElementById("documentoMode").src = 'res/documentoModeOff.png';
    else
        document.getElementById("documentoMode").src = 'res/documentoModeOn.png';
    documentoModeOn = !documentoModeOn;

    showPanel();
}

function showPanel() {
    //pido propuestas al servidor
    var node = selectedNode;
    if (node) {
        getHttp("doArbol.aspx?actn=getPropuestas&id=" + node.id + "&arbol=" + arbolPersonal.nombre,
            function (data) {
                recibirPropuestas(data);
                showPanel2();
            }
        );
    }
}

function showPanel2() {
    var panelIzq = document.getElementById("panelIzq");
    var panelDer = document.getElementById("panelDer");
    var panelIzq2 = document.getElementById("panelIzq2");
    var panelDer2 = document.getElementById("panelDer2");
    var panel;
    var node = selectedNode;
    var modelo = getModelo(node.modeloID);

    if (documentoModeOn && node.depth > 0) {
        //enseño panel
        if (tengoPropuestas(node)) {
            //puedo mostrar el documento
            if (node.x > 90) {
                //panel izquierdo
                //activo el izq
                if (panelIzq.style.visibility == "hidden") {
                    panelIzq.style.visibility = "visible";
                    efectoLeft(panelIzq, 0, -400 * scale, 20, TWEEN.Easing.Cubic.Out);
                }
                //desactivo el der
                hidePanelDer();

                panelIzq.style.top = "220px";
                panelIzq.style.width = 400 * scale + "px";
                panelIzq.style.height = 550 * scale + "px";

                panel = panelIzq2;
            }
            else {
                //panel derecho
                //activo el derecho
                if (panelDer.style.visibility == "hidden") {
                    panelDer.style.visibility = "visible";
                    efectoLeft(panelDer, 0, window.innerWidth, window.innerWidth - 400 * scale - 40, TWEEN.Easing.Cubic.Out);
                }
                //desactivo el izq
                hidePanelIzq();

                panelDer.style.top = "220px";
                panelDer.style.width = 400 * scale + "px";
                panelDer.style.height = 550 * scale + "px";

                panel = panelDer2;
            }

            //resuelvo el titulo del documento
            var path = getPath(node);
            var titulo = '';
            if (path.length > 0) {
                var propuesta = propuestas[path[0].id];
                titulo = propuesta.titulo;
            }
            
            //escribo propuesta
            var s = "<div class='titulo1'>" + titulo + "</div><br>";
            var propuesta = propuestas[node.id];
            for (var i in propuesta.textos) {
                var textoTema = propuesta.textos[i];
                s += "<div class='tema'>" + textoTema.titulo + "</div><br>";
                s += "<div class='texto'><pre>" + textoTema.texto + "</pre></div><br><br>";
            }
            s += HTMLFlores(node);

            //escribo comentarios
            if (propuesta.comentarios.length > 0) s += "<br>Comentarios:"
            for (var t in propuesta.comentarios) {
                //escribo comentario de propuesta
                s += "<div class='comentario'><pre>" + propuesta.comentarios[t] + "</pre></div>";
            }

            panel.innerHTML = s;
        }
        else {

        }
    }
    else {
        //oculto
        hidePanelIzq();
        hidePanelDer();
    }
}

function hidePanelIzq() {
    var panelIzq = document.getElementById("panelIzq");
    if (panelIzq.style.visibility == "visible") {
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

function doToggleVer() {
    //deshabilitado porque trae problemas al recibir nuevo arbol personal. Aparece lo que esta oculto otra vez porque cambian los datos
    var d = selectedNode;
    if (d.children) {
        d._children = d.children;
        d.children = null;
    } else {
        d.children = d._children;
        d._children = null;
    }

    if (d._children)
        document.getElementById("verocultar").src = 'res/ver.png';
    else
        document.getElementById("verocultar").src = 'res/ocultar.png';

    dibujarArbol(d);
}

function doAtras() {
    if (estado == 'menuppal') {
        //menu ppal voy a login
        efectoOpacity(document.getElementById("menuppal"), 0, 1, 0, TWEEN.Easing.Cubic.Out, function () { document.getElementById("menuppal").style.visibility = "hidden"; });

        arbolPersonal = null;
        propuestas = [];
        setCookie("nabu", "", 1);

        document.getElementById("panelUsuario").style.visibility = 'hidden';
        document.getElementById("titulo").style.visibility = "hidden";
        document.getElementById("email").value = '';
        document.getElementById("clave").value = '';
        setTimeout(loginEffectIn, 600); //doy tiempo al menu a irse

        document.getElementById("adminOptions").style.visibility = "hidden";

        document.getElementById("atras").style.visibility = "hidden";
        root = { "name": "?" };
        clearInterval(timerFlores);

        estado = '';
    }
    else if (estado == 'debates') {
        //debates voy a menuppal
        var objetivo = document.getElementById("objetivo");
        objetivo.innerHTML = arbolPersonal.objetivo;
        objetivo.style.visibility = 'hidden';

        //efecto de salida: podo el arbol
        arbolPersonal.podado = true;
        arbolPersonal.raiz.children = [];
        arbolPersonal.logDocumentos = [];
        dibujarArbol(arbolPersonal.raiz);

        document.getElementById("joystick").style.visibility = 'hidden';

        document.getElementById("panelConsenso").style.visibility = 'hidden';
        document.getElementById("documento").style.visibility = 'hidden';
        document.getElementById("btnProponer").style.visibility = 'hidden';
        if (menu) menu.style.visibility = "hidden";
        clearInterval(timerFlores);
        hidePanelDer();
        hidePanelIzq();

        //doy tiempo a salir al arbol
        setTimeout(function () {
            document.getElementById("svg").style.visibility = 'hidden';

            if (visual.level == 1) {
                document.getElementById("menuppal").style.visibility = "visible";
            }
            else {
                //menu ppal
                efectoOpacity(document.getElementById("menuppal"), 0, 0, 1, TWEEN.Easing.Cubic.Out);
            }

            timerCiclo = setInterval(function () {
                document.getElementById("ciclo").style.transform = 'rotate(' + rotacionCiclo++ + 'deg)';
            }, 100);

        }, 800); //doy tiempo a salir al arbol

        estado = 'menuppal';
    }
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

function getFloresDisponibles() {
    var ret = [];
    for (var i in arbolPersonal.usuario.flores) {
        var flor = arbolPersonal.usuario.flores[i];
        if (flor.id == 0)
            ret.push(flor);
    }
    return ret;
}

function doArbol() {
    if (arbolPersonal.podado)
        //el arbol esta podado, lo pido de nuevo
        getHttp("doArbol.aspx?actn=getArbolPersonal&email=" + arbolPersonal.usuario.email + "&arbol=" + arbolPersonal.nombre,
            function (data) {
                recibirArbolPersonal(data);
                doArbol();
            });
    else {
        //menu ppal
        if (visual.level == 1)
            document.getElementById("menuppal").style.visibility = "hidden";
        else
            efectoOpacity(document.getElementById("menuppal"), 0, 1, 0, TWEEN.Easing.Cubic.Out, function () { document.getElementById("menuppal").style.visibility = "hidden"; });

        //panel consenso
        document.getElementById("panelConsenso").style.visibility = 'visible';

        //joystick
        document.getElementById("joystick").style.visibility = 'visible';
        document.getElementById("joystick").style.top = (window.innerHeight - 190) + 'px';

        //objetivo
        var objetivo = document.getElementById("objetivo");
        objetivo.innerHTML = arbolPersonal.objetivo;
        objetivo.style.visibility = 'visible';

        //flores
        timerFlores = setInterval(rotarFlores, 200);

        //dibujo el arbol
        if (svg == null)
            crearArbol();
        else {
            document.getElementById("svg").style.visibility = 'visible';
            dibujarArbol(arbolPersonal.raiz);
        }

        actualizarDatosConsenso();

        //fijo intervalo de actualizacion del arbol
        timerArbol = setInterval(pedirArbol, refreshArbolInterval);

        estado = 'debates';
        clearInterval(timerCiclo);
    }
}

function pedirArbol() {
    if (arbolPersonal) {
        var now = (new Date()).getTime();
        if (now - lastArbolRecibidoTs > refreshArbolInterval)
            getHttp("doArbol.aspx?actn=getArbolPersonal&email=" + arbolPersonal.usuario.email + "&arbol=" + arbolPersonal.nombre, recibirArbolPersonal);
    }
}

