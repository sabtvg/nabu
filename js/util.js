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


function HTMLColor(v) {
    //el color de una celda del queso segun su valor (entre 0 y 10)
    return "rgba(" + (100 - v * 10) + "%, " + (v * 10) + "%, 50%, 0.8)";
}

function clone(obj) {
    if (null == obj || "object" != typeof obj) return obj;
    var copy = obj.constructor();
    for (var attr in obj) {
        if (obj.hasOwnProperty(attr)) copy[attr] = obj[attr];
    }
    return copy;
}

function setCookie(cname, cvalue, exdays) {
    cname = cname.replace(' ', '').toLowerCase();
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + "; " + expires;
}

function getCookie(cname) {
    var name = cname.replace(' ', '').toLowerCase() + "=";
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
        (visual.browser == "Safari" && (visual.version >= "6" || visual.version >= "0.0"))) {
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

function efectoLeft(obj, wait, duration, xini, xfin, efecto, complete) {
    if (visual.level == 1) {
        obj.style.visibility = "visible";
        obj.style.left = xfin.toFixed(0) + 'px';
        if (complete)
            complete();
    }
    else
        var tween = new TWEEN.Tween({ x: xini })
            .to({ x: xfin }, duration)
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
            if (callback)
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

    ret = "<div class='titulo3' style='margin: 0px;padding:0px;'><nobr>" + tr("Usuarios") + ": " + ap.usuarios + "<br>" + tr("Activos") + ": " + ap.activos + "</nobr></div>";
    ret += "<div class='titulo3' style='margin: 0px;padding:0px;'><nobr>" + tr("Si") + "&ge;" + ap.minSiValue + " (" + ap.minSiPc + "%)</nobr></div>";
    ret += "<div class='titulo3' style='margin: 0px;padding:0px;'><nobr>" + tr("No") + "&le;" + ap.maxNoValue + " (" + ap.maxNoPc + "%)</nobr></div>";
    panel.innerHTML = ret;
}

function actualizarDatosGrupo() {
    var panel = document.getElementById("panelGrupo");
    var ret = "";
    var ap = arbolPersonal;
    var born = new Date(ap.born.match(/\d+/)[0] * 1);
    var dias = Math.abs(new Date() - born) / (24 * 60 * 60 * 1000);
    ret = "<div class='titulo3' style='margin: 0px;padding:0px;'><nobr>" + tr("Dias") + ":" + dias.toFixed(0) + "</nobr></div>";
    ret += "<div class='titulo3' style='margin: 0px;padding:0px;'><nobr>" + tr("Usuarios") + ": " + ap.usuarios + "</nobr></div>";
    ret += "<div class='titulo3' style='margin: 0px;padding:0px;'><nobr>" + tr("Activos") + ": " + ap.activos + "</nobr></div>";
    ret += "<div class='titulo3' style='margin: 0px;padding:0px;'><nobr>" + tr("Decisiones") + ":" + ap.documentos + "</nobr></div>";
    //ret += "<div class='titulo3' style='margin: 0px;padding:0px;'><nobr>Comunes:0</nobr></div>";
    panel.innerHTML = ret;
    panel.style.display = 'inline';
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

function URIEncode(s) {
    while (s.indexOf("<") >= 0)
        s = s.replace("<", "&lt;");

    while (s.indexOf(">") >= 0)
        s = s.replace(">", "&gt;");

    s = encodeURIComponent(s);
    return (s);
}

function URIDencode(s) {
    while (s.indexOf("&lt;") >= 0)
        s = s.replace("&lt;", "<");

    while (s.indexOf("&gt;") >= 0)
        s = s.replace("&gt;", ">");

    s = dencodeURIComponent(s);
    return (s);
}

function getURLName(s) {
    while (s.indexOf('/') >= 0)
        s = s.substring(s.indexOf('/') + 1);
    return s;
}

function HTMLDecode(value) {
    // set the type of encoding to numerical entities e.g & instead of &
    Encoder.EncodeType = "numerical";

    // or to set it to encode to html entities e.g & instead of &
    Encoder.EncodeType = "entity";

    // HTML encode text from an input element
    // This will prevent double encoding.
    return Encoder.htmlEncode(value);

    // To encode but to allow double encoding which means any existing entities such as
    // &amp; will be converted to &amp;amp;
    //var dblEncoded = Encoder.htmlEncode(document.getElementById('input'), true);

}

function activarStyleEditor() {
    var areas = $(".editarrtf");
    for (var j = 0; j < areas.length; j++)
        //new Simditor({ textarea: areas[j] });
        new Simditor({
            textarea: areas[j], defaultImage: '', pasteImage: false, toolbar:
            ['bold', 'italic', 'underline', 'strikethrough', 'fontScale', 'color', 'ol', 'ul', 'link', 'indent', 'outdent', 'alignment']
        });
}

function setBackgroundImage() {
    var screenFactor = 1920 / 974;
    if (window.innerWidth / window.innerHeight > screenFactor)
        document.body.style.backgroundSize = window.innerWidth + "px " + (window.innerWidth / screenFactor).toFixed(0) + "px";
    else
        document.body.style.backgroundSize = (window.innerHeight * screenFactor).toFixed(0) + "px " + window.innerHeight + "px";
}