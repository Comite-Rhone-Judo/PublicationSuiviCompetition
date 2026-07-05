// ========== Variables globales ==========

var gReloading;                 // Pour les gestion de l'autoreload
var gUseAutoReload = true;      // Pour activer ou desactiver l'autoreload
var gDelayAutoReloadSec = 60;   // Pour definir le delai de l'autoreload en secondes

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

        if (typeof (gDelayAutoReloadSec) == undefined || isNaN(gDelayAutoReloadSec)) {
            timeoutms = 60000;    // Par defaut 1 min
        } else {
            timeoutms = gDelayAutoReloadSec * 1000;    // Par defaut 1 min
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
        setInSession(tabGroupName, tabName)
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

    // Les panneaux ouverts par defaut
    x = document.getElementsByClassName("tasOpenedPanelType");
    for (i = 0; i < x.length; i++) {

        // On les ouvre par defaut
        expandPanel(x[i].id);

        // Si l'etat est different en session
        if (getInSession(x[i].id) == "none") {
            collapsePanel(x[i].id);
        }
    }

    // Les panneaux fermes par defaut
    x = document.getElementsByClassName("tasClosedPanelType");
    for (i = 0; i < x.length; i++) {

        // On les fermes par defaut
        collapsePanel(x[i].id);

        // Si l'etat est different en session
        if (getInSession(x[i].id) == "block") {
            expandPanel(x[i].id);
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
    setInSession(elementName, newState);

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

// ========== Gestion de la session ==========

// enregistre une valeur dans la session
function setInSession(key, value) {
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

// ========== Gestion de la modale des Statistiques ==========

/**
 * Formate une valeur technique (ex: "12.5") en affichage français (ex: "12,5").
 * Gère les valeurs nulles, indéfinies ou vides.
 */
function formatFr(valueStr, defaultValue = '0') {
    if (!valueStr || valueStr === '') return defaultValue;
    return valueStr.replace('.', ',');
}

/**
 * Raccourci sécurisé pour injecter du texte dans un élément du DOM.
 */
function setModalText(id, text) {
    let el = document.getElementById(id);
    if (el) el.innerText = text;
}

/**
 * Met à jour une barre de progression W3.CSS et son label associé.
 */
function setJauge(lblId, barId, valueStr) {
    let barElement = document.getElementById(barId);
    if (barElement) {
        // Le CSS exige la donnée technique avec le point
        barElement.style.width = (valueStr || '0') + '%';
    }

    // Le label visuel utilise la fonction utilitaire française
    setModalText(lblId, formatFr(valueStr) + ' %');
}

/**
 * Charge les données du judoka cliqué dans la modale et l'affiche.
 */
function openJudokaStatsModal(rowElement) {
    // 1. En-tête de la modale
    setModalText('m-nom', rowElement.getAttribute('data-nom'));
    setModalText('m-cat', rowElement.getAttribute('data-cat'));

    // 2. Résultats globaux
    setModalText('d-combats', rowElement.getAttribute('data-cbts') || '0');
    setModalText('d-tauxvic', formatFr(rowElement.getAttribute('data-vic')) + ' %');

    // 3. Profil des victoires
    setJauge('lbl-ippon', 'bar-ippon', rowElement.getAttribute('data-ippon'));
    setJauge('lbl-wazaawa', 'bar-wazaawa', rowElement.getAttribute('data-wazaawa'));
    setJauge('lbl-waza', 'bar-waza', rowElement.getAttribute('data-waza'));
    setJauge('lbl-yuko', 'bar-yuko', rowElement.getAttribute('data-yuko'));
    setJauge('lbl-shido3', 'bar-shido3', rowElement.getAttribute('data-shido3'));
    setJauge('lbl-hansoku', 'bar-hansoku', rowElement.getAttribute('data-hansoku'));
    setJauge('lbl-amf', 'bar-amf', rowElement.getAttribute('data-amf'));
    setJauge('lbl-decision', 'bar-decision', rowElement.getAttribute('data-decision'));

    // 4. Durées de combat
    setModalText('d-tmin', rowElement.getAttribute('data-tmin') || '-');
    setModalText('d-tmoy', rowElement.getAttribute('data-tmoy') || '-');
    setModalText('d-tmax', rowElement.getAttribute('data-tmax') || '-');

    // 5. Golden Score
    let gsCbt = rowElement.getAttribute('data-gscbt') || '0';
    let gsPct = formatFr(rowElement.getAttribute('data-gspct'));
    setModalText('d-gscbt_pct', gsCbt + ' (' + gsPct + ' %)');
    setModalText('d-gsmoy', rowElement.getAttribute('data-gsmoy') || '-');

    // 6. Discipline
    setModalText('d-pen', formatFr(rowElement.getAttribute('data-pen')));

    // Bloquer le défilement de <body> ET de <html> (Correction du double ascenseur)
    document.body.style.overflow = 'hidden';
    document.documentElement.style.overflow = 'hidden';

    // Afficher la modale
    document.getElementById('statsModal').style.display = 'block';
}

/**
 * Ferme la modale
 */
function closeJudokaStatsModal() {
    // Masquer la modale
    document.getElementById('statsModal').style.display = 'none';

    // Restaurer le défilement normal de la page
    document.body.style.overflow = '';
    document.documentElement.style.overflow = '';
}