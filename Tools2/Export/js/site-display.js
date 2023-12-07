// Gestion de l'affichage des differents panneaux
function openTab(evt, animName) {
    var i, x, tablinks;
    x = document.getElementsByClassName("pane");
    for (i = 0; i < x.length; i++) {
        x[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("navButton");
    for (i = 0; i < x.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" w3-indigo", "");
    }
    document.getElementById(animName).style.display = "block";
    evt.currentTarget.className += " w3-indigo";

    closeElement('navigationPanel');
}

// Ouverture et fermeture de la barre de navigation
function openElement(elementName) {
    document.getElementById(elementName).style.display = "block";
}

function closeElement(elementName) {
    document.getElementById(elementName).style.display = "none";
}

// Permute l'affiche d'un element
function toggleElement(elementName) {
    var state = document.getElementById(elementName).style.display;
    var expandElement = elementName + "Expand";
    var collapseElement = elementName + "Collapse";
    var elementToShow;
    var newState;

    document.getElementById(expandElement).style.display = "none";
    document.getElementById(collapseElement).style.display = "none";


    if (state == "none") {
        newState = "block";
        elementToShow = collapseElement;
    }
    else {
        newState = "none";
        elementToShow = expandElement;
    }

    document.getElementById(elementName).style.display = newState;
    document.getElementById(elementToShow).style.display = "inline";
}

var reloading;  // Pour les gestion de l'autoreload

// A mettre sur le window.onload pour verifier automatiquement le reload toutes les 1 secondes
function checkReloading() {
    var timeoutms;

    if (window.location.hash == "#autoreload") {

        if (typeof (delayAutoreloadSec) == undefined || isNaN(delayAutoreloadSec)) {
            timeoutms = 60000;    // Par defaut 1 min
        } else {
            timeoutms = delayAutoreloadSec * 1000;    // Par defaut 1 min
        }

        reloading = setTimeout(function () { window.location.reload(); }, timeoutms);
        document.getElementById('cbActualiser').checked = true;
    }
}

function toggleAutoRefresh(cb) {
    if (cb.checked) {
        window.location.replace("#autoreload"); // Flag pour indiquer le autoreload
        reloading = setTimeout(function () { window.location.reload(); }, 100); // Pour faire un 1er refrech immediatement
    } else {
        window.location.replace("#");
        clearTimeout(reloading);
    }
}