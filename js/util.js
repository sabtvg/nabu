
function JSON_encode(s) {
    s = s.replace('&', '[amp]');  //este va primero
    s = s.replace('€', '[euro]'); //uso codigo particular porque si no falla al enviarlo  al servidor
    s = s.replace('£', '[pound]');
    s = s.replace('>', '[mayor]'); //uso codigo particular porque si no falla al enviarlo  al servidor
    s = s.replace('<', '[menor]'); //uso codigo particular porque si no falla al enviarlo  al servidor
    s = s.replace('º', '[deg]');
    s = s.replace('ª', '[ordf]');
    s = s.replace('@', '[h64]');
    s = s.replace('Ñ', '[Ntilde]');
    s = s.replace('ñ', '[ntilde]');
    s = s.replace('ç', '[ccedil]');
    s = s.replace('+', '[h43]');
    s = s.replace('-', '[h45]');
    s = s.replace('¿', '[iquest]');
    s = s.replace('?', '[h63]');
    s = s.replace('#', '[h35]');
    s = s.replace('/', '[frasl]');
    s = s.replace('\\', '[h92]');
    s = s.replace('=', '[h61]');
    s = s.replace('$', '[h36]');
    s = s.replace('|', '[h124]');
    s = s.replace('\'', '[lsquo]');
    s = s.replace('\"', '[ldquo]');
    s = s.replace(/\n/g, "\\n");
    return s;
}

function JSON_decode(s) {
    //esta funcion esta reptida del lado del servidor para poder generar consensos
    s = s.replace('[euro]', '€');
    s = s.replace('[pound]','£');
    s = s.replace('[mayor]','>');
    s = s.replace('[menor]','<');
    s = s.replace('[amp]','&');
    s = s.replace('[deg]','º');
    s = s.replace('[ordf]','ª');
    s = s.replace('[h64]', '@');
    s = s.replace('[Ntilde]', 'Ñ');
    s = s.replace('[ntilde]', 'ñ');
    s = s.replace('[ccedil]', 'ç');
    s = s.replace('[h43]','+');
    s = s.replace('[h45]','-');
    s = s.replace('[iquest]','¿');
    s = s.replace('[h63]','?');
    s = s.replace('[h35]','#');
    s = s.replace('[frasl]','/');
    s = s.replace('[h92]','\\');
    s = s.replace('[h61]','=');
    s = s.replace('[h36]','$');
    s = s.replace('[h124]','|');
    s = s.replace('[lsquo]','\'');
    s = s.replace('[ldquo]', '\"');
    s = s.replace("\\n", "<br>");
    return s;
}

function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + "; " + expires;
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1);
        if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
    }
    return "";
}

function msg(txt) {
    msgDiv.className = "error";
    msgDiv.innerHTML = txt;
    setTimeout(function () { msgDiv.className = ""; msgDiv.innerHTML = ""; }, 4000);
}

function doOpen(url, width, height) {
    getHttp(url, function (data) {

        if (width) {
            document.getElementById("help").style.left = (window.innerWidth / 2 - width / 2).toFixed(0) + "px";
            document.getElementById("help").style.width = width + "px";
        }
        else {
            if (visual.browser == "Safari") {
                document.getElementById("help").style.left = "20px";
                document.getElementById("help").style.width = (window.innerWidth - 90) + "px";
            }
            else {
                document.getElementById("help").style.left = "20px";
                document.getElementById("help").style.width = (window.innerWidth - 90) + "px";
            }
        }

        if (height) {
            document.getElementById("help").style.top = (window.innerHeight / 2 - height / 2).toFixed(0) + "px";
            document.getElementById("help").style.height = height + "px";
        }
        else {
            if (visual.browser == "Safari") {
                document.getElementById("help").style.top = "30px";
            }
            else {
                efectoTop(document.getElementById("help"), 0, -window.innerHeight + 50, 35, TWEEN.Easing.Cubic.Out);
            }
            document.getElementById("help").style.height = (window.innerHeight - 80) + "px";
        }

        document.getElementById("helpContent").innerHTML = data;
        document.getElementById("help").style.visibility = "visible";
    });
}

function efectoTop(obj, wait, yini, yfin, efecto, complete) {
    if (visual.level == 1) {
        obj.style.visibility = "visible";
        obj.style.top = yfin.toFixed(0) + 'px';
        if (complete)
            complete();
    }
    else
        var tween = new TWEEN.Tween({ y: yini })
            .to({ y: yfin }, 750)
            .easing(efecto)
            .onStart(function () {
                obj.style.top = yini;
                obj.style.visibility = "visible";
            })
            .onUpdate(function () {
                obj.style.top = this.y.toFixed(0) + 'px';
            })
            .onComplete(function () {
                if (complete)
                    complete();
            })
            .delay(wait)
            .start();
}

function getVisualizacion(config) {
    var visual = { browser: config.browser, type: config.type, version: config.version };

    if ((visual.browser == "Chrome" && visual.version >= "40") ||
        (visual.browser == "InternetExplorer" && visual.version >= "10") ||
        (visual.browser == "Firefox" && visual.version >= "34") ||
        (visual.browser == "Safari" && visual.version >= "6")) {
        visual.level = 10; //completo
        //alert("v10");
    }
    else if (visual.browser == "Safari" && visual.version >= "5") {
        visual.level = 1; //basico
        //alert("v1");
    }
    else {
        visual.level = 0; //no se puede ver
        //alert("v0");
    }

    //tamaño de pantalla minimo
    if (config.width < 800 || config.height < 600)
        visual.screen = true; //no se puede ver
    else
        visual.screen = true; //resolucion de pantalla correcta

    return visual;
}

function efectoLeft(obj, wait, xini, xfin, efecto, complete) {
    if (visual.level == 1) {
        obj.style.visibility = "visible";
        obj.style.left = xfin.toFixed(0) + 'px';
        if (complete)
            complete();
    }
    else
        var tween = new TWEEN.Tween({ x: xini })
            .to({ x: xfin }, 750)
            .easing(efecto)
            .onStart(function () {
                obj.style.left = xini;
                obj.style.visibility = "visible";
            })
            .onUpdate(function () {
                obj.style.left = this.x.toFixed(0) + 'px';
            })
            .onComplete(function () {
                if (complete)
                    complete();
            })
            .delay(wait)
            .start();
}

function efectoOpacity(obj, wait, oini, ofin, efecto, complete) {
    if (visual.level == 1) {
        obj.style.visibility = "visible";
        obj.style.opacity = this.o;
        if (complete)
            complete();
    }
    else
        var tween = new TWEEN.Tween({ o: oini })
            .to({ o: ofin }, 1000)
            .easing(efecto)
            .onStart(function () {
                obj.style.visibility = "visible";
            })
            .onUpdate(function () {
                obj.style.opacity = this.o;
            })
            .onComplete(function () {
                if (complete)
                    complete();
            })
            .delay(wait)
            .start();
}

function getHttp(url, callback) {
    xmlhttp = new XMLHttpRequest();
    xmlhttp.open('get', url, true);
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4) {
            callback(xmlhttp.responseText);
        }
    }
    xmlhttp.send();
}

function postHttp(url, post, callback) {
    xmlhttp = new XMLHttpRequest();
    xmlhttp.open('POST', url, true);

    xmlhttp.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
    //xmlhttp.setRequestHeader("Content-length", post.length);
    //xmlhttp.setRequestHeader("Connection", "close");

    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4) {
            callback(xmlhttp.responseText);
        }
    }
    xmlhttp.send(post);
}

function actualizarDatosConsenso() {
    var panel = document.getElementById("panelConsenso");
    var ret = "";
    var ap = arbolPersonal;

    ret = "<div class='titulo2' style='margin: 0px;padding:0px;'><nobr><b>Consenso</b></nobr></div>";
    ret += "<div class='titulo3' style='margin: 0px;padding:0px;'><nobr>Usuarios: " + ap.usuarios + "<br>Activos: " + ap.activos + "</nobr></div>";
    ret += "<div class='titulo3' style='margin: 0px;padding:0px;'><nobr>Si&ge;" + ap.minSiValue + " (" + ap.minSiPc + "%)</nobr></div>";
    ret += "<div class='titulo3' style='margin: 0px;padding:0px;'><nobr>No&le;" + ap.maxNoValue + " (" + ap.maxNoPc + "%)</nobr></div>";
    panel.innerHTML = ret;
}

function actualizarDatosGrupo() {
    var panel = document.getElementById("panelGrupo");
    var ret = "";
    var ap = arbolPersonal;
    var born = new Date(ap.born.match(/\d+/)[0] * 1);
    var dias = Math.abs(new Date() - born) / (24 * 60 * 60 * 1000);
    ret = "<div class='titulo2' style='margin: 0px;padding:0px;'><nobr><b>Grupo</b></nobr></div>";
    ret += "<div class='titulo3' style='margin: 0px;padding:0px;'><nobr>" + tr("D&iacute;as") + ":" + dias.toFixed(0) + "</nobr></div>";
    ret += "<div class='titulo3' style='margin: 0px;padding:0px;'><nobr>" + tr("Usuarios") + ": " + ap.usuarios + "</nobr></div>";
    ret += "<div class='titulo3' style='margin: 0px;padding:0px;'><nobr>" + tr("Activos") + ": " + ap.activos + "</nobr></div>";
    ret += "<div class='titulo3' style='margin: 0px;padding:0px;'><nobr>" + tr("Consensos") + ":" + ap.documentos + "</nobr></div>";
    //ret += "<div class='titulo3' style='margin: 0px;padding:0px;'><nobr>Comunes:0</nobr></div>";
    panel.innerHTML = ret;
    panel.style.visibility = 'visible';
}

function formatDate(d) {
    var day = d.getDate();
    var month = d.getMonth() + 1;
    var year = d.getFullYear();
    return (day + '/' + month + '/' + year);
}

function jsonToDate(jsondate) {
    return new Date(jsondate.match(/\d+/)[0] * 1);
}

function tr(mensaje) {
    //aplico traduccion
    if (grupo) {
        if (grupo.idioma = "ES")
            return mensaje;
    }
    else
        return mensaje; //por ahora
}
