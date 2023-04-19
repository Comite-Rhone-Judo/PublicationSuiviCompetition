var c_name = "";
var _minspeed = 5000;
var _minrefresh = 10000;
var _lastrefresh = new Date();
var _current_pos = 0;

$(document).ready(function () {
    var str = "defilement" + window.location.pathname;
    var i = 0, strLength = str.length;
    for (i; i < strLength; i++) {
        str = str.replace("/", "_");
    }
    //debugger;
    c_name = str;

    var defil = getCookie(c_name);
    if (defil != "") {
        anim1();
    }
});


function setDefilement() {
    //debugger;
    var defil = getCookie(c_name);
    if (defil == "") {
        setCookie(c_name, "true", 1);
        anim1();
    }
    else {
        $('html, body').stop(true, true);
        setCookie(c_name, "", 1);
    }
}

function anim1() {

    //debugger;
    _current_pos = -($(window).height() / 2);
    $('html, body').css("margin-top", "500px");
    $('html, body').animate({
        "margin-top": 0
    }, 250, 'linear', function () {
        loop1();
    });
}

function anim2() {
    //debugger;
    $('html, body').css("margin-top", "0px");
    $('html, body').stop(true, true).animate({
        "margin-top": "500px"
    }, 250, 'linear', function () {
        //debugger;
        location.reload(true);
    });
}


//function loop1() {
//    var position = 0;
//    var max = $(document).height() - $(window).height();
//    var increment = 200;

//    //debugger;

//    while (position < max) {
//        $('html, body').delay(2000).animate({
//            scrollTop: position
//        }, 200, 'linear');
//        position += increment;
//        debugger;
//    }

//    //var scrollBottom = $(document).height() - $(window).height();
//    ////debugger;

//    //var speed = scrollBottom * 10;
//    //if (speed < _minspeed) {
//    //    speed = _minspeed;
//    //}

//    //$('html, body').animate({
//    //    scrollTop: scrollBottom + 20
//    //}, speed, 'linear');

//    $('html, body').animate({
//        scrollTop: 0
//    }, 1, 'linear', function () {

//        var date = new Date();
//        if (date.getTime() - _lastrefresh.getTime() > _minrefresh) {
//            _lastrefresh = new Date();
//            anim2();
//        }
//        else {
//            loop1();
//        }
//        //debugger;            
//    });
//}

function loop1() {
    _current_pos += $(window).height() / 2;

    $('html, body').delay(2000).animate({
        scrollTop: _current_pos
    }, 200, 'linear', function () {

        if (_current_pos > $(document).height() - $(window).height()) {
            _current_pos = -($(window).height() / 2);
            var date = new Date();
            if (date.getTime() - _lastrefresh.getTime() > _minrefresh) {
                _lastrefresh = new Date();
                $('html, body').delay(2000).animate({
                    scrollTop: 0
                }, 200, 'linear', function () {
                    anim2();
                });
            }
            else {
                loop1();
            }
        }
        else {
            loop1();
        }
        //debugger;            
    });
}

var tabs = {};

function add_id_tabs(id_panel, id_tabs, type) {
    var obj = { panel: id_panel, tabs: id_tabs };

    if (tabs[type] == undefined) {
        tabs[type] = [];
    }

    tabs[type].push(obj);
}

function set_tab(index, type) {
    for (var i = 0; i < tabs[type].length; i++) {
        if (index - 1 == i) {

            tabs[type][i].panel.show();
            tabs[type][i].tabs.addClass('active');
        }
        else {
            tabs[type][i].panel.hide();
            tabs[type][i].tabs.removeClass('active');
        }
    }
}

$(window).load(function () {
    add_id_tabs($('#div1'), $('#tab1'), 'form');
    add_id_tabs($('#div2'), $('#tab2'), 'form');
    add_id_tabs($('#div3'), $('#tab3'), 'form');
    add_id_tabs($('#div4'), $('#tab4'), 'form');
    add_id_tabs($('#div5'), $('#tab5'), 'form');

    set_tab(3, 'form');
});

function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}


function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}