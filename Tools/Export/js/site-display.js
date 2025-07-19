// ========== Variables globales ==========

var gReloading;                 // Pour les gestion de l'autoreload
var gUseAutoReload = true;      // Pour activer ou desactiver l'autoreload
var gDelayAutoreloadSec = 60;   // Pour definir le delai de l'autoreload en secondes

window.onload = windowOnLoad;   // Gestionnaire d'evenements pour le chargement de la page par defaut

// ========== Gestion de la barre de navigation ==========
// Ouverture et fermeture de la barre de navigation
function openElement(elementName) {
    document.getElementById(elementName).style.display = "block";
}

function closeElement(elementName) {
    document.getElementById(elementName).style.display = "none";
}

// ========== Gestion de l'autoreload ==========

// A mettre sur le window.onload pour verifier automatiquement le reload toutes les 1 secondes
function checkReloading() {
    var timeoutms;

    if (window.location.hash == "#autoreload") {

        if (typeof (gDelayAutoreloadSec) == undefined || isNaN(gDelayAutoreloadSec)) {
            timeoutms = 60000;    // Par defaut 1 min
        } else {
            timeoutms = gDelayAutoreloadSec * 1000;    // Par defaut 1 min
        }

        gReloading = setTimeout(function () { window.location.reload(); }, timeoutms);
        document.getElementById('cbActualiser').checked = true;
    }
}

// Active le autoreload si la checkbox est cochee
function toggleAutoRefresh(cb) {
    if (cb.checked) {
        window.location.replace("#autoreload"); // Flag pour indiquer le autoreload
        gReloading = setTimeout(function () { window.location.reload(); }, 100); // Pour faire un 1er refrech immediatement
    } else {
        window.location.replace("#");
        clearTimeout(gReloading);
    }
}

// ========== Gestion des evenements ==========

// TODO Remplacer les gestionnaires d'evenements par le gestionnaire par defaut
// Callback pour le chargement de la page
function windowOnLoad() {
    if (gUseAutoReload) {
        // On verifie si on a un hash pour l'autoreload
        checkReloading();
    }

    // Charge les panels (categories, etc.)
    initPanels();

    // Les barres d'onglets
    initTabs();
}

// ========== Gestion des onglets ==========

// Ouvre un onglet d'une barre d'onglets, tabGroupName est le nom du groupe (data-tabgroup) auquel appartient l'onglet
// Les onglets doivent avoir la classe tasTabType et les boutons de la barre d'onglets la classe tasTabBtnType
function openTab(tabGroupName, tabName, saveIt) {
    var i, tabs, btns, s, query;

    // Ferme tous les onglets de la barre d'onglets
    query = "div.tasTabType[data-tabgroup='" + tabGroupName + "']";
    tabs = document.querySelectorAll(query);
    for (i = 0; i < tabs.length; i++) {
        tabs[i].style.display = "none";
    }

    // RAZ les boutons de la barre d'onglets
    query = "button.tasTabBtnType[data-tabgroup='" + tabGroupName + "']";
    btns = document.querySelectorAll(query);
    for (i = 0; i < tabs.length; i++) {
        btns[i].className = btns[i].className.replace(" w3-indigo", "");
    }

    // Affiche l'onglet et le bouton correspondant
    s = 'btn' + tabName;
    for (i = 0; i < btns.length; i++) {
        if (btns[i].id == s) {
            btns[i].className += " w3-indigo";
        }

        if (tabs[i].id == tabName) {
            tabs[i].style.display = "block";
        }
    }

    if (saveIt) {
        saveInSession(tabGroupName, tabName)
    }
}

// Initialise les onglets de la page, en ouvrant l'onglet qui est en session
function initTabs() {
    var tabs, v, grp;

    // Ferme tous les onglets de la barre d'onglets
    tabs = document.getElementsByClassName("tasTabType");
    for (i = 0; i < tabs.length; i++) {
        grp = tabs[i].dataset["tabgroup"];
        v = getInSession(grp);
        if (v == tabs[i].id) {
            openTab(grp, v, false)
        }
    }
}

// ========== Gestion des panneaux ==========

// Initialisation des panneaux au chargement de la page
function initPanels() {
    var x;

    x = document.getElementsByClassName("tasPanelType");
    for (i = 0; i < x.length; i++) {

        if (sessionStorage[x[i].id] == "block") {
            expandElement(x[i].id);
        }
    }
}

// Permute l'affiche d'un Panneau
function togglePanel(elementName) {
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
    // memorise l'etat dans le sessionStorage
    sessionStorage[elementName] = newState;

    document.getElementById(elementName).style.display = newState;
    document.getElementById(elementToShow).style.display = "inline";
}

// Permet d'expand un panneau et de cacher les autres
function expandPanel(elementName) {
    var expandElement = elementName + "Expand";
    var collapseElement = elementName + "Collapse";

    document.getElementById(expandElement).style.display = "none";
    document.getElementById(collapseElement).style.display = "none";

    document.getElementById(elementName).style.display = "block";
    document.getElementById(collapseElement).style.display = "inline";
}

// Permet de cacher un panneau
function collapsePanel(elementName) {
    var expandElement = elementName + "Expand";
    var collapseElement = elementName + "Collapse";

    document.getElementById(expandElement).style.display = "none";
    document.getElementById(collapseElement).style.display = "none";

    document.getElementById(elementName).style.display = "none";
    document.getElementById(expandElement).style.display = "inline";
}

// Initialise les panneaux de la page, en ouvrant ceux qui sont en session
initPanels() {
    var x;

    x = document.getElementsByClassName("tasPanelType");
    for (i = 0; i < x.length; i++) {

        if (sessionStorage[x[i].id] == "block") {
            expandPanel(x[i].id);
        }
    }
}

// ========== Gestion de la session ==========

// enregistre une valeur dans la session
function saveInSession(key, value) {
    let path = document.location.pathname;
    let fileName = path.substring(path.lastIndexOf('/') + 1);

    let fullKey = fileName + "," + key;

    sessionStorage.setItem(fullKey, value);
}

// Recupere une valeur dans la session
function getInSession(key) {
    let path = document.location.pathname;
    let fileName = path.substring(path.lastIndexOf('/') + 1);

    let fullKey = fileName + "," + key;

    return sessionStorage.getItem(fullKey);
}