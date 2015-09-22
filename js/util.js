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

function doOpen(url) {
    getHttp(url, function (data) {
        document.getElementById("helpContent").innerHTML = data;
        document.getElementById("help").style.width = (window.innerWidth - 300) + "px";
        document.getElementById("help").style.height = (window.innerHeight - 150) + "px";
        document.getElementById("help").style.left = "100px";
        efectoTop(document.getElementById("help"), 0, -window.innerHeight + 150, 75, TWEEN.Easing.Cubic.Out);
        document.getElementById("help").style.visibility = "visible";
    });
}

function efectoTop(obj, wait, yini, yfin, efecto, complete) {
    var tween = new TWEEN.Tween({ y: yini })
        .to({ y: yfin }, 750)
        .easing(efecto)
        .onStart(function () {
            obj.style.visibility = "visible";
        })
        .onUpdate(function () {
            obj.style.top = this.y + 'px';
        })
        .onComplete(function () {
            if (complete)
                complete();
        })
        .delay(wait)
        .start();
}

function efectoLeft(obj, wait, xini, xfin, efecto, complete) {
    var tween = new TWEEN.Tween({ x: xini })
        .to({ x: xfin }, 750)
        .easing(efecto)
        .onStart(function () {
            obj.style.visibility = "visible";
        })
        .onUpdate(function () {
            obj.style.left = this.x + 'px';
        })
        .onComplete(function () {
            if (complete)
                complete();
        })
        .delay(wait)
        .start();
}

function efectoOpacity(obj, wait, oini, ofin, efecto, complete) {
    var tween = new TWEEN.Tween({ o: oini })
        .to({ o: ofin }, 900)
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
