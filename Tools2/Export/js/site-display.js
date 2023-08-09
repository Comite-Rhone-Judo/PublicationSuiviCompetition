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